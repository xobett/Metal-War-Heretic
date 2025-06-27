using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("PAUSE MENU SETTINGS")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseGameButton;

    [Header("HUD PANEL")]
    [SerializeField] private GameObject hudPanel;

    [Header("PAUSE MENU OPTIONS PANELS")]
    [SerializeField] private GameObject controlsPanel;

    private List<GameObject> pauseMenuPanels = new List<GameObject>();

    [Header("\nPAUSE MENU OPTIONS BUTTONS")]
    [SerializeField] private Button continuePlayingButton;
    [SerializeField] private Button showControlsButton;
    [SerializeField] private Button returnToMainMenuButton;

    [SerializeField] private Button[] backButtons;

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

        continuePlayingButton.onClick.AddListener(ContinuePlaying);
        showControlsButton.onClick.AddListener(ShowControls);
        returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);

        foreach (Button backButton in backButtons)
        {
            backButton.onClick.AddListener(HeadBack);
        }

        pauseMenuPanels.Add(controlsPanel);
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

    public void ShowControls()
    {
        controlsPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GamePaused = false;
        SceneManager.LoadScene("Main Menu");
    }

    public void HeadBack()
    {
        foreach (GameObject panel in pauseMenuPanels)
        {
            panel.SetActive(false);
        }
    }

    #endregion SELECTION METHODS

    private bool IsPausing() => Input.GetKeyDown(KeyCode.Escape);
}