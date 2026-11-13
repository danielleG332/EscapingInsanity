
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryUI : MonoBehaviour
{
    public string mainMenuSceneName = "Menu";

    public void RetryGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
