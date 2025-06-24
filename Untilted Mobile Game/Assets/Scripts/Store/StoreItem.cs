using UnityEngine;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private ItemInfo item;

    public void ShowItemInfo()
    {
        StoreManager.Instance.DisplayItemInformation(item);
    }
}