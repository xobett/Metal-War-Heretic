using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coins;

    public PlayerSkin[] skin = new PlayerSkin[6];

    public PlayerData(GameManager data)
    {
        this.coins = data.coins;

        for (int i = 0; i < data.PlayerSkins.Length; i++)
        {
            this.skin[i] = data.PlayerSkins[i];
        }
    }
}
