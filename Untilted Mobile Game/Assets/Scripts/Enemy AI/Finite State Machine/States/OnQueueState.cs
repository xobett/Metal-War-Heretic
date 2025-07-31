using EnemyAI;
using UnityEngine;

public class OnQueueState : EnemyState
{
    public OnQueueState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SmoothResetRotation();
        enemy.QueryAttack();
    }
    public override void Update()
    {
        if (enemy.waitingPos != Vector3.zero)
        {
            enemy.agent.destination = enemy.waitingPos;
        }

    }

    public override void Exit()
    {

    }
}