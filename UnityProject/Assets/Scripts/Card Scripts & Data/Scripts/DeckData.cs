using DeckData;
using System.Collections.Generic;
using UnityEngine;

namespace DeckData
{
    [CreateAssetMenu(fileName = "Deck Data", menuName = "Card Stuff/Deck", order = 1)]
    public class DeckData : ScriptableObject
    {
        [SerializeField] private string deckId;
        [SerializeField] private string deckName;

        [SerializeField] private List<CardData> cardList;
        public List<CardData> CardList => cardList;

        public string DeckId => deckId;

        public string DeckName => deckName;
    }
}
