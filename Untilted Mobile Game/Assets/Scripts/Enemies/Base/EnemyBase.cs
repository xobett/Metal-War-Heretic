using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("ENEMY ATTACK SETTINGS")]
    [SerializeField] protected float damage;
    [SerializeField] protected float attackCooldown;

    private const float playerDetection_Range = 2f;

    [SerializeField] protected LayerMask whatIsPlayer;

    private bool isAttacking;

    [Header("ENEMY MOVEMENT SETTINGS")]
    [SerializeField] protected const float walkSpeed = 1.5f;

    [SerializeField] protected float stoppingDistance = 1.85f;

    protected NavMeshAgent agent;

    //Player references
    protected Transform player_transform;

    private void Start()
    {
        GetReferences();
        SetAgentSettings();
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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward * playerDetection_Range, out hit, playerDetection_Range, whatIsPlayer))
        {
            hit.collider.GetComponent<Health>().TakeDamage(damage);
        }
    }

    private IEnumerator StartBehaviour()
    {
        isAttacking = true;

        Attack();

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

        yield return null;
    }

    //Movement and rotation Methods

    protected void FollowPlayer()
    {
        agent.destination = player_transform.position;

        if (agent.velocity.magnitude == 0 && !isAttacking)
        {
            StartCoroutine(StartBehaviour());
        }
    }

    private void LookAtPlayer()
    {
        Vector3 lookDirection = player_transform.position - transform.position;
        Quaternion lookTarget = Quaternion.LookRotation(lookDirection);
        Quaternion lookRotation = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);

        transform.rotation = lookRotation;
    }


    //Start Methods

    private void GetReferences()
    {
        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void SetAgentSettings()
    {
        agent.speed = walkSpeed;
        agent.stoppingDistance = stoppingDistance;
    }

}
