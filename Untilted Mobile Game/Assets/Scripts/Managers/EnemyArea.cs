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

    private List<EnemyBase> positionQueue = new List<EnemyBase>();
    private List<Vector3> usedPositions = new List<Vector3>();

    private List<EnemyBase> attackQueue = new List<EnemyBase>();

    private List<EnemyBase> attackPosQueue = new List<EnemyBase>();
    [SerializeField] private List<Vector3> usedAttackPositions = new List<Vector3>();
    [SerializeField] private List<EnemyBase> attackingEnemies = new List<EnemyBase>();

    private const float waitPositionDistance = 4.0f;

    private GameObject player;

    private void Start()
    {
        Start_GetReferences();
        Start_SpawnEnemies();
        Start_RunPositionQueries();
    }

    #region START

    private void Start_GetReferences()
    {
        player = Player.Instance.gameObject;
    }

    private void Start_SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            GameObject enemy = Instantiate(enemiesToSpawn[i], GetRandomSpawnPos(), Quaternion.identity);
            AddEnemyToArea(enemy.GetComponent<EnemyBase>());
            enemy.GetComponent<EnemyBase>().AssignArea(this);
            enemy.transform.parent = transform.GetChild(0);
        }
    }

    private void Start_RunPositionQueries()
    {
        StartCoroutine(CR_HandleQueries());
    }

    #endregion START

    #region SPAWN

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
        if (enemy.lastAssignedPos != null)
        {
            usedPositions.Remove(enemy.lastAssignedPos);
        }

        positionQueue.Add(enemy);
    }

    private IEnumerator CR_HandleQueries()
    {
        yield return new WaitForSeconds(1f);

        if (positionQueue.Count != 0)
        {
            for (int i = positionQueue.Count - 1; i >= 0; i--)
            {
                Vector3 pos = GetWaitingPosition();
                positionQueue[i].SetWaitPosition(pos);
                positionQueue.RemoveAt(i);
            }
        }

        if (attackQueue.Count != 0)
        {
            for (int i = attackQueue.Count - 1; i >= 0; i--)
            {
                if (attackingEnemies.Count < 3)
                {
                    attackingEnemies.Add(attackQueue[i]);

                    attackQueue[i].SetAttackState();
                    attackQueue.RemoveAt(i);
                }
            }
        }

        if (attackPosQueue.Count != 0)
        {
            for (int i = attackPosQueue.Count - 1; i >= 0; i--)
            {
                Vector3 pos = GetAttackPosition();

                attackPosQueue[i].SetAttackPosition(pos);
                attackPosQueue.RemoveAt(i);
            }
        }

        StartCoroutine(CR_HandleQueries());
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

    private Vector3 GetAttackPosition()
    {
        int positions = 3;

        for (int i = 0 ; i < positions; i++)
        {
            float angle = (360 / positions) * i;

            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * 2f;
            Vector3 attackPos = player.transform.position + offset;

            if (!usedAttackPositions.Contains(attackPos))
            {
                usedAttackPositions.Add(attackPos);
                return attackPos;
            }
        }

        Vector3 fallback = player.transform.right * 2f;
        return fallback;
    }

    #endregion AREA

    #region ATTACK

    public int GetTotalAttackingEnemies()
    {
        return attackingEnemies.Count;
    }

    public void QueryAttack(EnemyBase enemy)
    {
        attackQueue.Add(enemy);
    }

    public void QueryAttackPos(EnemyBase enemy)
    {
        if (enemy.lastAttackPos != null)
        {
            usedAttackPositions.Remove(enemy.lastAttackPos);
        }

        attackPosQueue.Add(enemy);
    }

    public void RemoveAttackingEnemy(EnemyBase enemy)
    {
        attackingEnemies.Remove(enemy);
    }

    #endregion ATTACK

    #region TRIGGERS

    private void OnTrigger_AttackPlayer()
    {
        foreach (EnemyBase enemy in aliveEnemies)
        {
            enemy.ChangeState(State.Chase);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnTrigger_AttackPlayer();
        }
    }

    #endregion TRIGGERS

    private void OnDrawGizmos()
    {
        if (spawnOrigin != null)
        {
            Gizmos.DrawWireSphere(spawnOrigin.position, areaRadius);
        }
    }
}