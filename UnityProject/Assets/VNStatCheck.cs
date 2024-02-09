using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VNStatCheck : MonoBehaviour
{
    // Start is called before the first frame update
    public int characterID;
    public int passID;
    public int failID;
    public int passRequirement;
    public string targetStat;

    private void Start()
    {
        if(RunManager.instance.LocalPlayerID != RunManager.instance.pickingPlayer)
        {
            gameObject.GetComponent<Button>().enabled = false;
            Debug.Log("local player cant make a choice " + characterID);
        }
        else
        {
            Debug.Log("local player CAN make a choice " + characterID);
        }
        //if local player(fetched from AmbiManager) == character id
        // PlayerList.ElementAt(LocalPlayerID)

        //that means they can click

        //otherwise remove the ability to click and like grey it out
    }

    public void Activated()
    {
        var TargetPlayer = RunManager.instance.playerStatList[characterID];
        var _targetStat = (int)TargetPlayer.GetType().GetField(targetStat).GetValue(TargetPlayer);
        if(_targetStat >= passRequirement)
        {
            Debug.Log("player " + characterID + " passed a stat check for " + targetStat + " with an intensity of " + passRequirement);
            GameObject.Find("VNSyncer").GetComponent<VNSyncer>().CMDSkipTo(passID);
        }
        else
        {
            Debug.Log("player " + characterID + " FAILED a stat check for " + targetStat + " with an intensity of " + passRequirement);
            GameObject.Find("VNSyncer").GetComponent<VNSyncer>().CMDSkipTo(failID);
        }
    }
}
