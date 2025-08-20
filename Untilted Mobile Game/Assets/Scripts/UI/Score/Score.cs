using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        AssignContinueButton();
    }

    private void AssignContinueButton()
    {
        var currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Level 1")
        {
            continueButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("Loading Level 2"));
        }
        else
        {
            continueButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("Main Menu"));
        }
    }
}