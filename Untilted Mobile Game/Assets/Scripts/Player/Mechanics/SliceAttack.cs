using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SliceAttack : MonoBehaviour
{

    /// <summary>
    /// Attack logic and damage is done in the SliceAttackCollider
    /// </summary>

    private Animator animator;

    [Header("SLICE BUTTON")]
    [SerializeField] private Button sliceButton;

    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float speed;
    [SerializeField] private float duration = 0.2f;

    [Header("SLICE ATTACK ASSIST SETTINGS")]
    [SerializeField] private LayerMask whatIsEnemy;

    private bool isPressingSlice;

    [Header("SLICE COOLDOWN SETTINGS")]
    private float cooldownTime = 0.5f;
    private bool isCooling;
    
    public bool IsDashing { get; private set; }

    //Reference to the player's character controller
    private CharacterController charCtrlr;

    void Start()
    {
        GetReferences();
        AddButtonEvents();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.Instance.movementEnabled) return;

        SliceCheck();
        SliceMovement();
        SnapAssist();
    }

    #region SLICE ATTACK

    public void CancelSliceMovement()
    {
        IsDashing = false;
        animator.SetBool("isDashing", false);
    }

    private void SliceCheck()
    {
        if (IsHit) return;

        if (!isCooling && !IsDashing && (IsSlicing() || isPressingSlice))
        {
            StartCoroutine(StartSlice());

            animator.SetBool("isDashing", true);
            animator.CrossFade("Base Layer.Dash", 0f);
        }
    }

    private IEnumerator StartSlice()
    {
        //If slice button was pressed, returns it to false
        isPressingSlice = false;

        if (inCombat) GetComponent<MeleeAttack>().CancelCombatState();

        IsDashing = true;
        isCooling = true;

        AudioManager.Instance.PlaySFX("DASH", 1);

        yield return new WaitForSeconds(duration);

        if (IsDashing)
        {
            IsDashing = false;
            animator.SetBool("isDashing", false);
        }

        yield return new WaitForSeconds(cooldownTime);

        isCooling = false;
        yield break;
    }

    void SliceMovement()
    {
        if (IsDashing)
        {
            Vector3 dashMovement = Vector3.zero;

            if (inCombat)
            {
                dashMovement = GetCameraRelativeGC();

                float targetAngle = Mathf.Atan2(dashMovement.x, dashMovement.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }
            else
            {
                dashMovement = transform.rotation * Vector3.forward;
            }

            charCtrlr.Move(dashMovement * speed * Time.deltaTime);
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

    #endregion SLICE ATTACK

    #region SNAP ASSIST

    private void SnapAssist()
    {
        if (IsDashing)
        {
            Vector3 cameraGc = GetCameraRelativeGC();

            if (Physics.Raycast(transform.position, cameraGc * 5f, out RaycastHit hit, 3f, whatIsEnemy))
            {
                Vector3 direction = hit.collider.transform.position - transform.position;

                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 30f * Time.deltaTime);

                Vector3 nearEnemyPos = hit.transform.position + (transform.position - hit.transform.position).normalized;
                Vector3 moveDirection = nearEnemyPos - transform.position;

                float distance = Vector3.Distance(transform.position, nearEnemyPos);

                if (distance > 0.5f)
                {
                    charCtrlr.Move(moveDirection.normalized * speed * Time.deltaTime);
                }
            }
        }
    }

    #endregion SNAP ASSIST

    #region CHECKS

    private bool IsHit => GetComponent<PlayerMovement>().IsHit;

    private bool inCombat => GetComponent<MeleeAttack>().InCombat;

    #endregion CHECKS

    #region INPUT

    private void OnPressSliceButton()
    {
        isPressingSlice = true;
    }

    private bool IsSlicing()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    #endregion INPUT

    #region GET REFERENCES

    private void AddButtonEvents()
    {
        sliceButton.onClick.AddListener(OnPressSliceButton);
    }

    private void GetReferences()
    {
        charCtrlr = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        sliceButton = GameObject.FindGameObjectWithTag("Slice Button").GetComponent<Button>();
    }

    #endregion GET REFERENCES
}
