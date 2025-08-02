using EnemyAI;
using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyBase enemy) : base(enemy) { }

    private bool transitionedToQueue = false;

    public override void Enter()
    {
        Enter_SetChaseSettings();
        enemy.QueryWaitPosition();
    }

    #region ENTER

    private void Enter_SetChaseSettings()
    {
        enemy.agent.speed = enemy.runSpeed;
        enemy.agent.stoppingDistance = 0;

        enemy.ableToFace = false;

        transitionedToQueue = false;
    }

    #endregion ENTER

    public override void Update()
    {
        Update_MoveToWaitPos();

        Update_OnArriveToDestination();

        enemy.currentState = State.Chase;

    }

    #region UPDATE

    private void Update_MoveToWaitPos()
    {
        if (enemy.waitingPos != Vector3.zero)
        {
            enemy.agent.destination = enemy.waitingPos;
        }
    }

    private void Update_OnArriveToDestination()
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

    #endregion UPDATE

    public override void Exit()
    {

    }
}