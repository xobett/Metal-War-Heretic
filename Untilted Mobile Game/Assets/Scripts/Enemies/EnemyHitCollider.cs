using UnityEngine;

public class EnemyHitCollider : MonoBehaviour
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
