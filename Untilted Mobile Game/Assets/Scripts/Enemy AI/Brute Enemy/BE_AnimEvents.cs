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

    public void PlayStepSound()
    {
        bruteEnemy.AnimEvent_PlayStepSound();
    }
    public void PlayRunStepSound()
    {
        bruteEnemy.AnimEvent_PlayRunStepSound();
    }

    public void PlayAnticipationSound()
    {
        bruteEnemy.AnimEvent_PlayAnticipationSound();
    }

    public void PlayDriftSound()
    {
        bruteEnemy.AnimEvent_PlayDriftSound();
    }

    public void StopMoving()
    {
        bruteEnemy.AnimEvent_StopMoving();
    }

    public void StartRunning() 
    {
        bruteEnemy.AnimEvent_StartRunning();
    }

    public void EnableRunVfx()
    {
        bruteEnemy.AnimEvents_EnableRunVfX();
    }

    public void DisableRunVfx()
    {
        bruteEnemy.AnimEvents_DisableRunVfx();
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

    public void Death()
    {
        bruteEnemy.AnimEvent_OnDeath();
    }
}