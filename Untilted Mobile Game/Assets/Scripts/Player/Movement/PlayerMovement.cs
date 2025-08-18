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
        Start_GetReferences();
    }

    #region START

    private void Start_GetReferences()
    {
        charCtrlr = GetComponent<CharacterController>();
        groundCheck = transform.GetChild(0);
        animator = GetComponentInChildren<Animator>();
    }

    #endregion START

    void Update()
    {
        if (!UIManager.Instance.GamePaused && Player.Instance.movementEnabled)
        {
            MovementCheck();
            Gravity();
        }
    }

    #region MOVEMENT

    public void DisableMovementAnim()
    {
        animator.SetBool("isWalking", false);
    }

    private void MovementCheck()
    {
        if (IsHit)
        {
            PushedMovement();
        }
        else if (inCombat)
        {
            CombatMovement();
        }
        else
        {
            if (isDashing) return;
            NormalMovement();
        }
    }

    #region NORMAL MOVEMENT

    private void NormalMovement()
    {
        Vector3 move = new Vector3(JoystickManager.Instance.HorizontalInput(), 0, JoystickManager.Instance.ForwardInput());

        if (move.magnitude != 0)
        {
            animator.SetBool("isWalking", true);

            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
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
        if (inCombat) GetComponent<MeleeAttack>().CancelCombatState();

        hitDirection = pushedDirection;
        hitSpeed = pushedForce;
        hitForce = pushedTime;

        IsHit = true;
        animator.SetBool("isPushed", IsHit);
        animator.CrossFade($"Base Layer.Empuje", 0f);

        Invoke(nameof(DisableHitMovement), hitForce);
    }

    private void PushedMovement()
    {
        charCtrlr.Move(hitDirection * hitSpeed * Time.deltaTime);
    }

    private void DisableHitMovement()
    {
        IsHit = false;
        animator.SetBool("isPushed", IsHit);
    }

    #endregion HIT MOVEMENT

    #region MELEE ASSIST MOVEMENT

    private void CombatMovement()
    {
        Vector3 cameraGc = GetCameraRelativeGC();

        if (Physics.Raycast(transform.position, cameraGc * 5f, out RaycastHit hit, 1f, whatIsMelee))
        {
            Vector3 direction = hit.collider.transform.position - transform.position;

            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 30f * Time.deltaTime);

            //Vector3 nearEnemyPos = hit.collider.transform.position + hit.transform.forward * 1.5f;
            Vector3 nearEnemyPos = hit.transform.position + (transform.position - hit.transform.position).normalized;
            Vector3 moveDirection = nearEnemyPos - transform.position;

            float distance = Vector3.Distance(transform.position, nearEnemyPos);

            if (distance > 0.5f)
            {
                charCtrlr.Move(moveDirection.normalized * Mathf.Pow(movementSpeed, 2) * Time.deltaTime);
            }
        }
    }

    private Vector3 GetCameraRelativeGC()
    {
        Vector3 camFwd = Camera.main.transform.forward;
        camFwd.y = 0;
        camFwd.Normalize();

        Vector3 camRght = Camera.main.transform.right;
        camRght.y = 0;
        camRght.Normalize();

        Vector3 cameraGc = (camFwd * JoystickManager.Instance.ForwardInput()) + (camRght * JoystickManager.Instance.HorizontalInput());
        cameraGc.Normalize();

        return cameraGc;
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


    #region VISUAL DEBUG GIZMOS

    private void OnDrawGizmos()
    {
        if (JoystickManager.Instance == null) return;

        Vector3 cameraGc = GetCameraRelativeGC();

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, cameraGc * 5f);
    }

    #endregion VISUAL DEBUG GIZMOS
}
