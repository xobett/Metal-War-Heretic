using UnityEngine;
using EnemyAI;

public class AttackState : EnemyState
{
    public AttackState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.QueryAttackPos();
    }

    public override void Update()
    {
        if (enemy.attackPos != Vector3.zero)
        {
            enemy.agent.destination = enemy.attackPos;
        }


    }

    public override void Exit()
    {
        //Stop looking at player
    }

}