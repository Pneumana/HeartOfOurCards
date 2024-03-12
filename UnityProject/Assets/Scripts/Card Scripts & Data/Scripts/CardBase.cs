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
        //[SerializeField] protected Transform descriptionRoot;
        [SerializeField] protected Image cardImage;
        //[SerializeField] protected Image passiveImage;
        [SerializeField] protected TextMeshPro nameTextField;
        [SerializeField] protected TextMeshPro descTextField;
        [SerializeField] protected TextMeshPro manaTextField;


        public CardData CardData { get; set; }
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
        private void Start()
        {
            SetCard(CardData);
            UpdateCardText();
        }

        public virtual void SetCard(CardData targetProfile, bool isPlayable = true)
        {
            CardData = targetProfile;
            IsPlayable = isPlayable;
            nameTextField.text = CardData.CardName;
            descTextField.text = CardData.MyDescription;
            manaTextField.text = CardData.EnergyCost.ToString();
            //cardImage.sprite = CardData.CardSprite;
        }

        public virtual void Use(GenericBody self, GenericBody targetCharacter, List<EnemyGenericBody> allEnemies, List<GenericBody> allAllies, GenericBody healthPool, RunManager.PlayerStats playerStats = new RunManager.PlayerStats())
        {
            //Debug.Log(CardData.CardName + " was played"); <---- fuck this one in particular
            if (!IsPlayable) return;

            StartCoroutine(CardUseRoutine(self, targetCharacter, allEnemies, allAllies, healthPool));
        }

        private IEnumerator CardUseRoutine(GenericBody self, GenericBody targetCharacter, List<EnemyGenericBody> allEnemies, List<GenericBody> allAllies, GenericBody healthPool)
        {
            SpendEnergy(CardData.EnergyCost);

            foreach (var playerAction in CardData.CardActionDataList)
            {
                float actionTime = 0;
                do
                {
                    actionTime += Time.deltaTime;
                    //Debug.Log("player action " + playerAction.CardActionType);
                    
                    var targetList = DetermineTargets(targetCharacter, allEnemies, allAllies, healthPool, playerAction);

                    foreach (var target in targetList)
                    {
                        //Debug.Log("on target " + target.name);
                        CardActionProcessor.GetAction(playerAction.CardActionType).DoAction(new CardActionParameters(playerAction.ActionValue, target, self, healthPool, CardData, this));
                    }
                    //yield return new WaitForSeconds(0);
                } while (actionTime < playerAction.ActionDelay);
                
                    
            }
            yield return null;
            /* Add something here to check for exhaust*/
        }

        private static List<GenericBody> DetermineTargets(GenericBody targetCharacter, List<EnemyGenericBody> allEnemies, List<GenericBody> allAllies, GenericBody healthPool, CardActionData playerAction)
        {
            List<GenericBody> targetList = new List<GenericBody>();
            switch (playerAction.ActionTargetType)
            {
                case ActionTargetType.Enemy:
                    targetList.Add(targetCharacter);
                    break;
                case ActionTargetType.Ally:
                    targetList.Add(targetCharacter);
                    break;
                case ActionTargetType.AllEnemies:
                    foreach (var enemyBase in allEnemies)
                        targetList.Add(enemyBase);
                    break;
                case ActionTargetType.AllAllies:
                    foreach (var ally in allAllies)
                        targetList.Add(ally);
                    break;
                case ActionTargetType.RandomEnemy:
                    if (allEnemies.Count > 0)
                        targetList.Add(allEnemies[Random.Range(0, allEnemies.Count + 1)]);
                    break;
                case ActionTargetType.RandomAlly:
                    if (allAllies.Count > 0)
                        targetList.Add(allAllies[Random.Range(0, allAllies.Count + 1)]);
                    break;
                case ActionTargetType.TwoRandomEnemies:
                    if (allEnemies.Count > 0)
                    {
                        targetList.Add(allEnemies[Random.Range(0, allEnemies.Count + 1)]);
                        targetList.Add(allEnemies[Random.Range(0, allEnemies.Count + 1)]);
                    }
                    break;
                case ActionTargetType.HealthPool:
                        targetList.Add(healthPool);
                    break;
                default:
                    Debug.LogError("womp womp");
                    break;
            }
            //Debug.Log(targetList.Count);
            return targetList;
        }

        protected virtual void SpendEnergy(int value)
        {
            if (!IsPlayable) return;
        }

        public virtual void UpdateCardText()
        {
            CardData.UpdateDescription(GetComponentInParent<GenericBody>());
            nameTextField.text = CardData.CardName;
            descTextField.text = CardData.MyDescription;
            manaTextField.text = CardData.EnergyCost.ToString();
        }
    }
}