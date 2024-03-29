using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Managers;

namespace Characters
{
    public class StatusStats
    {
        public StatusType StatusType { get; set; }
        public int StatusValue { get; set; }
        public bool DecreaseOverTurn { get; set; } // If true, decrease on turn end
        public bool IsPermanent { get; set; } // If true, status can not be cleared during combat
        public bool IsActive { get; set; }
        public bool CanNegativeStack { get; set; }
        public bool ClearAtNextTurn { get; set; }

        public Action OnTriggerAction;
        public StatusStats(StatusType statusType, int statusValue, bool decreaseOverTurn = false, bool isPermanent = false, bool isActive = false, bool canNegativeStack = false, bool clearAtNextTurn = false)
        {
            StatusType = statusType;
            StatusValue = statusValue;
            DecreaseOverTurn = decreaseOverTurn;
            IsPermanent = isPermanent;
            IsActive = isActive;
            CanNegativeStack = canNegativeStack;
            ClearAtNextTurn = clearAtNextTurn;
        }
    }

    public abstract class GenericBody : NetworkBehaviour
    {
        [Header("Info")]
        [SerializeField] private CharacterType characterType;
        [Header("Health")]
        [SyncVar]public int health;
                 public int maxHealth;

        public RunManager.PlayerStats stats;
        public readonly Dictionary<StatusType, StatusStats> StatusDict = new Dictionary<StatusType, StatusStats>();

        public bool IsStunned { get; set; }
        public bool IsDeath { get; private set; }

        public Action OnDeath;
        public Action<int, int> OnHealthChanged;
        public Action<StatusType, int> OnStatusChanged;
        public Action<StatusType, int> OnStatusApplied;
        public Action<StatusType> OnStatusCleared;
        public Action OnHealAction;
        public Action OnTakeDamageAction;
        public Action OnShieldGained;
        public CharacterType CharacterType => characterType;
        //any % increases should use CeilToInt() so it all stays as ints

        //replace gameobject with netID


        private void Update()
        {
            //if (StatusDict[StatusType.Block].StatusValue > 0)
            //{
            //    Debug.Log(StatusDict[StatusType.Block].StatusValue + " on beneric body " + gameObject.name);
            //}

        }

        //[Command(requiresAuthority = false)]
        public void TakeDamage(int damageRecieved)
        {
            TakeDamageRPC(damageRecieved);
        }

        //[ClientRpc]
        public void TakeDamageRPC(int damageRecieved)
        {
            var damageToTake = damageRecieved;
            if (StatusDict[StatusType.Vulnerable].StatusValue > 0)
            {
                damageToTake = Mathf.CeilToInt(damageToTake * 1.5f);
            }
            Debug.Log("body taking " + damageRecieved + " damage");
            var shieldThisTurn = StatusDict[StatusType.Block].StatusValue;
            var usedBlock = Mathf.Clamp(damageToTake, 0, shieldThisTurn);

            if (shieldThisTurn > 0 && damageToTake > 0)
            {
                //var block = shieldThisTurn;
                damageToTake = damageToTake - usedBlock;
                damageToTake = Mathf.Clamp(damageToTake, 0, damageRecieved);
                shieldThisTurn = shieldThisTurn - usedBlock;
                health = health - damageToTake;
                //if(!isServer)
                ApplyStatus(StatusType.Block, -usedBlock);
            }
            else if (shieldThisTurn == 0 && damageToTake > 0)
            {
                health = health - damageToTake;
            }

            Debug.Log("damage blocked by " + (damageRecieved - damageToTake));
            OnHealthChanged?.Invoke(health, maxHealth);
            if (gameObject.name == "Health Pool")
                GameObject.FindFirstObjectByType<HealthBar>().GetHealthChange(health, maxHealth);
            if (GetComponent<EnemyGenericBody>() != null && health <=0)
            {
                TurnManager.instance.CheckWinCondition();
                Debug.Log("enemy died!");
                GetComponent<Collider>().enabled = false;
                TurnManager.instance.CurrentEnemiesList.Remove(GetComponent<EnemyGenericBody>());
                TurnManager.instance.enemyTeam.Remove(GetComponent<CardEnemyController>());
                var GB = GetComponent<EnemyGenericBody>();
                GB.gameObject.SetActive(false);
            }
        }

