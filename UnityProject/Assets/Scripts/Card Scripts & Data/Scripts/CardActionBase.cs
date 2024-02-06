using Characters;
using DeckData;
using Enums;


namespace CardActions
{
    public class CardActionParameters
    {
        public readonly int Value;
        public readonly GenericBody TargetCharacter;
        public readonly GenericBody SelfCharacter;
        public readonly CardData CardData;
        public readonly CardBase CardBase;
        public CardActionParameters(int value, GenericBody target, GenericBody self, CardData cardData, CardBase cardBase)
        {
            Value = value;
            TargetCharacter = target;
            SelfCharacter = self;
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
