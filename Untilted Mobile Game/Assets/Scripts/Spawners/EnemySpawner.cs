using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("ENEMY SPAWNER SETTINGS")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField, Range(0f, 3f)] private int timeBetweenSpawns;
    [SerializeField] private Transform[] spawnLocations;

    private Coroutine spawnActiveCoroutine;
    public void StartSpawn()
    {
        spawnActiveCoroutine = StartCoroutine(SpawnEnemy());
    }

    private int GetTotalEnemiesInScene()
    {
        GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");

        return enemiesInScene.Length;
    }

    private IEnumerator SpawnEnemy()
    {
        if (GetTotalEnemiesInScene() <= 5)
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            Instantiate(enemyPrefabs[RandomEnemyIndex()], spawnPoint.position, spawnPoint.rotation);
        }
        else if (GetTotalEnemiesInScene() > 5)
        {
            StopCoroutine(spawnActiveCoroutine);
        }

        yield return new WaitForSeconds(timeBetweenSpawns);

        StartCoroutine(SpawnEnemy());
    }

    private int RandomEnemyIndex() => Random.Range(0, enemyPrefabs.Length - 1);

    private Transform GetRandomSpawnPoint()
    {
        return spawnLocations[Random.Range(0, spawnLocations.Length - 1)];
    }
}
