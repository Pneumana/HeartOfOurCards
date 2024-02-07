using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Characters;
using Managers;
using CardActions;
using DeckData;

public class CardEnemyController : NetworkBehaviour
{

    public int currentEnergy;
    public int maxEnergy;
    public bool TurnEnded;
    GenericBody body;

    public GameObject displayCardPrefab;
    public GameObject currentDisplay;

    public CardDeck deck;
    //network stuff

    private void Start()
    {
        
        //NetworkServer.Spawn(gameObject, AmbidexterousManager.Instance.PlayerList[0].gameObject);
        body = GetComponent<GenericBody>();
        deck = GetComponent<CardDeck>();
        //pick a card to play
        ServerDisplayEnemyCard();
    }
    public void StartTurn()
    {
        TurnEnded = false;
        currentEnergy = maxEnergy;
    }
    public void EndTurn()
    {
        TurnEnded = true;
        GameObject.Find("TurnManager").GetComponent<TurnManager>().EnemyEndTurn(this);
    }
    private void Update()
    {
        if (!TurnEnded && body.health > 0)
        {
            //this is things the player can do while alive and it's their turn.
            //this needs to be a local player check
            /*            if (Input.GetKeyDown(KeyCode.Space))
                        {
                            EndTurn();
                        }*/
        }
        //you can still mouse over your hand while it isnt your turn
    }
    [Command(requiresAuthority =false)]
    public void TakeTurn()
    {
        //play picked card
        //NetworkIdentity targetNetID = null;

        //FIND LOCAL PLAYER NETID


            deck.ServerPlayCard(netId, transform.forward * 4, 0);
            EndTurn();
            deck.ServerDrawCard(1);
            ServerDisplayEnemyCard();
        
        //pick a new card to play. 
    }
    [Command(requiresAuthority =false)]
    public void ServerDisplayEnemyCard()
    {
        Debug.Log("server display enemy card");
        if (deck == null)
        {
            body = GetComponent<GenericBody>();
            deck = GetComponent<CardDeck>();
        }
        //await deck.ServerDrawCard(1);
        if (deck.hand.Count < 1)
            deck.DrawCard(1, 0);
        var pickedCard = Random.Range(0, deck.hand.Count - 1);
        //Debug.Log(deck.hand[0].CardName);
        PickCard(pickedCard);
    }
    [ClientRpc]
    void PickCard(int pickedCardIndex)
    {
        Debug.Log("client wants to display enemy card");
        if (deck == null)
        {
            body = GetComponent<GenericBody>();
            deck = GetComponent<CardDeck>();
        }
        //deck.DrawCard(1, pickedCardIndex);
        FirstCardDraw(pickedCardIndex);
        CardData pickedCard = null;
        if(deck.hand.Count > 0)
        {
            pickedCard = deck.hand[0];
        }
        if (currentDisplay != null)
        {
            Destroy(currentDisplay);
        }

        if (pickedCard != null)
        {
            Debug.Log("calling for card " + deck.hand[pickedCardIndex].CardName + " to be displayed");
            currentDisplay = Instantiate(displayCardPrefab);
            currentDisplay.transform.position = transform.position + (Vector3.up * 2) + (transform.forward * 1.5f);
            currentDisplay.GetComponent<CardBase>().CardData = pickedCard;
            currentDisplay.transform.LookAt(Camera.main.transform.position);
        }
        else
        {

        }
    }
    public void FirstCardDraw(int pickedCardIndex)
    {
        var drawnCard = pickedCardIndex;
        if (deck.deck.Count == 0)
        {
            if (deck.discardPile.Count > 0)
            {
                var hold = deck.discardPile.Count;
                //deck = hold;
                for (int i = 0; i < hold; i++)
                {
                    deck.deck.Add(deck.discardPile[i]);
                }
                //discardPile.Clear();
                Debug.Log("shuffling deck");
                deck.discardPile.Clear();
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
                if (drawnCard > deck.deck.Count - 1)
                {
                    drawnCard = deck.deck.Count - 1;
                }
                deck.hand.Add(deck.deck[drawnCard]);
                deck.deck.Remove(deck.deck[drawnCard]);
                //Debug.Log(deck.deck.Count);
                //Debug.Log("Drew card " + deck.hand[deck.hand.Count - 1].CardName);
            }
            catch { Debug.LogWarning(drawnCard + " is greater than the remaining deck " + (deck.deck.Count - 1)); }

        }
    }
}
