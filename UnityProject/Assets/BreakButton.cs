using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakButton : MonoBehaviour
{
    ConversationTable dungeonTable;
    // Start is called before the first frame update
    void Start()
    {
        dungeonTable = Resources.Load<ConversationTable>("Conversations/EncounterEvents/Breakroom/Breakroom");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Activated(int targetPlayer)
    {
        if(targetPlayer == RunManager.instance.LocalPlayerID || AmbidexterousManager.Instance.PlayerList.Count == 1)
        {
            if (RunManager.instance.playerStatList[0].usedBreak)
                return;
            var plr1 = RunManager.instance.playerStatList[targetPlayer];
            //var plr2 = RunManager.instance.playerStatList[1];

            RunManager.instance.playerStatList[0] = new RunManager.PlayerStats(plr1.DMG, plr1.INT, plr1.NRG, plr1.CON,
                plr1.Kitsune, plr1.Lich, plr1.Naga, plr1.Mermaid, plr1.Dragon, plr1.Vampire, plr1.Producer,
                plr1.Gold, plr1.RepDMG, plr1.RepINT, plr1.RepNRG, plr1.RepCON, true);
            //local player attempts to break
            var roll = Random.Range(0, dungeonTable.events.Count);
            var attempts = 1000;
            while (RunManager.instance.experiencedEvents.Contains(dungeonTable.events[roll].name))
            {
                attempts--;
                roll = Random.Range(0, dungeonTable.events.Count);
                if (attempts <= 0)
                {
                    Debug.LogWarning("loading new event used up all re-roll attempts!");
                    RunManager.instance.experiencedEvents.Clear();
                    break;
                }
            }
            RunManager.instance.ForceLoadConvoReference = dungeonTable.events[roll];
            RunManager.instance.experiencedEvents.Add(dungeonTable.events[roll].name);
            Debug.Log("loading event <color=#00FF00>" + RunManager.instance.ForceLoadConvoReference.name + "</color> with roll " + roll + " of " + (dungeonTable.events.Count - 1));
            AmbidexterousManager.Instance.ChangeScene("SampleScene");
        }
        else
        {
            //suggest other player breaks
        }
    }
}
