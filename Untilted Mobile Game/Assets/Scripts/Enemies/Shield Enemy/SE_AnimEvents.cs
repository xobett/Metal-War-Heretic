using UnityEngine;
using EnemyAI.ShieldEnemy;

public class SE_AnimEvents : MonoBehaviour
{
    private ShieldEnemy shieldEnemy;

    private void Start()
    {
        shieldEnemy = GetComponentInParent<ShieldEnemy>();
    }

    public void EnablePush()
    {
        shieldEnemy.AnimEvent_EnablePush();
    }

    public void DisablePush()
    {
        shieldEnemy.AnimEvent_DisablePush();
    }

    public void StopFacingAtPlayer()
    {
        shieldEnemy.AnimEvent_StopFacingAtPlayer();
    }

    public void RunGuardTime()
    {
        shieldEnemy.AnimEvent_RunGuardTime();
    }

    public void RunCooldown()
    {
        shieldEnemy.AnimEvent_RunCooldown();
    }

    public void ResetRotation()
    {
        shieldEnemy.AnimEvent_SmoothResetRotation();
    }

    public void FinishAttack()
    {
        shieldEnemy.AnimEvent_FinishAttack();
    }
}
