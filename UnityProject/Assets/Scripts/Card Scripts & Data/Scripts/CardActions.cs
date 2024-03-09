using Enums;
using System.Xml;
using UnityEngine;
using UnityEngine.VFX;


namespace CardActions
{
    public class AttackAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Attack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = actionParameters.Value + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

            if (actionParameters.HealthPool.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
        }
    }

    public class AttackTwiceAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.AttackTwice;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = actionParameters.Value + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

            if (actionParameters.HealthPool.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
        }
    }

    public class FireAttackAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.FireAttack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = actionParameters.Value + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

            if (targetCharacter.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value * 2);
                targetCharacter.ClearStatus(StatusType.Frozen);
            }

            if (actionParameters.HealthPool.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
        }
    }
    public class FrostAttackAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.FrostAttack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = actionParameters.Value + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

            if (targetCharacter.StatusDict[StatusType.Burn].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value * 2);
                targetCharacter.ClearStatus(StatusType.Burn);
            }

            if (actionParameters.HealthPool.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
        }
    }
    public class AllyBlockAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.AllyBlock;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //Debug.Log("trying to blovk");
            var newTarget = actionParameters.HealthPool;
            var selfCharacter = actionParameters.SelfCharacter;

            if (!newTarget) return;
            //Debug.Log("Block Successful");
            newTarget.ApplyStatus(StatusType.Block, Mathf.RoundToInt(actionParameters.Value + selfCharacter.StatusDict[StatusType.Dexterity].StatusValue));
        }
    }

    public class DrawAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Draw;
        public override void DoAction(CardActionParameters actionParameters)
        {

            //if (CollectionManager != null)
            //    CollectionManager.DrawCards(Mathf.RoundToInt(actionParameters.Value));
            //else
            //    Debug.LogError("There is no CollectionManager");
        }
    }

    public class GainEnergyAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EarnEnergy;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //if (CombatManager != null)
            //    CombatManager.GainEnergy(Mathf.RoundToInt(actionParameters.Value));
            //else
            //    Debug.LogError("There is no CombatManager");
        }
    }

    public class HealAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Heal;

        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.HealthPool;

            if (!newTarget) return;

            newTarget.HealDamage(Mathf.FloorToInt(actionParameters.Value));
        }
    }

    public class RegenAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Regen;

        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.HealthPool;

            if (!newTarget) return;

            newTarget.ApplyStatus(StatusType.Regen, Mathf.RoundToInt(actionParameters.Value));
        }
    }

    public class IncreaseStrengthAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.IncreaseStrength;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var newtarget = actionParameters.TargetCharacter
                ? actionParameters.TargetCharacter
                : actionParameters.SelfCharacter;

            if (!newtarget) return;

            newtarget.ApplyStatus(StatusType.Strength, Mathf.RoundToInt(actionParameters.Value));
        }
    }

    public class StunAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Stun;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //if (!actionParameters.TargetCharacter) return;

            //var value = actionParameters.Value;
            //actionParameters.TargetCharacter.CharacterStats.ApplyStatus(StatusType.Stun, Mathf.RoundToInt(value));
        }
    }

    public class BurnAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Burn;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var value = actionParameters.Value;

            targetCharacter.ApplyStatus(StatusType.Burn, Mathf.RoundToInt(value));
        }
    }

    public class FrostAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Frost;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var value = actionParameters.Value;

            targetCharacter.ApplyStatus(StatusType.Frozen, Mathf.RoundToInt(value));
        }
    }

    public class TokenCardAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.SpawnCards;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //if (CollectionManager != null)
            //    CollectionManager.AddCards(Mathf.RoundToInt(actionParameters.Value));
            //else
            //    Debug.LogError("There is no CollectionManager");
        }
    }

    public class VulnerableAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Vulnerable;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var value = actionParameters.Value;

            targetCharacter.ApplyStatus(StatusType.Vulnerable, Mathf.RoundToInt(value));
        }
    }

    public class AttackScaleNothing : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.AttackScaleNothing;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = actionParameters.Value;

            if (actionParameters.HealthPool.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
        }
    }

    public class AttackFiveAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Attack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = actionParameters.Value + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

            if (actionParameters.HealthPool.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
            targetCharacter.TakeDamage(value);
            targetCharacter.TakeDamage(value);
            targetCharacter.TakeDamage(value);
            targetCharacter.TakeDamage(value);
        }
    }

    public class EnemyAttackAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyAttack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            var selfCharacter = actionParameters.SelfCharacter;

            var value = actionParameters.Value + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

            if (selfCharacter.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
        }
    }

    public class EnemyBlockAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyBlock;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.TargetCharacter
                ? actionParameters.TargetCharacter
                : actionParameters.SelfCharacter;

            if (!newTarget) return;

            newTarget.ApplyStatus(StatusType.Block, Mathf.RoundToInt(actionParameters.Value + actionParameters.SelfCharacter.StatusDict[StatusType.Dexterity].StatusValue));
        }
    }

    public class EnemyHealAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyHeal;

        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.TargetCharacter
                ? actionParameters.TargetCharacter
                : actionParameters.SelfCharacter;

            if (!newTarget) return;

            newTarget.HealDamage(Mathf.FloorToInt(actionParameters.Value));
        }
    }

    public class EnemyVulnAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyVulnerability;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            targetCharacter.ApplyStatus(StatusType.Vulnerable, Mathf.RoundToInt(actionParameters.Value));
        }
    }

    public class EnemyBleedAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyBleed;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            targetCharacter.ApplyStatus(StatusType.Bleed, Mathf.RoundToInt(actionParameters.Value));
        }
    }

    public class EnemyRegenAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyRegen;

        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.SelfCharacter;

            if (!newTarget) return;

            newTarget.ApplyStatus(StatusType.Regen, Mathf.RoundToInt(actionParameters.Value));
        }
    }

    public class EnemyFreezeAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyFreeze;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            targetCharacter.ApplyStatus(StatusType.Frozen, Mathf.RoundToInt(actionParameters.Value));
        }
    }

    public class EnemyBurnAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyBurn;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            targetCharacter.ApplyStatus(StatusType.Burn, Mathf.RoundToInt(actionParameters.Value));
        }
    }
}