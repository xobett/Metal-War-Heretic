using UnityEngine;

public class Player : MonoBehaviour
{
    private Renderer rndr;

    public static Player Instance { get; private set; }

    private float timeBeforeNextHealth = 5;
    private Health playerHealth;

    private MeleeAttack melee;
    internal EnemyArea lastEnemyAreaEntered;

    internal bool movementEnabled;
    internal bool combatEnabled;

    private bool healthRegained = false;

    private void Awake()
    {
        Awake_GetReferences();

        movementEnabled = true;
        combatEnabled = true;
    }

    #region AWAKE
    private void Awake_GetReferences()
    {
        Instance = this;
        rndr = GetComponentInChildren<Renderer>();
        playerHealth = GetComponent<Health>();
        melee = GetComponent<MeleeAttack>();
    }

    #endregion AWAKE

    void Start()
    {
        Start_EquipSavedSkin();
    }

    #region START

    private void Start_EquipSavedSkin()
    {
        rndr.material = GameManager.Instance.EquippedSkin.skinMTL;
    }

    #endregion START

    #region MOVEMENT

    public void DisableMovement()
    {
        movementEnabled = false;
        GetComponent<PlayerMovement>().DisableMovementAnim();
    }

    public void DisableMovement(float time)
    {
        movementEnabled = false;
        GetComponent<PlayerMovement>().DisableMovementAnim();
        Invoke(nameof(EnableMovement), time);
    }

    public void EnableMovement()
    {
        movementEnabled = true;
    }

    #endregion MOVEMENT

    #region HEALTH

    private void PassiveHealthRegain_Update()
    {
        if (healthRegained) return;
        if (playerHealth.CurrentHealth == 100 || melee.InCombat) return;

        healthRegained = true;

        playerHealth.AddHealth(10);
        Invoke(nameof(EnableHealthRegain), timeBeforeNextHealth);
    }

    private void EnableHealthRegain()
    {
        healthRegained = false;
    }

    #endregion HEALTH

    #region ENEMY AREA

    public void SetEnemyArea(EnemyArea area)
    {
        lastEnemyAreaEntered = area;
    }

    public void ResetOnAreaValue()
    {
        if (lastEnemyAreaEntered == null) return;
        lastEnemyAreaEntered.playerIsOnArea = false;
    }

    #endregion ENEMY AREA

    // Update is called once per frame
    void Update()
    {
        PassiveHealthRegain_Update();
    }
}
