using EnemyAI;
using UnityEngine;

public class OnQueueState : EnemyState
{
    public OnQueueState(EnemyBase enemy) : base(enemy) { }

    private float timer;
    private const float timeBeforeMoving = 5f;

    public override void Enter()
    {
        Enter_ResetRotation();
        timer = timeBeforeMoving;
        enemy.QueryWaitPosition();
    }

    #region ENTER

    private void Enter_ResetRotation()
    {
        enemy.SmoothResetRotation();
    }

    #endregion ENTER

    public override void Update()
    {
        enemy.currentState = State.OnQueue;
        Update_MoveToWaitPos();
        Update_RunTimer();
        Update_HandleNavigation();
    }

    #region UPDATE

    private void Update_MoveToWaitPos()
    {
        if (enemy.waitingPos != Vector3.zero)
        {
            enemy.agent.destination = enemy.waitingPos;
        }
    }

    private void Update_RunTimer()
    {
        timer -= Time.deltaTime;
    }

    private void Update_HandleNavigation()
    {
        if (timer <= 0)
        {
            timer = timeBeforeMoving;
            RunQueries();
        }
    }

    private void RunQueries()
    {
        enemy.QueryAttack();
        enemy.QueryWaitPosition();
    }

    #endregion UPDATE

    public override void Exit()
    {
        enemy.RemoveWaitPos();
        Debug.Log($"{enemy.gameObject.name} exit queue ");
    }
}