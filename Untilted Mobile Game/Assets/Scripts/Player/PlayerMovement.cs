using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Player settings
    private CharacterController charCtrlr;

    [Header("ANIMATOR SETTINGS")]
    [SerializeField] public Animator playerAnimator;

    [Header("NORMAL MOVEMENT SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float movementSpeed;
    [SerializeField, Range(0f, 1f)] private float rotationSpeed;

    private float velocity;

    [Header("HIT MOVEMENT SETTINGS")]
    private float hitSpeed;
    private float hitForce;

    private Vector3 hitDirection;

    [HideInInspector] public bool IsHit {  get; private set; }

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
        if (!IsHit)
        {
            if (!inCombat)
            {
                if (aimAssistActive)
                {
                    AimMovement();
                }
                else
                {
                    NormalMovement();
                } 
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
            playerAnimator.SetBool("isWalking", true);

            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref velocity, rotationSpeed);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = transform.rotation * Vector3.forward;

            charCtrlr.Move(moveDirection * movementSpeed * Time.deltaTime);
        }
        else
        {
            playerAnimator.SetBool("isWalking", false);
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
        StartCoroutine(StartHitMovementTimer());
    }

    public void HitMovement()
    {
        charCtrlr.Move(hitDirection * hitSpeed * Time.deltaTime);
    }

    private IEnumerator StartHitMovementTimer()
    {
        yield return new WaitForSeconds(hitForce);

        IsHit = false;
        yield return null;
    }

    #endregion HIT MOVEMENT

    #region AIM MOVEMENT

    private void AimMovement()
    {
        Vector3 aimMove = Vector3.right * JoystickManager.Instance.HorizontalInput() + Vector3.forward * JoystickManager.Instance.ForwardInput();
        charCtrlr.Move(aimMove * movementSpeed * Time.deltaTime);
    }

    private bool aimAssistActive => gameObject.GetComponent<MeleeAttack>().aimAssitActive;

    #endregion AIM MOVEMENT

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
    }

    #endregion START
}
