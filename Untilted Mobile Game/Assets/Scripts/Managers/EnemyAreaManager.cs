using EnemyAI;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAreaManager : MonoBehaviour
{
    public EnemyAreaManager Instance { get; private set; }

    [Header("SPAWN SETTINGS")]
    [SerializeField] private GameObject[] areaEnemies;

    [SerializeField] private List<EnemyBase> aliveEnemies = new List<EnemyBase>();
    private List<EnemyBase> attackingEnemies = new List<EnemyBase>();

    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private float areaRadius;

    private void Start()
    {
        SpawnEnemies();
    }

    #region SPAWN
    private void SpawnEnemies()
    {
        for (int i = 0; i < areaEnemies.Length; i++)
        {
            GameObject enemy = Instantiate(areaEnemies[i], GetRandomSpawnPos(), Quaternion.identity);
            AddEnemyToArea(enemy.GetComponent<EnemyBase>());
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

    #endregion SPAWN

    #region AREA

    public void AddEnemyToArea(EnemyBase enemy)
    {
        aliveEnemies.Add(enemy);
    }

    public void RemoveEnemyFromArea(EnemyBase enemy)
    {
        aliveEnemies.Remove(enemy);
    }

    #endregion AREA

    #region ATTACK

    public int GetTotalAttackingEnemies()
    {
        return attackingEnemies.Count;
    }

    public void AddAttackingEnemy(EnemyBase enemy)
    {
        attackingEnemies.Add(enemy);
    }

    public void RemoveAttackingEnemy(EnemyBase enemy)
    {
        attackingEnemies.Remove(enemy);
    }

    private void StartAttack()
    {
        foreach (EnemyBase enemy in aliveEnemies)
        {
            enemy.GetBehavior();
        }
    }

    #endregion ATTACK

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartAttack();
        }
    }
}