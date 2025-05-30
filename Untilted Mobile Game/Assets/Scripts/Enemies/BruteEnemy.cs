using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BruteEnemy : EnemyBase
{

    [SerializeField] private float nearStoppingDistance = 1.85f;
    [SerializeField] private float farStoppingDistance = 8f;

    [SerializeField] private float runningTime;

    [SerializeField] private bool isRunning;

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
        float distance = Vector3.Distance(transform.position, player_transform.position);

        Vector3 direction = player_transform.position - transform.position;

        if (distance > 4)
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
        Vector3 direction = player_transform.position - transform.position;

        agent.destination = direction;

        yield return new WaitForSeconds(runningTime);
    }

}
