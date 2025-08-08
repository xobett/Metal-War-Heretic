using UnityEngine;

public class LockedDoor_AnimEvents : MonoBehaviour
{
    [SerializeField] private GameObject colliderGo;
    public void UnlockDoor()
    {
        colliderGo.SetActive(false);
    }
}