using Characters;
using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class EnemyGenericBody : GenericBody
    {
        [SerializeField] protected EnemyCanvas enemyCanvas;

        public EnemyCanvas EnemyCanvas => enemyCanvas;

        public void Start()
        {
            SetAllStatus();
            EnemyCanvas.InitCanvas();

            OnHealthChanged += EnemyCanvas.UpdateHealthText;
            OnStatusChanged += EnemyCanvas.UpdateStatusText;
            OnStatusApplied += EnemyCanvas.ApplyStatus;
            OnStatusCleared += EnemyCanvas.ClearStatus;

            OnHealthChanged?.Invoke(health, maxHealth);
        }
        public void OnEnemyTurnStart()
        {
            TriggerStatus(StatusType.Block);
            TriggerStatus(StatusType.Bleed);
            TriggerStatus(StatusType.Burn);
            TriggerStatus(StatusType.Regen);
            TriggerStatus(StatusType.Frozen);
            TriggerStatus(StatusType.Vulnerable);
        }
    }
}
