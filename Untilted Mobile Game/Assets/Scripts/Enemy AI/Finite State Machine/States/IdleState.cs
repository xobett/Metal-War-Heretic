using System.Collections;
using UnityEngine;
using EnemyAI;

public class IdleState : EnemyState
{
    public IdleState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        Enter_SetIdleSettings();
    }

    #region ENTER

    private void Enter_SetIdleSettings()
    {
        enemy.agent.speed = enemy.walkSpeed;
        enemy.agent.stoppingDistance = 0;

        enemy.ableToFace = false;
    }

    #endregion ENTER

    public override void Update()
    {

    }

    #region UPDATE

    #endregion UPDATE

    public override void Exit()
    {

    }
}