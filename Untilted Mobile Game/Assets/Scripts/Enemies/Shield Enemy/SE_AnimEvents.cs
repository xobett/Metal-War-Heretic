using UnityEngine;
using EnemyAI.ShieldEnemy;

public class SE_AnimEvents : MonoBehaviour
{
    private ShieldEnemy shieldEnemy;
    [SerializeField] Collider[] hitColliders;

    private void Start()
    {
        shieldEnemy = GetComponentInParent<ShieldEnemy>();
    }

    public void EnablePushAttack()
    {
        foreach (Collider collider in hitColliders)
        {
            collider.isTrigger = true;
        }

        shieldEnemy.rgIsPushing = true;
    }

    public void DisablePushAttack()
    {

        foreach (Collider collider in hitColliders)
        {
            collider.isTrigger = false;
        }

        shieldEnemy.rgIsPushing = false;
    }
}
