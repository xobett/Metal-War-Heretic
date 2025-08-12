using EnemyAI;
using UnityEngine;

public class AttackState : EnemyState
{
    public AttackState(Enemy enemy) : base(enemy) { }

    private float attackTimer;
    private const float maxAttackTime = 38f;

    private float attackNavTimer;
    private float timeBeforeNavigating = 3f;

    private bool transitionedToQueue;

    private bool isStunned = false;

    public override void Enter()
    {
        Enter_SetTimerSettings();
        Enter_SetEnemySettings();
    }

    #region ENTER

    private void Enter_SetTimerSettings()
    {
        attackTimer = maxAttackTime;
        attackNavTimer = timeBeforeNavigating;
    }

    private void Enter_SetEnemySettings()
    {
        enemy.agent.avoidancePriority = 10;
        enemy.QueryAttackPosition();

        transitionedToQueue = false;
    }

    #endregion ENTER

    public override void Update()
    {
        Update_MoveToAttackPos();

        //Update_RunAttackTimer();
        //Update_HandleAttackTime();

        Update_RunNavigationTimer();
        Update_HandleNavigationUpdate();

        HandleOnArrive();

        enemy.currentState = State.Attack;
    }

    #region UPDATE

    public void ResetTimer()
    {
        attackTimer = maxAttackTime;
        attackNavTimer = timeBeforeNavigating;

        if (enemy.agent.isActiveAndEnabled)
        {
            isStunned = true;
            enemy.agent.isStopped = true;
        }
    }

    private void Update_MoveToAttackPos()
    {
        if (enemy.attackPos != Vector3.zero && !isStunned)
        {
            enemy.agent.destination = enemy.attackPos;
        }
    }

    private void HandleOnArrive()
    {
        if (Vector3.Distance(enemy.transform.position, enemy.attackPos) < 0.2f)
        {
            if (enemy.attackCooldown) return;
            enemy.ExecuteAttack();
        }
    }

    private void Update_RunAttackTimer()
    {
        attackTimer -= Time.deltaTime;
    }
    private void Update_HandleAttackTime()
    {
        if (attackTimer <= 0)
        {
            if (enemy.isExecutingAttack) return;
            TransitionToQueue();
        }
    }

    private void Update_RunNavigationTimer()
    {
        attackNavTimer -= Time.deltaTime;
    }

    private void Update_HandleNavigationUpdate()
    {
        if (attackNavTimer <= 0)
        {
            if (enemy.isExecutingAttack || PlayerIsNear())
            {
                attackNavTimer = timeBeforeNavigating;
                return;
            }

            attackNavTimer = timeBeforeNavigating;
            isStunned = false;
            enemy.agent.isStopped = false;
            enemy.QueryAttackPosition();
        }
    }

    private bool PlayerIsNear()
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);

        return distance < 3f;
    }


    private void TransitionToQueue()
    {
        if (enemy.isExecutingAttack) return;

        if (transitionedToQueue) return;

        transitionedToQueue = true;
        enemy.ChangeState(State.OnQueue);
    }

    #endregion UPDATE

    public override void Exit()
    {
        Exit_ResetSettings();
        Exit_FinishAttack();
    }

    #region EXIT

    private void Exit_ResetSettings()
    {
        enemy.ableToFace = false;
        enemy.agent.avoidancePriority = 50;
    }

    private void Exit_FinishAttack()
    {
        enemy.RunAttackStateCooldown();
        enemy.RemoveAttackPos();
        enemy.RemoveFromAttackList();
        enemy.ResetUpdatePositionValue();
    }

    #endregion EXIT

}