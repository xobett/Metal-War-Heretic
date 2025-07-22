using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Renderer rndr;
    [SerializeField] private Material newMTL;

    void Start()
    {
        ChangeSkin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeSkin()
    {
        rndr = GetComponentInChildren<Renderer>();

        rndr.material = GameManager.Instance.EquippedSkin.skinMTL;
    }
}
