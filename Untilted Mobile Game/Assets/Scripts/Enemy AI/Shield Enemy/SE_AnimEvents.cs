using EnemyAI.BruteEnemy;
using EnemyAI.ShieldEnemy;
using UnityEngine;

public class SE_AnimEvents : MonoBehaviour
{
    private ShieldEnemy shieldEnemy;

    private void Start()
    {
        shieldEnemy = GetComponentInParent<ShieldEnemy>();
    }

    public void PlayStepSound()
    {
        shieldEnemy.AnimEvent_PlayStepSound();
    }

    public void PlayShieldStompSound()
    {
        shieldEnemy.AnimEvent_PlayShieldsStompSound();
    }

    public void PlayPushShieldsSound()
    {
        shieldEnemy.AnimEvent_PlayPushShieldsSound();
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

    public void Death()
    {
        shieldEnemy.AnimEvent_OnDeath();
    }
}
