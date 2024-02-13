
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Characters;
using Managers;

public class CardPlayerController : NetworkBehaviour
{
    public int currentEnergy;
    public int maxEnergy;
    public bool TurnEnded;
    IndividualPlayerGenericBody body;
    public CardDeck deck;
    public ReadEnergyFromPlayer energydisplay;

    bool started = false;
    //network stuff

    private void Start()
    {
        currentEnergy = maxEnergy;
        body = GetComponent<IndividualPlayerGenericBody>();
        deck = GetComponent<CardDeck>();
        //load from run manager?
        /*        if(SceneManager.GetActiveScene().name == "Game")
                deck.DrawCard(4);*/
    }
    [Command(requiresAuthority = false)]
    public void CMDStartEncounter()
    {
        StartEncounter();
    }
    [ClientRpc]
    public void StartEncounter()
    {

        if (started)
            return;
        Debug.Log("drawing cards at start");
        currentEnergy = maxEnergy;
        body = GetComponent<IndividualPlayerGenericBody>();
        deck = GetComponent<CardDeck>();
        if (isOwned)
        {
            deck.ServerDrawCard(4);
        }
        

        GetComponent<HandManager>().HandPosition = transform.position + (transform.forward * 2) + Vector3.up;
        GetComponent<HandManager>().RefreshHand();
        started = true;
    }
    [Command(requiresAuthority = false)]
    public void CMDStartTurn()
    {
        
        StartTurn();

    }
    [ClientRpc]
    public void StartTurn()
    {
        //body.OnPlayerTurnStart();
        deck.ServerDrawCard(1);
        TurnEnded = false;
        currentEnergy = maxEnergy;
        GetComponent<HandManager>().RefreshHand();
        //have some script read the hand and create the cards
    }
    public void EndTurn()
    {
        if (isLocalPlayer)
        {
            TurnEnded = true;
            Debug.Log("ended turn on client");
            GameObject.Find("TurnManager").GetComponent<TurnManager>().ServerPlayerEndTurn(netId);
        }
        
    }
    private void Update()
    {
        //if(!TurnEnded&&body.health > 0)
        //{
                //StartEncounter();
            //this is things the player can do while alive and it's their turn.
            //this needs to be a local player check
/*            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndTurn();
            }*/
        //}
        //you can still mouse over your hand while it isnt your turn
    }
}
