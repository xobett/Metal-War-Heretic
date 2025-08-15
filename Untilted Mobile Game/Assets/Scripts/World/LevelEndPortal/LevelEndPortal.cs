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
            Player.Instance.DisableMovement();
            anim.SetTrigger("Teleport");
        }
    }
}
