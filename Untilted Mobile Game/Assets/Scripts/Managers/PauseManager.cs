using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject joystick;

    public bool GamePaused { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (IsPausing() && !GamePaused)
        {
            Time.timeScale = 0f;
            GamePaused = true;
            pausePanel.SetActive(true);
            joystick.SetActive(false);
        }
        else if (IsPausing() && GamePaused)
        {
            Time.timeScale = 1f;
            GamePaused = false;
            pausePanel.SetActive(false);
            joystick.SetActive(true);
        }
    }

    private bool IsPausing() => Input.GetKeyDown(KeyCode.Escape);
}