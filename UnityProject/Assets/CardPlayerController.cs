
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

    public bool started = false;
    //network stuff
    [SyncVar] int loadedPlayers;
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
        /*if (connectionToClient.isReady)
        {
            StartEncounter();
        }
        else
        {
            StartCoroutine(StartEncounterCoroutine());
        }*/
        //StartEncounter();
        loadedPlayers++;
        if (loadedPlayers == NetworkServer.connections.Count)
        {
            //if(isServer)
            StartEncounter();
            Debug.Log("all players loaded");
            
        }
            
    }

    IEnumerator StartEncounterCoroutine()
    {
        while (!connectionToServer.isReady)
        {
            yield return new WaitForSeconds(0.1f);
        }
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
            Debug.Log("started encounter and needs to draw " + (5 + Mathf.FloorToInt(RunManager.instance.playerStatList[0].INT / 3)) + " cards due to bonuses");
            if (GetComponent<GenericBody>().CharacterType == Enums.CharacterType.P1) 
            {
                deck.ServerDrawCard(5 + Mathf.FloorToInt(RunManager.instance.playerStatList[0].INT / 3));
            }

        }
        

        //GetComponent<HandManager>().HandPosition = transform.position + (transform.forward * 2) + Vector3.up;
        GetComponent<HandManager>().RefreshHand();
        started = true;
    }
    [Command(requiresAuthority = false)]
    public void CMDStartTurn()
    {
        //if(TurnEnded)
            StartTurn();

    }
    [ClientRpc]
    public void StartTurn()
    {
        if (TurnEnded == false)
            return;
        //body.OnPlayerTurnStart();
        if (isOwned)
        {
            deck.ServerDrawCard(5);

        }
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

    void TryStartEncounter()
    {

    }

    private void Update()
    {
        //if(!TurnEnded&&body.health > 0)
        //{
/*        if(!started && SceneManager.GetActiveScene().name == "ConnorTest")
            StartEncounter();*/
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
