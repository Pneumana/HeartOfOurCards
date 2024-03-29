using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Steamworks;
using TMPro;
using UnityEngine.SceneManagement;
using DeckData;

public class GetPlayerID : NetworkBehaviour
{
    // Start is called before the first frame update
    //public int id;

    public ulong PlayerSteamID;
    public string PlayerName;
    [SyncVar]public int ConnID;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;

    public GameObject combatSceneUI;
    public GameObject combatScene;
    public GameObject player2combatSceneUI;
    public GameObject player2combatScene;

    public GameObject firstUsername;
    public GameObject lastUsername;

    bool avatarRecieved;

    //decks controlled by this player
    public List<List<CardData>> decks = new List<List<CardData>>();

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    [SerializeField] Image healthBar;
    
    private void Awake()
    {
        
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        //StartedScene(SceneManager.GetActiveScene().name);
        //StartCoroutine(Startup());
        if (netIdentity.isClientOnly)
        {
            //RenameOnAll(2, netId);
        }
        else
        {
            //RenameOnAll(1, netId);
        }

    }
    [Command(requiresAuthority =false)]
    public void RenameOnAll(int connID, uint netID)
    {
        Rename(connID, netID);
    }
    [ClientRpc]
    void Rename(int connID, uint _netID)
    {
        if (netId == _netID)
        {
            Debug.Log("renaming " + gameObject.name + " to " + "Player" + connID);
            gameObject.name = "Player" + connID;
        }

    }
    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
            return;
    }

