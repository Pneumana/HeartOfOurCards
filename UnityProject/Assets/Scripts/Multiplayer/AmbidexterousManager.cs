using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using kcp2k;
using Mirror.FizzySteam;
using System.Linq;
using TMPro;

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

    //vars
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private AmbidexterousManager manager;

    public TextMeshProUGUI LobbyText;

    public NetworkManagerHUD offlineHud;
    public GameObject onlineHud;
    //public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController> { };
    public override void Start()
    {
        base.Start();



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
        base.OnServerAddPlayer(conn);

        Debug.Log($"There are {numPlayers} on the server");
        for (int i = 0; i < NetworkServer.connections.Count; i++)
        {
            var player = NetworkServer.connections.ElementAt(i).Value;
            var playerStartup = player.identity.GetComponent<GetPlayerID>();
            playerStartup.RenameOnAll(i, player.identity.netId);
            if (SteamManager.Initialized)
            {
                playerStartup.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)CurrentLobbyID, i);
                //foreach()
            }

            else
            {
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
}
