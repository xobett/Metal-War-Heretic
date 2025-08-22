using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("LOADING SCENE SETTINGS")]
    [SerializeField] private string sceneName;

    [SerializeField] private GameObject loadingIcon;
    [SerializeField] private GameObject loadingText;

    [SerializeField] private Button continueButton;

    private bool changeScene;
    
    void Start()
    {
        loadingIcon.SetActive(true);
        loadingText.SetActive(true);

        continueButton.interactable = false;
        continueButton.gameObject.SetActive(false);
        
        StartCoroutine(CR_LoadScene());
    }

    private IEnumerator CR_LoadScene()
    {
        AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(sceneName);
        loadSceneOp.allowSceneActivation = false;

        yield return null;

        float loadProgress = 0;
        do
        {
            yield return new WaitForSeconds(1f);

            loadProgress = Mathf.Clamp01(loadSceneOp.progress / 0.9f);
        }
        while (loadProgress < 1);

        yield return new WaitForSeconds(2);

        loadingIcon.SetActive(false);
        loadingText.SetActive(false);

        continueButton.onClick.AddListener(AllowSceneChange);
        continueButton.interactable = true;
        continueButton.gameObject.SetActive(true);

        yield return new WaitUntil(() => changeScene);

        loadSceneOp.allowSceneActivation = true;
    }

    private void AllowSceneChange()
    {
        changeScene = true;
    }
}
