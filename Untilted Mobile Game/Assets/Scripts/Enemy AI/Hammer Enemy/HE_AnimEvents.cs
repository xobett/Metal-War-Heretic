using EnemyAI.BruteEnemy;
using EnemyAI.HammerEnemy;
using EnemyAI.ShieldEnemy;
using UnityEngine;

public class HE_AnimEvents : MonoBehaviour
{
    private HammerEnemy hammerEnemy;

    private void Start()
    {
        hammerEnemy = GetComponentInParent<HammerEnemy>();
    }

    public void StopFacingAtPlayer()
    {
        hammerEnemy.AnimEvent_StopFacingAtPlayer();
    }

    public void ResetRotation()
    {
        hammerEnemy.AnimEvent_SmoothResetRotation();
    }

    public void FinishAttack()
    {
        hammerEnemy.AnimEvent_FinishAttack();
    }

    public void DisableCombat()
    {
        hammerEnemy.AnimEvent_OnDeathStart();
    }

    public void Death()
    {
        hammerEnemy.AnimEvent_OnDeath();
    }

}