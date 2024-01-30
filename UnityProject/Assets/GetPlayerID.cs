using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Steamworks;
using TMPro;
using UnityEngine.SceneManagement;

public class GetPlayerID : NetworkBehaviour
{
    // Start is called before the first frame update
    //public int id;

    public ulong PlayerSteamID;
    public string PlayerName;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;

    public GameObject combatSceneUI;
    public GameObject combatScene;
    public GameObject player2combatSceneUI;
    public GameObject player2combatScene;

    bool avatarRecieved;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Awake()
    {
        
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        StartedScene();
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
                transform.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                //load a default from resources
            }

        }
    }
    public override void OnStartClient()
    {
        if(SceneManager.GetActiveScene().name == "MultiplayerTest")
        {
            AmbidexterousManager.Instance.PlayerList.Add(this);
            var nm = GameObject.Find("NetworkManager").GetComponent<AmbidexterousManager>();
            nm.RenameLobby();
            nm.UpdateAllPlayers();
        }
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
        combatSceneUI.transform.SetParent(transform);
        //DontDestroyOnLoad(gameObject);
    }
    public void StartedScene()
    {
        if (SceneManager.GetActiveScene().name == "MultiplayerTest")
        {
            //transform.SetParent(GameObject.Find("PlayerList").transform);
        }
        if (SceneManager.GetActiveScene().name == "ConnorTest")
        {

            combatScene.SetActive(true);
            combatScene.transform.position = Vector3.zero + (Vector3.down * 1.89f);
            combatSceneUI.SetActive(true);
            combatSceneUI.transform.SetParent(GameObject.Find("Canvas").transform);
            combatSceneUI.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }
    }
}
