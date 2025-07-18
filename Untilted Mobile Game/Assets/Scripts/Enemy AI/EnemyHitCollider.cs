using UnityEngine;
using EnemyAI;

public class EnemyHitCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponentInParent<EnemyBase>().HitPlayer(other);
        }
    }
}
