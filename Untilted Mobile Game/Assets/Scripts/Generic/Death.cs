using UnityEngine;

public class Death : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().PlayerDeath();
        }
        else if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Health>().TakeDamage(1000);
        }
    }
}
