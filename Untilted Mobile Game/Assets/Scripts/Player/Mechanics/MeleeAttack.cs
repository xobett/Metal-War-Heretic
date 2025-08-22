using UnityEngine;
using UnityEngine.UI;

public class MeleeAttack : MonoBehaviour
{
    private Animator animator;

    [Header("ATTACK BUTTON")]
    [SerializeField] private Button attackButton;
    private bool isPressingHit;

    [Header("MELEE ATTACK SETTINGS")]
    [SerializeField, Range(10f, 50f)] private float damage;
    [SerializeField] private float range = 0.5f;
    [SerializeField] private LayerMask whatIsMelee;

    [SerializeField] public SOShakeData playerHitShake;

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

    private void Start()
    {
        GetReferences();
        AddButtonEvents();
    }

    private void Update()
    {
        if (!Player.Instance.combatEnabled) return;

        Melee_Update();
        CombatState_Update();
    }

    #region COMBAT STATE

    private void CombatState_Update()
    {
        RunTimers();
        SetCombatState();
    }

    private void RunTimers()
    {
        cooldownTimer -= Time.deltaTime;
        combatTimer -= Time.deltaTime;
    }

    private void SetCombatState()
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

    private void Melee_Update()
    {
        HitCheck();
    }

    private void HitCheck()
    {
        if (PlayerIsPushed) return;

        if (cooldownTimer <= 0 && (IsHitting() || isPressingHit))
        {
            if (IsDashing) GetComponent<SliceAttack>().CancelSliceMovement();

            SetMeleeAnimations();
            SetCombatAndCooldownDurations();

            isPressingHit = false;
        }
    }

    public void HitEnemy(Collider enemyCollider)
    {
        enemyCollider.GetComponent<IDamageable>().OnDamage(damage);
        GetComponent<ComboCounter>().IncreaseComboCount();
    }

    private void SetMeleeAnimations()
    {
        if (!InCombat)
        {
            animator.CrossFade($"Base Layer.Golpes TODOS", 0f);
        }
    }

    #endregion MELEE

    #region GET REFERENCES

    private void GetReferences()
    {
        animator = GetComponentInChildren<Animator>();
        attackButton = GameObject.FindGameObjectWithTag("Attack Button").GetComponent<Button>();
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

}