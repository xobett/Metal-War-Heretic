using UnityEngine;
using EnemyAI;

public class EnemyHitCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponentInParent<Enemy>().HitPlayer(other);
        }
    }
}