        //[Command(requiresAuthority = false)]
        public void SetAllStatus()
        {
            SetAllStatusRPC();
        }

        //[ClientRpc]
        public void SetAllStatusRPC()
        {
            for (int i = 0; i < Enum.GetNames(typeof(StatusType)).Length; i++)
                StatusDict.Add((StatusType)i, new StatusStats((StatusType)i, 0));

            StatusDict[StatusType.Bleed].DecreaseOverTurn = true;
            StatusDict[StatusType.Bleed].OnTriggerAction += DamageBleed;

            StatusDict[StatusType.NagaPoison].DecreaseOverTurn = false;
            StatusDict[StatusType.NagaPoison].OnTriggerAction += NagaPoison;

            StatusDict[StatusType.Burn].DecreaseOverTurn = true;
            StatusDict[StatusType.Burn].OnTriggerAction += DamageBurn;

            StatusDict[StatusType.Block].ClearAtNextTurn = true;
            StatusDict[StatusType.TempStr].ClearAtNextTurn = true;
            StatusDict[StatusType.TempDex].ClearAtNextTurn = true;

            StatusDict[StatusType.Strength].CanNegativeStack = true;
            StatusDict[StatusType.Dexterity].CanNegativeStack = true;

            StatusDict[StatusType.Stun].DecreaseOverTurn = true;
            StatusDict[StatusType.Stun].OnTriggerAction += CheckStunStatus;

            StatusDict[StatusType.Vulnerable].DecreaseOverTurn = true;

            StatusDict[StatusType.Frozen].DecreaseOverTurn = true;

            StatusDict[StatusType.Regen].DecreaseOverTurn = true;
            StatusDict[StatusType.Regen].OnTriggerAction += RegenHeal;

            StatusDict[StatusType.TempDex].OnTriggerAction += TempDex;
            StatusDict[StatusType.TempStr].OnTriggerAction += TempStr;
        }

        //[Command(requiresAuthority = false)]
        public void ApplyStatus(StatusType targetStatus, int value)
        {
            ApplyStatusRPC(targetStatus, value);
        }

        //[ClientRpc]
        public void ApplyStatusRPC(StatusType targetStatus, int value)
        {
            //if(targetStatus == StatusType.Block)
            if (StatusDict[targetStatus].IsActive)
            {
                StatusDict[targetStatus].StatusValue += value;
                if (targetStatus != StatusType.NagaPoison)
                {
                    OnStatusChanged?.Invoke(targetStatus, StatusDict[targetStatus].StatusValue);
                }
                Debug.Log("Status Increased " + targetStatus);
            }
            else
            {
                StatusDict[targetStatus].StatusValue = value;
                StatusDict[targetStatus].IsActive = true;
                if (targetStatus != StatusType.NagaPoison)
                {
                    OnStatusApplied?.Invoke(targetStatus, StatusDict[targetStatus].StatusValue);
                }
                Debug.Log("Status Active: " + targetStatus);
            }

            if (StatusDict[targetStatus].StatusValue == 0)
            {
                ClearStatus(targetStatus);
            }
        }

        //[Command(requiresAuthority = false)]
        public void HealDamage(int damageRecieved)
        {
            HealDamageRPC(damageRecieved);
        }

        //[ClientRpc]
        public void HealDamageRPC(int healRecieved)
        {
            health += healRecieved;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            OnHealthChanged?.Invoke(health, maxHealth);
            if (gameObject.name == "Health Pool")
                GameObject.FindFirstObjectByType<HealthBar>().GetHealthChange(health, maxHealth);
        }

