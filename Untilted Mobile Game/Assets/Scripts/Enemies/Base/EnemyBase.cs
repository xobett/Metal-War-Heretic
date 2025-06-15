using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("--- ENEMY BASE SETTINGS ---\n")]
    protected NavMeshAgent agent;

    [Header("PLAYER REFERENCES")]
    [SerializeField] protected LayerMask whatIsPlayer;
    protected GameObject player;
    [SerializeField] private float playerDetectionRadius;

    [Header("CAMERA REFERENCES")]
    protected PlayerCamera playerCam; 

    [Header("ATTACK SETTINGS")]
    [SerializeField] protected float damage;

    [SerializeField, Range(1f, 6f)] protected int timeBetweenAttacks;

    private const float playerDetection_Range = 3f;
    protected bool isAttacking;

    [Header("MOVEMENT SETTINGS")]
    [SerializeField] protected float walkSpeed = 1.5f;
    [SerializeField] protected float stoppingDistance;

    protected Quaternion currentFacePlayerRot;

    [Header("NAVIGATION SETTINGS")]
    [SerializeField] private LayerMask whatIsCollision;
    [SerializeField] private float areaRadius = 10f;
    private Vector3 nextPos;


    private void Start()
    {
        GetReferences();
        SetEnemySettings();
        StartCoroutine(AssignWaitPosition());
    }

    protected virtual void Update()
    {
        LookAtPlayer();
        GetCurrentPlayerRot();

        //Attack mode
        //BehaviourCheck();
        //FollowPlayer();
    }

    #region ON DAMAGE

    public void OnDamage(float damage)
    {
        GetComponent<Health>().TakeDamage(damage);
    }

    #endregion ON DAMAGE

    #region ATTACK
    private void AttackTriggerCheck()
    {
        if (agent.remainingDistance <= stoppingDistance && CheckPlayerIsNear() && !isAttacking)
        {
            StartCoroutine(StartAttack());
        }
    }

    private IEnumerator StartAttack()
    {
        Debug.Log("Stopped");
        //Briefly stops and waits before executing its attack
        isAttacking = true;
        yield return new WaitForSeconds(timeBetweenAttacks);

        Debug.Log("Start attack");
        Attack();
    }

    protected virtual void Attack()
    {
        //Default behaviour for melee attack type enemies
        if (Physics.Raycast(transform.position, transform.forward * playerDetection_Range, playerDetection_Range, whatIsPlayer))
        {
            playerCam.CameraShake();
            player.GetComponent<Health>().TakeDamage(damage);
        }

        isAttacking = false;
    }

    protected void PushPlayer(float damageUponHit)
    {
        playerCam.CameraShake();

        player.GetComponent<Health>().TakeDamage(damageUponHit);

        float randomSideValue = Random.Range(-2f, 2f);
        float randomTimePushed = Random.Range(0.4f, 0.5f);
        float randomPushedForce = Random.Range(7f, 8f);

        Vector3 pushedDirection = transform.forward + transform.right * randomSideValue;

        player.GetComponent<PlayerMovement>().SetHitMovement(pushedDirection, randomPushedForce, randomTimePushed);
    }


    #endregion ATTACK

    #region MOVEMENT AND ROTATION

    protected virtual void FollowPlayer()
    {
        if (!isAttacking)
        {
            agent.destination = player.transform.position; 
        }
    }

    protected virtual void LookAtPlayer()
    {
        transform.rotation = currentFacePlayerRot;
    }

    private void GetCurrentPlayerRot()
    {
        Vector3 lookDirection = player.transform.position - transform.position;
        Quaternion lookTarget = Quaternion.LookRotation(lookDirection);
        currentFacePlayerRot = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);
    }

    #endregion MOVEMENT AND ROTATION

    #region POSITION GETTER

    [ContextMenu("Test Check")]
    void TestCheck()
    {
        StartCoroutine(AssignWaitPosition());
    }

    private IEnumerator AssignWaitPosition()
    {
        Vector3 calculatedPos = GetRandomPosition();

        int attemptsDone = 0;
        while (Physics.CheckSphere(calculatedPos, 3, whatIsCollision))
        {
            if (attemptsDone >= 3)
            {
                areaRadius += 0.5f;
                Debug.Log("Area was incremented");
            }

            calculatedPos = GetRandomPosition();
            attemptsDone++;
            Debug.Log("New position was generated");
            yield return null;
        }

        nextPos = calculatedPos;

        agent.destination = nextPos;

        yield return new WaitForSeconds(10);

        StartCoroutine(AssignWaitPosition());
        yield return null;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 pos = Random.insideUnitSphere * areaRadius;
        pos.y = 0;
        pos = player.transform.position + pos;
        return pos;
    }

    #endregion POSITION GETTER

    #region START

    private void GetReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCam = Camera.main.GetComponent<PlayerCamera>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void SetEnemySettings()
    {
        agent.speed = walkSpeed;
        agent.stoppingDistance = stoppingDistance;
    }

    #endregion START

    #region PLAYER DETECTION

    private bool CheckPlayerIsNear()
    {
        return Physics.CheckSphere(transform.position, playerDetectionRadius, whatIsPlayer);
    }

    #endregion PLAYER DETECTION 

    #region DEBUG VISUAL GIZMOS

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.transform.position, areaRadius); 
        }
    }

    #endregion DEBUG VISUAL GIZMOS
}
