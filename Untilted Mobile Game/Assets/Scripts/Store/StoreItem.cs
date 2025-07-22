using UnityEngine;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private SOPlayerSkin skinItem;

    public void ShowItemInfo()
    {
        StoreManager.Instance.SelectItem(skinItem);
    }
}