using EnemyAI;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAreaManager : MonoBehaviour
{
    public EnemyAreaManager Instance { get; private set; }

    [Header("SPAWN SETTINGS")]
    [SerializeField] private GameObject[] areaEnemies;

    [SerializeField] private List<EnemyBase> aliveEnemies = new List<EnemyBase>();

    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private float areaRadius;

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < areaEnemies.Length; i++)
        {
            GameObject enemy = Instantiate(areaEnemies[i], GetRandomSpawnPos(), Quaternion.identity);
            aliveEnemies.Add(enemy.GetComponent<EnemyBase>());
            enemy.GetComponent<EnemyBase>().AssignArea(this);
            enemy.transform.parent = transform.GetChild(0);
        }
    }

    private Vector3 GetRandomSpawnPos()
    {
        Vector3 pos = Random.insideUnitSphere * areaRadius;
        pos.y = 0;
        pos = spawnOrigin.position + pos;
        return pos;
    }

    public void RemoveEnemyFromArea(EnemyBase enemy)
    {
        aliveEnemies.Remove(enemy);
    }

    #region VISUAL DEBUG GIZMOS

    private void OnDrawGizmos()
    {
        if (spawnOrigin != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(spawnOrigin.position, areaRadius);
        }
    }

    #endregion VISUAL DEBUG GIZMOS
}