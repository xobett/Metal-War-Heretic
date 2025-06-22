using UnityEngine;
using UnityEngine.UI;

public class MeleeAttack : MonoBehaviour
{
    private Animator playerAnimator;

    [Header("ATTACK BUTTON")]
    [SerializeField] private Button attackButton;

    [Header("MELEE ATTACK SETTINGS")]
    [SerializeField, Range(10f, 50f)] private float damage;
    [SerializeField] private float hitRange = 0.5f;
    [SerializeField] private LayerMask whatIsMelee;

    private int hitsMade;

    private PlayerCamera playerCam;

    [Header("MELEE COOLDOWN SETTINGS")]
    [SerializeField] private float afterComboCooldownTime = 1.5f;
    private const float inComboCooldownTime = 0.1f;

    //Timer used to limit player from spamming attack
    private float meleeCooldownTimer;
    private float meleeCooldownDuration;

    [Header("COMBAT STATE SETTINGS")]
    //Timer used to handle combat state of player
    //Controls animations and player movement independently from melee cooldown time
    [SerializeField] private float inComboCombatDuration = 1f;
    [SerializeField] private float afterComboCombatDuration;

    private float combatTimer;
    private float combatDuration;

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
        HitCheck();
        AimAssit();

        RunTimers();
        CheckCombatState();
    }

    #region TIMERS

    private void RunTimers()
    {
        meleeCooldownTimer -= Time.deltaTime;
        combatTimer -= Time.deltaTime;
    }

    #endregion TIMERS

    #region COMBAT STATE

    private void CheckCombatState()
    {
        if (combatTimer > 0)
        {
            InCombat = true;
        }
        else
        {
            InCombat = false;

            //Resets hits count to avoid animator bugs and melee coldown incorrect cooldown timing
            hitsMade = 0;
        }

        playerAnimator.SetBool("inCombat", InCombat);
    }

    private void SetCombatAndCooldownDurations()
    {
        hitsMade++;

        if (hitsMade <= 3)
        {
            meleeCooldownDuration = inComboCooldownTime;
            combatDuration = inComboCombatDuration;
        }
        else
        {
            meleeCooldownDuration = afterComboCooldownTime;
            combatDuration = afterComboCombatDuration;

            //Resets hits count upon last combo hit.
            hitsMade = 0;
        }

        meleeCooldownTimer = meleeCooldownDuration;
        combatTimer = combatDuration;
    }

    #endregion COMBAT STATE

    #region MELEE

    public void HitCheck()
    {
        if (playerIsPushed) return;

        if (meleeCooldownTimer <= 0 && IsHitting())
        {
            SetCombatAnimations();
            SetCombatAndCooldownDurations();
            //Punch();
        }
    }

    private void Punch()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward * hitRange, out hit, hitRange))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                hit.collider.GetComponent<IDamageable>().OnDamage(damage);
                playerCam.CameraShake();
                GetComponent<ComboCounter>().IncreaseComboCount();
            }
        }
    }

    public void HitEnemy(Collider enemyCollider)
    {
        enemyCollider.GetComponent<IDamageable>().OnDamage(damage);
        playerCam.CameraShake();
        GetComponent<ComboCounter>().IncreaseComboCount();
    }

    #endregion MELEE

    #region COMBAT ANIMATIONS
    private void SetCombatAnimations()
    {
        if (hitsMade == 0)
        {
            playerAnimator.Play($"Combo {GetRandomComboAnimation()} - 1", 0, 0.15f);
        }
        else
        {
            playerAnimator.SetTrigger("Hit");
        }
    }

    private int GetRandomComboAnimation()
    {
        return Random.Range(1, 4);
    }

    #endregion COMBAT ANIMATIONS

    #region ASSIST

    private void AimAssit()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, radius, whatIsMelee, QueryTriggerInteraction.UseGlobal);

        if (enemyColliders.Length != 0 && InCombat && (!isDashing && !playerIsPushed))
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
        playerAnimator = GetComponent<PlayerMovement>().playerAnimator;
    }

    private void AddButtonEvents()
    {
        attackButton.onClick.AddListener(Punch);
    }

    #endregion GET REFERENCES

    #region INPUT
    public bool IsHitting() => Input.GetKeyDown(KeyCode.E);

    #endregion INPUT

    #region CHECKS

    private bool isDashing => GetComponent<SliceAttack>().IsDashing;

    private bool playerIsPushed => GetComponent<PlayerMovement>().IsHit;

    #endregion CHECKS

    #region VISUAL DEBUG GIZMOS

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * hitRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    #endregion VISUAL DEBUG GIZMOS

}