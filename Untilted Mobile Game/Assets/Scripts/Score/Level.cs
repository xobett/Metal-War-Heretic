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
        Start_SetLevelSettings();
    }


    #region START

    private void Start_SetLevelSettings()
    {
        LevelScore = 0;

        Invoke(nameof(PlayLevelMusic), 2f);

    }

    #endregion START

    private void PlayLevelMusic()
    {
        AudioManager.Instance.PlayMusic("XPMECHA GAMEPLAY");
    }

    public void AddScore(int score)
    {
        LevelScore += score;
    }
}