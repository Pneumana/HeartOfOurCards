using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DeckData;
using Enums;

namespace CardActions
{
    public class CardBase : MonoBehaviour
    {
        [Header("Base References")]
        [SerializeField] protected Transform descriptionRoot;
        [SerializeField] protected Image cardImage;
        [SerializeField] protected Image passiveImage;
        [SerializeField] protected TextMeshProUGUI nameTextField;
        [SerializeField] protected TextMeshProUGUI descTextField;
        [SerializeField] protected TextMeshProUGUI manaTextField;


        public CardData CardData { get; private set; }
        public bool IsInactive { get; protected set; }
        protected Transform CachedTransform { get; set; }
        protected WaitForEndOfFrame CachedWaitFrame { get; set; }
        public bool IsPlayable { get; protected set; } = true;

        public bool IsExhausted { get; private set; }


        protected virtual void Awake()
        {
            CachedTransform = transform;
            CachedWaitFrame = new WaitForEndOfFrame();
        }

        public virtual void SetCard(CardData targetProfile, bool isPlayable = true)
        {
            CardData = targetProfile;
            IsPlayable = isPlayable;
            nameTextField.text = CardData.CardName;
            descTextField.text = CardData.MyDescription;
            manaTextField.text = CardData.EnergyCost.ToString();
            cardImage.sprite = CardData.CardSprite;
        }

        //public virtual void Use(PlayerGenericBody self, GenericBody targetCharacter, List<GenericBody> allEnemies, List<PlayerGenericBody> allAllies)
        //{
        //    if (!IsPlayable) return;

        //    StartCoroutine(CardUseRoutine(self, targetCharacter, allEnemies, allAllies));
        //}

        //private IEnumerator CardUseRoutine(PlayerGenericBody self, GenericBody targetCharacter, List<GenericBody> allEnemies, List<PlayerGenericBody> allAllies)
        //{
        //    SpendEnergy(CardData.EnergyCost);

        //    foreach (var playerAction in CardData.CardActionDataList)
        //    {
        //        yield return new WaitForSeconds(playerAction.ActionDelay);
        //        var targetList = DetermineTargets(targetCharacter, allEnemies, allAllies, playerAction);

        //        foreach (var target in targetList)
        //            CardActionProcessor.GetAction(playerAction.CardActionType).DoAction(new CardActionParameters (playerAction.ActionValue, target, self, CardData, this));
        //    }
        //    /* Add something here to check for exhaust*/
        //}

        //private static List<GenericBody> DetermineTargets(GenericBody targetCharacter, List<GenericBody> allEnemies, List<PlayerGenericBody> allAllies, CardActionData playerAction)
        //{
        //    List<GenericBody> targetList = new List<GenericBody>();
        //    switch (playerAction.ActionTargetType)
        //    {
        //        case ActionTargetType.Enemy:
        //            targetList.Add(targetCharacter);
        //            break;
        //        case ActionTargetType.Ally:
        //            targetList.Add(targetCharacter);
        //            break;
        //        case ActionTargetType.AllEnemies:
        //            foreach (var enemyBase in allEnemies)
        //                targetList.Add(enemyBase);
        //            break;
        //        case ActionTargetType.AllAllies:
        //            foreach (var allyBase in allAllies)
        //                targetList.Add(allyBase);
        //            break;
        //        case ActionTargetType.RandomEnemy:
        //            if (allEnemies.Count > 0)
        //                targetList.Add(allEnemies.RandomItem());

        //            break;
        //        case ActionTargetType.RandomAlly:
        //            if (allAllies.Count > 0)
        //                targetList.Add(allAllies.RandomItem());
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }

        //    return targetList;
        //}

        protected virtual void SpendEnergy(int value)
        {
            if (!IsPlayable) return;
        }

        public virtual void UpdateCardText()
        {
            CardData.UpdateDescription();
            nameTextField.text = CardData.CardName;
            descTextField.text = CardData.MyDescription;
            manaTextField.text = CardData.EnergyCost.ToString();
        }
    }
}
