using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Player settings
    private CharacterController charCtrlr;

    private Animator animator;

    [Header("NORMAL MOVEMENT SETTINGS")]
    [SerializeField] private float movementSpeed;
    [SerializeField, Range(0f, 1f)] private float rotationSpeed;

    private float velocity;

    [Header("HIT MOVEMENT SETTINGS")]
    private float hitSpeed;
    private float hitForce;

    private Vector3 hitDirection;

    [Header("MELEE ASSIST MOVEMENT SETTINGS")]
    [SerializeField] private LayerMask whatIsMelee;

    [HideInInspector] public bool IsHit { get; private set; }

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

    // Update is called once per frame
    void Update()
    {
        if (!PauseManager.Instance.GamePaused)
        {
            MovementCheck();
            Gravity();
        }
    }

    #region MOVEMENT

    private void MovementCheck()
    {
        if (!IsHit && !isDashing)
        {
            if (inCombat)
            {
                MeleeAssistMovement();
                Debug.Log("In combat movement");
            }
            else
            {
                NormalMovement();
            }
        }
        else
        {
            HitMovement();
        }
    }

    #region NORMAL MOVEMENT

    private void NormalMovement()
    {
        Vector3 move = new Vector3(JoystickManager.Instance.HorizontalInput(), 0, JoystickManager.Instance.ForwardInput());

        if (move.magnitude != 0)
        {
            animator.SetBool("isWalking", true);

            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref velocity, rotationSpeed);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = transform.rotation * Vector3.forward;

            charCtrlr.Move(moveDirection * movementSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    #endregion NORMAL MOVEMENT

    #region HIT MOVEMENT

    public void SetHitMovement(Vector3 pushedDirection, float pushedForce, float pushedTime)
    {
        hitDirection = pushedDirection;
        hitSpeed = pushedForce;
        hitForce = pushedTime;

        IsHit = true;
        Invoke(nameof(DisableHitMovement), hitForce);
    }

    private void HitMovement()
    {
        charCtrlr.Move(hitDirection * hitSpeed * Time.deltaTime);
    }

    private void DisableHitMovement()
    {
        IsHit = false;
    }

    #endregion HIT MOVEMENT

    #region MELEE ASSIST MOVEMENT

    private void MeleeAssistMovement()
    {
        Vector3 faceDirection = new Vector3(JoystickManager.Instance.HorizontalInput(), 0f, JoystickManager.Instance.ForwardInput());

        if (Physics.Raycast(transform.position, faceDirection * 5f, out RaycastHit hit, 3f, whatIsMelee))
        {
            Vector3 direction = hit.collider.transform.position - transform.position;

            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 30f * Time.deltaTime);

            Vector3 nearEnemyPos = hit.collider.transform.position + hit.transform.forward * 1.5f;
            Vector3 moveDirection = nearEnemyPos - transform.position;

            float distance = Vector3.Distance(transform.position, nearEnemyPos);

            if (distance > 0.1f)
            {
                charCtrlr.Move(moveDirection.normalized * Mathf.Pow(movementSpeed, 2) * Time.deltaTime);
            }
        }
    }

    #endregion MELEE ASSIST MOVEMENT

    #endregion

    #region GRAVITY

    private void Gravity()
    {
        if (!isDashing)
        {
            gravity.y -= gravityForce * Time.deltaTime;
        }

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

    #endregion GRAVITY

    #region CHECKS

    private bool isDashing => GetComponent<SliceAttack>().IsDashing;

    private bool inCombat => GetComponent<MeleeAttack>().InCombat;

    #endregion CHECKS

    #region START

    private void GetReferences()
    {
        charCtrlr = GetComponent<CharacterController>();
        groundCheck = transform.GetChild(0);
        animator = GetComponentInChildren<Animator>();
    }

    #endregion START
}
