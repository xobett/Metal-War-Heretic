using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyAI.EnemyBase enemy) : base(enemy) { }

    private Vector3 waitPosition;

    private float waitPositionDistance = Random.Range(2f, 3f);

    public override void Enter()
    {

    }

    #region ENTER

    #endregion ENTER

    public override void Update()
    {
        enemy.agent.destination = enemy.waitingPos;
    }

    public override void Exit()
    { 

    }
}