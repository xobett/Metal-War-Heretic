using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI playerCoins;
    [SerializeField] private TextMeshProUGUI itemCostText;

    [SerializeField] private Button equipButton;
    [SerializeField] private Button buyButton;

    [SerializeField] private SOPlayerSkin selectedSkin;

    /// <summary>
    /// DISABLE BUY BUTTON UPON SKIN PURCHASED
    /// </summary>


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdatePlayerCoinsText();
    }

    public void SelectItem(SOPlayerSkin skin)
    {
        selectedSkin = skin;

        DisplaySkinCost();
        UpdateItemState();
    }

    public void DisplaySkinCost()
    {
        if (selectedSkin.isPurchased)
        {
            itemCostText.text = "OWNED";
        }
        else
        {
            itemCostText.text = $"Cost: {selectedSkin.cost}";
        }
    }

    public void BuySelectedItem()
    {
        if (GameManager.Instance.coins >= selectedSkin.cost)
        {
            selectedSkin.isPurchased = true;
            selectedSkin.isEquipped = true;

            GameManager.Instance.coins -= selectedSkin.cost;
            GameManager.Instance.AddPurchasedSkin(selectedSkin);
            GameManager.Instance.EquipSkin(selectedSkin);

            UpdateItemState();
            UpdatePlayerCoinsText();
        }
    }

    private void UpdateItemState()
    {
        if (selectedSkin.isPurchased)
        {
            buyButton.interactable = false;

            if (selectedSkin.isEquipped)
            {
                equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "EQUIPPED";
                equipButton.interactable = false;
            }
            else
            {
                equipButton.interactable = true;
                equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "EQUIP";
            }
        }
        else
        {
            buyButton.interactable = true;
            equipButton.interactable = false;

            equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "EQUIP";
        }
    }

    private void UpdatePlayerCoinsText()
    {
        playerCoins.text = $"Coins: {GameManager.Instance.coins}";
    }

    public void EquipSelectedSkin()
    {
        GameManager.Instance.EquipSkin(selectedSkin);
        selectedSkin.isEquipped = true;
        UpdateItemState();
    }
}

