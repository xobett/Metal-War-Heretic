using Microsoft.Win32.SafeHandles;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BruteEnemy : EnemyBase
{
    [Header("--- BRUTE SETTINGS ---")]

    [SerializeField, Range(2f, 6f)] private int beforeRunTime;
    [SerializeField, Range(2f, 6f)] private int runningTime;
    [SerializeField, Range(2f, 6f)] private int runCooldownTime;

    [SerializeField] private int minimumDistanceToRun;
    private float playerDistance;

    [SerializeField] private bool isRunning;
    private bool runAbilityActive;

    private Quaternion currentLookAtPlayer;

    protected override void Update()
    {
        base.Update();

        RampageRun();
        RampageRunCheck();
        GetCurrentLookAtPlayerRot();
    }

    private void GetCurrentLookAtPlayerRot()
    {
        if (runAbilityActive)
        {
            Vector3 lookDirection = player_transform.position - transform.position;
            Quaternion lookTarget = Quaternion.LookRotation(lookDirection);
            currentLookAtPlayer = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0); 
        }
    }

    protected override void FollowPlayer()
    {
        if (!runAbilityActive)
        {
            base.FollowPlayer(); 
        }
    }

    protected override void LookAtPlayer()
    {
        if (!isRunning)
        {
            base.LookAtPlayer(); 
        }
    }

    private void RampageRunCheck()
    {
        playerDistance = Vector3.Distance(transform.position, player_transform.position);

        Vector3 direction = player_transform.position - transform.position;

        if ((!runAbilityActive && !isRunning) && playerDistance > minimumDistanceToRun)
        {
            runAbilityActive = true;
            StartCoroutine(StartRampageRun());
        }
    }

    private IEnumerator StartRampageRun()
    {
        Debug.Log("Entered");
        //Stops following the player for a period of time
        agent.destination = transform.position; 
        yield return new WaitForSeconds(beforeRunTime);

        //While looking at the players position, it moves towards it at a fast pace during a period of time.
        isRunning = true;
        agent.speed = 15f;
        yield return new WaitForSeconds(runningTime);

        //Sets speed to 0 and waits until the enemy slows down.
        agent.speed = 0f;
        yield return new WaitUntil(() => agent.velocity.magnitude == 0);

        //Looks smoothly at player before returning to its default state
        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentLookAtPlayer, time);
            time += Time.deltaTime * 0.6f;
            yield return null;
        }
        transform.rotation = currentLookAtPlayer;

        //Stops running and looks at player again but remains steady for a period of time.
        isRunning = false;
        agent.destination = transform.position;
        yield return new WaitForSeconds(runCooldownTime);

        runAbilityActive = false;
    }

    private void RampageRun()
    {
        if (isRunning)
        {
            agent.destination = agent.transform.position + agent.transform.forward * 5f; 
        }
    }

    private void OnDrawGizmos()
    {
        if (player_transform != null)
        {
            if (playerDistance > minimumDistanceToRun)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(transform.position, player_transform.position); 
        }
    }
}
