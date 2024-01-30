using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using kcp2k;
using Mirror.FizzySteam;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class AmbidexterousManager : NetworkManager
{
    //public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController> { };
    // Start is called before the first frame update
    public KcpTransport offline;
    public FizzySteamworks online;
    public bool ignoreSteam;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public List<GetPlayerID> PlayerList = new List<GetPlayerID>();

    //use a unique prefab for UI
    public List<GetPlayerID> PlayerLobby = new List<GetPlayerID>();
    //public List<GetPlayerID> PlayerList = new List<GetPlayerID>();
    //vars
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private AmbidexterousManager manager;

    public TextMeshProUGUI LobbyText;

    public NetworkManagerHUD offlineHud;
    public GameObject onlineHud;

    public bool PlayerObjectCreated = false;

    [SerializeField] public GetPlayerID GamePlayerPrefab;

    //keep track of all players here
    public static AmbidexterousManager Instance;

    public NetworkConnectionToClient hostConnection;

    int loadedPlayers;

    //public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController> { };
    public override void Awake()
    {
        if (Instance == null)
            Instance = this;
        offlineHud = GetComponent<NetworkManagerHUD>();
        if (!ignoreSteam)
        {
            if (SteamManager.Initialized)
            {
                string name = SteamFriends.GetPersonaName();
                Debug.Log(name);
            }



            try
            {
                Debug.Log("online as " + SteamFriends.GetPersonaName().ToString());
                transport = online;
                LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
                JoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
                LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
                offlineHud.enabled = false;
                onlineHud.SetActive(true);
            }
            catch
            {
                Debug.Log("offline as DefaultName");
                transport = offline;
            }
        }
        else
        {
            Debug.Log("offline as DefaultName");
            transport = offline;
        }
        DontDestroyOnLoad(transport.gameObject);
        base.Awake();
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //base.OnServerAddPlayer(conn);
        if(SceneManager.GetActiveScene().name == "MultiplayerTest")
        {
            Debug.Log("added Player");
            GetPlayerID player = Instantiate(GamePlayerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);

            Debug.Log($"There are {numPlayers} on the server");
            if (PlayerList.Count == 0)
                hostConnection = conn;
        }
        

    }
    //Steam specific functions
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }
        StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        if (NetworkServer.active) { return; }
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }

    public void RenameLobby()
    {
        //it's not gonna be initialized if ignoreSteam is set to true so i can just check this
        if (SteamManager.Initialized)
        {
            //CurrentLobbyID = manager.GetComponent<SteamLobby>().CurrentLobbyID;
            LobbyText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
        }

        else
        {
            LobbyText.text = "Local Lobby";
        }
    }

    public void UpdateAllPlayers()
    {
        for (int i = 0; i < NetworkServer.connections.Count; i++)
        {
            var player = NetworkServer.connections.ElementAt(i).Value;
            if (player != null)
                player.identity.GetComponent<GetPlayerID>().SetPlayerValues();
        }
    }
    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, maxConnections);
        onlineHud.SetActive(false);
    }
    public void CloseLobby()
    {
    }
    public void StartGame()
    {
        /*        for (int i = 0; i < NetworkServer.connections.Count; i++)
                {
                    var player = NetworkServer.connections.ElementAt(i).Value;
                    player.identity.GetComponent<GetPlayerID>().RecallWorldObjects();
                }*/
        ServerChangeScene("ConnorTest");
    }
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        NetworkClient.ready = false;
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    }
    public override void OnClientNotReady()
    {
        base.OnClientNotReady();
    }
    public override void OnServerSceneChanged(string newSceneName)
    {
        //base.OnServerSceneChanged(newSceneName);
        Debug.Log("server loaded");
        for (int i = 0; i < NetworkServer.connections.Count; i++)
        {
            var player = NetworkServer.connections.ElementAt(i).Value;
            //player.identity.GetComponent<GetPlayerID>().StartedScene();
        }
        NetworkClient.ready = true;
        NetworkServer.SetClientReady(hostConnection);
        //NetworkClient.PrepareToSpawnSceneObjects();
        if (newSceneName == "ConnorTest")
        {
            Debug.LogWarning("entered card game scene");
            var enemySpawner = GameObject.Find("EnemySpawner");

            Debug.Log("found enemy spawner");
            //enemySpawner.GetComponent<EnemySpawner>().EnemySpawns = Random.Range(1, 3);
            enemySpawner.gameObject.GetComponent<EnemySpawner>().SpawnEnemy(Random.Range(1, 3));
        }

        foreach(GetPlayerID plr in PlayerList)
        {
            plr.StartedScene();
        }
            

    }
    public override void OnClientSceneChanged()
    {
        EnemySpawner.instance.gameObject.SetActive(true);
        base.OnClientSceneChanged();

        Debug.Log("client loaded");
        loadedPlayers++;
        Debug.Log(loadedPlayers + " total clients loaded");

        foreach (GetPlayerID plr in PlayerList)
        {
            plr.StartedScene();
        }

        var enemySpawner = GameObject.Find("EnemySpawner");
        if(enemySpawner != null)
            Debug.Log("found enemy spawner");
        else
        {
            Debug.Log("unable to find enemy spawner");
        }
        //enemySpawner.GetComponent<EnemySpawner>().EnemySpawns = Random.Range(1, 3);

        //EnemySpawner.instance.SpawnEnemy(Random.Range(1, 3));
    }
    IEnumerator FindEnemySpawner()
    {
        yield return new WaitForSeconds(0.1f);
        
        yield return null;
    }
    void UpdatePlayerList()
    {

        Debug.Log("updating player list");
        if (!PlayerObjectCreated) { CreateHostPlayerItem(); } //Host
        if (PlayerLobby.Count < PlayerList.Count) { CreateClientPlayerItem(); }
        if (PlayerLobby.Count > PlayerList.Count) { RemovePlayerItem(); }
        if (PlayerLobby.Count == PlayerList.Count) { UpdatePlayerItem(); }
    }

    void CreateHostPlayerItem()
    {
        Debug.Log("creating host player");
        foreach (GetPlayerID player in PlayerList)
        {
            GameObject newPlayerItem = Instantiate(GamePlayerPrefab).gameObject;
            GetPlayerID NewPlayerItemScript = newPlayerItem.GetComponent<GetPlayerID>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            //NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            //NewPlayerItemScript.ready = player.PlayerReady;
            NewPlayerItemScript.SetPlayerValues();

            //newPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;
            //newPlayerItem.name = "LocalPlayer";

            PlayerLobby.Add(NewPlayerItemScript);

            //LocalPlayerObject = newPlayerItem;
        }

        PlayerObjectCreated = true;
    }



    void CreateClientPlayerItem()
    {

    }
    void RemovePlayerItem()
    {

    }
    void UpdatePlayerItem()
    {

    }

}
