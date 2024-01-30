using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEnemyController : NetworkBehaviour
{

    public int currentEnergy;
    public int maxEnergy;
    public bool TurnEnded;
    GenericBody body;

    public GameObject displayCardPrefab;
    GameObject currentDisplay;

    public CardDeck deck;
    //network stuff

    private void Start()
    {
        
        //NetworkServer.Spawn(gameObject, AmbidexterousManager.Instance.PlayerList[0].gameObject);
        body = GetComponent<GenericBody>();
        deck = GetComponent<CardDeck>();
        //pick a card to play
        PickCard();
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

    public void TakeTurn()
    {
        //play picked card
        deck.ServerPlayCard(netId, transform.forward, -1);
        EndTurn();
        PickCard();
        //pick a new card to play. 
    }
    void PickCard()
    {
        deck.DrawCard();
        ConnorCard pickedCard = null;
        if(deck.hand.Count > 0)
            pickedCard = deck.hand[Random.Range(0, deck.hand.Count - 1)];
        if(currentDisplay!=null)
            Destroy(currentDisplay);
        if (pickedCard != null)
        {
            currentDisplay = Instantiate(displayCardPrefab);
            currentDisplay.transform.position = transform.position + (Vector3.up * 2) + (transform.forward * 1.5f);
            currentDisplay.GetComponent<ConnorCardController>().card = pickedCard;
            currentDisplay.transform.LookAt(Camera.main.transform.position);
        }
    }
}
