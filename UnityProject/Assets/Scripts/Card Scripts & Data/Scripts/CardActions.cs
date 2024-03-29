using DeckData;
using Enums;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;
using Managers;
//Claire <3


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

            var value = Int32.Parse(actionParameters.Value) + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

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

            var value = Int32.Parse(actionParameters.Value) + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

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

            var value = Int32.Parse(actionParameters.Value) + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

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

    public class EnemyFireAttackAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyFireAttack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = Int32.Parse(actionParameters.Value) + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

            if (targetCharacter.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value * 2);
                targetCharacter.ClearStatus(StatusType.Frozen);
            }

            if (actionParameters.SelfCharacter.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
        }
    }

    public class EnemyFrostAttackAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyFrostAttack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = Int32.Parse(actionParameters.Value) + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

            if (targetCharacter.StatusDict[StatusType.Burn].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value * 2);
                targetCharacter.ClearStatus(StatusType.Burn);
            }
            if (actionParameters.SelfCharacter.StatusDict[StatusType.Frozen].StatusValue >= 1)
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
            newTarget.ApplyStatus(StatusType.Block, Mathf.RoundToInt(Int32.Parse(actionParameters.Value) + selfCharacter.StatusDict[StatusType.Dexterity].StatusValue));
        }
    }

    public class DrawAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Draw;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var deck = actionParameters.TargetCharacter.GetComponent<CardDeck>();

            deck.ServerDrawCard(Int32.Parse(actionParameters.Value));
        }
    }

    public class GainEnergyAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EarnEnergy;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var deck = actionParameters.TargetCharacter.GetComponent<CardPlayerController>();

            deck.currentEnergy += (Int32.Parse(actionParameters.Value));
        }
    }


    public class HealAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Heal;

        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.HealthPool;

            if (!newTarget) return;

            newTarget.HealDamage(Mathf.FloorToInt(Int32.Parse(actionParameters.Value)));
        }
    }

    public class RegenAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Regen;

        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.HealthPool;

            if (!newTarget) return;

            newTarget.ApplyStatus(StatusType.Regen, Mathf.RoundToInt(Int32.Parse(actionParameters.Value)));
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
            var value = Int32.Parse(actionParameters.Value);

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
            var value = Int32.Parse(actionParameters.Value);

            targetCharacter.ApplyStatus(StatusType.Frozen, Mathf.RoundToInt(value));
        }
    }

    //used for cards the generate temporary cards
    public class TokenCardAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.SpawnCards;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if(actionParameters.Value != "Steal")
            {
                actionParameters.SelfCharacter.GetComponent<CardPlayerController>().deck.hand.Add(Resources.Load<CardData>("CardData/Data/Token Cards/" + actionParameters.Value));
            }
            else
            {
                CardData stolenCard = GameObject.Instantiate(ScriptableObject.CreateInstance<CardData>());
                var enemyCard = actionParameters.TargetCharacter.GetComponent<CardEnemyController>().deck.hand[0];
                //enemyCard = actionParameters.TargetCharacter.GetComponent<CardEnemyController>().deck.hand[0];



                stolenCard.cardName = "Stolen " + enemyCard.cardName;
                stolenCard.deleteAfterDiscard = true;
                stolenCard.deleteAfterPlay = true;

                //change enemyCard to effect enemies

                stolenCard.cardActionDataList = enemyCard.CardActionDataList;
                stolenCard.cardDescriptionDataList = enemyCard.CardDescriptionDataList;

                foreach (CardActionData actionData in stolenCard.cardActionDataList)
                {
                    //action target type
                    if(actionData.ActionTargetType == ActionTargetType.Enemy || actionData.ActionTargetType == ActionTargetType.AllEnemies)
                        actionData.actionTargetType = ActionTargetType.HealthPool;
                    if (actionData.ActionTargetType == ActionTargetType.HealthPool)
                        actionData.actionTargetType = ActionTargetType.Enemy;
                    //card action type swap
                    if (actionData.CardActionType == CardActionType.EnemyAttack)
                        actionData.cardActionType = CardActionType.Attack;
                    if (actionData.CardActionType == CardActionType.EnemyBleed)
                        actionData.cardActionType = CardActionType.Bleed;
                    if (actionData.CardActionType == CardActionType.EnemyRegen)
                        actionData.cardActionType = CardActionType.Regen;
                    if (actionData.CardActionType == CardActionType.EnemyHeal)
                        actionData.cardActionType = CardActionType.Heal;
                }
                actionParameters.SelfCharacter.GetComponent<CardPlayerController>().deck.hand.Add(stolenCard);
                actionParameters.TargetCharacter.GetComponent<CardEnemyController>().deck.ServerDiscard(1, new int[] { 0 });

                GameObject.Destroy(actionParameters.TargetCharacter.GetComponent<CardEnemyController>().currentDisplay);
            }
            actionParameters.SelfCharacter.GetComponent<HandManager>().RefreshHand();
        }
    }

    public class ApplyStatusAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.ApplyStatus;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //use regex split to use String Number for applying a status
            var split = Regex.Split(actionParameters.Value, " ");
            if(split.Length < 2)
            {
                Debug.LogError("No value supplied for ApplyStatus Action on card " + actionParameters.CardData.name + "!");
                return;
            }
            actionParameters.TargetCharacter.ApplyStatus((StatusType)System.Enum.Parse(typeof(StatusType), split[0]), Mathf.RoundToInt(Int32.Parse(split[1])));
        }
    }

    public class CreateFieldCard : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.CreateFieldCard;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //tell the FieldCardHolder add a card to its display list.
            FieldCardData fieldCard = Resources.Load<FieldCardData>("FieldCardData/" + actionParameters.Value);
            bool playerOwned = false;
            if(actionParameters.SelfCharacter.CharacterType == CharacterType.P1 || actionParameters.SelfCharacter.CharacterType == CharacterType.P2)
                playerOwned = true;
    
            //field card owner team is set to actionParameters.SelfCharacter.CharacterType
            FieldCardHolder.instance.AddFieldCard(fieldCard, playerOwned);
        }
    }



    public class VulnerableAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Vulnerable;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var value = Int32.Parse(actionParameters.Value);

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

            var value = Int32.Parse(actionParameters.Value);

            if (actionParameters.HealthPool.StatusDict[StatusType.Frozen].StatusValue >= 1)
            {
                value = Mathf.CeilToInt(value / 2);
            }

            targetCharacter.TakeDamage(value);
        }
    }

    public class AttackFive : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.AttackFive;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            var value = Int32.Parse(actionParameters.Value) + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

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

            var value = Int32.Parse(actionParameters.Value) + selfCharacter.StatusDict[StatusType.Strength].StatusValue;

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

            newTarget.ApplyStatus(StatusType.Block, Mathf.RoundToInt(Int32.Parse(actionParameters.Value) + actionParameters.SelfCharacter.StatusDict[StatusType.Dexterity].StatusValue));
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

            newTarget.HealDamage(Mathf.FloorToInt(Int32.Parse(actionParameters.Value)));
        }
    }

    public class EnemyVulnAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyVulnerability;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            targetCharacter.ApplyStatus(StatusType.Vulnerable, Mathf.RoundToInt(Int32.Parse(actionParameters.Value)));
        }
    }

    public class EnemyBleedAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyBleed;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            targetCharacter.ApplyStatus(StatusType.Bleed, Mathf.RoundToInt(Int32.Parse(actionParameters.Value)));
        }
    }

    public class EnemyRegenAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyRegen;

        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.SelfCharacter;

            if (!newTarget) return;

            newTarget.ApplyStatus(StatusType.Regen, Mathf.RoundToInt(Int32.Parse(actionParameters.Value)));
        }
    }

    public class EnemyFreezeAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyFreeze;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            targetCharacter.ApplyStatus(StatusType.Frozen, Mathf.RoundToInt(Int32.Parse(actionParameters.Value)));
        }
    }

    public class EnemyBurnAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EnemyBurn;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var targetCharacter = actionParameters.HealthPool;

            if (!targetCharacter) return;

            targetCharacter.ApplyStatus(StatusType.Burn, Mathf.RoundToInt(Int32.Parse(actionParameters.Value)));
        }
    }


    #region Actions that are really more for items rather than cards
    public class ModifySharedStat : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.ChangeSharedStat;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //use regex split to use String Number for applying a status
            var split = Regex.Split(actionParameters.Value, " ");
            if (split.Length < 2)
            {
                Debug.LogError("No value supplied for ModifySharedStat Action on card " + actionParameters.CardData.name + "!");
                return;
            }
            RunManager.instance.ChangeSharedStat(split[0], Int32.Parse(split[1]));
        }
    }

    public class ModifyIndividualStat : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.ChangeIndividualStat;
        public override void DoAction(CardActionParameters actionParameters)
        {
            //use regex split to use String Number for applying a status
            var split = Regex.Split(actionParameters.Value, " ");
            if (split.Length < 2)
            {
                Debug.LogError("No value supplied for ModifyIndividualStat Action on card " + actionParameters.CardData.name + "!");
                return;
            }

            if (actionParameters.TargetCharacter.CharacterType == CharacterType.P1)
                RunManager.instance.ChangeStat(0, split[0], Int32.Parse(split[1]));
            else if (actionParameters.TargetCharacter.CharacterType == CharacterType.P2)
                RunManager.instance.ChangeStat(1, split[0], Int32.Parse(split[1]));
            else if (actionParameters.TargetCharacter.CharacterType == CharacterType.Ally)
            {
                RunManager.instance.ChangeStat(0, split[0], Int32.Parse(split[1]));
                RunManager.instance.ChangeStat(1, split[0], Int32.Parse(split[1]));
            }
            else
                Debug.LogError("Card action ModifyIndividualStat can only be used on cards that target players!");
        }
    }
    #endregion
}