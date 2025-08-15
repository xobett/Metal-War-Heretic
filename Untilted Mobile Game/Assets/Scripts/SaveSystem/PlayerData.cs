using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coins;

    public PlayerData(GameManager data)
    {
        this.coins = data.score;
    }
}
