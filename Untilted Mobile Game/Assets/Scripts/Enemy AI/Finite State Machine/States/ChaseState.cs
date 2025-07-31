using EnemyAI;
using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyBase enemy) : base(enemy) { }

    private bool transitionedToQueue = false;

    public override void Enter()
    {
        Enter_SetChaseSettings();
        Enter_GetChasePosition();
    }

    #region ENTER

    private void Enter_SetChaseSettings()
    {
        enemy.agent.speed = enemy.runSpeed;
        enemy.agent.stoppingDistance = 0;

        enemy.ableToFace = false;

        transitionedToQueue = false;
    }

    private void Enter_GetChasePosition()
    {
        enemy.QueryWaitPosition();
    }

    #endregion ENTER

    public override void Update()
    {
        if (enemy.waitingPos != Vector3.zero)
        {
            enemy.agent.destination = enemy.waitingPos;
        }

        OnArriveToDestination();
    }

    private void OnArriveToDestination()
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.waitingPos);

        if (distance < 0.2f)
        {
            TransitionToAttackQueue();
        }
    }

    private void TransitionToAttackQueue()
    {
        if (transitionedToQueue) return;
        transitionedToQueue = true;
        enemy.ChangeState(State.OnQueue);
    }

    public override void Exit()
    {

    }
}