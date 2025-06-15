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

    private bool rmpPlayerHit;

    private float playerDistance;

    [Header("TIMING SETTINGS")]
    [SerializeField, Range(2f, 6f)] private int rmpBeforeRunTime;
    [SerializeField, Range(2f, 6f)] private int rmpRunningTime;
    [SerializeField, Range(2f, 6f)] private int rmpAfterRunTime;

    [SerializeField, Range(10f, 15f)] private int rmpCooldown;

    private bool rmpIsRunning; // Used to state when its running, also used to stop rotating when doing so
    private bool rmpAbilityActive; // Used to avoid entering the run ability method continuosly, until the ability is completed
    private bool rmpCoolingDown; // Used to avoid the run ability method from executing when its cooling down

    #region BASE OVERRIDES

    protected override void Update()
    {
        base.Update();

        RampageRunMovement();
        RampageRunTriggerCheck();
        GetDistanceFromPlayer();
    }

    #region BASE MOVEMENT AND ROTATION

    protected override void LookAtPlayer()
    {
        if (!rmpIsRunning)
        {
            base.LookAtPlayer(); 
        }
    }

    #endregion BASE MOVEMENT AND ROTATION

    #endregion BASE OVERRIDES

    #region RAMPAGE RUN

    private void GetDistanceFromPlayer()
    {
        playerDistance = Vector3.Distance(transform.position, player.transform.position);
    }

    private void RampageRunMovement()
    {
        if (rmpIsRunning)
        {
            agent.destination = agent.transform.position + agent.transform.forward * 5f; 
        }
    }

    private void RampageRunTriggerCheck()
    {
        if (!rmpCoolingDown && !isAttacking)
        {
            //Checks if rampage run ability is not active, and distance is within range to trigger the ability
            if ((!rmpAbilityActive && !rmpIsRunning) && playerDistance > minimumDistanceToRun)
            {
                isAttacking = true;
                StartCoroutine(StartRampageRun());
            }
        }
    }

    private IEnumerator StartRampageRun()
    {
        rmpAbilityActive = true;
        rmpCoolingDown = true;

        Debug.Log("Entered");
        //Stops following the player for a period of time
        agent.destination = transform.position; 
        yield return new WaitForSeconds(rmpBeforeRunTime);

        //While looking at the players position, it moves towards it at a fast pace during a period of time.
        rmpIsRunning = true;
        agent.speed = rampageRunSpeed;
        yield return new WaitForSeconds(rmpRunningTime);

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
        rmpIsRunning = false;
        agent.destination = transform.position;
        yield return new WaitForSeconds(rmpAfterRunTime);

        //Ability is no longer active and should follow player
        agent.speed = walkSpeed;
        rmpAbilityActive = false;
        isAttacking = false;

        StartCoroutine(StartRampageRunCooldown());
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (rmpIsRunning && !rmpPlayerHit))
        {
            //Prevents the player from getting pushed again if slices against the enemy
            rmpPlayerHit = true;

            PushPlayer(rampageRunDamage);
            StopCoroutine(StartRampageRun());
            StartCoroutine(SlowDownPostHit());
        }
    }

    private IEnumerator SlowDownPostHit()
    {        
        //Slows down and Stops the enemy upon hit
        agent.speed = 0;
        agent.destination = transform.position;

        //In case that the player gets pushed away from the enemy's view, rotates it again to the player.
        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
            time += Time.deltaTime * 0.6f;
            yield return null;
        }
        transform.rotation = currentFacePlayerRot;

        rmpIsRunning = false;
        rmpPlayerHit = false;
        yield return new WaitForSeconds(rmpAfterRunTime);

        agent.speed = walkSpeed;
        rmpAbilityActive = false;
        isAttacking = false;

        StartCoroutine(StartRampageRunCooldown());
        yield return null;
    }

    private IEnumerator StartRampageRunCooldown()
    {
        yield return new WaitForSeconds(rmpCooldown);
        rmpCoolingDown = false;

        yield return null;
    }

    #endregion RAMPAGE RUN
}
