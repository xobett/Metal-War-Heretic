using EnemyAI;
using UnityEngine;

public class AttackState : EnemyState
{
    public AttackState(EnemyBase enemy) : base(enemy) { }

    private float attackTimer;
    private const float maxAttackTime = 38f;

    private float attackNavTimer;
    private float timeBeforeNavigating;

    private bool transitionedToQueue = false;

    public override void Enter()
    {
        timeBeforeNavigating = Random.Range(1.3f, 2f);

        Enter_SetTimerSettings();
        Enter_SetEnemySettings();
        enemy.QueryAttackPosition();
        transitionedToQueue = false;
    }

    #region ENTER

    private void Enter_SetTimerSettings()
    {
        attackTimer = maxAttackTime;
    }

    private void Enter_SetEnemySettings()
    {
        enemy.agent.avoidancePriority = 0;
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
        if (enemy.agent.isActiveAndEnabled)
        {
            enemy.agent.isStopped = true;
        }
    }

    private void Update_MoveToAttackPos()
    {
        if (enemy.attackPos != Vector3.zero)
        {
            enemy.agent.destination = enemy.attackPos;
        }
    }

    private void HandleOnArrive()
    {
        if (Vector3.Distance(enemy.transform.position, enemy.attackPos) < 0.2f)
        {
            if (enemy.attacked) return;
            enemy.attacked = true;
            enemy.Attack();
            enemy.RunAttackCooldown();
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
            attackNavTimer = timeBeforeNavigating;
            enemy.agent.isStopped = false;
            enemy.QueryAttackPosition();
        }
    }


    private void TransitionToQueue()
    {
        if (transitionedToQueue) return;
        transitionedToQueue = true;
        enemy.ChangeState(State.OnQueue);
    }

    #endregion UPDATE

    public override void Exit()
    {
        Debug.Log($"{enemy.gameObject.name} exit attack state");
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