using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Globalization;
using CardActions;
using UnityEngine.UI;

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
    //This is the player that can click on the map and is the main character for dialouge.
    [SyncVar]public int pickingPlayer;

    public int LocalPlayerID;

    public float localMapCamZoom;
    public Vector2 localMapCamPos;

    //
    [HideInInspector]public string ForceLoadConvo;
    [HideInInspector]public TextFieldConversation ForceLoadConvoReference;

    [HideInInspector]public GameObject forceLoadCombatEncounter;

    public List<string> experiencedEvents = new List<string>();

    public int Health;
    public int MaxHealth;

    public int Stamina = 8;

    public List<HeldItem> player1Items = new List<HeldItem>();
    public List<HeldItem> player2Items = new List<HeldItem>();

    //used to move between dungeon tiles

    public int Ratings;

    public bool fightingBoss;
    public interface IPlayerStats
    {
        int Gold { get;  set; }
    }
    public static PlayerStats EmptyPlayer = new PlayerStats(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,false);
    [SerializeField]
    public struct PlayerStats
    {
        public int DMG; //Overkill
        public int INT; //Logic
        public int NRG; //Velocity
        public int CON;//or evasion? //Endurance

        public int RepDMG;
        public int RepINT;
        public int RepNRG;
        public int RepCON;

        public int Gold;

        public int Kitsune;
        public int Lich;
        public int Naga;
        public int Mermaid;
        public int Dragon;
        public int Vampire;

        public int Producer;

        public bool usedBreak;

        public PlayerStats(int _dmg = 0, int _int = 0, int _nrg = 0, int _con = 0, int _kit = 0, int _lic = 0, int _nag = 0, int _mer = 0, int _dra = 0, int _vam = 0, int _pro = 0, int _gold = 0, int _repdmg = 0, int _repint = 0, int _repnrg = 0, int _repcon = 0, bool _usedBreak = false)
        {
            DMG = _dmg;
            INT = _int;
            NRG = _nrg;
            CON = _con;
            Kitsune = _kit;
            Lich = _lic;
            Naga = _nag;
            Mermaid = _mer;
            Dragon = _dra;
            Vampire = _vam;
            Producer = _pro;
            Gold = _gold;

            RepCON = _repcon;
            RepINT = _repint;
            RepDMG = _repdmg;
            RepNRG = _repnrg;

            usedBreak = _usedBreak;
        }
    }
    //public PlayerStats player1Stats;
    //public PlayerStats player2Stats;

    [SyncVar]public List<PlayerStats> playerStatList = new List<PlayerStats>() { new PlayerStats(3, 2, 3, 3), new PlayerStats(2, 2,3, 4) };

    Rect p1 = new Rect(20, 20, 120, 120);
    Rect p2 = new Rect(140, 20, 120, 120);

    void Awake()
    {
        if (instance != null && instance!= this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (MaxHealth == 0)
            {
                MaxHealth = (20 + (playerStatList[0].CON * 2) + (playerStatList[1].CON * 2));
                Health = MaxHealth;
                //if
            }

            CardActionProcessor.Initialize();
        }
    }
    private void Start()
    {
        /*player1Stats.DMG = 10;
        player1Stats.INT = 10;
        player1Stats.NRG = 10;
        player1Stats.CON = 10;

        player2Stats.DMG = 10;
        player2Stats.INT = 10;
        player2Stats.NRG = 10;
        player2Stats.CON = 10;*/



    }

    // Update is called once per frame
    void Update()
    {
/*        if(Health > MaxHealth)
        {
            Health = MaxHealth;
        }*/
    }
    private void OnGUI()
    {
        p1 = GUI.Window(0, p1, Player1Window, "Player 1 Stats");
        p2 = GUI.Window(1, p2, Player2Window, "Player 2 Stats");
    }
    void Player1Window(int windowID)
    {
        if (GUI.Button(new Rect(0, 20, 100, 20), "DMG = " + playerStatList[0].DMG))
        {
            //playerStatList[0].DMG++;
        }
        if (GUI.Button(new Rect(0, 40, 100, 20), "INT = " + playerStatList[0].INT))
        {
            //playerStatList[0].INT++;
        }
        if (GUI.Button(new Rect(0, 60, 100, 20), "NRG = " + playerStatList[0].NRG))
        {
            //player1Stats.NRG++;
        }
        if (GUI.Button(new Rect(0, 80, 120, 20), "CON = " + playerStatList[0].CON))
        {
            //player1Stats.CON++;
        }
    }
    void Player2Window(int windowID)
    {
        if (GUI.Button(new Rect(0, 20, 100, 20), "DMG = " + playerStatList[1].DMG))
        {
            //player2Stats.DMG++;
        }
        if (GUI.Button(new Rect(0, 40, 100, 20), "INT = " + playerStatList[1].INT))
        {
            //player2Stats.INT++;
        }
        if (GUI.Button(new Rect(0, 60, 100, 20), "NRG = " + playerStatList[1].NRG))
        {
            //player2Stats.NRG++;
        }
        if (GUI.Button(new Rect(0, 80, 120, 20), "CON = " + playerStatList[1].CON))
        {
            //player2Stats.CON++;
        }
    }

    /*    public override void OnStartAuthority()
        {

        }*/
    public void TryStartGame(string map)
    {
        Debug.Log("trying to start game");
        if (isServer && !AmbidexterousManager.Instance.loadingScene)
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
    [Command]
    public void VNConvoOverride(string convo)
    {
        ConvoOverride(convo);
    }
    [ClientRpc]
    public void ConvoOverride(string convo)
    {
        ForceLoadConvo = convo;
    }
    [Command(requiresAuthority =false)]
    public void CMDChangeStam(int change)
    {
        Debug.Log("server change stam");
        ChangeStam(change);
    }
    [ClientRpc]
    public void ChangeStam(int change)
    {
        var old = Stamina;
        Stamina += change;
        Debug.Log("client change stam " + old + " => " + Stamina);
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
    [ClientRpc]
    public void ChangeStat(int playerIndex, string stat, int change)
    {
        var mp = new PlayerStats();
        mp = playerStatList[playerIndex];

        mp.GetType().GetField(stat).SetValueDirect(__makeref(mp), (int)mp.GetType().GetField(stat).GetValue(mp) + change);

        instance.playerStatList[playerIndex] = mp;
    }
    [Command]
    public void CMDChangeStat(int playerIndex, string stat, int change)
    {
        ChangeStat(playerIndex, stat, change);
    }
    [Command(requiresAuthority = false)]
    public void CMDChangeSharedStat(string stat, int change)
    {
        ChangeSharedStat(stat, change);
    }
    [ClientRpc]
    public void ChangeSharedStat(string stat, int change)
    {
        //mp = playerStatList[playerIndex];
        Debug.Log("stat " + stat + " has a value of " + instance.GetType().GetField(stat).GetValue(instance));
        instance.GetType().GetField(stat).SetValueDirect(__makeref(instance), (int)instance.GetType().GetField(stat).GetValue(instance) + change);
    }


    [Command(requiresAuthority = false)]
    public void CMDAddItem(string item, int player)
    {
        AddItem(item, player);
    }
    [ClientRpc]
    public void AddItem(string item, int player)
    {
        var n = new GameObject();
        n.name = "Player " + player + "'s " + item;
        n.AddComponent<HeldItem>();
        var load = Resources.Load<ItemBase>("Items/" + item);
        n.GetComponent<HeldItem>().itemData = load;

        //add a UI element item to target player

        var uiParent = GameObject.Find("Player" + (player + 1) + "Items").transform;

        var u = new GameObject();
        u.AddComponent<Image>();
        u.GetComponent<Image>().sprite = load.sprite;
        u.transform.SetParent(uiParent);

        n.transform.SetParent(transform);

        if (player == 0)
            player1Items.Add(n.GetComponent<HeldItem>());
        else
            player2Items.Add(n.GetComponent<HeldItem>());
    }
    [ContextMenu("Add Item Rage")]
    public void AddItem()
    {
        CMDAddItem("Rage", 0);
    }

    [ContextMenu("Add Item Rage to Player 2")]
    public void AddItem2()
    {
        CMDAddItem("Rage", 1);
    }

}
