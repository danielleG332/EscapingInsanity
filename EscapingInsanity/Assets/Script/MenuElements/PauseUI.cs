
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanelUI;
    public GameObject levelUpPanel;
    public GameObject winPanel;
    public GameObject losePanel;

    public static bool isPaused = false;

    private bool isGameEnded = false;

    void Start()
    {
        if (pausePanelUI != null)
            pausePanelUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        Debug.Log("PauseUI initialized.");
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("Escape key detected!");

            if (isGameEnded)
                return;

            if (levelUpPanel != null && levelUpPanel.activeSelf)
                return;

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        if (pausePanelUI != null)
            pausePanelUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        Debug.Log("Game resumed.");
    }

    public void Pause()
    {
        if (isGameEnded)
            return;

        if (pausePanelUI != null)
            pausePanelUI.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        Debug.Log("Game paused.");
    }

    public void GameEnded()
    {
        isGameEnded = true;
        isPaused = false;

        if (pausePanelUI != null)
            pausePanelUI.SetActive(false);
    }
}
