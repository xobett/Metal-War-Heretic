using EnemyAI.ElectricEnemy;
using UnityEngine;

public class EE_AnimEvents : MonoBehaviour
{
    ElectricEnemy electricEnemy;

    private void Start()
    {
        electricEnemy = GetComponentInParent<ElectricEnemy>();
    }

    public void ExecuteDistanceAttack()
    {
        electricEnemy.ThrowElectricBall();
    }

    public void StopFacingAtPlayer()
    {
        electricEnemy.AnimEvent_FaceAtPlayer();
    }

    public void FaceAtPlayer()
    {
        electricEnemy.AnimEvent_StopFacingAtPlayer();
    }

    public void FinishAttack()
    {
        electricEnemy.AnimEvent_FinishAttack();
    }
}
