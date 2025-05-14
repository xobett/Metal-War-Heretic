using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private const float range = 1.5f;
    [SerializeField] private LayerMask whatIsInteraction;

    private void Update()
    {
        if (IsInteracting())
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward * range, out hit, range, whatIsInteraction))
            {
                hit.collider.GetComponent<IInteractable>().OnInteract();
            } 
        }
    }

    private bool IsInteracting()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
}