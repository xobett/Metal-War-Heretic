using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("LOADING SCENE SETTINGS")]
    [SerializeField] private string sceneName;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private Button continueButton;

    private float targetValue;
    private const float loadingBarSpeed = 1.2f;

    private bool changeScene;
    
    void Start()
    {
        //Deactivate interaction with continue button
        loadingBar.interactable = false;
        continueButton.interactable = false;
        
        StartCoroutine(LoadScene());
    }

    private void Update()
    {
        UpdateLoadingSceneBar();
    }

    private void UpdateLoadingSceneBar()
    {
        loadingBar.value = Mathf.MoveTowards(loadingBar.value, targetValue, loadingBarSpeed * Time.deltaTime);
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(sceneName);
        loadSceneOp.allowSceneActivation = false;

        yield return null;

        float loadProgress = 0;
        do
        {
            yield return new WaitForSeconds(1f);

            loadProgress = Mathf.Clamp01(loadSceneOp.progress / 0.9f);
            targetValue = loadProgress;
        }
        while (loadProgress < 1);

        yield return new WaitUntil(() => loadingBar.value == 1);

        continueButton.interactable = true;
        continueButton.onClick.AddListener(AllowSceneChange);

        yield return new WaitUntil(() => changeScene);

        loadSceneOp.allowSceneActivation = true;
    }

    private void AllowSceneChange()
    {
        changeScene = true;
    }
}
