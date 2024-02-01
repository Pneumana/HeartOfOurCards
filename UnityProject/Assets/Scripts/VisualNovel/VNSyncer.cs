using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VNSyncer : NetworkBehaviour
{
    public DialougeDisplayer dialougeDisplayer;

    private void Update()
    {
        if (isLocalPlayer)
        {

        }
    }
    public void DoInput()
    {
        CMDProgressDialouge();
    }
    [Command(requiresAuthority =false)]
    public void CMDSkipTo(int id)
    {
        ClientSkipTo(id);
    }
    [ClientRpc]
    public void ClientSkipTo(int id)
    {
        dialougeDisplayer.SkipTo(id, true);
    }

    [Command(requiresAuthority = false)]
    public void CMDProgressDialouge()
    {
        //on click run this
        Debug.Log("running progress dia on server");
        ClientProgressDialouge();
    }
    [ClientRpc]
    public void ClientProgressDialouge()
    {
        Debug.Log("recieved progress dialouge command");
        //dialougeDisplayer
        if (!dialougeDisplayer.displayingChoice)
        {
            dialougeDisplayer.SkipTo(-1, false);
            Debug.Log("running progress dia on client");
        }
            
    }
}
