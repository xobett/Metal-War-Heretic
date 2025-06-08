using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("--- ENEMY BASE SETTINGS ---\n")]

    [Header("PLAYER REFERENCES")]
    [SerializeField] protected LayerMask whatIsPlayer;
    protected GameObject player;
    [SerializeField] private float detectionRadius;

    [Header("ATTACK SETTINGS")]
    [SerializeField] protected float damage;

    [SerializeField, Range(1f, 6f)] protected int timeBetweenAttacks;

    private const float playerDetection_Range = 3f;
    protected bool isAttacking;
    private bool ableToAttack;

    [Header("MOVEMENT SETTINGS")]
    [SerializeField] protected float walkSpeed = 1.5f;
    [SerializeField] protected float stoppingDistance;

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
            player.GetComponent<Health>().TakeDamage(damage);
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
        Vector3 lookDirection = player.transform.position - transform.position;
        Quaternion lookTarget = Quaternion.LookRotation(lookDirection);
        Quaternion lookRotation = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);

        transform.rotation = lookRotation;
    }

    #endregion MOVEMENT AND ROTATION

    #region START

    private void GetReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        return Physics.CheckSphere(transform.position, detectionRadius, whatIsPlayer);
    }

    #endregion PLAYER DETECTION 

    #region DEBUG VISUAL GIZMOS

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    #endregion DEBUG VISUAL GIZMOS
}
