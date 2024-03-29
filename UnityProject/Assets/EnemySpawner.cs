using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Managers;
using System;
using Random = UnityEngine.Random;

public class EnemySpawner : NetworkBehaviour
{
    public int EnemySpawns;
    public List<GameObject> enemyPrefabs;

    public static EnemySpawner instance;

    bool serverOk = false;
    bool setUp;

    [SerializeField] float xStart = -5;
    [SerializeField] float xEnd = 5;

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
        //AmbidexterousManager.Instance.spawnPrefabs.Add(prefab);
        //if encounter is boss



        Debug.Log("Running server enemy spawn");
        EnemySpawns = _EnemySpawns;
        serverOk = true;


        var initOffset = (EnemySpawns - 1) / 2 * Vector3.left;


        for(int i = 0; i < EnemySpawns; i++)
        {
            var number = Random.Range(1, 3);
            var percentSpawned = (i + 1) / (EnemySpawns + 1f);
            var enemy = Instantiate(AmbidexterousManager.Instance.spawnPrefabs[number]);
            spawnPosition.x = Mathf.Lerp(xStart, xEnd, percentSpawned);
            enemy.transform.position += spawnPosition;
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
