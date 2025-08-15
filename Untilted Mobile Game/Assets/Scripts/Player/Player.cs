using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Renderer rndr;
    private Material newMTL;

    public static Player Instance { get; private set; }

    public bool movementEnabled;

    private void Awake()
    {
        Awake_GetReferences();

        movementEnabled = true;
    }

    #region AWAKE
    private void Awake_GetReferences()
    {
        Instance = this;
        rndr = GetComponentInChildren<Renderer>();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
