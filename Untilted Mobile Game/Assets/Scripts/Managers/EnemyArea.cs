using EnemyAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State = EnemyAI.State;

public class EnemyArea : MonoBehaviour
{
    [Header("ENEMY PREFABS")]
    [SerializeField] private GameObject hammerEnemyPf;
    [SerializeField] private GameObject electricEnemyPf;
    [SerializeField] private GameObject shieldEnemyPf;
    [SerializeField] private GameObject bruteEnemyPf;

    [Header("AREA ENEMY SETTINGS")]
    [SerializeField] private GameObject[] enemiesToSpawn;

    private LayerMask whatIsEnemy;

    [Header("LOCKED DOOR SETTINGS")]
    [SerializeField] private GameObject lockedDoor;
    [SerializeField] private SOShakeData openDoorShake;

    [Header("SPAWN SETTINGS")]
    [SerializeField] private GameObject vfxEnemySpawn;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private SOShakeData respawnPosShake;
    [SerializeField] private SOShakeData respawnRotShake;

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
    float[] stoppingDistances = { 1.5f, 4.5f };

    private Vector3 playerPos;

    private bool enteredArea = false;
    internal bool playerIsOnArea;

    private void Start()
    {
        Start_GetReferences();
        Start_SpawnEnemies();
        Start_RunQueriesHandler();
    }

    #region START

    private void Start_GetReferences()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        whatIsEnemy = LayerMask.GetMask("Enemy");
    }

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

    #region RESPAWN

    public void RespawnEnemy(Enemy enemy, float respawnTime)
    {
        StartCoroutine(CR_RespawnEnemy(enemy, respawnTime));
    }

    private IEnumerator CR_RespawnEnemy(Enemy enemy, float timeBeforeRespawn)
    {
        yield return new WaitForSeconds(timeBeforeRespawn);

        if (aliveEnemies.Count == 0) yield break;

        Vector3 spawnPos = GetRandomSpawnPos();

        GameObject vfx = Instantiate(vfxEnemySpawn, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
        Destroy(vfx, 1.5f);

        audioSource.clip = AudioManager.Instance.GetClip("SPAWN ENEMIGOS");
        audioSource.Play();

        ShakeEventManager.Instance.AddShakeEvent(respawnPosShake);
        ShakeEventManager.Instance.AddShakeEvent(respawnRotShake);

        GameObject enemyPf = Instantiate(GetEnemyPf(enemy), spawnPos, Quaternion.identity);

        Enemy respawnedEnemy = enemyPf.GetComponent<Enemy>();
        AddEnemyToArea(respawnedEnemy);
        respawnedEnemy.AssignArea(this);
        respawnedEnemy.OnRespawn();

        enemyPf.transform.parent = transform.GetChild(0);

        yield break;
    }

    private GameObject GetEnemyPf(Enemy enemy)
    {
        GameObject enemyGo = null;

        switch (enemy.enemyType)
        {
            case EnemyType.Hammer:
                {
                    enemyGo = hammerEnemyPf;
                    break;
                }
            case EnemyType.Electric:
                {
                    enemyGo = electricEnemyPf;
                    break;
                }
            case EnemyType.Shield:
                {
                    enemyGo = shieldEnemyPf;
                    break;
                }
            case EnemyType.Brute:
                {
                    enemyGo = bruteEnemyPf;
                    break;
                }
        }

        return enemyGo;
    }

    #endregion RESPAWN

    #region AREA

    #region QUERIES

    public int UsedAttackPosCount => usedAttackPositions.Count;

    public void ResetUpdatePositionValue()
    {
        foreach (Enemy enemy in aliveEnemies)
        {
            enemy.UpdatedPosition = false;
        }
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

    #endregion QUERIES

    #region QUERY HANDLERS

    private IEnumerator CR_QueriesHandler()
    {
        yield return new WaitForSeconds(0.7f);

        playerPos = Player.Instance.gameObject.transform.position;

        HandleAttackStateQueries();

        HandleGetAttackPosQueries();

        HandleGetWaitPosQueries();

        StartCoroutine(CR_QueriesHandler());
    }

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
    
    public void RemoveAttackingEnemy(Enemy enemy)
    {
        if (attackingEnemies.Contains(enemy))
        {
            attackingEnemies.Remove(enemy);
        }
    }

    public int GetTotalAttackingEnemies()
    {
        return attackingEnemies.Count;
    }

    public void OnForced_AddAttackingEnemy(Enemy forcedAddedEnemy)
    {
        Debug.Log("Entered");
        attackingEnemies.Add(forcedAddedEnemy);
        RemoveRandomEnemy(forcedAddedEnemy);
    }

    public void RemoveRandomEnemy(Enemy forcedAddedEnemy)
    {
        if (attackingEnemies.Count == 1)
        {
            Debug.Log("RETURNED");
            return;
        }

        Enemy removedEnemy = null;

        foreach(Enemy enemy in attackingEnemies)
        {
            if (enemy != forcedAddedEnemy)
            {
                Debug.Log($"Removed {enemy.gameObject.name}");
                removedEnemy = enemy;
                break;
            }
        }

        Invoke(nameof(RepositionEnemies), 1f);
    }

    private void UnlockLockedDoor()
    {
        if (lockedDoor != null)
        {
            Animator[] anims = new Animator[2];

            anims[0] = lockedDoor.transform.GetChild(0).GetComponent<Animator>();
            anims[1] = lockedDoor.transform.GetChild(1).GetComponent<Animator>();

            foreach (Animator anim in anims)
            {
                anim.SetTrigger("Unlock");
            }
        }

        AudioManager.Instance.PlayDelayedDoorSound();
        ShakeEventManager.Instance.AddShakeEvent(openDoorShake);
        Destroy(gameObject);
    }

    #endregion AREA

    #region TRIGGERS

    private void OnEnterArea_AttackPlayer()
    {
        foreach (Enemy enemy in aliveEnemies)
        {
            if (enemy.enemyType == EnemyType.Shield || enemy.enemyType == EnemyType.Brute) continue;
            enemy.ChangeState(State.OnQueue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOnArea = true;
            other.GetComponent<Player>().SetEnemyArea(this);

            if (enteredArea) return;
            enteredArea = true;

            Invoke(nameof(OnEnterArea_AttackPlayer), 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOnArea = false;
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