using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    public float Score { get; private set; }
    private const float maxAddedScore = 100f;
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

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void IncreaseScore(float scoreToAdd)
    {
        if (scoreToAdd > maxAddedScore)
        {
            Score += maxAddedScore;
        }
        else
        {
            Score += scoreToAdd;
        }
    }
}
