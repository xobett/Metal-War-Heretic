using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
 
    [SerializeField] public int coins;

    public PlayerSkin[] PlayerSkins = new PlayerSkin[6];

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
}
