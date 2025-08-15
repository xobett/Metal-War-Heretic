using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI playerCoins;
    [SerializeField] private TextMeshProUGUI itemCostText;

    [SerializeField] private Button equipButton;

    [SerializeField] private Sprite equipSprite;
    [SerializeField] private Sprite equippedSprite;
    [SerializeField] private Sprite nonEquipableSprite;

    [SerializeField] private Button buyButton;

    [SerializeField] private Sprite buySprite;
    [SerializeField] private Sprite boughtSprite;

    private SOPlayerSkin selectedSkin;


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
            itemCostText.text = selectedSkin.cost.ToString();
        }
    }

    public void BuySelectedItem()
    {
        if (GameManager.Instance.score >= selectedSkin.cost)
        {
            selectedSkin.isPurchased = true;
            selectedSkin.isEquipped = true;

            GameManager.Instance.score -= selectedSkin.cost;
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
            buyButton.image.sprite = boughtSprite;

            if (selectedSkin.isEquipped)
            {
                //equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "EQUIPPED";
                equipButton.image.sprite = equippedSprite;
                equipButton.interactable = false;
            }
            else
            {
                equipButton.interactable = true;
                equipButton.image.sprite = equipSprite;
                //equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "EQUIP";
            }
        }
        else
        {
            buyButton.interactable = true;
            buyButton.image.sprite = buySprite;

            equipButton.interactable = false;
            equipButton.image.sprite = nonEquipableSprite;

            //equipButton.GetComponentInChildren<TextMeshProUGUI>().text = "EQUIP";
        }
    }

    private void UpdatePlayerCoinsText()
    {
        playerCoins.text = GameManager.Instance.score.ToString();
    }

    public void EquipSelectedSkin()
    {
        GameManager.Instance.EquipSkin(selectedSkin);
        selectedSkin.isEquipped = true;
        UpdateItemState();
    }
}

