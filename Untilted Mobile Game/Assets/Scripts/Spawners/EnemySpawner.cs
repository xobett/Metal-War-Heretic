using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("ENEMY SPAWNER SETTINGS")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField, Range(0f, 3f)] private int timeBetweenSpawns;
    [SerializeField] private Transform[] spawnLocations;

    [SerializeField] private int spawnCount;

    private Coroutine spawnActiveCoroutine;
    public void StartSpawn()
    {
        spawnActiveCoroutine = StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        if (spawnCount <= 5)
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            Instantiate(enemyPrefabs[RandomEnemyIndex()], spawnPoint.position, spawnPoint.rotation);
            spawnCount++;
        }
        else 
        {
            StopCoroutine(spawnActiveCoroutine);
        }

        yield return new WaitForSeconds(timeBetweenSpawns);

        spawnActiveCoroutine = StartCoroutine(SpawnEnemy());
    }

    private int GetTotalEnemiesInScene()
    {
        GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");

        return enemiesInScene.Length;
    }

    private int RandomEnemyIndex() => Random.Range(0, enemyPrefabs.Length - 1);

    private Transform GetRandomSpawnPoint()
    {
        return spawnLocations[Random.Range(0, spawnLocations.Length - 1)];
    }
}
