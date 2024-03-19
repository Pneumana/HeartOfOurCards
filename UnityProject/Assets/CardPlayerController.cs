
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
        if (loadedPlayers == NetworkServer.connections.Count && NetworkServer.connections.Count > 1)
        {
            //if(isServer)
            StartEncounter();
            Debug.Log("all players loaded");

        }
        else if(NetworkServer.connections.Count == 1)
        {
            StartEncounter();
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
        {
            Debug.Log("unable to draw cards at start, already started encounter on " + gameObject.name);

            return;
        }
        Debug.Log("drawing cards at start");
        currentEnergy = maxEnergy;
        body = GetComponent<IndividualPlayerGenericBody>();
        deck = GetComponent<CardDeck>();
        if (isOwned)
        {
            Debug.Log(gameObject.name + "started encounter and needs to draw " + (5 + Mathf.FloorToInt(RunManager.instance.playerStatList[0].INT / 3)) + " cards due to bonuses");
            
            if (GetComponent<GenericBody>().CharacterType == Enums.CharacterType.P1) 
            {
                deck.ServerDrawCard(5 + Mathf.FloorToInt(RunManager.instance.playerStatList[0].INT / 3));
            }
            else if (GetComponent<GenericBody>().CharacterType == Enums.CharacterType.P2)
            {
                deck.ServerDrawCard(5 + Mathf.FloorToInt(RunManager.instance.playerStatList[1].INT / 3));
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
        Debug.Log("starting turn on " + gameObject.name);
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
            
            if(NetworkServer.connections.Count > 1)
            {
                TurnEnded = true;
                Debug.Log("ended turn on client");
                GameObject.Find("TurnManager").GetComponent<TurnManager>().ServerPlayerEndTurn(netId);
            }
            else
            {
                var tm = GameObject.Find("TurnManager").GetComponent<TurnManager>();
                var plrTeam = tm.playerTeam;
                foreach (CardPlayerController plr in plrTeam)
                {
                    plr.TurnEnded = true;
                    tm.ServerPlayerEndTurn(plr.netId);
                }
            }
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
