using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{
    public string cardTitle;
    public CardDescription cardDescription;
    public CardAmount cardCost;
    public CardAmount cardEffect;
    public CardAmount buffAmount;
    public Sprite cardIcon;
    public CardType cardType;
    public enum CardType { Attack, Skill, Power }
    public CardClass cardClass;
    public enum CardClass { normal, monster, heartbreak }
    public CardTargetType cardTargetType;
    public enum CardTargetType { self, enemy };

    public int GetCardCostAmount()
    {
        return cardCost.baseAmount;
    }
    public int GetCardEffectAmount()
    {
        return cardEffect.baseAmount;
    }
    public string GetCardDescriptionAmount()
    {
        return cardDescription.baseAmount;
    }
    public int GetBuffAmount()
    {
        return buffAmount.baseAmount;
    }
}

    [System.Serializable]
    public struct CardAmount
    {
        public int baseAmount;
    }   
    [System.Serializable]
    public struct CardDescription
    {
        public string baseAmount;
    }
    [System.Serializable]
    public struct CardBuffs
    {
        public Buffs.type buffType;
        public CardAmount buffAmount;
    }

