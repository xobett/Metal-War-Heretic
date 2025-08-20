using UnityEngine;

public class MeleeHandCollider : MonoBehaviour
{
    [SerializeField] private GameObject hitEnemyVfx;
    [SerializeField] private SOShakeData playerHitShake;

    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (enemyCollider.isTrigger || enemyCollider.CompareTag("Player")) return;

        AudioManager.Instance.PlaySFX("HIT A ENEMIGO", 0);
        ShakeEventManager.Instance.AddShakeEvent(playerHitShake);

        if (enemyCollider.CompareTag("Enemy"))
        {
            Vector3 hitPos = enemyCollider.transform.position + enemyCollider.transform.forward * 0.4f;
            GameObject vfx = Instantiate(hitEnemyVfx, hitPos, Quaternion.identity);
            Destroy(vfx, 1f);

            GetComponentInParent<MeleeAttack>().HitEnemy(enemyCollider);
        }
    }
}