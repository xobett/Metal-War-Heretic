using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetReferences();
    }

    // Update is called once per frame
    void Update()
    {
        FaceCamera();
    }

    private void FaceCamera()
    {
        Vector3 direction = transform.position - cameraTransform.position;
        Quaternion lookAtTarget = Quaternion.LookRotation(direction);
        Quaternion lookRotation = Quaternion.Euler(lookAtTarget.eulerAngles.x, lookAtTarget.eulerAngles.y, 0);

        transform.rotation = lookRotation;

    }

    private void GetReferences()
    {
        cameraTransform = Camera.main.transform;
    }
}
