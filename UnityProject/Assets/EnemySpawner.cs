using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Managers;

public class EnemySpawner : NetworkBehaviour
{
    public int EnemySpawns;
    public GameObject enemyPrefab;

    public static EnemySpawner instance;

    bool serverOk = false;
    bool setUp;

    Vector3 spawnPosition = new Vector3(0, -1.8f, 4);
    // Start is called before the first frame update
    void Awake()
    {
        if(instance==null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(!setUp)
        {
            setUp = true;
            //ClientSpawnEnemy();
            
        }
    }
    [Server]
    public void SpawnEnemy(int _EnemySpawns)
    {
        Debug.Log("Running server enemy spawn");
        EnemySpawns = _EnemySpawns;
        serverOk = true;

        int spawns = 0;
        while(EnemySpawns > 0)
        {
            var enemy = Instantiate(AmbidexterousManager.Instance.spawnPrefabs[2]);
            enemy.transform.position += Vector3.left * spawns;
            spawns++;
            EnemySpawns--;
            NetworkServer.Spawn(enemy);
            //TurnManager.instance.enemyTeam.Add(enemy.GetComponent<CardEnemyController>());
        }
        TurnManager.instance.CMDGetEnemyList();
    }
    /*[ClientRpc]
    public void ClientSpawnEnemy()
    {
        Debug.Log("Running client enemy spawn");
        var spacing = 1f / EnemySpawns;
        int previousSpawns = -EnemySpawns / 2;
        while (EnemySpawns > 0)
        {
            var newEnemy = Instantiate(enemyPrefab);
            newEnemy.transform.position = spawnPosition + (Vector3.right * 4 * spacing * previousSpawns);
            previousSpawns++;
            EnemySpawns--;
        }
    }*/
}
