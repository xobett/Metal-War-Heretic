using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd_AnimEvents : MonoBehaviour
{
    private LevelEndPortal levelEndPortal;

    private void Start()
    {
        levelEndPortal = GetComponentInParent<LevelEndPortal>();
    }

    public void DisappearPlayer()
    {
        Player.Instance.gameObject.SetActive(false);
    }

    public void EndLevel()
    {
        UIManager.Instance.DisplayLevelScore();
    }

    public void PlayTeleportSound()
    {
        levelEndPortal.AnimEvent_PlayTeleportSound();
    }

    public void PlayTeleportHitSound()
    {
        levelEndPortal.AnimEvent_PlayTeleportHitSound();
    }
}
