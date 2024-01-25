using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<CardPlayerController> playerTeam = new List<CardPlayerController>();
    public List<CardEnemyController> enemyTeam = new List<CardEnemyController> ();

    public List<CardPlayerController> turnEnded = new List<CardPlayerController>();
    public List<CardEnemyController> enemyTurnEnded = new List<CardEnemyController>();

    public bool isPlayerTurn;

    public int endedTurns;



    //enemy plays card(s), waits about 0.5s then the next enemy takes it's turn
    private void Update()
    {
        
    }
    public void PlayerEndTurn(CardPlayerController turnEnder)
    {
        if (isPlayerTurn)
        {
            if (playerTeam.Contains(turnEnder) && !turnEnded.Contains(turnEnder))
            {
                Debug.Log(turnEnder.gameObject.name + " ended turn");
                turnEnded.Add(turnEnder);
            }
            if(turnEnded.Count == playerTeam.Count)
            {
                isPlayerTurn = false;
                turnEnded.Clear();
                Debug.Log("player turn ended");
                StartCoroutine(EnemyTurnLoop());
            }
        }
        
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
