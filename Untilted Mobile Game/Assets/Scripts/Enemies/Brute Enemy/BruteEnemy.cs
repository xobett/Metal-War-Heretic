using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BruteEnemy : EnemyBase
{
    [Header("--- BRUTE ENEMY SETTINGS ---\n")]

    [Header("RAMPAGE RUN ABILITY\n")]

    [Header("RUN SPEED AND DAMAGE SETTINGS")]
    [SerializeField] private float rampageRunDamage;
    [SerializeField, Range(0.2f, 1f)] private float rampageRunHitForce;
    [SerializeField] private int rampageRunSpeed;
    [SerializeField] private int minimumDistanceToRun;

    private bool playerHit;

    private float playerDistance;

    [Header("TIMING SETTINGS")]
    [SerializeField, Range(2f, 6f)] private int beforeRunTime;
    [SerializeField, Range(2f, 6f)] private int runningTime;
    [SerializeField, Range(2f, 6f)] private int afterRunTime;

    [SerializeField, Range(10f, 15f)] private int rampageRunCooldown;

    private bool isRunning; // Used to state when its running, also used to stop rotating when doing so
    private bool runAbilityActive; // Used to avoid entering the run ability method continuosly, until the ability is completed
    private bool runCoolingDown; // Used to avoid the run ability method from executing when its cooling down

    #region BASE OVERRIDES

    protected override void Update()
    {
        base.Update();

        RampageRunMovement();
        RampageRunCheck();
    }

    #region BASE MOVEMENT AND ROTATION

    protected override void LookAtPlayer()
    {
        if (!isRunning)
        {
            base.LookAtPlayer(); 
        }
    }

    #endregion BASE MOVEMENT AND ROTATION

    #endregion BASE OVERRIDES

    #region RAMPAGE RUN
    private void RampageRunMovement()
    {
        if (isRunning)
        {
            agent.destination = agent.transform.position + agent.transform.forward * 5f; 
        }
    }

    private void RampageRunCheck()
    {
        playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (!runCoolingDown && !isAttacking)
        {
            //Checks if rampage run ability is not active, and distance is within range to trigger the ability
            if ((!runAbilityActive && !isRunning) && playerDistance > minimumDistanceToRun)
            {
                isAttacking = true;
                StartCoroutine(StartRampageRun());
            }
        }
    }

    private IEnumerator StartRampageRun()
    {
        runAbilityActive = true;
        runCoolingDown = true;

        Debug.Log("Entered");
        //Stops following the player for a period of time
        agent.destination = transform.position; 
        yield return new WaitForSeconds(beforeRunTime);

        //While looking at the players position, it moves towards it at a fast pace during a period of time.
        isRunning = true;
        agent.speed = rampageRunSpeed;
        yield return new WaitForSeconds(runningTime);

        //Sets speed to 0 and waits until the enemy slows down.
        agent.speed = 0f;
        yield return new WaitUntil(() => agent.velocity.magnitude == 0);

        //Looks smoothly at player before returning to its default state
        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
            time += Time.deltaTime * 0.6f;
            yield return null;
        }
        transform.rotation = currentFacePlayerRot;

        //Stops running and looks at player again but remains steady for a period of time.
        isRunning = false;
        agent.destination = transform.position;
        yield return new WaitForSeconds(afterRunTime);

        //Ability is no longer active and should follow player
        agent.speed = walkSpeed;
        runAbilityActive = false;
        isAttacking = false;

        StartCoroutine(StartRampageRunCooldown());
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (isRunning && !playerHit))
        {
            PushPlayer(rampageRunDamage);
            StopCoroutine(StartRampageRun());
            StartCoroutine(SlowDownPostHit());
        }
    }

    private IEnumerator SlowDownPostHit()
    {
        //Prevents the player from getting pushed again if slices against the enemy
        playerHit = true;
        
        //Slows down and Stops the enemy upon hit
        agent.speed = 0;
        agent.destination = transform.position;

        yield return new WaitUntil(() => agent.velocity.magnitude == 0);

        //In case that the player gets pushed away from the enemy's view, rotates it again to the player.
        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
            time += Time.deltaTime * 0.6f;
            yield return null;
        }
        transform.rotation = currentFacePlayerRot;

        isRunning = false;
        playerHit = false;
        yield return new WaitForSeconds(afterRunTime);

        agent.speed = walkSpeed;
        runAbilityActive = false;
        isAttacking = false;

        StartCoroutine(StartRampageRunCooldown());
        yield return null;
    }

    private IEnumerator StartRampageRunCooldown()
    {
        yield return new WaitForSeconds(rampageRunCooldown);
        runCoolingDown = false;

        yield return null;
    }

    #endregion RAMPAGE RUN
}
