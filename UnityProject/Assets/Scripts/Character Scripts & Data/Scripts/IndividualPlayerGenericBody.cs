using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Characters
{
    public class IndividualPlayerGenericBody : GenericBody
    {

        public RunManager RM;
        public CardPlayerController Controller;
        [SerializeField] private AllyCanvas allyCanvas;

        public AllyCanvas AllyCanvas => allyCanvas;

        void Start()
        {
            RM = RunManager.instance;
            Controller = GetComponent<CardPlayerController>();
            
            SetAllStatus();
            try
            {
                AllyCanvas.InitCanvas();
            }
            catch { };


            if (CharacterType == CharacterType.P1)
            {
                ApplyStatus(StatusType.Strength, Mathf.FloorToInt(RM.playerStatList[0].DMG / 3));
                ApplyStatus(StatusType.Dexterity, Mathf.FloorToInt(RM.playerStatList[0].NRG / 3));
                Controller.maxEnergy = (3 + Mathf.FloorToInt(RM.playerStatList[0].CON / 6));
            }
            else if (CharacterType == CharacterType.P2)
            {
                ApplyStatus(StatusType.Strength, Mathf.FloorToInt(RM.playerStatList[1].DMG / 3));
                ApplyStatus(StatusType.Dexterity, Mathf.FloorToInt(RM.playerStatList[1].NRG / 3));
                Controller.maxEnergy = (3 + Mathf.FloorToInt(RM.playerStatList[1].CON / 6));
            }
        }

        public void OnPlayerTurnEnd()
        {
            TriggerStatus(StatusType.Frozen);
        }
    }
}
