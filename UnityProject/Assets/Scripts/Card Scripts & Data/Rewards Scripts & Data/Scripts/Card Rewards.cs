using CardActions;
using DeckData;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardRewards : NetworkBehaviour
{
    [Header("Rarity Pools")]
    [SerializeField] private CardRarieties commonCards;
    [SerializeField] private CardRarieties uncommonCards;
    [SerializeField] private CardRarieties rareCards;

    [Header("Rarity Chances. Make sure numbers add to 100")]
    [SerializeField] private int commonChance;
    [SerializeField] private int uncommonChance;
    [SerializeField] private int rareChance;

    [Header("Misc")]
    [SerializeField] private List<GameObject> spawnLocations;
    [SerializeField] private GameObject cardPrefab;
    List<GameObject> CardRewardList = new List<GameObject>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void CMDCardGeneration()
    {
        if (RunManager.instance.pickingPlayer == RunManager.instance.LocalPlayerID) return;
        for (int i = 0; i < 3; i++)
        {
            var roll = Random.Range(1, 101);
            var roll2 = Random.Range(0, commonCards.Cards.Count + 1);
            var roll3 = Random.Range(0, uncommonCards.Cards.Count + 1);
            var roll4 = Random.Range(0, rareCards.Cards.Count + 1);
            CardGeneration(roll, roll2, roll3, roll4, i);
        }
    }

    [ClientRpc]
    private void CardGeneration(int roll, int roll2, int roll3, int roll4, int i)
    {
        var CardRewardListTemp = new List<CardData>();

        if (roll <= commonChance)
        {
            CardRewardListTemp.Add(commonCards.Cards[roll2]);
        }
        else if (roll <= commonChance + uncommonChance && roll > commonChance)
        {
            CardRewardListTemp.Add(uncommonCards.Cards[roll3]);
        }
        else if (roll > commonChance + uncommonChance)
        {
            CardRewardListTemp.Add(rareCards.Cards[roll4]);
        }

        foreach (var Card in CardRewardListTemp)
        {
            var n = Instantiate(cardPrefab, spawnLocations[i].transform);
            CardRewardList.Add(n);
            n.transform.forward = -n.transform.forward;
            n.GetComponent<CardBase>().CardData = Card;
        }
    }

    [Command(requiresAuthority = false)]
    public void CMDCardDestroy()
    {
        CardDestroy();
    }

    [ClientRpc]
    private void CardDestroy()
    {
        foreach (GameObject card in CardRewardList)
        {
            Destroy(card);
        }
        CardRewardList.Clear();
    }

    [Command(requiresAuthority = false)]
    public void CMDAddCard(int number)
    {
        AddCard(number);
    }

    //[ClientRpc]
    //public void AddCard1()
    //{
    //    var card = CardRewardList[0];
    //    string name = card.GetComponent<CardBase>().CardData.CardName;
    //    AddCard(name);
    //}

    //[Command(requiresAuthority = false)]
    //public void CMDAddCard2()
    //{
    //    AddCard2();
    //}

    //[ClientRpc]
    //public void AddCard2()
    //{
    //    var card = CardRewardList[1];
    //    string name = card.GetComponent<CardBase>().CardData.CardName;
    //    AddCard(name);
    //}

    //[Command(requiresAuthority = false)]
    //public void CMDAddCard3()
    //{
    //    AddCard3();
    //}

    //[ClientRpc]
    //public void AddCard3()
    //{
    //    var card = CardRewardList[2];
    //    string name = card.GetComponent<CardBase>().CardData.CardName;
    //    AddCard(name);
    //}

    public void AddCard(int number)
    {
        var card = CardRewardList[number];
        string name = card.GetComponent<CardBase>().CardData.CardName;
        var load = Resources.Load<CardData>("CardData/" + name);

        if (NetworkServer.connections.Count > 1)
        {
            AmbidexterousManager.Instance.PlayerList[RunManager.instance.pickingPlayer].decks[0].Add(load);
        }
        else
        {
            AmbidexterousManager.Instance.PlayerList[0].decks[RunManager.instance.pickingPlayer].Add(load);
        }
    }
}
