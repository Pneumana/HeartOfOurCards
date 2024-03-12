using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
//using static UnityEditor.ShaderData;

public class MonsterPromStatCheck : MonoBehaviour
{
    public int autoPass;
    public int characterID;

    public string thisChoiceStat;
    public string otherChoiceStat;

    public int passID;
    public int failID;

    //public string label;

    // Start is called before the first frame update
    void Start()
    {
        if (NetworkServer.connections.Count == 1)
            return;
        if (RunManager.instance.LocalPlayerID != RunManager.instance.pickingPlayer)
        {
            gameObject.GetComponent<Button>().enabled = false;
            Debug.Log("local player cant make a choice " + characterID);
        }
        else
        {
            Debug.Log("local player CAN make a choice " + characterID);
        }
    }

    public void Activated()
    {
        Debug.Log(RunManager.instance.playerStatList.Count + " <- playerStats");
        var TargetPlayer = RunManager.instance.playerStatList[characterID];
        var _targetStat = (int)TargetPlayer.GetType().GetField(thisChoiceStat).GetValue(TargetPlayer);
        var _targetStatREP = (int)TargetPlayer.GetType().GetField("Rep"+thisChoiceStat).GetValue(TargetPlayer);
        var _otherStat = (int)TargetPlayer.GetType().GetField(otherChoiceStat).GetValue(TargetPlayer);
        var _otherStatREP = (int)TargetPlayer.GetType().GetField("Rep" + otherChoiceStat).GetValue(TargetPlayer);

        if (_targetStat >= _otherStat || _targetStat + _targetStatREP >= autoPass && _otherStat + _otherStatREP >= autoPass)
        {
            //pass
            GameObject.Find("VNSyncer").GetComponent<VNSyncer>().CMDSkipTo(passID);
        }
        else
        {
            //fail
        }
        /*if (_targetStat >= passRequirement)
        {
            Debug.Log("player " + characterID + " passed a stat check for " + thisChoiceStat + " with an intensity of " + passRequirement);
            GameObject.Find("VNSyncer").GetComponent<VNSyncer>().CMDSkipTo(passID);
        }
        else
        {
            Debug.Log("player " + characterID + " FAILED a stat check for " + thisChoiceStat + " with an intensity of " + passRequirement);
            GameObject.Find("VNSyncer").GetComponent<VNSyncer>().CMDSkipTo(failID);
        }*/
    }
}
