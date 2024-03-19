using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Characters 
{
    public class PlayerGenericBody : GenericBody
    {
        [Header("Health")]
        public int _health;
        //public int maxHealth;

        [SerializeField] private AllyCanvas allyCanvas;

        public AllyCanvas AllyCanvas => allyCanvas;

        public RunManager RM;

        public HealthBar sharedHealthBar;
        //Start can be ran because this only needs to be set up once per combat
        public void Start()
        {
            RM = RunManager.instance;
            maxHealth = (20 + (RM.playerStatList[0].CON * 2) + (RM.playerStatList[1].CON * 2));
            _health = RM.Health;
            SetAllStatus();
            AllyCanvas.InitCanvas();

            OnHealthChanged += AllyCanvas.UpdateHealthText;
            //OnHealthChanged += sharedHealthBar.GetHealthChange;
            OnStatusChanged += AllyCanvas.UpdateStatusText;
            OnStatusApplied += AllyCanvas.ApplyStatus;
            OnStatusCleared += AllyCanvas.ClearStatus;

            OnHealthChanged?.Invoke(_health, maxHealth);
        }

        public void PlayerTakeDamage(int damageRecieved)
        {
            //Debug.Log("player body taking " + damageRecieved + " damage");
            var damageToTake = damageRecieved;
            var shieldThisTurn = StatusDict[StatusType.Block].StatusValue;
            //shield damage block
            if (StatusDict[StatusType.Vulnerable].StatusValue > 0)
            {
                damageToTake = Mathf.CeilToInt(damageToTake * 1.5f);
            }
            while (shieldThisTurn > 0 && damageToTake > 0)
            {
                shieldThisTurn--;
                damageToTake--;
            }
            _health -= damageToTake;
            OnHealthChanged?.Invoke(_health, maxHealth);
            
            //sync damage here
        }

        public void OnPlayerTurnStart()
        {
            TriggerStatus(StatusType.Block);
            TriggerStatus(StatusType.Bleed);
            TriggerStatus(StatusType.Burn);
            TriggerStatus(StatusType.Regen);
            TriggerStatus(StatusType.Vulnerable);
        }

        public void OnPlayerTurnEnd()
        {
            TriggerStatus(StatusType.Frozen);
        }

        private void OnDestroy()
        {
            RunManager.instance.Health = health;
        }
    }
}
