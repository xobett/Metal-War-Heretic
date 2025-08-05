using UnityEngine;

public class TurnCameraTrigger : MonoBehaviour
{
    [SerializeField] private CameraFollow.TargetCameraRotation targetCameraRotation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<CameraFollow>().TurnCamera(targetCameraRotation);
        }
    }
}