using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    private List<SOPlayerSkin> purchasedSkins = new List<SOPlayerSkin>();

    public SOPlayerSkin EquippedSkin;
 
    [SerializeField] public int coins;

    private const int maxAddedScore = 100;

    public int RoundCount { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseScore(int scoreToAdd)
    {
        if (scoreToAdd > maxAddedScore)
        {
            coins += maxAddedScore;
        }
        else
        {
            coins += scoreToAdd;
        }
    }

    public void EquipSkin(SOPlayerSkin skin)
    {
        if (purchasedSkins.Contains(skin))
        {
            if (EquippedSkin != null)
            {
                EquippedSkin.isEquipped = false;
            }

            EquippedSkin = skin;
        }
        else
        {
            Debug.Log("Unexpected bug!");
        }
    }

    public void AddPurchasedSkin(SOPlayerSkin skin)
    {
        purchasedSkins.Add(skin);
    }
}
