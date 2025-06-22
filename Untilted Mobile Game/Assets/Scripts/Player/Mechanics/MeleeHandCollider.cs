using UnityEngine;

public class MeleeHandCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (isHitting && enemyCollider.CompareTag("Enemy"))
        {
            GetComponentInParent<MeleeAttack>().HitEnemy(enemyCollider);
            Debug.Log(name);
        }
    }

    private bool isHitting => GetComponentInParent<MeleeAttack>().InCombat;
}