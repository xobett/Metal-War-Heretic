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

    [Header("ATTACK SETTINGS\n")]
    [SerializeField] protected float damage;
    [SerializeField] protected float attackCooldown;
    [SerializeField, Range(1f, 4f)] protected float beforeAttackTime;

    private const float playerDetection_Range = 2f;
    private bool isAttacking;
    private bool ableToAttack;

    [Header("MOVEMENT SETTINGS")]
    [SerializeField] protected float walkSpeed = 1.5f;
    [SerializeField] protected float stoppingDistance;

    protected NavMeshAgent agent;

    private void Start()
    {
        GetReferences();
        SetAgentSettings();
        StartCoroutine(ToggleAttackStatus());
    }

    private IEnumerator ToggleAttackStatus()
    {
        //To avoid enemy dealing damage at its spawn time, adds a time before letting it attack.
        yield return new WaitForSeconds(2);
        ableToAttack = true;
    }

    protected virtual void Update()
    {
        FollowPlayer();
        LookAtPlayer();
    }

    //Attack Methods

    public void OnDamage(float damage)
    {
        GetComponent<Health>().TakeDamage(damage);
    }

    protected virtual void Attack()
    {
        //Default behaviour for melee attack type enemies
        if (Physics.Raycast(transform.position, transform.forward * playerDetection_Range, playerDetection_Range, whatIsPlayer))
        {
            player.GetComponent<Health>().TakeDamage(damage);
        }
    }

    private IEnumerator StartBehaviour()
    {
        isAttacking = true;
        yield return new WaitForSeconds(beforeAttackTime);

        Attack();
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;

        yield return null;
    }

    //Movement and rotation Methods

    protected virtual void FollowPlayer()
    {
        agent.destination = player.transform.position;

        if (agent.velocity.magnitude == 0 && !isAttacking)
        {
            if (!ableToAttack) return;
            StartCoroutine(StartBehaviour());
        }
    }

    protected virtual void LookAtPlayer()
    {
        Vector3 lookDirection = player.transform.position - transform.position;
        Quaternion lookTarget = Quaternion.LookRotation(lookDirection);
        Quaternion lookRotation = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);

        transform.rotation = lookRotation;
    }


    //Start Methods

    private void GetReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    private void SetAgentSettings()
    {
        agent.speed = walkSpeed;
        agent.stoppingDistance = stoppingDistance;
    }

}
