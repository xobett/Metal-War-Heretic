using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BruteEnemy : EnemyBase
{

    [SerializeField] private float nearStoppingDistance = 1.85f;
    [SerializeField] private float farStoppingDistance = 8f;

    [SerializeField] private float runningTime;

    protected override void Update()
    {
        base.Update();
        Behaviour();
    }

    private void Behaviour()
    {
        float distance = Vector3.Distance(transform.position, player_transform.position);

        Debug.Log(distance);
    }

    private void RampageRun()
    {
          
    }

    private IEnumerator StartRun()
    {
        Vector3 direction = player_transform.position - transform.position;

        agent.destination = direction;

        yield return new WaitForSeconds(runningTime);
    }

}
