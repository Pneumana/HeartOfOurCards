using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class GetPlayerID : NetworkBehaviour
{
    // Start is called before the first frame update
    //public int id;

    public ulong PlayerSteamID;
    public string PlayerName;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;

    bool avatarRecieved;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    void Start()
    {

        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        transform.SetParent(GameObject.Find("PlayerList").transform);
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
        var nm = GameObject.Find("NetworkManager").GetComponent<AmbidexterousManager>();
        nm.RenameLobby();
        nm.UpdateAllPlayers();
    }


}
