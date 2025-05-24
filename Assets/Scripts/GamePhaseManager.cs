using UnityEngine;
using Unity.Netcode;
using System.Collections;

[System.Serializable]
public class SpawnPoint
{
    public Transform location;
    public GameObject enemyPrefab;
}

public class GamePhaseManager : NetworkBehaviour
{
    [Header("Spawn Settings")]
    public SpawnPoint[] spawnPoints;
    public float globalSpawnInterval = 5f;

    private Transform defaultDestination;

    private void Start()
    {
        GameObject destinationObj = GameObject.FindGameObjectWithTag("flag");
        if (destinationObj != null)
        {
            defaultDestination = destinationObj.transform;
        }

        if (IsServer)
        {
            StartCoroutine(SpawnEnemiesRoutine());
        }
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(globalSpawnInterval);
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        if (!IsServer) return;
        if (spawnPoints.Length == 0) return;

        SpawnPoint randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        if (randomSpawnPoint.location == null || randomSpawnPoint.enemyPrefab == null) return;

        GameObject enemy = Instantiate(randomSpawnPoint.enemyPrefab, 
            randomSpawnPoint.location.position, 
            randomSpawnPoint.location.rotation);
        
        if (defaultDestination != null)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.defaultDestination = defaultDestination;
            }
        }

        NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    }
} 