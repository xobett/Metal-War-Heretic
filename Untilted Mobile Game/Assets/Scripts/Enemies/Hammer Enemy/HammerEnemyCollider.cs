using UnityEngine;

public class HammerEnemyCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hitting player");
            GetComponentInParent<EnemyBase>().HitPlayer(other);  
        }
    }
}
