using System.Collections;
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

    private PlayerCamera cam;

    [Header("SLICE ATTACK ASSIST SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float assistRadius;
    [SerializeField] private LayerMask whatIsEnemy;

    private bool isPressingSlice;

    [Header("SLICE COOLDOWN SETTINGS")]
    [SerializeField, Range(0.5f, 2f)] private float cooldownTime;

    [SerializeField] private bool isCooling;
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
        SliceCheck();
        SliceMovement();
        SnapAssist();
    }

    #region SLICE ATTACK

    public void CancelSliceMovement()
    {
        IsDashing = false;
    }

    private void SliceCheck()
    {
        if (!isCooling && !IsDashing && (IsSlicing() || isPressingSlice))
        {
            StartCoroutine(StartSlice());

            //Change from trigger to play for better transition
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
        yield return new WaitForSeconds(duration);

        IsDashing = false;
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
                dashMovement = Vector3.right * JoystickManager.Instance.HorizontalInput() + Vector3.forward * JoystickManager.Instance.ForwardInput();

                Vector3 move = new Vector3(JoystickManager.Instance.HorizontalInput(), 0, JoystickManager.Instance.ForwardInput());
                float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }
            else
            {
                dashMovement = transform.rotation * Vector3.forward;
            }

            charCtrlr.Move(dashMovement * speed * Time.deltaTime);
        }
    }

    #endregion SLICE ATTACK

    #region SNAP ASSIST

    private void SnapAssist()
    {
        if (IsDashing)
        {
            Collider[] enemyColliders = Physics.OverlapSphere(transform.position, assistRadius, whatIsEnemy, QueryTriggerInteraction.UseGlobal);

            if (enemyColliders.Length != 0)
            {
                GameObject lockedEnemy = enemyColliders[0].gameObject;
                transform.position = Vector3.MoveTowards(transform.position, lockedEnemy.transform.position, Time.deltaTime * Mathf.Pow(speed, 2));
            }
        }
    }

    private bool aimAssistActive => gameObject.GetComponent<MeleeAttack>().aimAssitActive;

    #endregion SNAP ASSIST

    #region CHECKS

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
        cam = Camera.main.GetComponent<PlayerCamera>();
        animator = GetComponentInChildren<Animator>();
    }

    #endregion GET REFERENCES
}
