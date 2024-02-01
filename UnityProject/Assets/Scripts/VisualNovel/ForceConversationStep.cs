using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceConversationStep : MonoBehaviour
{
    [Header("Leave -1 to make the option just progress dialouge")]
    public int skipTo = -1;
    public void Activate()
    {
        GameObject.Find("VNSyncer").GetComponent<VNSyncer>().CMDSkipTo(skipTo);
    }
}
