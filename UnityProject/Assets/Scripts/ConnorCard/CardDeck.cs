using CardActions;
using Characters;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Managers;
using DeckData;
using static UnityEngine.GraphicsBuffer;

public class CardDeck : NetworkBehaviour
{
    public List<CardData> hand = new List<CardData>();
    public List<CardData> deck = new List<CardData> ();
    public List<CardData> discardPile = new List<CardData>();

    bool ownedByEnemy;
    //public List<CardBase> cardHand = new List<CardBase>();

    private HandManager hm;

    // Start is called before the first frame update
    void Start()
    {
        hm = GetComponent<HandManager>();
        if (GetComponent<CardEnemyController>() != null)
            ownedByEnemy = true;
    }

    // Update is called once per frame
    void Update()
    {
/*        if (Input.GetKeyDown(KeyCode.Q))
        {
            DrawCard();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayCard(transform.forward);
        }*/
    }
    [Command(requiresAuthority =false)]
    public void ServerDrawCard(int drawcount)
    {
        Debug.Log("drawing " + drawcount + " cards");
        int drawnCard = Random.Range(0, deck.Count - 1);
        Debug.Log(gameObject.name + " " + drawnCard + " was drawn from deck of " + (deck.Count-1));
        //deck.Remove(deck[drawnCard]);
        DrawCard(drawcount, drawnCard);
    }
    [ClientRpc]
    public void DrawCard(int drawcount, int drawnCard)
    {
        while(drawcount > 0)
        {
            if (deck.Count == 0)
            {
                if (discardPile.Count > 0)
                {
                    var hold = discardPile.Count;
                    //deck = hold;
                    for (int i = 0; i < hold; i++)
                    {
                        deck.Add(discardPile[i]);
                    }
                    //discardPile.Clear();
                    Debug.Log("shuffling deck");
                    discardPile.Clear();
                    drawcount++;
                }
                else
                {
                    Debug.Log("deck is empty");
                    //continue;
                }
            }
            else
            {
                //var drawnCard = Random.Range(0, deck.Count - 1);
                try
                {
                    if (drawnCard > deck.Count - 1)
                    {
                        drawnCard = deck.Count - 1;
                    }
                    hand.Add(deck[drawnCard]);
                    deck.Remove(deck[drawnCard]);
                    //Debug.Log(deck.Count);
                    //Debug.Log("Drew card " + hand[hand.Count - 1].CardName);
                }
                catch { Debug.LogWarning(drawnCard + " is greater than the remaining deck " + (deck.Count - 1)); }

            }
            drawcount--;
        }
        try
        {
            GetComponent<HandManager>().RefreshHand();
        }
        catch { }


    }
    [Command(requiresAuthority = false)]
    public void ServerDiscard(int numberOfCards, int[] specificCards)
    {
        Debug.Log("server wants to discard " + numberOfCards + ", " + specificCards.Length + " of which are specific");
        if(specificCards.Length > 0)
        {
            foreach(int card in specificCards)
            {
                ClientDiscard(card, 1);
            }
        }
        else
        {
            ClientDiscard(Random.Range(0, deck.Count - 1), numberOfCards);
        }
        //

    }

    [ClientRpc]
    void ClientDiscard(int discardedCardID, int discardCount)
    {
        /*if(discardedCardID)
        hand.Remove(deck[discardedCardID]);
        discardPile.Add(deck[discardedCardID]);
        try
        {
            GetComponent<HandManager>().RefreshHand();
        }
        catch { }*/
        Debug.Log("client discarding");
        while (discardCount > 0 && hand.Count > 0)
        {
                //var drawnCard = Random.Range(0, deck.Count - 1);
                try
                {
                    if (discardedCardID > hand.Count - 1)
                    {
                        discardedCardID = hand.Count - 1;
                    }

                if (!hand[discardedCardID].deleteAfterDiscard)
                    discardPile.Add(hand[discardedCardID]);
                Debug.Log(gameObject.name + "discarded " + hand[discardedCardID].CardName);
                hand.Remove(hand[discardedCardID]);
                //Debug.Log(deck.Count);

            }
                catch { Debug.LogWarning(discardedCardID + " is greater than the remaining hand " + (hand.Count - 1)); }
            discardCount--;
        }
        try
        {
            GetComponent<HandManager>().RefreshHand();
        }
        catch { }
    }
    [Command(requiresAuthority =false)]
    public void ServerPlayCard(uint netID, Vector3 target, int playedCard)
    {
        if (!isServer)
            return;
        Debug.Log("server recieved play request");
        if (playedCard == -1)
            playedCard = Random.Range(0, hand.Count - 1);


        //if(!ownedByEnemy)
            PlayCard(netID, target, playedCard);
/*        else
        {
            PlayEnemyCard(netID, target, playedCard);
        }*/
        //run play card on all clients, checking for netID match so the same player uses them
    }

