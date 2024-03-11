using CardActions;
using DeckData;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardRewards : MonoBehaviour
{
    [Header("Rarity Pools")]
    [SerializeField] private CardRarieties commonCards;
    [SerializeField] private CardRarieties uncommonCards;
    [SerializeField] private CardRarieties rareCards;

    [Header("Rarity Chances")]
    [Header("Make sure numbers add to 100")]
    [SerializeField] private int commonChance;
    [SerializeField] private int uncommonChance;
    [SerializeField] private int rareChance;

    [Header("Misc")]
    [SerializeField] private List<GameObject> spawnLocations;
    [SerializeField] private GameObject cardPrefab;
    List<CardData> CardRewardList;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CardGeneration()
    {
        int amountOfCards = 0;

        for (int i = 0; i < 3; i++)
        {
            var number = Random.Range(1, 101);

            if (number < commonChance)
            {
                var Card = Random.Range(0, commonCards.Cards.Count - 1);
                CardRewardList.Add(commonCards.Cards[Card]);
            }
            else if (number < commonChance + uncommonChance && number > commonChance)
            {
                var Card = Random.Range(0, uncommonCards.Cards.Count - 1);
                CardRewardList.Add(uncommonCards.Cards[Card]);
            }
            else if (number > commonChance + uncommonChance)
            {
                var Card = Random.Range(0, rareCards.Cards.Count - 1);
                CardRewardList.Add(rareCards.Cards[Card]);
            }
        }

        foreach (var Card in CardRewardList)
        {
            var n = Instantiate(cardPrefab, spawnLocations[amountOfCards].transform);
            n.GetComponent<CardBase>().CardData = Card;
            amountOfCards++;
        }
    }
}
