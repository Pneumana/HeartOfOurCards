using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : NetworkBehaviour
{
    public List<CardPlayerController> playerTeam = new List<CardPlayerController>();
    public List<CardEnemyController> enemyTeam = new List<CardEnemyController> ();

    public List<CardPlayerController> turnEnded = new List<CardPlayerController>();
    public List<CardEnemyController> enemyTurnEnded = new List<CardEnemyController>();

    public bool isPlayerTurn;

    public int endedTurns;



    private void Awake()
    {
        //SceneManager.sceneLoaded += this.OnLoadCallback;
        Invoke("OnLoadCallback", 0.1f);
    }
    private void OnDisable()
    {
        //SceneManager.sceneLoaded -= this.OnLoadCallback;
    }
    void OnLoadCallback()
    {
        Debug.Log("scene load callback");
        foreach (CardPlayerController player in FindObjectsByType<CardPlayerController>(FindObjectsSortMode.None))
        {
            Debug.Log(player.gameObject);
            if(player!=null)
                playerTeam.Add(player);
        }
    }
    //enemy plays card(s), waits about 0.5s then the next enemy takes it's turn
    private void Update()
    {

        /*if(FindObjectsByType<CardPlayerController>(FindObjectsSortMode.None).Length != playerTeam.Count)
        {
            foreach (CardPlayerController player in FindObjectsByType<CardPlayerController>(FindObjectsSortMode.None))
            {
                playerTeam.Add(player);
            }
        }*/
    }
    [Command(requiresAuthority =false)]
    public void ServerPlayerEndTurn(uint netID)
    {
        Debug.Log("ended turn on server");
        PlayerEndTurn(netID);
    }
    [ClientRpc]
    public void PlayerEndTurn(uint netID)
    {
        Debug.Log("ended turn on client rpc");
        //CardPlayerController turnEnder = null;
        //find object with netID
        foreach (NetworkIdentity netid in FindObjectsByType<NetworkIdentity>(FindObjectsSortMode.None))
        {
            if (netid.netId == netID)
            {
                
                Debug.Log("found netID match");
                //turnEnder = netid.gameObject.GetComponent<CardPlayerController>();
                var localPlayerControllers = netid.gameObject.GetComponentsInChildren<CardPlayerController>();
                Debug.Log("matched player has " + localPlayerControllers.Length + " player controllers");
                foreach (CardPlayerController turnEnder in localPlayerControllers)
                {
                    Debug.Log("looping though controllers");
                    if (isPlayerTurn)
                    {
                        if (playerTeam.Contains(turnEnder) && !turnEnded.Contains(turnEnder))
                        {
                            //Debug.Log(turnEnder.gameObject.name + " ended turn");
                            turnEnded.Add(turnEnder);
                        }
                    }
                }
                //Debug.Log(netid.gameObject.name + " is DONE");
            }
        }
        if (turnEnded.Count == playerTeam.Count)
        {
            isPlayerTurn = false;
            turnEnded.Clear();
            Debug.Log("player turn ended");
            ServerStartEnemyTurns();
        }
        /*if (isPlayerTurn)
        {
            if (playerTeam.Contains(turnEnder) && !turnEnded.Contains(turnEnder))
            {
                //Debug.Log(turnEnder.gameObject.name + " ended turn");
                turnEnded.Add(turnEnder);
            }
            if(turnEnded.Count == playerTeam.Count)
            {
                isPlayerTurn = false;
                turnEnded.Clear();
                Debug.Log("player turn ended");
                ServerStartEnemyTurns();
            }
        }*/

    }
    public void EnemyEndTurn(CardEnemyController turnEnder)
    {
        if (!isPlayerTurn)
        {
            if (enemyTeam.Contains(turnEnder) && !enemyTurnEnded.Contains(turnEnder))
            {
                Debug.Log(turnEnder.gameObject.name + " ended turn");
                enemyTurnEnded.Add(turnEnder);
            }
            if (enemyTurnEnded.Count == enemyTeam.Count)
            {
                isPlayerTurn = true;
                enemyTurnEnded.Clear();
                Debug.Log("enemy turn ended");
                foreach(CardPlayerController plr in playerTeam)
                {
                    plr.StartTurn();
                }
            }
        }
    }

    [ClientRpc]
    void ServerStartEnemyTurns()
    {
        StartCoroutine(EnemyTurnLoop());
    }

    IEnumerator EnemyTurnLoop()
    {
        int enemyLoopIndex = 0;
        do
        {
            enemyTeam[enemyLoopIndex].TakeTurn();
            yield return new WaitForSeconds(0.5f);
            enemyLoopIndex++;
        }
        while (enemyLoopIndex<enemyTeam.Count);
        yield return null;
    }
}