    public void PlayEnemyCard(uint netID, Vector3 target, int playedCard)
    {

        if (netID == netId)
        {
            GameObject targetObj = null;
            float dist = float.MaxValue;
            foreach (GenericBody go in GameObject.FindObjectsByType<GenericBody>(FindObjectsSortMode.None))
            {
                var pos = go.gameObject.transform.position;
                var comp = Vector3.Distance(target, pos);
                if (comp < dist && go != gameObject)
                {
                    dist = comp;
                    targetObj = go.gameObject;
                }
            }
            if (GetComponent<CardEnemyController>().currentDisplay != null)
                GetComponent<CardEnemyController>().currentDisplay.GetComponent<CardBase>().Use(GetComponent<GenericBody>(), targetObj.GetComponent<GenericBody>(), TurnManager.instance.CurrentEnemiesList, TurnManager.instance.CurrentAlliesList, TurnManager.instance.CurrentMainAlly);
        }
    }

    [Command(requiresAuthority =false)]
    public void ServerSuggestCard(uint netID, Vector3 targetNetID, int playedCard)
    {
        Debug.Log("card suggestions");
    }
    [ClientRpc]
    public void PlayCard(uint netID, Vector3 target, int playedCard)
    {
        Debug.Log("playing card on client");
        //find object with netID
        
        if (netID == netId)
        {
            GameObject targetObj = null;
            float dist = float.MaxValue;
            foreach (GenericBody go in GameObject.FindObjectsByType<GenericBody>(FindObjectsSortMode.None))
            {
                var pos = go.gameObject.transform.position;
                var comp = Vector3.Distance(target, pos);
                if (comp < dist && go != gameObject)
                {
                    dist = comp;
                    targetObj = go.gameObject;
                }
            }
            //Debug.Log("playing card " + hand[playedCard].CardName + " on " + targetObj.name);
            //if it doesn't meet the energy requirment, uhh yeah
            if (hand.Count == 0)
            {
                Debug.Log("hand is empty");
                return;
            }
            try
            {
                //player using a card
                //if(isOwned)
                    hm.cards[playedCard].GetComponent<CardBase>().Use(GetComponent<GenericBody>(), targetObj.GetComponent<GenericBody>(), TurnManager.instance.CurrentEnemiesList, TurnManager.instance.CurrentAlliesList, TurnManager.instance.CurrentMainAlly);
            }
            catch
            {
                //enemies don't have hands so yeah
                if(GetComponent<CardEnemyController>().currentDisplay!=null)
                    GetComponent<CardEnemyController>().currentDisplay.GetComponent<CardBase>().Use(GetComponent<GenericBody>(), targetObj.GetComponent<GenericBody>(), TurnManager.instance.CurrentEnemiesList, TurnManager.instance.CurrentAlliesList, TurnManager.instance.CurrentMainAlly);
            }

            //CardBase.

            /*foreach (GameObject proj in hand[playedCard].attackPrefab)
            {
                var attack = Instantiate(proj);
                attack.transform.position = transform.position + (transform.forward * 2);
                attack.transform.forward = new Vector3(target.x, attack.transform.position.y, target.z) - attack.transform.position;
                attack.GetComponent<Rigidbody>().AddForce(attack.transform.forward * 10, ForceMode.Impulse);
            }*/
            if (!hand[playedCard].deleteAfterPlay)
                discardPile.Add(hand[playedCard]);
            Debug.Log("Played card " + hand[playedCard].CardName);
            hand.Remove(hand[playedCard]);
        }
        try
        {
            GetComponent<HandManager>().RefreshHand();
        }
        catch { }
    }
    public void AllToDeck()
    {
        foreach(CardData dat in discardPile)
        {
            deck.Add(dat);
        }
        discardPile.Clear();
        foreach (CardData dat in hand)
        {
            deck.Add(dat);
        }
        hand.Clear();
    }
    //Resources.Load<CardData>("CardData/" + cardName)
}
