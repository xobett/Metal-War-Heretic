using UnityEngine;

public class TurnCameraTrigger : MonoBehaviour
{
    [Header("NORMAL FLOW TARGET")]
    [SerializeField] private CameraFollow.TargetCameraRotation normalFlowTarget;

    [Header("REVERSE FLOW TARGET")]
    [SerializeField] private CameraFollow.TargetCameraRotation reverseFlowTarget;

    private CameraFollow camFollow;

    private void Start()
    {
        camFollow = Camera.main.GetComponentInParent<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (camFollow.lastTarget == normalFlowTarget)
            {
                camFollow.TurnCamera(reverseFlowTarget);
            }
            else
            {
                camFollow.TurnCamera(normalFlowTarget);
            }
        }
    }
}