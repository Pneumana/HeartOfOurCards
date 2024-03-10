using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakButton : MonoBehaviour
{
    [SerializeField]ConversationTable dungeonTable;
    [SerializeField] bool requiresBreak = true;
    [SerializeField] bool sequential = false;
    // Start is called before the first frame update
    void Start()
    {
        if(dungeonTable==null)
            dungeonTable = Resources.Load<ConversationTable>("Conversations/EncounterEvents/Breakroom/Breakroom");

        sequential = dungeonTable.sequential;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Activated(int targetPlayer)
    {
        if (targetPlayer == -1)
            targetPlayer = RunManager.instance.pickingPlayer;

        if(targetPlayer == RunManager.instance.LocalPlayerID || AmbidexterousManager.Instance.PlayerList.Count == 1)
        {
            if (RunManager.instance.playerStatList[targetPlayer].usedBreak && requiresBreak)
                return;
            var plr1 = RunManager.instance.playerStatList[targetPlayer];
            //var plr2 = RunManager.instance.playerStatList[1];

            if(requiresBreak)
                RunManager.instance.playerStatList[0] = new RunManager.PlayerStats(plr1.DMG, plr1.INT, plr1.NRG, plr1.CON,
                    plr1.Kitsune, plr1.Lich, plr1.Naga, plr1.Mermaid, plr1.Dragon, plr1.Vampire, plr1.Producer,
                    plr1.Gold, plr1.RepDMG, plr1.RepINT, plr1.RepNRG, plr1.RepCON, true);
            //local player attempts to break
            var roll = Random.Range(0, dungeonTable.events.Count);
            if (sequential)
                roll = 0;
            var attempts = 1000;
            while (RunManager.instance.experiencedEvents.Contains(dungeonTable.events[roll].name))
            {
                attempts--;
                if (!sequential)
                    roll = Random.Range(0, dungeonTable.events.Count);
                else
                    roll++;
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
