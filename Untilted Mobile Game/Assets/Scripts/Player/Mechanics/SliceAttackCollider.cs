using UnityEngine;

public class SliceAttackCollider : MonoBehaviour
{
    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float sliceDamage;

    [SerializeField] private GameObject hitEnemyVfx;
    [SerializeField] private SOShakeData playerHitShake;

    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (enemyCollider.gameObject.layer == LayerMask.NameToLayer("Enemy") && isDashing)
        {
            AudioManager.Instance.PlaySFX("HIT A ENEMIGO", 0);

            GetComponentInParent<SliceAttack>().CancelSliceMovement();

            Vector3 direction = enemyCollider.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            Player.Instance.DisableMovement(0.28f);

            Vector3 hitPos = enemyCollider.transform.position + enemyCollider.transform.forward * 0.4f;
            GameObject vfx = Instantiate(hitEnemyVfx, hitPos, hitEnemyVfx.transform.rotation);
            Destroy(vfx, 1f);

            ShakeEventManager.Instance.AddShakeEvent(playerHitShake);

            enemyCollider.GetComponent<IDamageable>().OnDamage(sliceDamage);
        }
    }

    private bool isDashing => GetComponentInParent<SliceAttack>().IsDashing;
}
