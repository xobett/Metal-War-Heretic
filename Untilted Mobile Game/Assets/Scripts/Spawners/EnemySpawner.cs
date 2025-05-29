using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("ENEMY SPAWNER SETTINGS")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Range(0f, 3f)] private int timeBetweenSpawns;
    [SerializeField] private Transform[] spawnLocations;

    void Start()
    {
        StartCoroutine(SpawnEnemy());       
    }
    
    private IEnumerator SpawnEnemy()
    {
        Transform spawnPoint = GetRandomSpawnPoint();
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        yield return new WaitForSeconds(timeBetweenSpawns);

        StartCoroutine(SpawnEnemy());
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnLocations[Random.Range(0, spawnLocations.Length - 1)];
    }
}
