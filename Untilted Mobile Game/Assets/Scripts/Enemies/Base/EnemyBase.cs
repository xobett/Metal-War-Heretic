using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    protected bool isExecutingAttack;

    [SerializeField] protected bool isAttacking;

    [Header("MOVEMENT SETTINGS")]
    [SerializeField] protected float walkSpeed = 1.5f;
    [SerializeField] protected float stoppingDistance;

    protected Quaternion currentFacePlayerRot;

    private float lastYRotation;

    [Header("NAVIGATION SETTINGS")]
    [SerializeField] private LayerMask whatIsCollision;
    private Vector3 nextPos;

    [SerializeField] private bool isMoving;

    [SerializeField] private float navigationTimer;
    [SerializeField] private float maxNavigationTime = 12f;

    private Coroutine navigationActiveCoroutine;

    private void Awake()
    {
        GetReferences();
        SetEnemySettings();
    }

    private void Start()
    {
        //navigationActiveCoroutine = StartCoroutine(AssignWaitPosition());
        //isAttacking = true;
        lastYRotation = transform.rotation.eulerAngles.y;
    }

    protected virtual void Update()
    {
        LookAtPlayer();
        GetCurrentPlayerRot();

        if (isAttacking)
        {
            //ATTACK BEHAVIOUR
            AttackTriggerCheck();
            FollowPlayer();
        }
        else
        {
            //NAVIGATION BEHAVIOUR
            //RunNavigationTimer();
            //HandleStuckNavigation();
            //HandleEnemyExitingArea();
        }
    }

    #region BEHAVIOUR CHECK

    private void GetBehaviour()
    {
        if (!isMoving && EnemyManager.instance.ActiveAttackingEnemiesCount < 3 && !isAttacking)
        {
            EnemyManager.instance.AddAttackingEnemy(this);
            SetAttackBehaviourSettings();
            isAttacking = true;
        }
    }

    #endregion BEHAVIOUR CHECK


    #region ON DAMAGE AND DESTROY

    public void OnDamage(float damage)
    {
        GetComponent<Health>().TakeDamage(damage);

        if (isMoving)
        {

        }
    }

    private void OnDestroy()
    {
        if (isAttacking && EnemyManager.instance != null)
        {
            EnemyManager.instance.RemoveAttackingEnemy(this);
        }
    }

    #endregion ON DAMAGE AND DESTROY

    #region ATTACK

    private void SetAttackBehaviourSettings()
    {
        agent.speed = walkSpeed;
        agent.stoppingDistance = stoppingDistance;
    }

    private void SetNonAttackBehaviourSettings()
    {
        agent.speed = walkSpeed * 2;
        agent.stoppingDistance = 0;
    }

    private void AttackTriggerCheck()
    {
        if (agent.remainingDistance <= stoppingDistance && CheckPlayerIsNear() && !isExecutingAttack)
        {
            StartCoroutine(StartAttack());
        }
    }

    private IEnumerator StartAttack()
    {
        //Briefly stops and waits before executing its attack
        isExecutingAttack = true;

        Debug.Log("Stopped");
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

        isExecutingAttack = false;
    }

    public void HitPlayer(Collider playerCollider)
    {
        playerCam.CameraShake();
        player.GetComponent<Health>().TakeDamage(damage);
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
        if (!isExecutingAttack && !isMoving)
        {
            agent.destination = player.transform.position;
        }
    }

    protected virtual void LookAtPlayer()
    {
        if (!isMoving)
        {
            transform.rotation = currentFacePlayerRot;

            float currentYRotation = transform.eulerAngles.y;
            float deltaAngle = Mathf.DeltaAngle(lastYRotation, currentYRotation);
            if (Mathf.Abs(deltaAngle) > 0.1f)
            {
                if (deltaAngle > 0)
                {
                    Debug.Log("Its rotating to the right");
                }
                else if (deltaAngle < 0)
                {
                    Debug.Log("Its rotating to the left");
                }
            }

            lastYRotation = transform.eulerAngles.y;
        }
    }

    private void GetCurrentPlayerRot()
    {
        Vector3 lookDirection = player.transform.position - transform.position;
        Quaternion lookTarget = Quaternion.LookRotation(lookDirection);
        currentFacePlayerRot = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);
    }

    #endregion MOVEMENT AND ROTATION

    #region AI NAVIGATION
    private void RunNavigationTimer()
    {
        navigationTimer -= Time.deltaTime;
    }

    private IEnumerator AssignWaitPosition()
    {
        navigationTimer = maxNavigationTime;
        isMoving = true;

        Vector3 calculatedPos = EnemyManager.instance.AssignMovingPosition();

        int attemptsDone = 0;
        while (Physics.CheckSphere(calculatedPos, agent.radius, whatIsCollision))
        {
            if (attemptsDone >= 2)
            {
                EnemyManager.instance.IncreaseAreaRadius();
            }

            calculatedPos = EnemyManager.instance.AssignMovingPosition();
            attemptsDone++;
            Debug.Log("New position was generated");
            yield return null;
        }

        nextPos = calculatedPos;
        agent.destination = nextPos;
        //Waits until remaining distance is called and until enemy arrives to its destination
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
            time += Time.deltaTime * 0.6f;
            yield return null;
        }
        transform.rotation = currentFacePlayerRot;

        //Stops the enemy upon arrival
        agent.speed = 0;
        isMoving = false;
        Debug.Log("Set to false");

        GetBehaviour();

        yield return null;
    }

    private void HandleStuckNavigation()
    {
        if (navigationTimer < 0 && isMoving)
        {
            StopCoroutine(navigationActiveCoroutine);
            agent.speed += 1.5f;
            navigationActiveCoroutine = StartCoroutine(AssignWaitPosition());
        }
    }

    private void HandleEnemyExitingArea()
    {
        if (!isMoving && Vector3.Distance(transform.position, player.transform.position) > EnemyManager.instance.AreaRadius)
        {
            agent.speed = walkSpeed * 2;
            StartCoroutine(AssignWaitPosition());
        }
    }

    #endregion AI NAVIGATION

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
        agent.stoppingDistance = 1;
        agent.autoBraking = false;
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
    }

    #endregion DEBUG VISUAL GIZMOS
}
