using EnemyAI.BruteEnemy;
using EnemyAI.ElectricEnemy;
using UnityEngine;

public class BE_AnimEvents : MonoBehaviour
{
    private BruteEnemy bruteEnemy;

    private void Start()
    {
        bruteEnemy = GetComponentInParent<BruteEnemy>();
    }

    public void StopMoving()
    {
        bruteEnemy.AnimEvent_StopMoving();
    }

    public void StartRunning() 
    {
        bruteEnemy.AnimEvent_StartRunning();
    }

    public void StopAcceleration()
    {
        bruteEnemy.AnimEvent_StopAcceleration();
    }

    public void ResetSpeed()
    {
        bruteEnemy.AnimEvent_ResetSpeed();
    }

    public void StopFacingAtPlayer()
    {
        bruteEnemy.AnimEvent_StopFacingAtPlayer();
    }

    public void ResetRotation()
    {
        bruteEnemy.AnimEvent_SmoothResetRotation();
    }

    public void FinishAttack()
    {
        bruteEnemy.AnimEvent_FinishAttack();
    }

}