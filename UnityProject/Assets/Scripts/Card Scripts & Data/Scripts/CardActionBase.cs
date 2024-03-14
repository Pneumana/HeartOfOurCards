using Characters;
using DeckData;
using Enums;
using UnityEngine;


namespace CardActions
{
    public class CardActionParameters
    {
        public readonly string Value;
        public readonly GenericBody TargetCharacter;
        public readonly GenericBody SelfCharacter;
        public readonly GenericBody HealthPool;
        public readonly CardData CardData;
        public readonly CardBase CardBase;
        public CardActionParameters(string value, GenericBody target, GenericBody self, GenericBody both, CardData cardData, CardBase cardBase)
        {
            Value = value;
            TargetCharacter = target;
            SelfCharacter = self;
            HealthPool = both;
            CardData = cardData;
            CardBase = cardBase;
        }
    }
    public abstract class CardActionBase
    {
        protected CardActionBase() { }
        public abstract CardActionType ActionType { get; }
        public abstract void DoAction(CardActionParameters actionParameters);
    }
}
