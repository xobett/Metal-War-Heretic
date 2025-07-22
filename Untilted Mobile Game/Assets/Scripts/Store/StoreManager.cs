using TMPro;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance {  get; private set; }

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemCostText;

    /// <summary>
    /// DISABLE BUY BUTTON UPON SKIN PURCHASED
    /// </summary>

    private StoreItem itemToBuy;

    private void Awake()
    {
        Instance = this;
    }

    public void DisplayItemInformation(StoreItem item)
    {
        itemNameText.text = $"Outfit Name: \"{item.skinItem.name}\"";
        itemCostText.text = $"Outfit Cost: ${item.skinItem.cost}";
    }

    public void SelectItem(StoreItem item)
    {
        itemToBuy = item;
    }

    public void BuySelectedItem()
    {
        Debug.Log("Entered");
        if (GameManager.Instance.coins >= itemToBuy.skinItem.cost)
        {
            for (int i = 0; i < GameManager.Instance.PlayerSkins.Length; i++)
            {
                if (GameManager.Instance.PlayerSkins[i].name == itemToBuy.skinItem.name)
                {
                    GameManager.Instance.PlayerSkins[i].purchased = true;
                    GameManager.Instance.coins -= itemToBuy.skinItem.cost;
                    break;
                }
            }
        }
    }
}

