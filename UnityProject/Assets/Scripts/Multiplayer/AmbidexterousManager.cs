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

    //public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController> { };
    public override void Start()
    {
        base.Start();
        if (Instance == null)
            Instance = this;


    }
    public override void Awake()
    {
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


        base.Awake();
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //base.OnServerAddPlayer(conn);
        if(SceneManager.GetActiveScene().name == "MultiplayerTest")
        {

/*        if (SceneManager.GetActiveScene().name == "MultiplayerTest")
        {
            Debug.Log("Player joined server");
            //GetPlayerID GamePlayerInstance = Instantiate(GamePlayerPrefab);
            //GamePlayerInstance.ConnectionID = conn.connectionId;
            //GamePlayerInstance.PlayerIDNumber = GamePlayers.Count + 1;
            //GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.CurrentLobbyID, PlayerList.Count);
            //if (PlayerList.Count == 0)
                //hostConnection = conn;
                //NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }*/

        Debug.Log("added Player");
        GetPlayerID player = Instantiate(GamePlayerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player.gameObject);

        Debug.Log($"There are {numPlayers} on the server");
            for (int i = 0; i < NetworkServer.connections.Count; i++)
            {
            //NetworkServer.connections.ElementAt(i).Value
                
                
                var playerStartup = player.GetComponent<GetPlayerID>();
                
                if (SteamManager.Initialized)
                {
                    playerStartup.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)CurrentLobbyID, i);
                    //foreach()
                }

                else
                {
                }
                if (NetworkServer.connections.Count == 1)
                {
                    //enable Player2
                    player.GetComponent<GetPlayerID>().ReadyOfflinePlayer2();
                }
                else
                {
                    player.GetComponent<GetPlayerID>().DisableOfflinePlayer2();
                }

            playerStartup.RenameOnAll(i, player.GetComponent<NetworkIdentity>().netId);
        }
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
    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        for (int i = 0; i < NetworkServer.connections.Count; i++)
        {
            var player = NetworkServer.connections.ElementAt(i).Value;
            //player.identity.GetComponent<GetPlayerID>().StartedScene();
        }
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
