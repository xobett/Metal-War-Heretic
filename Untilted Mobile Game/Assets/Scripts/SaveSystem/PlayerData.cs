using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coinsAmount;
    public bool[] purchasedSkins = new bool[6];

    public PlayerData(PlayerData data)
    {
        coinsAmount = data.coinsAmount;

        for (int i = 0; i < data.purchasedSkins.Length; i++)
        {
            purchasedSkins[i] = data.purchasedSkins[i];
        }
    }
}
