using Enums;
using UnityEngine;
using UnityEngine.VFX;


namespace CardActions
{
    public class AttackAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Attack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //if (!actionParameters.TargetCharacter) return;

            //var targetCharacter = actionParameters.TargetCharacter;
            //var selfCharacter = actionParameters.SelfCharacter;

            //var value = actionParameters.Value + selfCharacter.CharacterStats.StatusDict[StatusType.Strength].StatusValue;

            //targetCharacter.CharacterStats.Damage(Mathf.RoundToInt(value)); 
        }
    }

    public class AllyBlockAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.AllyBlock;
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
            //var newTarget = actionParameters.TargetCharacter
            //    ? actionParameters.TargetCharacter
            //    : actionParameters.SelfCharacter;

            //if (!newTarget) return;

            //newTarget.CharacterStats.Heal(Mathf.RoundToInt(actionParameters.Value));
        }
    }

    public class RegenAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Regen;

        public override void DoAction(CardActionParameters actionParameters)
        {
            //var newTarget = actionParameters.TargetCharacter
            //    ? actionParameters.TargetCharacter
            //    : actionParameters.SelfCharacter;

            //if (!newTarget) return;

            //newTarget.CharacterStats.Heal(Mathf.RoundToInt(actionParameters.Value));
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
            //if (!actionParameters.TargetCharacter) return;

            //var value = actionParameters.Value;
            //actionParameters.TargetCharacter.CharacterStats.ApplyStatus(StatusType.Burn, Mathf.RoundToInt(value));
        }
    }

    public class FrostAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Frost;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //if (!actionParameters.TargetCharacter) return;

            //var value = actionParameters.Value;
            //actionParameters.TargetCharacter.CharacterStats.ApplyStatus(StatusType.Frost, Mathf.RoundToInt(value));
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
            //if (!actionParameters.TargetCharacter) return;

            //var value = actionParameters.Value;
            //actionParameters.TargetCharacter.CharacterStats.ApplyStatus(StatusType.Vulnerable, Mathf.RoundToInt(value));
        }
    }
}