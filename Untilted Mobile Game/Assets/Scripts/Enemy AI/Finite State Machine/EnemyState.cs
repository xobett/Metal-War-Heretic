using EnemyAI;
using UnityEngine;

public abstract class EnemyState
{
    protected EnemyBase enemy;

    public EnemyState (EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}