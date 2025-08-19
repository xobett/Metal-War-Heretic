using UnityEngine;
using EnemyAI;

public class EnemyHitCollider : MonoBehaviour
{
    [SerializeField] private GameObject hitPlayerVfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 hitPos = other.transform.position + other.transform.forward * 0.4f;

            GameObject vfx = Instantiate(hitPlayerVfx, hitPos, hitPlayerVfx.transform.rotation);
            GetComponentInParent<Enemy>().HitPlayer(other);
            Destroy(vfx, 1f);

            ShakeEventManager.Instance.AddShakeEvent(GetComponentInParent<Enemy>().enemyHitShake);
        }
    }
}
