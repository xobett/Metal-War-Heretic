using EnemyAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    [Header("AREA ENEMY SETTINGS")]
    [SerializeField] private GameObject[] enemiesToSpawn;
    [SerializeField] private GameObject vfxEnemySpawn;

    private LayerMask whatIsEnemy;

    [Header("LOCKED DOOR SETTINGS")]
    [SerializeField] private GameObject[] lockedDoors;

    [Header("SPAWN SETTINGS")]
    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private float areaRadius = 10f;

    private List<Enemy> aliveEnemies = new List<Enemy>();
    [SerializeField] private List<Enemy> attackingEnemies = new List<Enemy>();

    //QUEUES
    [SerializeField] private List<Enemy> getWaitPositionQueue = new List<Enemy>();
    [SerializeField] private List<Enemy> getAttackPositionQueue = new List<Enemy>();
    [SerializeField] private List<Enemy> getAttackStateQueue = new List<Enemy>();

    private Dictionary<Enemy, Vector3> usedWaitPositions = new Dictionary<Enemy, Vector3>();
    private Dictionary<Enemy, Vector3> usedAttackPositions = new Dictionary<Enemy, Vector3>();

    private const float waitPositionDistance = 9.0f;

    private Vector3 playerPos;

    private bool enteredArea = false;

    private void Start()
    {
        whatIsEnemy = LayerMask.GetMask("Enemy");
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

            AddEnemyToArea(enemy.GetComponent<Enemy>());
            enemy.GetComponent<Enemy>().AssignArea(this);

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
        foreach (Enemy enemy in aliveEnemies)
        {
            enemy.UpdatedPosition = false;
        }
    }

    #region DEBUG

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

    #endregion DEBUG

    #region QUERIES

    public void OnForced_AddAttackingEnemy(Enemy enemy)
    {
        attackingEnemies.Add(enemy);
        RemoveRandomEnemy();
    }

    public void RemoveRandomEnemy()
    {
        Enemy removedEnemy = attackingEnemies[0];
        removedEnemy.OnForced_TransitionToQueue();
        Invoke(nameof(RepositionEnemies), 1f);
    }

    public void RepositionEnemies()
    {
        foreach (Enemy enemy in aliveEnemies)
        {
            if (enemy.currentState == State.OnQueue)
            {
                enemy.QueryWaitPosition();
            }
        }
    }

    public void QueryAttackState(Enemy enemy)
    {
        if (getAttackStateQueue.Contains(enemy)) return;
        getAttackStateQueue.Add(enemy);
    }

    public void QueryWaitPosition(Enemy enemy)
    {
        OnQuery_RemoveUsedPositions(enemy);
        getWaitPositionQueue.Add(enemy);
    }

    public void QueryAttackPos(Enemy enemy)
    {
        OnQuery_RemoveUsedPositions(enemy);
        getAttackPositionQueue.Add(enemy);
    }

    private void OnQuery_RemoveUsedPositions(Enemy enemy)
    {
        RemoveWaitPos(enemy);
        RemoveAttackPos(enemy);
    }

    public void RemoveWaitPos(Enemy enemy)
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

    public void RemoveAttackPos(Enemy enemy)
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

    #endregion QUERIES

    private IEnumerator CR_QueriesHandler()
    {
        yield return new WaitForSeconds(0.7f);

        playerPos = Player.Instance.gameObject.transform.position;

        HandleAttackStateQueries();

        HandleGetAttackPosQueries();

        HandleGetWaitPosQueries();

        StartCoroutine(CR_QueriesHandler());
    }

    #region QUERY HANDLERS

    private void HandleGetWaitPosQueries()
    {
        if (getWaitPositionQueue.Count != 0)
        {
            for (int i = getWaitPositionQueue.Count - 1; i >= 0; i--)
            {
                Vector3 pos = GetWaitingPosition(getWaitPositionQueue[i]);
                getWaitPositionQueue[i].SetWaitPosition(pos);
                getWaitPositionQueue.RemoveAt(i);
            }
        }
    }

    private void HandleGetAttackPosQueries()
    {
        if (getAttackPositionQueue.Count != 0)
        {
            for (int i = getAttackPositionQueue.Count - 1; i >= 0; i--)
            {
                Vector3 pos = GetAttackPosition(getAttackPositionQueue[i]);

                getAttackPositionQueue[i].SetAttackPosition(pos);
                getAttackPositionQueue.RemoveAt(i);
            }
        }
    }

    private void HandleAttackStateQueries()
    {
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
    }

    #endregion QUERY HANDLERS

    private void UnlockLockedDoor()
    {
        if (lockedDoors.Length > 0)
        {
            Animator[] anims = new Animator[lockedDoors.Length];

            for (int i = 0; i < anims.Length; i++)
            {
                anims[i] = lockedDoors[i].GetComponentInChildren<Animator>();
            }

            foreach (Animator anim in anims)
            {
                anim.SetTrigger("Unlock");
            }
        }
    }

    private Vector3 GetWaitingPosition(Enemy enemy)
    {
        int positions = attackingEnemies.Count == 3 ? enemiesToSpawn.Length - 3 : enemiesToSpawn.Length;

        for (int i = 0; i < positions; i++)
        {
            float angle = (360f / positions) * i;

            Vector3 offsetPos = Quaternion.Euler(0, angle, 0f) * Vector3.forward * 8f;
            Vector3 waitPos = playerPos + offsetPos;

            if (!usedWaitPositions.ContainsValue(waitPos) && !Physics.CheckSphere(waitPos, 1f, whatIsEnemy))
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

    private Vector3 GetAttackPosition(Enemy enemy)
    {
        int positions = 3;
        float[] stoppingDistances = { 1.5f, 4.5f };

        for (int i = 0; i < positions; i++)
        {
            float angle = (360 / positions) * i;

            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * enemy.stoppingDistance;
            Vector3 attackPos = playerPos + offset;

            bool isInSameAngle = false;

            foreach (float sd in stoppingDistances)
            {
                Vector3 testPos = playerPos + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * sd;

                foreach (Vector3 assignedPos in usedAttackPositions.Values)
                {
                    Vector3 apDir = (assignedPos - playerPos).normalized;
                    Vector3 testDir = (testPos - playerPos).normalized;

                    float angleBetween = Vector3.Angle(apDir, testDir);

                    if (angleBetween < 1f)
                    {
                        isInSameAngle = true;
                        break;
                    }

                    if (isInSameAngle) break;
                }
            }

            if (!usedAttackPositions.ContainsValue(attackPos) && !isInSameAngle)
            {
                usedAttackPositions[enemy] = attackPos;
                return attackPos;
            }
        }

        float incrementedDistance = waitPositionDistance + Random.Range(7f, 9f);
        float fallbackAngle = (360f / positions * 2) * Random.Range(1, (positions * 2) + 1);
        Vector3 fallBackPos = Quaternion.Euler(0f, fallbackAngle, 0) * Vector3.forward * incrementedDistance;
        fallBackPos = playerPos + fallBackPos;

        return fallBackPos;
    }

    #endregion AREA

    #region MODIFY AREA

    public void AddEnemyToArea(Enemy enemy)
    {
        aliveEnemies.Add(enemy);
    }

    public void RemoveEnemyFromArea(Enemy enemy)
    {
        if (getAttackStateQueue.Contains(enemy)) getAttackStateQueue.Remove(enemy);

        RemoveAttackingEnemy(enemy);
        RemoveAttackPos(enemy);
        RemoveWaitPos(enemy);

        aliveEnemies.Remove(enemy);
        if (aliveEnemies.Count == 0)
        {
            UnlockLockedDoor();
        }
    }

    public int GetTotalAttackingEnemies()
    {
        return attackingEnemies.Count;
    }

    public void RemoveAttackingEnemy(Enemy enemy)
    {
        if (attackingEnemies.Contains(enemy))
        {
            attackingEnemies.Remove(enemy); 
        }
    }

    #endregion MODIFY AREA

    #region TRIGGERS

    private void OnTrigger_AttackPlayer()
    {
        foreach (Enemy enemy in aliveEnemies)
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

    #region VISUAL DEBUG

    private void OnDrawGizmos()
    {
        if (spawnOrigin == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnOrigin.position, areaRadius);
    }

    #endregion VISUAL DEBUG
}