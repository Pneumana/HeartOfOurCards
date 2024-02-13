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
            AllyCanvas.InitCanvas();

            if (CharacterType == CharacterType.P1)
            {
                ApplyStatus(StatusType.Strength, Mathf.FloorToInt(RM.player1Stats.DMG / 3));
                ApplyStatus(StatusType.Dexterity, Mathf.FloorToInt(RM.player1Stats.NRG / 3));
                Controller.maxEnergy = (3 + Mathf.FloorToInt(RM.player1Stats.CON / 6));
            }
            else if (CharacterType == CharacterType.P2)
            {
                ApplyStatus(StatusType.Strength, Mathf.FloorToInt(RM.player2Stats.DMG / 3));
                ApplyStatus(StatusType.Dexterity, Mathf.FloorToInt(RM.player2Stats.NRG / 3));
                Controller.maxEnergy = (3 + Mathf.FloorToInt(RM.player1Stats.CON / 6));
            }
        }
    }
}
