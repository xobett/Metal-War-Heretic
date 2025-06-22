using UnityEngine;

public class MeleeHandCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (enemyCollider.CompareTag("Enemy"))
        {
            GetComponentInParent<MeleeAttack>().HitEnemy(enemyCollider);
        }
    }
}