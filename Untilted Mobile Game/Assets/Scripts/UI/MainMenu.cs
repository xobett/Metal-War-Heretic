using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("MAIN MENU PANELS")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject exitPanel;

    [SerializeField] private GameObject darkPanel;

    [Header("MAIN MENU BUTTONS")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    [Header("EXTRA BUTTONS")]
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button[] backButtons;

    private List<GameObject> panels = new List<GameObject>();

    void Start()
    {
        AssignButtonEvents();


    }

    #region PLAY - EXIT METHODS
    public void LoadGame()
    {
        SceneManager.LoadScene("Loading Game");
    }
    private void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region BUTTON EVENTS & PANELS ASSIGNMENT
    private void AssignButtonEvents()
    {
        playButton.onClick.AddListener(LoadGame);

        optionsButton.onClick.AddListener(ShowOptions);
        storeButton.onClick.AddListener(ShowStore);
        creditsButton.onClick.AddListener(ShowCredits);
        exitButton.onClick.AddListener(ShowExit);

        exitGameButton.onClick.AddListener(ExitGame);

        foreach (Button button in backButtons)
        {
            button.onClick.AddListener(HidePanels);
        }

        panels.Add(optionsPanel);
        panels.Add(storePanel);
        panels.Add(creditsPanel);
        panels.Add(exitPanel);
    }
    #endregion BUTTON EVENTS & PANELS ASSIGNMENT

    #region SHOW SELECTION METHODS
    private void ShowOptions()
    {
        darkPanel.SetActive(true);
        optionsPanel.SetActive(true);
    }

    private void ShowStore()
    {
        darkPanel.SetActive(true);
        storePanel.SetActive(true);
    }

    private void ShowCredits()
    {
        darkPanel.SetActive(true);
        creditsPanel.SetActive(true);
    }

    private void ShowExit()
    {
        darkPanel.SetActive(true);
        exitPanel.SetActive(true);
    }
    #endregion SHOW SELECTION METHODS

    #region GO BACK METHODS
    private void HidePanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        darkPanel.SetActive(false);
    }
    #endregion GO BACK METHODS
}
