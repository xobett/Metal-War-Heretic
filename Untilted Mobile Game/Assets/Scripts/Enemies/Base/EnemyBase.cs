using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("--- ENEMY BASE SETTINGS ---\n")]

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

    protected NavMeshAgent agent;

    private void Start()
    {
        GetReferences();
        SetEnemySettings();
    }

    protected virtual void Update()
    {
        FollowPlayer();
        LookAtPlayer();
        GetCurrentPlayerRot();
        BehaviourCheck();
    }

    #region ON DAMAGE

    public void OnDamage(float damage)
    {
        GetComponent<Health>().TakeDamage(damage);
    }

    #endregion ON DAMAGE

    #region ATTACK
    private void BehaviourCheck()
    {
        if (agent.velocity.magnitude == 0 &&  CheckPlayerIsNear() && !isAttacking)
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

            isAttacking = false;
        }
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
    }

    #endregion DEBUG VISUAL GIZMOS
}
