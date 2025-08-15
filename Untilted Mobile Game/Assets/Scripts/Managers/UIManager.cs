using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("PAUSE MENU SETTINGS")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseGameButton;

    [Header("HUD PANEL")]
    [SerializeField] private GameObject hudPanel;

    [Header("SCORE PANEL")]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private TMP_Text scoreOutlineText;
    [SerializeField] private TMP_Text scoreNormalText;

    [Header("\nPAUSE MENU OPTIONS BUTTONS")]
    [SerializeField] private Button menuButton;
    [SerializeField] private Button continueButton;

    public bool GamePaused { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AdddButtonsEvents();
    }

    private void Update()
    {
        if (IsPausing() && !GamePaused)
        {
            PauseGame();

        }
        else if (IsPausing() && GamePaused)
        {
            UnPauseGame();
        }
    }

    private void AdddButtonsEvents()
    {
        pauseGameButton.onClick.AddListener(PauseGame);

        continueButton.onClick.AddListener(ContinuePlaying);
        menuButton.onClick.AddListener(ReturnToMainMenu);
    }

    #region PAUSE METHODS

    private void UnPauseGame()
    {
        Time.timeScale = 1f;
        GamePaused = false;
        pausePanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        GamePaused = true;
        pausePanel.SetActive(true);
        hudPanel.SetActive(false);
    }

    #endregion PAUSE METHODS

    #region SELECTION METHODS

    public void ContinuePlaying()
    {
        UnPauseGame();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GamePaused = false;
        SceneManager.LoadScene("Main Menu");
    }

    #endregion SELECTION METHODS

    #region SCORE

    [ContextMenu("Test Show Score")]
    public void DisplayLevelScore()
    {
        scoreOutlineText.text = Level.Instance.LevelScore.ToString();
        scoreNormalText.text = Level.Instance.LevelScore.ToString();

        scorePanel.SetActive(true);
    }

    #endregion SCORE

    private bool IsPausing() => Input.GetKeyDown(KeyCode.Escape);
}