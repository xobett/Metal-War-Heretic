using System.Collections;
using UnityEngine;

public class IdleState : EnemyState
{
    public IdleState(EnemyAI.EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        SetNavMeshSettings();
    }

    #region ENTER

    private void SetNavMeshSettings()
    {
        enemy.agent.speed = enemy.walkSpeed;
        enemy.agent.stoppingDistance = 0;
    }

    #endregion ENTER

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}