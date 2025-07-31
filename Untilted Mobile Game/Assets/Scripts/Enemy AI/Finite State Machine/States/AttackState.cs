using UnityEngine;

public class AttackState : EnemyState
{
    public AttackState(EnemyAI.EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("Enter Attack State");
    }

    public override void Exit()
    {
        Debug.Log("Exit Attack State");
    }

    public override void Update()
    {
        Debug.Log("Attack is active");
    }
}