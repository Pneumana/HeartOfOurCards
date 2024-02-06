using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Globalization;

public class RunManager : NetworkBehaviour
{
    //List player1 items
    //list player1 deck
    //list player1 library

    //currencies?

    //list player 2 items
    //list player2 deck
    //list player2 library

    //it's all stored here, bby

    //this data can be synced between players, using RPC to modify any of the values

    public static RunManager instance;

    public List<Vector3Int> explorableRooms = new List<Vector3Int>();
    public List<Vector3Int> revealedRooms = new List<Vector3Int>();
    public List<Vector3Int> completedRooms = new List<Vector3Int>();

    public int seed;

    public bool secondPlayerPick;

    public int LocalPlayerID;

    public struct PlayerStats
    {
        public int DMG; //Overkill
        public int INT; //Logic
        public int NRG; //Velocity
        public int CON;//or evasion? //Endurance
        public PlayerStats(int _dmg = 0, int _int = 0, int _nrg = 0, int _con = 0)
        {
            DMG = _dmg;
            INT = _int;
            NRG = _nrg;
            CON = _con;
        }
    }
    public PlayerStats player1Stats;
    public PlayerStats player2Stats;

    [SyncVar]public List<PlayerStats> playerStatList = new List<PlayerStats>();

    Rect p1 = new Rect(20, 20, 120, 120);
    Rect p2 = new Rect(140, 20, 120, 120);

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        player1Stats.DMG = 10;
        player1Stats.INT = 10;
        player1Stats.NRG = 10;
        player1Stats.CON = 10;

        player2Stats.DMG = 10;
        player2Stats.INT = 10;
        player2Stats.NRG = 10;
        player2Stats.CON = 10;



    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        //p1 = GUI.Window(0, p1, Player1Window, "Player 1 Stats");
        //p2 = GUI.Window(1, p2, Player2Window, "Player 2 Stats");
    }
    void Player1Window(int windowID)
    {
        if (GUI.Button(new Rect(0, 20, 100, 20), "DMG = " + player1Stats.DMG))
        {
            player1Stats.DMG++;
        }
        if (GUI.Button(new Rect(0, 40, 100, 20), "INT = " + player1Stats.INT))
        {
            player1Stats.INT++;
        }
        if (GUI.Button(new Rect(0, 60, 100, 20), "NRG = " + player1Stats.NRG))
        {
            player1Stats.NRG++;
        }
        if (GUI.Button(new Rect(0, 80, 120, 20), "CON = " + player1Stats.CON))
        {
            player1Stats.CON++;
        }
    }
    void Player2Window(int windowID)
    {
        if (GUI.Button(new Rect(0, 20, 100, 20), "DMG = " + player2Stats.DMG))
        {
            player2Stats.DMG++;
        }
        if (GUI.Button(new Rect(0, 40, 100, 20), "INT = " + player2Stats.INT))
        {
            player2Stats.INT++;
        }
        if (GUI.Button(new Rect(0, 60, 100, 20), "NRG = " + player2Stats.NRG))
        {
            player2Stats.NRG++;
        }
        if (GUI.Button(new Rect(0, 80, 120, 20), "CON = " + player2Stats.CON))
        {
            player2Stats.CON++;
        }
    }

    /*    public override void OnStartAuthority()
        {

        }*/
    public void TryStartGame(string map)
    {
        Debug.Log("trying to start game");
        if (isServer)
        {
            var _seed = Random.Range(int.MinValue, int.MaxValue);
            Debug.Log("command to set seed to " + _seed);
            ChangeServerSeed(_seed, map);
        }
    }
    [Command(requiresAuthority = false)]
    public void ChangeServerSeed(int seed, string map)
    {
        Debug.Log("command recieved");
        ChangeClientSeed(seed, map);
    }
    [ClientRpc]
    public void ChangeClientSeed(int seed, string map)
    {
        Debug.Log("client command recieved");
        if (AmbidexterousManager.Instance.seed == 0)
        {
            AmbidexterousManager.Instance.ServerChangeSeed(seed);
        }
        AmbidexterousManager.Instance.ChangeScene(map);
    }
/*    [Command(requiresAuthority = false)]
    public void ChangePlayerMapTurn()
    {
        if(AmbidexterousManager.Instance.PlayerList.Count > 1)
        {
            ClientChangeMapTurn();
            //toggle map turns (this is who can pick where they go)
        }
    }
    [ClientRpc]
    public void ClientChangeMapTurn()
    {
        if (secondPlayerPick)
        {
            secondPlayerPick = false;
        }
        else
        {
            secondPlayerPick = true;
        }
    }*/
}
