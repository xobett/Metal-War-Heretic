using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Follow settings
    [SerializeField] private Transform target;

    //Camera settings
    [SerializeField] private Vector3 offsetPosition;

    [SerializeField, Range(0f, 1f)] private float followSpeed;

    private Vector3 velocity;


    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offsetPosition, ref velocity, followSpeed * Time.deltaTime);
    }
}
