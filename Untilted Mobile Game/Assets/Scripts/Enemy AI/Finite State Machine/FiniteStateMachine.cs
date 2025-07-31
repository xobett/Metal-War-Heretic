using EnemyAI;
using UnityEngine;

public class FiniteStateMachine
{
    public EnemyState CurrentState { get; private set; }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void Initialize(EnemyState state)
    {
        CurrentState = state;
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}