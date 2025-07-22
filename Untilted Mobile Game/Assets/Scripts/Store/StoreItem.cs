using UnityEngine;

public class StoreItem : MonoBehaviour
{
    public PlayerSkin skinItem;

    public void ShowItemInfo()
    {
        StoreManager.Instance.DisplayItemInformation(this);
        StoreManager.Instance.SelectItem(this);
    }
}

[System.Serializable]
public struct PlayerSkin
{
    [SerializeField] public string name;
    [SerializeField] public int cost;
    [SerializeField] public bool purchased;
}