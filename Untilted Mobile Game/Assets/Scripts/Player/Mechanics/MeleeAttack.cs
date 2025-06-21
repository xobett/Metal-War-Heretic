using UnityEngine;
using UnityEngine.UI;

public class MeleeAttack : MonoBehaviour
{
    private Animator playerAnimator;

    [Header("ATTACK BUTTON")]
    [SerializeField] private Button attackButton;

    [Header("MELEE ATTACK SETTINGS")]
    [SerializeField, Range(10f, 50f)] private float damage;
    [SerializeField] private float hitRate;
    [SerializeField] private float hitRange = 0.5f;

    [SerializeField] private LayerMask whatIsMelee;

    private PlayerCamera playerCam;

    private float meleeTimer;
    [SerializeField, Range(0f, 1f)] private float meleeCooldownTime;
    private bool ableToMelee;

    [Header("AIM ASSIT SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float radius;
    public bool aimAssitActive;

    [Header("COMBO ATTACK SETTINGS")]
    private float combatTimer;
    [SerializeField, Range(0f, 1f)] private float combatTime;
    public bool InCombat { get; private set; }

    private bool engagedCombat;

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
        CombatCheck();
        MeleeCheck();
    }

    #region MELEE AND COMBAT TIMERS / CHECKS

    private void CombatCheck()
    {
        if (combatTimer > 0)
        {
            InCombat = true;
        }
        else
        {
            InCombat = false;
        }

        playerAnimator.SetBool("inCombat", InCombat);
    }

    private void MeleeCheck()
    {
        if (meleeTimer < 0)
        {
            ableToMelee = true;
        }
        else
        {
            ableToMelee = false;
        }
    }

    private void RunTimers()
    {
        combatTimer -= Time.deltaTime;
        meleeTimer -= Time.deltaTime;
    }

    #endregion MELEE AND COMBAT TIMERS / CHECKS

    #region MELEE

    public void HitCheck()
    {
        if (ableToMelee && IsHitting())
        {
            //Adds a time window to add combo punches
            combatTimer = combatTime;

            //Due to unity placing trigger parameters on queue and not executing them right away, it plays the animation instantly if its not engaging in combat.
            if (!InCombat)
            {
                playerAnimator.Play("Golpe derecha 2 1", 0, 0.15f);
                meleeTimer = meleeCooldownTime;
            }
            //If the player is engaged in combat, then it executes a normal trigger behaviour.
            else
            {
                meleeTimer = 0.2f;
                playerAnimator.SetTrigger("Hit");
            }

            Debug.Log("Hitting");
            Punch();
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

    #endregion MELEE

    #region ASSIST

    private void AimAssit()
    {
        //Check for enemies in range

        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, radius, whatIsMelee, QueryTriggerInteraction.UseGlobal);

        if (enemyColliders.Length != 0 && !IsDashing)
        {
            //Look at enemy directly

            GameObject enemy = enemyColliders[0].gameObject;

            Quaternion direction = Quaternion.LookRotation(enemy.transform.position - transform.position);
            Quaternion target = Quaternion.Euler(0, direction.eulerAngles.y, 0);
            Quaternion lookRotation = Quaternion.Slerp(transform.rotation, target, 3.5f * Time.deltaTime);
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
    public bool IsActivatingAim() => Input.GetKeyDown(KeyCode.O);
    public bool IsHitting() => Input.GetKeyDown(KeyCode.E);

    #endregion INPUT

    private bool IsDashing => gameObject.GetComponent<SliceAttack>().isDashing;

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