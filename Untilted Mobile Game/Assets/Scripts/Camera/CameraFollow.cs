using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("NEW CAMERA FOLLOW SETTINGS")]
    [SerializeField] private Transform followTarget;

    [SerializeField] private float maxDistance;

    [SerializeField] private Vector2 cameraSensitivity;

    Vector2 orbitAngle = new Vector2(-90 * Mathf.Deg2Rad, 35 * Mathf.Deg2Rad);

    [SerializeField] private float upLimit;
    [SerializeField] private float downLimit;

    private GameObject player;

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
        //Establece el frame rate a 60 fps.
        Application.targetFrameRate = 60;

        player = Player.Instance.gameObject;
        cam = Camera.main;

        GetCameraNearPlaneSize();
    }

    void Update()
    {
        
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
            orbitAngle.x -= MouseHorizontalInput() * Mathf.Deg2Rad * cameraSensitivity.x * Time.deltaTime;
        }

        if (MouseVerticalInput() != 0)
        {
            orbitAngle.y -= MouseVerticalInput() * Mathf.Deg2Rad * cameraSensitivity.y * Time.deltaTime;

            orbitAngle.y = Mathf.Clamp(orbitAngle.y, downLimit * Mathf.Deg2Rad, upLimit * Mathf.Deg2Rad);
        }
    }
    private void FollowAndOrbit()
    {
        //Se crea un Vector3 donde se almacenara la rotacion activa alrededor del objetivo.
        Vector3 orbitValue;

        //En un circulo, para calcular la posicion alrededor de este se usa la funcion de Coseno para calcular posicion en X, y la funcion Seno para calcular la posicion en Y.
        //Dicho eso, el vector crea por asi decirlo un circulo alrededor del angulo que recibe en radianes, calculando la posicion X con el COS y la posicion Y con SIN.
        Vector3 normalOrbit = new Vector3(Mathf.Cos(orbitAngle.x) * Mathf.Cos(orbitAngle.y), Mathf.Sin(orbitAngle.y), Mathf.Sin(orbitAngle.x) * Mathf.Cos(orbitAngle.y));

        orbitValue = normalOrbit;

        float orbitDistance = maxDistance;
        Vector3[] collisionPoints = CalculateCollisionPoints(orbitValue);

        foreach (Vector3 collisionPoint in collisionPoints)
        {
            if (Physics.Raycast(collisionPoint, orbitValue, out hit, maxDistance, whatIsCollision))
            {
                orbitDistance = Mathf.Min((hit.point - followTarget.position).magnitude, orbitDistance);
            }
        }

        //La posicion de la camara sigue la posicion del jugador, sumandole los valores del Vector3 que controla la rotacion orbita creada, multiplicando la distancia de la orbita por un float.
        Vector3 orbitMovement = followTarget.position + orbitValue * orbitDistance;
        //Se interpola esfericamente la posicion de la camara.
        transform.position = Vector3.Slerp(transform.position, orbitMovement, orbitSmooth * Time.deltaTime);

        Quaternion lookAtTarget = Quaternion.LookRotation(followTarget.position - transform.position, Vector3.up);
        //Mira constantemente al jugador.
        transform.rotation = lookAtTarget;
    }

    #region TURN CAMERA

    public enum TargetCameraRotation
    {
        Right,
        Left,
    }
    public void TurnCamera(TargetCameraRotation target)
    {
        StartCoroutine(CR_TurnCamera(target));
    }

    private IEnumerator CR_TurnCamera(TargetCameraRotation target)
    {
        float value = 0;

        switch (target)
        {
            case TargetCameraRotation.Right:
                {
                    value = orbitAngle.x + (90 * Mathf.Deg2Rad);
                    break;
                }
            case TargetCameraRotation.Left:
                {
                    value = orbitAngle.x + (-90 * Mathf.Deg2Rad);
                    break;
                }
        }

        float time = 0;
        while (time < 1)
        {
            orbitAngle.x = Mathf.Lerp(orbitAngle.x, value, time);
            time += Time.deltaTime * 0.7f;
            yield return null;
        }

        yield break;
    }

    #endregion TURN CAMERA

    #region Inputs
    private float MouseHorizontalInput() => Input.GetAxis("Mouse X");
    private float MouseVerticalInput() => Input.GetAxis("Mouse Y");

    #endregion

}
