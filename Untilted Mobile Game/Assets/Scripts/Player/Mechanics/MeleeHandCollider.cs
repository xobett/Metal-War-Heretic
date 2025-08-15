using UnityEngine;

public class MeleeHandCollider : MonoBehaviour
{
    [SerializeField] private GameObject hitEnemyVfx;

    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (enemyCollider.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlaySound("HIT A ENEMIGO", 0);

            Vector3 hitPos = enemyCollider.transform.position + enemyCollider.transform.forward * 0.4f;
            GameObject vfx = Instantiate(hitEnemyVfx, hitPos, hitEnemyVfx.transform.rotation);
            Destroy(vfx, 1f);

            GetComponentInParent<MeleeAttack>().HitEnemy(enemyCollider);
        }
    }
}