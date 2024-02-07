using CardActions;
using Characters;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Managers;
using DeckData;

public class CardDeck : NetworkBehaviour
{
    public List<CardData> hand = new List<CardData>();
    public List<CardData> deck = new List<CardData> ();
    public List<CardData> discardPile = new List<CardData>();

    //public List<CardBase> cardHand = new List<CardBase>();

    private HandManager hm;

    // Start is called before the first frame update
    void Start()
    {
        hm = GetComponent<HandManager>();
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
        int drawnCard = Random.Range(0, deck.Count - 1);
        Debug.Log(drawnCard + " was drawn from deck of " + (deck.Count-1));
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
                }
                else
                {
                    Debug.Log("deck is empty");
                    return;
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
    [Command(requiresAuthority =false)]
    public void ServerPlayCard(uint netID, Vector3 target, int playedCard)
    {
        Debug.Log("server recieved play request");
        if (playedCard == -1)
            playedCard = Random.Range(0, hand.Count - 1);
        PlayCard(netID, target, playedCard);
        //run play card on all clients, checking for netID match so the same player uses them
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
            //if it doesn't meet the energy requirment, uhh yeah
            if (hand.Count == 0)
            {
                Debug.Log("hand is empty");
                return;
            }
            try
            {
                //player using a card
                hm.cards[playedCard].GetComponent<CardBase>().Use(GetComponent<GenericBody>(), targetObj.GetComponent<GenericBody>(), TurnManager.instance.CurrentEnemiesList, TurnManager.instance.CurrentAlliesList);
            }
            catch
            {
                //enemies don't have hands so yeah
                GetComponent<CardEnemyController>().currentDisplay.GetComponent<CardBase>().Use(GetComponent<GenericBody>(), targetObj.GetComponent<GenericBody>(), TurnManager.instance.CurrentEnemiesList, TurnManager.instance.CurrentAlliesList);
            }
            
            //CardBase.
            
            /*foreach (GameObject proj in hand[playedCard].attackPrefab)
            {
                var attack = Instantiate(proj);
                attack.transform.position = transform.position + (transform.forward * 2);
                attack.transform.forward = new Vector3(target.x, attack.transform.position.y, target.z) - attack.transform.position;
                attack.GetComponent<Rigidbody>().AddForce(attack.transform.forward * 10, ForceMode.Impulse);
            }*/
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
}
