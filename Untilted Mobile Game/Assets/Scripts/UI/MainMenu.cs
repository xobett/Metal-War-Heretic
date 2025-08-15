using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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
        playButton.onClick.AddListener(LoadLevel);
        settingsButton.onClick.AddListener(ShowSettings);
        storeButton.onClick.AddListener(ShowStore);

        settingsBackButton.onClick.AddListener(ReturnFromSettings);
        storeBackButton.onClick.AddListener(ReturnFromStore);
    }

    private void LoadLevel()
    {
        SceneManager.LoadSceneAsync("Loading Game");
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
