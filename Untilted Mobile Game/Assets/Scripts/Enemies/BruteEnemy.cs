using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BruteEnemy : EnemyBase
{
    [Header("--- BRUTE ENEMY ---\n")]

    [Header("MAIN ABILITY SETTINGS")]
    [SerializeField, Range(2f, 6f)] private int beforeRunTime;
    [SerializeField] private float runningTime;

    private float playerDistance;

    private bool isRunning;
    private bool abilityActive;

    protected override void Update()
    {
        if (!isRunning)
        {
            base.Update(); 
        }

        Behaviour();
    }

    private void Behaviour()
    {
        playerDistance = Vector3.Distance(transform.position, player_transform.position);

        Vector3 direction = player_transform.position - transform.position;

        if (playerDistance > 4)
        {
            isRunning = true;
            agent.destination = agent.transform.position + agent.transform.forward * 2f;
        }
        else
        {
            isRunning = false;
        }
    }

    private IEnumerator RampageRun()
    {
        yield return new WaitForSeconds(beforeRunTime);
    }

    private void OnDrawGizmos()
    {
        if (player_transform != null)
        {
            if (playerDistance > 4)
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