        public void OnTurnStart()
        {
            TriggerStatus(StatusType.TempDex);
            TriggerStatus(StatusType.TempStr);
            TriggerStatus(StatusType.Block);
            TriggerStatus(StatusType.Bleed);
            TriggerStatus(StatusType.NagaPoison);
            TriggerStatus(StatusType.Burn);
            TriggerStatus(StatusType.Regen);
            TriggerStatus(StatusType.Frozen);
            TriggerStatus(StatusType.Vulnerable);
        }

        public void TriggerStatus(StatusType targetStatus)
        {
            StatusDict[targetStatus].OnTriggerAction?.Invoke();

            //One turn only statuses
            if (StatusDict[targetStatus].ClearAtNextTurn)
            {
                ClearStatus(targetStatus);
                OnStatusChanged?.Invoke(targetStatus, StatusDict[targetStatus].StatusValue);
                Debug.Log(targetStatus + " changed on " + gameObject.name);
                return;
            }

            //Check status
            if (StatusDict[targetStatus].StatusValue <= 0)
            {
                if (StatusDict[targetStatus].CanNegativeStack)
                {
                    if (StatusDict[targetStatus].StatusValue == 0 && !StatusDict[targetStatus].IsPermanent)
                        ClearStatus(targetStatus);
                }
                else
                {
                    if (!StatusDict[targetStatus].IsPermanent)
                        ClearStatus(targetStatus);
                }
            }

            if (StatusDict[targetStatus].DecreaseOverTurn)
                StatusDict[targetStatus].StatusValue--;

            if (StatusDict[targetStatus].StatusValue == 0)
                if (!StatusDict[targetStatus].IsPermanent)
                    ClearStatus(targetStatus);

            OnStatusChanged?.Invoke(targetStatus, StatusDict[targetStatus].StatusValue);
        }

        public void DamageBleed()
        {
            if (StatusDict[StatusType.Bleed].StatusValue <= 0) return;
            TakeDamage(StatusDict[StatusType.Bleed].StatusValue);
        }

        public void DamageBurn()
        {
            if (StatusDict[StatusType.Burn].StatusValue <= 0) return;
            TakeDamage(StatusDict[StatusType.Burn].StatusValue);
        }

        public void RegenHeal()
        {
            if (StatusDict[StatusType.Regen].StatusValue <= 0) return;
            HealDamage(StatusDict[StatusType.Regen].StatusValue);
        }

        public void TempDex()
        {
            if (StatusDict[StatusType.TempDex].StatusValue <= 0) return;
            ApplyStatus(StatusType.Dexterity, -StatusDict[StatusType.TempDex].StatusValue);
        }

        public void TempStr()
        {
            if (StatusDict[StatusType.TempStr].StatusValue <= 0) return;
            ApplyStatus(StatusType.Strength, -StatusDict[StatusType.TempStr].StatusValue);
        }

        public void NagaPoison()
        {
            if (StatusDict[StatusType.NagaPoison].StatusValue <= 0) return;
            if (StatusDict[StatusType.NagaPoison].StatusValue < StatusDict[StatusType.Bleed].StatusValue) return;
            int difference = StatusDict[StatusType.NagaPoison].StatusValue - StatusDict[StatusType.Bleed].StatusValue;
            ApplyStatus(StatusType.Bleed, difference);
        }

        public void CheckStunStatus()
        {
            if (StatusDict[StatusType.Stun].StatusValue <= 0)
            {
                IsStunned = false;
                return;
            }

            IsStunned = true;
        }

        public void ClearAllStatus()
        {
            foreach (var status in StatusDict)
                ClearStatus(status.Key);
        }

        public void ClearStatus(StatusType targetStatus)
        {
            StatusDict[targetStatus].IsActive = false;
            StatusDict[targetStatus].StatusValue = 0;
            OnStatusCleared?.Invoke(targetStatus);
        }

        public CharacterType GetCharacterType()
        {
            return CharacterType;
        }

        public GenericBody GetCharacterBase()
        {
            return this;
        }
    }
}
