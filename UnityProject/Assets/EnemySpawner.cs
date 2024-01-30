using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    public int EnemySpawns;
    public GameObject enemyPrefab;

    Vector3 spawnPosition = new Vector3(0, -1.8f, 4);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Command(requiresAuthority = false)]
    public void SpawnEnemy(int _EnemySpawns)
    {
        Debug.Log("Running server enemy spawn");
        EnemySpawns = _EnemySpawns;
        ClientSpawnEnemy();
    }
    [ClientRpc]
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
    }

}
