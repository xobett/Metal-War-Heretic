using EnemyAI;
using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(Enemy enemy) : base(enemy) { }

    private bool transitionedToQueue = false;

    private GameObject player;

    public override void Enter()
    {
        player = Player.Instance.gameObject;
        Enter_SetChaseSettings();
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
        enemy.agent.destination = player.transform.position;
    }

    private void Update_OnArriveToDestination()
    {
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);

        if (distance < 3f)
        {
            TransitionToAttackQueue();
        }
    }

    private void TransitionToAttackQueue()
    {
        if (transitionedToQueue) return;
        transitionedToQueue = true;

        enemy.agent.destination = enemy.transform.position;
        enemy.ChangeState(State.OnQueue);
    }

    #endregion UPDATE

    public override void Exit()
    {
        if (enemy.forcedAttackState)
        {
            enemy.waitingPos = enemy.transform.position;
            enemy.agent.destination = enemy.transform.position;
            enemy.ableToFace = true;
        }
    }
}