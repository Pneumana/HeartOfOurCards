
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlayerController : MonoBehaviour
{
    public int currentEnergy;
    public int maxEnergy;
    public bool TurnEnded;
    GenericBody body;
    public CardDeck deck;
    //network stuff

    private void Start()
    {
        body = GetComponent<GenericBody>();
        deck = GetComponent<CardDeck>();
        //load from run manager?
    }
    public void StartTurn()
    {
        deck.DrawCard();
        TurnEnded = false;
        currentEnergy = maxEnergy;
        deck.DrawCard();
        //have some script read the hand and create the cards
    }
    public void EndTurn()
    {
        TurnEnded = true;
        GameObject.Find("TurnManager").GetComponent<TurnManager>().PlayerEndTurn(this);
    }
    private void Update()
    {
        if(!TurnEnded&&body.health > 0)
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
}
