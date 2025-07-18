using UnityEngine;
using UnityEngine.UI;

public class MeleeAttack : MonoBehaviour
{
    private Animator animator;

    private PlayerCamera playerCam;

    [Header("ATTACK BUTTON")]
    [SerializeField] private Button attackButton;
    private bool isPressingHit;

    [Header("MELEE ATTACK SETTINGS")]
    [SerializeField, Range(10f, 50f)] private float damage;
    [SerializeField] private float range = 0.5f;
    [SerializeField] private LayerMask whatIsMelee;

    [Header("MELEE COOLDOWN SETTINGS")]

    private float cooldownTimer;
    private float hitRate = 0.18f;

    [Header("COMBAT STATE SETTINGS")]

    private float combatTimer;
    private const float combatDuration = 0.7f;

    [HideInInspector] public bool InCombat { get; private set; }

    [Header("AIM ASSIT SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float radius;
    [HideInInspector] public bool aimAssitActive;

    public bool usedRightHand = false;

    private void Start()
    {
        GetReferences();
        AddButtonEvents();
    }

    private void Update()
    {
        HitCheck();
        AimAssit();

        RunTimers();
        CheckCombatState();
    }

    #region COMBAT STATE

    private void RunTimers()
    {
        cooldownTimer -= Time.deltaTime;
        combatTimer -= Time.deltaTime;
    }

    private void CheckCombatState()
    {
        if (combatTimer > 0)
        {
            InCombat = true;
        }
        else
        {
            InCombat = false;
        }

        animator.SetBool("inCombat", InCombat);
    }

    public void CancelCombatState()
    {
        combatTimer = 0;
    }

    private void SetCombatAndCooldownDurations()
    {
        cooldownTimer = hitRate;
        combatTimer = combatDuration;
    }

    #endregion COMBAT STATE

    #region MELEE

    private void HitCheck()
    {
        if (PlayerIsPushed) return;

        if (cooldownTimer <= 0 && (IsHitting() || isPressingHit))
        {
            SetCombatAnimations();
            SetCombatAndCooldownDurations();

            isPressingHit = false;
        }
    }

    public void HitEnemy(Collider enemyCollider)
    {
        enemyCollider.GetComponent<IDamageable>().OnDamage(damage);
        playerCam.CameraShake();
        GetComponent<ComboCounter>().IncreaseComboCount();
    }

    private void CheckUpperCutTrigger()
    {
        //Enemies spawned per area
        //Get the EnemyAreaManager
        //If its the last enemy and its health is below the damage dealth
        //Play uppercut animation
    }

    #endregion MELEE

    #region COMBAT ANIMATIONS
    private void SetCombatAnimations()
    {
        if (!usedRightHand)
        {
            animator.CrossFade($"Base Layer.Golpe derecha {GetRandomComboIndex()}", 0f, 0, 0f, 1f);
            usedRightHand = true;
        }
        else
        {
            animator.CrossFade($"Base Layer.Golpe izquierda {GetRandomComboIndex()}", 0f, 0, 0f, 1f);
            usedRightHand = false;
        }
    }

    private int GetRandomComboIndex()
    {
        return Random.Range(1, 4);
    }

    #endregion COMBAT ANIMATIONS

    #region ASSIST

    private void AimAssit()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, radius, whatIsMelee, QueryTriggerInteraction.UseGlobal);

        if (enemyColliders.Length != 0 && InCombat && (!IsDashing && !PlayerIsPushed))
        {
            GameObject enemy = enemyColliders[0].gameObject;

            Quaternion direction = Quaternion.LookRotation(enemy.transform.position - transform.position);
            Quaternion target = Quaternion.Euler(0, direction.eulerAngles.y, 0);
            Quaternion lookRotation = Quaternion.Slerp(transform.rotation, target, 10.5f * Time.deltaTime);
            transform.rotation = lookRotation;

            aimAssitActive = true;
        }
        else
        {
            aimAssitActive = false;
        }
    }

    #endregion ASSIST

    #region GET REFERENCES

    private void GetReferences()
    {
        playerCam = Camera.main.GetComponent<PlayerCamera>();
        animator = GetComponent<PlayerMovement>().playerAnimator;
    }

    private void AddButtonEvents()
    {
        attackButton.onClick.AddListener(PressHitButton);
    }

    #endregion GET REFERENCES

    #region INPUT

    private void PressHitButton()
    {
        isPressingHit = true;
    }

    public bool IsHitting() => Input.GetKeyDown(KeyCode.E);

    #endregion INPUT

    #region CHECKS

    private bool IsDashing => GetComponent<SliceAttack>().IsDashing;

    private bool PlayerIsPushed => GetComponent<PlayerMovement>().IsHit;

    #endregion CHECKS

    #region VISUAL DEBUG GIZMOS

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * range);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    #endregion VISUAL DEBUG GIZMOS

}