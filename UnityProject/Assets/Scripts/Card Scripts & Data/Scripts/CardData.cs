using System;
using System.Collections.Generic;
using System.Text;
using Enums;
using Managers; 
using UnityEngine;
using Extentions;
using Characters;

namespace DeckData
{
    [CreateAssetMenu(fileName = "Card Data", menuName = "Card Stuff/Card Base", order = 0)]
    public class CardData : ScriptableObject
    {
        [Header("Card Profile")]
        [SerializeField] private string id;
        [SerializeField] private string cardName;
        [SerializeField] private int energyCost;
        [SerializeField] private Sprite cardSprite;
        [SerializeField] private List<CardType> cardType;

        [Header("Action Settings")]
        [SerializeField] private bool usableWithoutTarget;
        [SerializeField] private bool exhaustAfterPlay;
        [SerializeField] private List<CardActionData> cardActionDataList;

        [Header("Description")]
        [SerializeField] private List<CardDescriptionData> cardDescriptionDataList;
        [SerializeField] private List<SpecialKeywords> specialKeywordsList;

        public string Id => id;
        public bool UsableWithoutTarget => usableWithoutTarget;
        public int EnergyCost => energyCost;
        public string CardName => cardName;
        public Sprite CardSprite => cardSprite;
        public List<CardActionData> CardActionDataList => cardActionDataList;
        public List<CardDescriptionData> CardDescriptionDataList => cardDescriptionDataList;
        public List<SpecialKeywords> KeywordsList => specialKeywordsList;
        public string MyDescription { get; set; }
        public bool ExhaustAfterPlay => exhaustAfterPlay;
        public List<CardType> CardType => cardType;


        public void UpdateDescription(GenericBody GB)
        {
            var str = new StringBuilder();

            foreach (var descriptionData in cardDescriptionDataList)
            {
                str.Append(descriptionData.UseModifier
                    ? descriptionData.GetModifiedValue(this, GB)
                    : descriptionData.GetDescription());
            }

            //[STR]

            MyDescription = str.ToString();
        }
    }

    [Serializable]
    public class CardActionData
    {
        [SerializeField] private CardActionType cardActionType;
        [SerializeField] private ActionTargetType actionTargetType;
        [SerializeField] private int actionValue;
        [SerializeField] private float actionDelay;

        public ActionTargetType ActionTargetType => actionTargetType;
        public CardActionType CardActionType => cardActionType;
        public int ActionValue => actionValue;
        public float ActionDelay => actionDelay;
    }

    [Serializable]
    public class CardDescriptionData
    {
        [Header("Text")]
        [SerializeField] private string descriptionText;
        [SerializeField] private bool enableOverrideColor;
        [SerializeField] private Color overrideColor = Color.black;

        [Header("Modifer")]
        [SerializeField] private bool useModifier;
        [SerializeField] private int modifiedActionValueIndex;
        [SerializeField] private StatusType modiferStats;
        [SerializeField] private bool usePrefixOnModifiedValue;
        [SerializeField] private string modifiedValuePrefix = "*";
        [SerializeField] private bool overrideColorOnValueScaled;

        public string DescriptionText => descriptionText;
        public bool EnableOverrideColor => enableOverrideColor;
        public Color OverrideColor => overrideColor;
        public bool UseModifier => useModifier;
        public int ModifiedActionValueIndex => modifiedActionValueIndex;
        public StatusType ModiferStats => modiferStats;
        public bool UsePrefixOnModifiedValue => usePrefixOnModifiedValue;
        public string ModifiedValuePrefix => modifiedValuePrefix;
        public bool OverrideColorOnValueScaled => overrideColorOnValueScaled;

        private TurnManager turnManager => TurnManager.instance;
        //public GameObject StatCheck;

        public string GetDescription()
        {
            var str = new StringBuilder();

            str.Append(DescriptionText);

            if (EnableOverrideColor && !string.IsNullOrEmpty(str.ToString()))
                str.Replace(str.ToString(), ColorExtentions.ColorString(str.ToString(), OverrideColor));

            return str.ToString();
        }

        public string GetModifiedValue(CardData cardData, GenericBody GB)
        {
            if (cardData.CardActionDataList.Count <= 0) return "";

            if (ModifiedActionValueIndex >= cardData.CardActionDataList.Count)
                modifiedActionValueIndex = cardData.CardActionDataList.Count - 1;

            if (ModifiedActionValueIndex < 0)
                modifiedActionValueIndex = 0;

            var str = new StringBuilder();
            var value = cardData.CardActionDataList[ModifiedActionValueIndex].ActionValue;
            var modifer = 0;
            //GenericBody GB = StatCheck.GetComponentInParent<GenericBody>();

            if (GB)
            {
                modifer = GB.StatusDict[ModiferStats].StatusValue;
                value += modifer;

                if (modifer != 0)
                {
                    if (usePrefixOnModifiedValue)
                        str.Append(modifiedValuePrefix);
                }
            }

            str.Append(value);

            if (EnableOverrideColor)
            {
                if (OverrideColorOnValueScaled)
                {
                    if (modifer != 0)
                        str.Replace(str.ToString(), ColorExtentions.ColorString(str.ToString(), OverrideColor));
                }
                else
                {
                    str.Replace(str.ToString(), ColorExtentions.ColorString(str.ToString(), OverrideColor));
                }

            }

            return str.ToString();
        }
    }
}