using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd_AnimEvents : MonoBehaviour
{
    public void DisappearPlayer()
    {
        Player.Instance.gameObject.SetActive(false);
    }

    public void EndLevel()
    {
        SceneManager.LoadScene("Credits");
    }
}
