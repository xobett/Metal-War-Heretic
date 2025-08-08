using Unity.VisualScripting;
using UnityEngine;

public class LevelEndPortal : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.movementEnabled = false;
            other.GetComponent<PlayerMovement>().DisableMovement();
            anim.SetTrigger("Teleport");
        }
    }
}
