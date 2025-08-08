using EnemyAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    [Header("AREA ENEMY SETTINGS")]
    [SerializeField] private GameObject[] enemiesToSpawn;
    [SerializeField] private GameObject vfxEnemySpawn;

    [Header("SPAWN SETTINGS")]
    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private float areaRadius = 10f;

    private List<EnemyBase> aliveEnemies = new List<EnemyBase>();
    [SerializeField] private List<EnemyBase> attackingEnemies = new List<EnemyBase>();

    //QUEUES
    [SerializeField] private List<EnemyBase> getWaitPositionQueue = new List<EnemyBase>();
    [SerializeField] private List<EnemyBase> getAttackPositionQueue = new List<EnemyBase>();
    [SerializeField] private List<EnemyBase> getAttackStateQueue = new List<EnemyBase>();

    private Dictionary<EnemyBase, Vector3> usedWaitPositions = new Dictionary<EnemyBase, Vector3>();
    private Dictionary<EnemyBase, Vector3> usedAttackPositions = new Dictionary<EnemyBase, Vector3>();

    private const float waitPositionDistance = 9.0f;

    private float[] enemiesDistances = { 2f, 4.5f, 2.5f };

    private Vector3 playerPos;

    private bool enteredArea = false;

    private void Start()
    {
        Start_SpawnEnemies();
        Start_RunQueriesHandler();
    }

    #region START

    private void Start_SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPos();

            GameObject vfx = Instantiate(vfxEnemySpawn, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
            Destroy(vfx, 1.5f);
            GameObject enemy = Instantiate(enemiesToSpawn[i], spawnPos, Quaternion.identity);

            AddEnemyToArea(enemy.GetComponent<EnemyBase>());
            enemy.GetComponent<EnemyBase>().AssignArea(this);

            enemy.transform.parent = transform.GetChild(0);
            enemy.name = enemiesToSpawn[i].name + $" {i}";
        }
    }

    private void Start_RunQueriesHandler()
    {
        StartCoroutine(CR_QueriesHandler());
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

    public int UsedAttackPosCount => usedAttackPositions.Count;

    public void ResetUpdatePositionValue()
    {
        foreach (EnemyBase enemy in aliveEnemies)
        {
            enemy.UpdatedPosition = false;
        }
    }

    #region QUERIES
    public void QueryAttackState(EnemyBase enemy)
    {
        if (getAttackStateQueue.Contains(enemy)) return;
        getAttackStateQueue.Add(enemy);
    }

    public void QueryWaitPosition(EnemyBase enemy)
    {
        OnQuery_RemoveUsedPositions(enemy);
        getWaitPositionQueue.Add(enemy);
    }

    public void QueryAttackPos(EnemyBase enemy)
    {
        OnQuery_RemoveUsedPositions(enemy);
        getAttackPositionQueue.Add(enemy);
    }

    private void OnQuery_RemoveUsedPositions(EnemyBase enemy)
    {
        RemoveWaitPos(enemy);
        RemoveAttackPos(enemy);
    }

    public void RemoveWaitPos(EnemyBase enemy)
    {
        if (usedWaitPositions.ContainsKey(enemy))
        {
            usedWaitPositions.Remove(enemy);
        }

        if (getWaitPositionQueue.Contains(enemy))
        {
            getWaitPositionQueue.Remove(enemy);
        }
    }

    public void RemoveAttackPos(EnemyBase enemy)
    {
        if (usedAttackPositions.ContainsKey(enemy))
        {
            usedAttackPositions.Remove(enemy);
        }

        if (getAttackPositionQueue.Contains(enemy))
        {
            getAttackPositionQueue.Remove(enemy);
        }
    }

    [ContextMenu("Print Wait Positions")]
    private void Debug_PrintAttackPositions()
    {
        foreach (var kvp in usedWaitPositions)
        {
            Debug.Log($"{kvp.Key.name} & {kvp.Value}");
        }
    }

    [ContextMenu("Print Attack Positions")]
    private void Debug_PrintWaitPositions()
    {
        foreach (var kvp in usedWaitPositions)
        {
            Debug.Log($"{kvp.Key.name} & {kvp.Value}");
        }
    }

    private IEnumerator CR_QueriesHandler()
    {
        yield return new WaitForSeconds(0.7f);

        playerPos = Player.Instance.gameObject.transform.position;

        if (getAttackStateQueue.Count != 0)
        {
            for (int i = getAttackStateQueue.Count - 1; i >= 0; i--)
            {
                if (attackingEnemies.Count < 3)
                {
                    attackingEnemies.Add(getAttackStateQueue[i]);

                    getAttackStateQueue[i].ChangeState(State.Attack);
                    getAttackStateQueue.RemoveAt(i);
                }
            }
        }

        if (getAttackPositionQueue.Count != 0)
        {
            for (int i = getAttackPositionQueue.Count - 1; i >= 0; i--)
            {
                Vector3 pos = GetAttackPosition(getAttackPositionQueue[i]);

                getAttackPositionQueue[i].SetAttackPosition(pos);
                getAttackPositionQueue.RemoveAt(i);
            }
        }

        if (getWaitPositionQueue.Count != 0)
        {
            for (int i = getWaitPositionQueue.Count - 1; i >= 0; i--)
            {
                Vector3 pos = GetWaitingPosition(getWaitPositionQueue[i]);
                getWaitPositionQueue[i].SetWaitPosition(pos);
                getWaitPositionQueue.RemoveAt(i);
            }
        }

        StartCoroutine(CR_QueriesHandler());
    }

    #endregion QUERIES

    public void AddEnemyToArea(EnemyBase enemy)
    {
        aliveEnemies.Add(enemy);
    }

    public void RemoveEnemyFromArea(EnemyBase enemy)
    {
        aliveEnemies.Remove(enemy);
    }

    private Vector3 GetWaitingPosition(EnemyBase enemy)
    {
        int positions = attackingEnemies.Count == 3 ? enemiesToSpawn.Length - 3 : enemiesToSpawn.Length;

        for (int i = 0; i < positions; i++)
        {
            float angle = (360f / positions) * i;

            Vector3 offsetPos = Quaternion.Euler(0, angle, 0f) * Vector3.forward * 8f;
            Vector3 waitPos = playerPos + offsetPos;

            if (!usedWaitPositions.ContainsValue(waitPos))
            {
                usedWaitPositions[enemy] = waitPos;
                return waitPos;
            }
        }

        float incrementedDistance = waitPositionDistance + Random.Range(7f, 9f);
        float fallbackAngle = (360f / positions * 2) * Random.Range(1, (positions * 2) + 1);
        Vector3 fallBackPos = Quaternion.Euler(0f, fallbackAngle, 0) * Vector3.forward * incrementedDistance;
        fallBackPos = playerPos + fallBackPos;

        return fallBackPos;
    }

    private Vector3 GetAttackPosition(EnemyBase enemy)
    {
        int positions = 3;

        for (int i = 0; i < positions; i++)
        {
            float angle = (360 / positions) * i;

            //Make a variable that holds info of all the enemies positions
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * enemy.stoppingDistance;
            Vector3 attackPos = playerPos + offset;

            if (!usedAttackPositions.ContainsValue(attackPos))
            {
                usedAttackPositions[enemy] = attackPos;
                return attackPos;
            }
        }

        Vector3 fallback = playerPos * 2f;
        return fallback;
    }

    #endregion AREA

    #region ATTACK

    public int GetTotalAttackingEnemies()
    {
        return attackingEnemies.Count;
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
            if (enteredArea) return;
            enteredArea = true;

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