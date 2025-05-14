using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    //Player settings
    private CharacterController charCtrlr;
    private JoystickManager joystick;

    [Header("MOVEMENT SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float movementSpeed;
    [SerializeField, Range(0f, 1f)] private float rotationSpeed;

    private float velocity;

    [Header("GRAVITY SETTINGS")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float gravityForce = -9.81f;
    
    private Vector2 gravity;

    private Transform groundCheck;
    private const float radius = 0.2f;


    void Start()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        charCtrlr = GetComponent<CharacterController>();
        joystick = FindFirstObjectByType<JoystickManager>();
        groundCheck = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseManager.Instance.GamePaused)
        {
            Movement();
            Gravity(); 
        }
    }

    private void Movement()
    {
        Vector3 move = new Vector3(joystick.HorizontalInput(), 0, joystick.ForwardInput());

        if (move.magnitude != 0)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref velocity, rotationSpeed);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = transform.rotation * Vector3.forward;

            charCtrlr.Move(moveDirection * movementSpeed * Time.deltaTime);
        }
    }

    private void Gravity()
    {
        gravity.y -= gravityForce * Time.deltaTime;

        if (IsGrounded() && gravity.y < 0)
        {
            gravity.y = 0;
        }

        charCtrlr.Move(gravity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, radius, whatIsGround);
    }
}
