using UnityEngine;

public class Camera : MonoBehaviour
{
    //Follow settings
    private Transform playerPos;

    //Camera settings

    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] private Quaternion offsetRotation;

    [SerializeField, Range(0f, 1f)] private float followSpeed;

    private Vector3 velocity;

    void Start()
    {
        GetReferences();        
    }

    private void LateUpdate()
    {
        PlayerFollowing();
    }

    void PlayerFollowing()
    {
        Vector3 offset = offsetRotation * offsetPosition;

        transform.position = Vector3.SmoothDamp(transform.position, playerPos.position + offset, ref velocity, followSpeed * Time.deltaTime);
    }

    void GetReferences()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
