using CardActions;
using DeckData;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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
    public TextMeshProUGUI text;
    List<GameObject> CardRewardList = new List<GameObject>();
    List<CardData> CardDataList = new List<CardData>();
    private int cardsPicked = 0;
    private int picking;

    void Start()
    {
        picking = RunManager.instance.pickingPlayer;
        text.text = "Player " + (RunManager.instance.pickingPlayer + 1) + " is picking";
    }

    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void CMDCardGeneration()
    {
        //if (RunManager.instance.pickingPlayer == RunManager.instance.LocalPlayerID) return;
        if (CardRewardList.Count > 0) return;
        cardsPicked = 0;
        for (int i = 0; i < 3; i++)
        {
            var roll = Random.Range(1, 101);
            var roll2 = Random.Range(0, commonCards.Cards.Count);
            var roll3 = Random.Range(0, uncommonCards.Cards.Count);
            var roll4 = Random.Range(0, rareCards.Cards.Count);
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
            CardDataList.Add(Card);
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
        CardDataList.Clear();
    }

    [Command(requiresAuthority = false)]
    public void CMDAddCard(int number)
    {
        AddCard(number);
    }

    public void AddCard(int number)
    {
        cardsPicked++;
        var card = CardRewardList[number];
        var cardD = CardDataList[number];
        //string name = card.GetComponent<CardBase>().CardData.CardName;
        //var load = Resources.Load<CardData>("CardData/" + name);
        var decks = AmbidexterousManager.Instance.PlayerList[0].combatScene.transform.Find("Player" + (RunManager.instance.pickingPlayer + 1)).GetComponent<CardDeck>();


        if (NetworkServer.connections.Count > 1)
        {
            decks.deck.Add(cardD);
        }
        else
        {
            decks.deck.Add(cardD);
        }

        Destroy(card);
        //CardRewardList.Remove(card);

        if (cardsPicked == 2)
        {
            foreach (var cards in CardRewardList)
            {
                Destroy(cards);
            }
            CardRewardList.Clear();
            CardDataList.Clear();
            cardsPicked = 0;
        }
    }

    [Command(requiresAuthority = false)]
    public void CMDChangePickingPlayer()
    {
        ChangePickingPlayer();
    }

    [ClientRpc]
    void ChangePickingPlayer()
    {
        if (RunManager.instance.pickingPlayer == 0)
        {
            RunManager.instance.pickingPlayer = 1;
        }
        else
        {
            RunManager.instance.pickingPlayer = 0;
        }

        text.text = "Player " + (RunManager.instance.pickingPlayer + 1) + " is picking";
    }

    [Command(requiresAuthority = false)]
    public void CMDLeave()
    {
        Leave();
    }

    [ClientRpc]
    private void Leave()
    {
        RunManager.instance.pickingPlayer = picking;
    }
}
