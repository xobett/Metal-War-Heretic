using EnemyAI;
using UnityEngine;
using UnityEngine.AI;

public class OnQueueState : EnemyState
{
    public OnQueueState(Enemy enemy) : base(enemy) { }

    private float timer;
    private const float timeBeforeMoving = 8f;

    private bool rotated = false;

    public override void Enter()
    {
        Enter_ResetRotation();
        timer = 4f;
        enemy.QueryWaitPosition();
    }

    #region ENTER

    private void Enter_ResetRotation()
    {
        //enemy.SmoothResetRotation();
    }

    #endregion ENTER

    public override void Update()
    {
        enemy.currentState = State.OnQueue;
        Update_MoveToWaitPos();
        Update_RunTimer();
        Update_HandleNavigation();
        HandleOnArrive();
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

        if (enemy.AttackPositionsAssigned)
        {
            if (enemy.UpdatedPosition) return;
            enemy.UpdatedPosition = true;
            enemy.QueryWaitPosition();
            enemy.ableToFace = false;
        }
    }

    private void HandleOnArrive()
    {
        if (Vector3.Distance(enemy.transform.position, enemy.waitingPos) < 0.2f)
        {
            if (rotated) return;
            rotated = true;
            enemy.SmoothResetRotation();
        }
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

        enemy.RemoveWaitPos();
    }
}