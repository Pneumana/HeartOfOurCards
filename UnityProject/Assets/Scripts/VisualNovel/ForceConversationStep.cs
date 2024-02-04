using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public class ForceConversationStep : MonoBehaviour
{
    [Header("Leave -1 to make the option just progress dialouge")]
    public int skipTo = -1;
    public int characterID;
    private void Start()
    {
        if (RunManager.instance.LocalPlayerID != characterID)
        {
            gameObject.GetComponent<Button>().enabled = false;
        }
    }
    public void Activate()
    {
        GameObject.Find("VNSyncer").GetComponent<VNSyncer>().CMDSkipTo(skipTo);
    }
}
