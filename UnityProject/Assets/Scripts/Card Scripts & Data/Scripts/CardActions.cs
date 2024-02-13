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

            targetCharacter.TakeDamage(value);
        }
    }
    public class AllyBlockAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.AllyBlock;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.HealthPool;

            if (!newTarget) return;

            newTarget.ApplyStatus(StatusType.Block, Mathf.RoundToInt(actionParameters.Value + actionParameters.SelfCharacter.StatusDict[StatusType.Dexterity].StatusValue));
        }
    }
    public class EnemyBlockAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyBlock;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //var newTarget = actionParameters.TargetCharacter
            //    ? actionParameters.TargetCharacter
            //    : actionParameters.SelfCharacter;

            //if (!newTarget) return;

            //newTarget.CharacterStats.ApplyStatus(StatusType.Block, 
            //    Mathf.RoundToInt(actionParameters.Value + actionParameters.SelfCharacter.CharacterStats.StatusDict[StatusType.Dexterity].StatusValue));
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
            //var newTarget = actionParameters.TargetCharacter
            //    ? actionParameters.TargetCharacter
            //    : actionParameters.SelfCharacter;

            //if (!newTarget) return;

            //newTarget.CharacterStats.ApplyStatus(StatusType.Strength, Mathf.RoundToInt(actionParameters.Value));
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
}