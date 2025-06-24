using TMPro;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance {  get; private set; }

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemCostText;

    private void Awake()
    {
        Instance = this;
    }

    public void DisplayItemInformation(ItemInfo itemInfo)
    {
        itemNameText.text = $"Outfit Name: \"{itemInfo.itemName}\"";
        itemCostText.text = $"Outfit Cost: ${itemInfo.itemCost}";
    }
}

