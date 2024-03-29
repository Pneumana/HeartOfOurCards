using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
            AllyCanvas.InitCanvas();

            OnHealthChanged += allyCanvas.UpdateHealthText;
            OnStatusChanged += allyCanvas.UpdateStatusText;
            OnStatusApplied += allyCanvas.ApplyStatus;
            OnStatusCleared += allyCanvas.ClearStatus;


            if (CharacterType == CharacterType.P1)
            {
                if (Mathf.FloorToInt(RM.playerStatList[0].DMG / 3) >= 1)
                {
                    ApplyStatus(StatusType.Strength, Mathf.FloorToInt(RM.playerStatList[0].DMG / 3));
                }
                if (Mathf.FloorToInt(RM.playerStatList[0].NRG / 3) >= 1)
                {
                    ApplyStatus(StatusType.Dexterity, Mathf.FloorToInt(RM.playerStatList[0].NRG / 3));
                }
                Controller.maxEnergy = (3 + Mathf.FloorToInt(RM.playerStatList[0].CON / 6));
            }
            else if (CharacterType == CharacterType.P2)
            {
                if (Mathf.FloorToInt(RM.playerStatList[1].DMG / 3) >= 1)
                {
                    ApplyStatus(StatusType.Strength, Mathf.FloorToInt(RM.playerStatList[1].DMG / 3));
                }
                if (Mathf.FloorToInt(RM.playerStatList[1].NRG / 3) >= 1)
                {
                    ApplyStatus(StatusType.Dexterity, Mathf.FloorToInt(RM.playerStatList[1].NRG / 3));
                }
                Controller.maxEnergy = (3 + Mathf.FloorToInt(RM.playerStatList[1].CON / 6));
            }
        }

        public void OnPlayerTurnStart()
        {

        }

        public void OnPlayerTurnEnd()
        {

        }

    }
}
