using EnemyAI;
using UnityEngine;

public class FiniteStateMachine
{
    private EnemyState currentState;

    public void Update()
    {
        currentState?.Update();
    }

    public void Initialize(EnemyState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}