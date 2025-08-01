using UnityEngine;

public class Player : MonoBehaviour
{
    private Renderer rndr;
    private Material newMTL;

    public static Player Instance { get; private set; }

    private void Awake()
    {
        Awake_GetReferences();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
