using EnemyAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    [Header("AREA ENEMY SETTINGS")]
    [SerializeField] private GameObject[] enemiesToSpawn;

    [Header("SPAWN SETTINGS")]
    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private float areaRadius = 10f;

    private List<EnemyBase> aliveEnemies = new List<EnemyBase>();
    private List<EnemyBase> attackingEnemies = new List<EnemyBase>();

    private List<EnemyBase> positionQueryQueue = new List<EnemyBase>();
    private List<Vector3> usedPositions = new List<Vector3>();

    private const float waitPositionDistance = 4.0f;

    private GameObject player;
    private LayerMask whatIsEnemy;

    private void Start()
    {
        SpawnEnemies();
        whatIsEnemy = LayerMask.GetMask("Enemy");
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(CR_AssignPosition());
    }

    #region SPAWN
    private void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            GameObject enemy = Instantiate(enemiesToSpawn[i], GetRandomSpawnPos(), Quaternion.identity);
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

    public void QueryWaitPosition(EnemyBase enemy)
    {
        positionQueryQueue.Add(enemy);
    }

    private IEnumerator CR_AssignPosition()
    {
        yield return new WaitForSeconds(1f);

        if (positionQueryQueue.Count != 0)
        {
            for (int i = positionQueryQueue.Count - 1; i >= 0 ; i--)
            {
                Vector3 pos = GetWaitingPosition();
                positionQueryQueue[i].SetWaitPosition(pos);
                positionQueryQueue.RemoveAt(i);
            }
        }

        StartCoroutine(CR_AssignPosition());
    }

    private Vector3 GetWaitingPosition()
    {
        int positions = enemiesToSpawn.Length;

        for (int i = 0; i < positions; i++)
        {
            float angle = (360f / positions) * i;

            Vector3 offsetPos = Quaternion.Euler(0, angle, 0f) * Vector3.forward * 5;
            Vector3 noCollisionPos = player.transform.position + offsetPos;

            if (!usedPositions.Contains(noCollisionPos))
            {
                usedPositions.Add(noCollisionPos);
                return noCollisionPos;
            }
        }

        float incrementedDistance = waitPositionDistance + Random.Range(7f, 9f);
        float fallbackAngle = (360f / positions * 2) * Random.Range(1, (positions * 2) + 1);
        Vector3 fallBackPos = Quaternion.Euler(0f, fallbackAngle, 0) * Vector3.forward * incrementedDistance;
        fallBackPos = player.transform.position + fallBackPos;

        return fallBackPos;
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

    }

    #endregion ATTACK

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartAttack();
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnOrigin != null)
        {
            Gizmos.DrawWireSphere(spawnOrigin.position, areaRadius);
        }
    }
}