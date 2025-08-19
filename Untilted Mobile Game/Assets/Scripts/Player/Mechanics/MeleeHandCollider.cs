using UnityEngine;

public class MeleeHandCollider : MonoBehaviour
{
    [SerializeField] private GameObject hitEnemyVfx;

    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (enemyCollider.isTrigger) return;

        AudioManager.Instance.PlaySFX("HIT A ENEMIGO", 0);

        Vector3 hitPos = enemyCollider.transform.position + enemyCollider.transform.forward * 0.4f;
        GameObject vfx = Instantiate(hitEnemyVfx, hitPos, Quaternion.identity);
        Destroy(vfx, 1f);

        ShakeEventManager.Instance.AddShakeEvent(GetComponentInParent<MeleeAttack>().playerHitShake);

        if (enemyCollider.CompareTag("Enemy"))
        {
            GetComponentInParent<MeleeAttack>().HitEnemy(enemyCollider);
        }
    }
}