using EnemyAI.ElectricEnemy;
using UnityEngine;

public class EE_AnimEvents : MonoBehaviour
{
    ElectricEnemy electricEnemy;

    private void Start()
    {
        electricEnemy = GetComponentInParent<ElectricEnemy>();
    }

    public void ThrowElectricBall()
    {
        electricEnemy.AnimEvent_ThrowElectricBall();
    }

    public void SpawnElectricVFX()
    {
        electricEnemy.AnimEvent_SpawnElectricVFX();
    }

    public void SpawnElectricArea()
    {
        electricEnemy.AnimEvent_SpawnElectricArea();
    }

    public void StopFacingAtPlayer()
    {
        electricEnemy.AnimEvent_StopFacingAtPlayer();
    }

    public void FaceAtPlayer()
    {
        electricEnemy.AnimEvent_FaceAtPlayer();
    }

    public void ResetRotation()
    {
        electricEnemy.SmoothResetRotation();
    }

    public void FinishAttack()
    {
        electricEnemy.AnimEvent_FinishAttack();
    }
}
