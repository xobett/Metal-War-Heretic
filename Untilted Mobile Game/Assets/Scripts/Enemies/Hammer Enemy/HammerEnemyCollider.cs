using UnityEngine;

public class HammerEnemyCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is hitting");
        }
    }
}
