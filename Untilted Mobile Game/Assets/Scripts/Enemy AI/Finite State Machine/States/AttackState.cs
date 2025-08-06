using UnityEngine;
using EnemyAI;

public class AttackState : EnemyState
{
    public AttackState(EnemyBase enemy) : base(enemy) { }

    private float attackTimer;
    private const float maxAttackTime = 18f;

    private float attackNavTimer;
    private const float timeBeforeNavigating = 7f;

    private bool transitionedToQueue = false;

    public override void Enter()
    {
        Enter_SetTimerSettings();
        Enter_SetEnemySettings();
        enemy.QueryAttackPosition();
        ResetTimer();
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

        Update_RunAttackTimer();
        Update_HandleAttackTime();

        enemy.currentState = State.Attack;
    }

    #region UPDATE

    public void ResetTimer()
    {
        attackTimer = maxAttackTime;
    }

    private void Update_MoveToAttackPos()
    {
        if (enemy.attackPos != Vector3.zero)
        {
            enemy.agent.destination = enemy.attackPos;
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
            enemy.Attack();
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
        enemy.RunAttackCooldown();
        enemy.RemoveAttackPos();
        enemy.RemoveFromAttackList();
        enemy.ResetUpdatePositionValue();
    }
    #endregion EXIT

}