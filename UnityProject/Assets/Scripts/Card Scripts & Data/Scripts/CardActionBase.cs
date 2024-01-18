using DeckData;
using Enums;
//using Characters;
//using Managers;
using UnityEngine.VFX;

namespace CardActions
{
    public class CardActionParameters
    {
        public readonly float Value;
        //public readonly CharacterBase TargetCharacter;
        //public readonly CharacterBase SelfCharacter;
        public readonly CardData CardData;
        //public readonly CardBase CardBase;
        public CardActionParameters(float value, /*CharacterBase target, CharacterBase self,*/ CardData cardData/*, CardBase cardBase*/)
        {
            Value = value;
            //TargetCharacter = target;
            //SelfCharacter = self;
            CardData = cardData;
            //CardBase = cardBase;
        }
    }
    public abstract class CardActionBase
    {
        protected CardActionBase() { }
        public abstract CardActionType ActionType { get; }
        public abstract void DoAction(CardActionParameters actionParameters);
    }
}