/*    public void ChangeReadyState()
    {
        if (ready)
        {
            //Debug.Log("readied");
            //ready = false;
            readyLight.color = Color.green;
        }
        else
        {
            //Debug.Log("un readied");
            //ready = true;
            readyLight.color = Color.red;
        }
    }*/

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D tex = null;
        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));
            if (isValid)
            {
                tex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                tex.LoadRawTextureData(image);
                tex.Apply();
            }
        }


        avatarRecieved = true;
        return tex;
    }
    void GetPlayerIcon()
    {
        int imageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (imageID == -1)
        {
            Debug.Log("failed to load image for steamID " + PlayerSteamID);
            return;
        }
        Debug.Log("loaded image for steamID " + PlayerSteamID);
        PlayerIcon.texture = GetSteamImageAsTexture(imageID);
    }
    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        //ChangeReadyState();
        if (!avatarRecieved)
        {
            if (SteamManager.Initialized)
            {
                GetPlayerIcon();
                PlayerIcon.transform.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                //load a default from resources
            }

        }
    }
    public override void OnStartClient()
    {
        var nm = GameObject.Find("NetworkManager").GetComponent<AmbidexterousManager>();
        if (SceneManager.GetActiveScene().name == "MultiplayerTest")
        {
            nm.RenameLobby();
            nm.UpdateAllPlayers();
            nm.PlayerList.Add(this);
            nm.PlayerNetIDs.Add(netId);
        }
        //Invoke("LateAddPlayers", 0.02f);
    }
    void LateAddPlayers()
    {
        var nm = AmbidexterousManager.Instance;
        if (!nm.PlayerList.Contains(this))
        {
            Debug.Log("adding to playerlist");
            
        }
    }
    public override void OnStartAuthority()
    {
        gameObject.name = "LocalPlayer";
        RunManager.instance.LocalPlayerID = ConnID;
        
    }
    public void ReadyOfflinePlayer2()
    {
        player2combatScene.SetActive(true);
        player2combatSceneUI.SetActive(true);
    }
    public void DisableOfflinePlayer2()
    {
        player2combatScene.SetActive(false);
        player2combatSceneUI.SetActive(false);
    }
    public void RecallWorldObjects()
    {
        //reparents all objects to this
        player2combatSceneUI.transform.SetParent(combatSceneUI.transform);

        firstUsername.transform.SetParent(combatSceneUI.transform);
        lastUsername.transform.SetParent(combatSceneUI.transform);
        combatSceneUI.transform.SetParent(transform);
        player2combatScene.transform.SetParent(combatScene.transform);
        healthBar.transform.SetParent(combatSceneUI.transform);
        healthBar.gameObject.SetActive(false);
        combatScene.transform.SetParent(transform);



        var playerControllers = combatScene.GetComponentsInChildren<CardPlayerController>();
        
        foreach (CardPlayerController cpc in playerControllers)
        {
            //cpc.gameObject.GetComponent<CardDeck>().ServerDiscard(cpc.gameObject.GetComponent<CardDeck>().hand.Count, new int[0]);
            cpc.gameObject.GetComponent<CardDeck>().AllToDeck();
        }
        
        PlayerIcon.transform.SetParent(transform);
        //DontDestroyOnLoad(gameObject);

        foreach (CardPlayerController cpc in playerControllers)
        {
            if (cpc.gameObject.activeSelf)
                cpc.started = false;
        }
    }
    public void StartedScene(string SceneName)
    {
        if(RunManager.instance.Health > RunManager.instance.MaxHealth)
        {
            RunManager.instance.Health = RunManager.instance.MaxHealth;
        }
        //40 

        Debug.Log("started scene " + SceneManager.GetActiveScene().name + " on player " + ConnID);
        //disable player 2
        /*if (RunManager.instance.playerStatList.Count < NetworkServer.connections.Count)
            RunManager.instance.playerStatList.Add(new RunManager.PlayerStats(10, 10, 10, 10));*/
        Debug.Log("player Stat List count = " + RunManager.instance.playerStatList.Count);
        /*if (RunManager.instance.playerStatList.Count > NetworkServer.connections.Count)
            RunManager.instance.playerStatList.RemoveAt(1);
        Debug.Log("player Stat List count = " + RunManager.instance.playerStatList.Count);*/

        if (NetworkServer.connections.Count == 1)
        {
            //enable localP2
            player2combatScene.SetActive(true);
            player2combatSceneUI.SetActive(true);
        }
        else
        {
            if (SteamManager.Initialized)
            {

                if (PlayerSteamID == 0)
                    PlayerSteamID = 76561198272619590;
                
                var isLocal = (this == AmbidexterousManager.Instance.PlayerList[RunManager.instance.LocalPlayerID]);

                var loc = RunManager.instance.LocalPlayerID;
                var col = RunManager.instance.LocalPlayerID;
                if (col == 0)
                    col = 1;
                else
                    col = 0;

                if (isLocal)
                {
                    var steamID = AmbidexterousManager.Instance.PlayerList[loc].PlayerSteamID;
                    var username = SteamFriends.GetFriendPersonaName((CSteamID)steamID);
                    if (username == "") username = "Player " + (loc + 1);
                    GameObject.Find("FirstPlayerUsername").GetComponent<TextMeshProUGUI>().text = username;
                    //combatSceneUI.transform.Find("LastPlayerUsername").GetComponent<TextMeshProUGUI>().text = "Player 2";
                }
                else
                {
                    var steamID = AmbidexterousManager.Instance.PlayerList[col].PlayerSteamID;
                    var username = SteamFriends.GetFriendPersonaName((CSteamID)steamID);
                    if (username == "") username = "Player " + (col + 1);
                    GameObject.Find("FirstPlayerUsername").GetComponent<TextMeshProUGUI>().text = username;
                }
            }
            else
            {
                var isLocal = (this == AmbidexterousManager.Instance.PlayerList[RunManager.instance.LocalPlayerID]);

                var loc = RunManager.instance.LocalPlayerID;
                var col = RunManager.instance.LocalPlayerID;
                if (col == 0)
                    col = 1;
                else
                    col = 0;

                if (isLocal)
                {
                    GameObject.Find("FirstPlayerUsername").GetComponent<TextMeshProUGUI>().text = "Player " + (loc + 1);
                    //combatSceneUI.transform.Find("LastPlayerUsername").GetComponent<TextMeshProUGUI>().text = "Player 2";
                }
                else
                    GameObject.Find("FirstPlayerUsername").GetComponent<TextMeshProUGUI>().text = "Player " + (col + 1);
            }
            //disable
            player2combatScene.SetActive(false);
            player2combatSceneUI.SetActive(false);
        }

       
        if (SceneName == "MultiplayerTest")
        {
            //transform.SetParent(GameObject.Find("PlayerList").transform);
        }
        if (SceneName == "ConnorTest")
        {
            Debug.Log("combat scene loaded by Local player");
            
            combatScene.SetActive(true);
            combatSceneUI.SetActive(true);
            healthBar.gameObject.SetActive(true);
            healthBar.transform.SetParent(GameObject.Find("Canvas").transform);
            healthBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 50);





            if (AmbidexterousManager.Instance.PlayerList[0] == this)
            {
                //this is p1
                Debug.Log("player1");
                combatScene.transform.position = (Vector3.zero) + (Vector3.down * 1.89f);


                firstUsername.transform.SetParent(GameObject.Find("Player1TurnUI").transform);
                firstUsername.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                //set first name to Player1TurnUI
                /*GameObject.Find("FirstPlayerUsername").transform.SetParent(GameObject.Find("Player1TurnUI").transform);

                GameObject.Find("FirstPlayerUsername").GetComponent<RectTransform>().anchoredPosition = Vector3.zero;*/



            }
            else
            {
                Debug.Log("player 2");
                combatScene.transform.position = (Vector3.right * 4) + (Vector3.down * 1.89f);
                combatScene.transform.Find("Player1").GetComponent<HandManager>().HandPosition = new Vector3(4, -1, -2.82f);
                combatScene.transform.Find("Player1").GetComponent<HandManager>().cardSpread = -3;

                firstUsername.transform.SetParent(GameObject.Find("Player2TurnUI").transform);

                firstUsername.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }
            //combatScene.transform.position = Vector3.zero + (Vector3.down * 1.89f);

            

            player2combatSceneUI.transform.SetParent(GameObject.Find("Player2TurnUI").transform);
            player2combatSceneUI.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;


            combatSceneUI.transform.SetParent(GameObject.Find("Canvas").transform);
            combatSceneUI.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            PlayerIcon.transform.SetParent(GameObject.Find("PlayerIconAnchor").transform);
            PlayerIcon.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            //combatscene CardPlayerController.handmanager.UpdateHand pos
            var playerControllers = combatScene.GetComponentsInChildren<CardPlayerController>();
            Debug.Log("issuing draw requests for " + playerControllers.Length + " player controllers on scene start");
            //if (isServerOnly && NetworkServer.connections.Count > 1)
                //return;
            //if()
            //if(isOwned)
                foreach (CardPlayerController cpc in playerControllers)
                {
                if (cpc.gameObject.activeSelf)
                {
                    Debug.Log(cpc.gameObject.name + " issued command to start encounter");
                    cpc.CMDStartEncounter();
                }

                }
        }
    }
}
