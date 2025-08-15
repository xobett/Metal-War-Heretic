using UnityEngine;
using UnityEngine.UIElements;

public class Level : MonoBehaviour
{
    public static Level Instance { get; private set; }

    public int LevelScore { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LevelScore = 0;
    }

    public void AddScore(int score)
    {
        LevelScore += score;
    }
}