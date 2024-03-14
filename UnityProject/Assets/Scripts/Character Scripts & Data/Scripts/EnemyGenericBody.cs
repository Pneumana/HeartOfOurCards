using Characters;
using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

        [Command(requiresAuthority = false)]
        public void OnEnemyTurnStart()
        {
            OnEnemyTurnStartRPC();
        }

        [ClientRpc]
        public void OnEnemyTurnStartRPC()
        {
            TriggerStatus(StatusType.Block);
            TriggerStatus(StatusType.Bleed);
            TriggerStatus(StatusType.Burn);
            TriggerStatus(StatusType.Regen);
            TriggerStatus(StatusType.Vulnerable);
        }

        [Command(requiresAuthority = false)]
        public void OnEnemyTurnEnd()
        {
            OnEnemyTurnEndRPC();
        }

        [ClientRpc]
        public void OnEnemyTurnEndRPC()
        {
            TriggerStatus(StatusType.Frozen);
        }
    }
}
