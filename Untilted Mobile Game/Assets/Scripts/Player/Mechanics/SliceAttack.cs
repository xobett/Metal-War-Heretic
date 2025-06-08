using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliceAttack : MonoBehaviour
{
    [Header("SLICE BUTTON")]
    [SerializeField] private Button sliceButton;

    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float sliceSpeed;
    private const float sliceDuration = 0.2f;

    private PlayerCamera cam;

    [Header("SLICE ATTACK ASSIST SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float assistRadius;
    [SerializeField] private LayerMask whatIsEnemy;

    private bool isPressingSlice;

    [Header("SLICE COOLDOWN SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float cooldownTime;

    [SerializeField] private bool isCooling;
    public bool isDashing { get; private set; }

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

    private void SliceCheck()
    {
        if (!isCooling && !isDashing && (IsSlicing() || isPressingSlice))
        {
            StartCoroutine(StartSlice());
        }
    }

    private IEnumerator StartSlice()
    {
        //If slice button was pressed, returns it to false
        isPressingSlice = false;

        isDashing = true;
        isCooling = true;
        yield return new WaitForSeconds(sliceDuration);

        isDashing = false;
        yield return new WaitForSeconds(cooldownTime);

        isCooling = false;
        StopCoroutine(StartSlice());
    }

    void SliceMovement()
    {
        if (isDashing)
        {
            Vector3 dashMovement = Vector3.zero;

            if (aimAssistActive && (JoystickManager.Instance.HorizontalInput() != 0 || JoystickManager.Instance.ForwardInput() != 0))
            {
                dashMovement = Vector3.right * JoystickManager.Instance.HorizontalInput() + Vector3.forward * JoystickManager.Instance.ForwardInput();
            }
            else
            {
                dashMovement = transform.rotation * Vector3.forward;
            }

            charCtrlr.Move(dashMovement * sliceSpeed * Time.deltaTime);
        }
    }

    #endregion SLICE ATTACK

    #region VISUAL DEBUG GIZMOS

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, assistRadius);
    }

    #endregion VISUAL DEBUG GIZMOS

    #region SNAP ASSIST

    private void SnapAssist()
    {
        if (isDashing)
        {
            Collider[] enemyColliders = Physics.OverlapSphere(transform.position, assistRadius, whatIsEnemy, QueryTriggerInteraction.UseGlobal);

            if (enemyColliders.Length != 0)
            {
                GameObject lockedEnemy = enemyColliders[0].gameObject;
                transform.position = Vector3.MoveTowards(transform.position, lockedEnemy.transform.position, Time.deltaTime * Mathf.Pow(sliceSpeed, 2));
            }
        }
    }

    private bool aimAssistActive => gameObject.GetComponent<MeleeAttack>().aimAssitActive;

    #endregion SNAP ASSIST

    #region INPUT

    private void PressSlice()
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
        sliceButton.onClick.AddListener(PressSlice);
    }

    private void GetReferences()
    {
        charCtrlr = GetComponent<CharacterController>();
        cam = Camera.main.GetComponent<PlayerCamera>();
    }

    #endregion GET REFERENCES
}
