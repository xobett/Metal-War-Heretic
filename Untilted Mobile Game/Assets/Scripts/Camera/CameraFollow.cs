using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("FOLLOW SETTINGS")]
    [SerializeField] private Transform followTarget;

    [SerializeField] private float maxDistance;

    [SerializeField] private Vector2 cameraSensitivity;

    Vector2 orbitAngle = new Vector2(-tiltAngle, 64);
    const float tiltAngle = 42;

    [SerializeField] private float upLimit;
    [SerializeField] private float downLimit;

    public TargetCameraRotation lastTarget = TargetCameraRotation.Front;

    private const float orbitSmooth = 15f;

    [SerializeField] private float test;

    [Header("CAMERA COLLISION SETTINGS")]
    private Camera cam;

    [SerializeField] private LayerMask whatIsCollision; //Layer que controla en que layers se detecta colision.
    [SerializeField] private Vector2 nearPlaneSize;
    [SerializeField, Range(0f, 1f)] private float safeDistance;
    private RaycastHit hit;

    void Start()
    {
        Start_GetReferences();
        Start_SetSettings();
        TurnCamera(lastTarget);
    }

    #region START

    private void Start_SetSettings()
    {
        Application.targetFrameRate = 60;

        GetCameraNearPlaneSize();
    }

    private void Start_GetReferences()
    {
        cam = Camera.main;

        if (followTarget == null)
        {
            try
            {
                followTarget = GameObject.FindGameObjectWithTag("Camera Target").transform;
            }
            catch
            {
                Debug.LogError("No follow target reference / tag has been assigned!");
            }
        }
    }

    #endregion START

    void Update()
    {
        orbitAngle.x -= test * cameraSensitivity.x * Time.deltaTime;
    }

    private void LateUpdate()
    {
        FollowAndOrbit();
    }

    private void GetCameraNearPlaneSize()
    {
        float planeHeight = Mathf.Tan((cam.fieldOfView * Mathf.Deg2Rad / 2) * cam.nearClipPlane);
        float planeWidth = planeHeight * cam.aspect;

        nearPlaneSize = new Vector2(planeWidth, planeHeight);
    }

    private Vector3[] CalculateCollisionPoints(Vector3 orbitDirection)
    {
        Vector3 position = followTarget.position;
        Vector3 center = position + orbitDirection * (cam.nearClipPlane + safeDistance);

        Vector3 right = transform.right * nearPlaneSize.x;
        Vector3 up = transform.up * nearPlaneSize.y;

        return new Vector3[]
        {
            center - right + up,
            center + right + up,
            center - right - up,
            center + right - up
        };
    }

    private void OrbitRotationInput()
    {
        if (MouseHorizontalInput() != 0)
        {
            orbitAngle.x -= MouseHorizontalInput() * cameraSensitivity.x * Time.deltaTime;
        }

        if (MouseVerticalInput() != 0)
        {
            orbitAngle.y -= MouseVerticalInput() * cameraSensitivity.y * Time.deltaTime;

            orbitAngle.y = Mathf.Clamp(orbitAngle.y, downLimit, upLimit);
        }
    }
    private void FollowAndOrbit()
    {
        float radX = orbitAngle.x * Mathf.Deg2Rad;
        float radY = orbitAngle.y * Mathf.Deg2Rad;

        Vector3 orbitValue = new Vector3(
            Mathf.Cos(radX) * Mathf.Cos(radY),
            Mathf.Sin(radY),
            Mathf.Sin(radX) * Mathf.Cos(radY)); ;

        float orbitDistance = maxDistance;
        Vector3[] collisionPoints = CalculateCollisionPoints(orbitValue);

        foreach (Vector3 collisionPoint in collisionPoints)
        {
            if (Physics.Raycast(collisionPoint, orbitValue, out hit, maxDistance, whatIsCollision))
            {
                orbitDistance = Mathf.Min((hit.point - followTarget.position).magnitude, orbitDistance);
            }
        }

        Vector3 orbitMovement = followTarget.position + orbitValue * orbitDistance;

        transform.position = Vector3.Slerp(transform.position, orbitMovement, orbitSmooth * Time.deltaTime);

        Quaternion lookAtTarget = Quaternion.LookRotation(followTarget.position - transform.position, Vector3.up);

        if (!inCombat)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTarget, 0.3f);
        }
    }

    #region TURN CAMERA

    public enum TargetCameraRotation
    {
        Front,
        Right,
        Left,
        Back
    }
    public void TurnCamera(TargetCameraRotation target)
    {
        lastTarget = target;
        StartCoroutine(CR_TurnCamera(target));
    }

    private IEnumerator CR_TurnCamera(TargetCameraRotation target)
    {
        float value = 0;

        switch (target)
        {
            case TargetCameraRotation.Front:
                {
                    value = (0 - tiltAngle);
                    break;
                }
            case TargetCameraRotation.Right:
                {
                    value = (90 - tiltAngle);
                    break;
                }
            case TargetCameraRotation.Left:
                {
                    value = (270 - tiltAngle);
                    break;
                }
            case TargetCameraRotation.Back:
                {
                    value = (180 - tiltAngle);
                    break;
                }
        }

        float time = 0;
        while (time < 1)
        {
            orbitAngle.x = Mathf.LerpAngle(orbitAngle.x, value, time);
            time += Time.deltaTime * 0.7f;
            yield return null;
        }

        yield break;
    }

    #endregion TURN CAMERA

    #region CHECKS

    private bool inCombat => Player.Instance.GetComponent<MeleeAttack>().InCombat;

    #endregion CHECKS

    #region Inputs
    private float MouseHorizontalInput() => Input.GetAxis("Mouse X");
    private float MouseVerticalInput() => Input.GetAxis("Mouse Y");

    #endregion

}
