using DeckData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rewards Data", menuName = "Card Stuff/Card Rarieties", order = 0)]
public class CardRarieties : ScriptableObject
{
    [SerializeField] public List<CardData> Cards;
}
