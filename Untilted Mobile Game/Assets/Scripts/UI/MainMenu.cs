using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("LEVEL SELECTOR PANEL")]
    [SerializeField] private GameObject levelSelectorPanel;

    [SerializeField] private Button loadLevelOneButton;
    [SerializeField] private Button loadLevelTwoButton;
    [SerializeField] private Button menuButton;

    [Header("MAIN MENU PANELS")]
    [SerializeField] private Animator optionsPanel;
    [SerializeField] private Animator settingsPanel;
    [SerializeField] private Animator storePanel;

    [Header("MAIN MENU BUTTONS")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button creditsButton;

    [Header("EXTRA BUTTONS")]
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button storeBackButton;

    private void Start()
    {
        playButton.onClick.AddListener(ShowLevelSelector);
        loadLevelOneButton.onClick.AddListener(LoadLevelOne);
        loadLevelTwoButton.onClick.AddListener(LoadLevelTwo);
        menuButton.onClick.AddListener(ShowMenu);

        settingsButton.onClick.AddListener(ShowSettings);
        storeButton.onClick.AddListener(ShowStore);

        settingsBackButton.onClick.AddListener(ReturnFromSettings);
        storeBackButton.onClick.AddListener(ReturnFromStore);

        AudioManager.Instance.PlayMusic("XPMECHA MENU");
    }

    private void ShowLevelSelector()
    {
        optionsPanel.gameObject.SetActive(false);
        levelSelectorPanel.SetActive(true);
    }
    private void ShowMenu()
    {
        levelSelectorPanel.SetActive(false);
        optionsPanel.gameObject.SetActive(true);
    }

    private void LoadLevelOne()
    {
        SceneManager.LoadSceneAsync("Loading Level 1");
    }

    private void LoadLevelTwo()
    {
        SceneManager.LoadSceneAsync("Loading Level 2");
    }

    private void ShowSettings()
    {
        optionsPanel.SetTrigger("Swipe Up");
        settingsPanel.SetTrigger("Swipe In");
    }


    private void ShowStore()
    {
        optionsPanel.SetTrigger("Swipe Up");
        storePanel.SetTrigger("Swipe In");
    }

    private void ReturnFromSettings()
    {
        settingsPanel.SetTrigger("Swipe Out");
        optionsPanel.SetTrigger("Swipe Down");
    }

    private void ReturnFromStore()
    {
        storePanel.SetTrigger("Swipe Out");
        optionsPanel.SetTrigger("Swipe Down");
    }
}
