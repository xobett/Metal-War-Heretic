using UnityEngine;

public class MeleeHandCollider : MonoBehaviour
{
    [SerializeField] private GameObject hitEnemyVfx;

    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (enemyCollider.CompareTag("Enemy"))
        {
            Vector3 hitPos = enemyCollider.transform.position + enemyCollider.transform.forward * 0.4f;

            GameObject vfx = Instantiate(hitEnemyVfx, hitPos, hitEnemyVfx.transform.rotation);
            GetComponentInParent<MeleeAttack>().HitEnemy(enemyCollider);
            Destroy(vfx, 1f);
        }
    }
}