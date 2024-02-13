using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters 
{
    public class PlayerGenericBody : GenericBody
    {
        [Header("Health")]
/*        public int health;
        public int maxHealth;*/

        [SerializeField] private AllyCanvas allyCanvas;

        public AllyCanvas AllyCanvas => allyCanvas;

        public RunManager RM;

        public void Start()
        {
            RM = RunManager.instance;
            maxHealth = ((RM.player1Stats.CON * 2) + (RM.player2Stats.CON * 2));
            SetAllStatus();
            AllyCanvas.InitCanvas();

            OnHealthChanged += AllyCanvas.UpdateHealthText;
            OnStatusChanged += AllyCanvas.UpdateStatusText;
            OnStatusApplied += AllyCanvas.ApplyStatus;
            OnStatusCleared += AllyCanvas.ClearStatus;

            OnHealthChanged?.Invoke(health, maxHealth);
        }

        public void PlayerTakeDamage(int damageRecieved)
        {
            Debug.Log("body taking " + damageRecieved + " damage");
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
            health -= damageToTake;
            OnHealthChanged?.Invoke(health, maxHealth);
            //sync damage here
        }
        public void OnPlayerTurnStart()
        {
            TriggerStatus(StatusType.Block);
            TriggerStatus(StatusType.Bleed);
            TriggerStatus(StatusType.Burn);
            TriggerStatus(StatusType.Regen);
            TriggerStatus(StatusType.Frozen);
            TriggerStatus(StatusType.Vulnerable);
        }

        public void HealDamage(int healRecieved)
        {
            health += healRecieved;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }

    }
}
