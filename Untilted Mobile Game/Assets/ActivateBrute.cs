using UnityEngine;

public class ActivateBrute : MonoBehaviour
{
    [SerializeField] private GameObject brute;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            brute.SetActive(true);
        }
    }
}
