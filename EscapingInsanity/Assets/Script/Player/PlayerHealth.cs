
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public HealthBar healthBar;
    public GameObject losePanel;
    public TMP_Text loseText;

    [Header("Win UI")]
    public GameObject winPanel;
    public TMP_Text winTimeSurvivedText;

    [Header("Survival Timer")]
    public TMP_Text timeSurvivedText;

    private float survivalTimer = 0f;

    private bool isDead = false;
    private PlayerSanity sanity;

    [Header("Pause UI")]
    public PauseUI pauseUI;

    void Start()
    {
        currentHealth = maxHealth;
        sanity = GetComponent<PlayerSanity>();

        // Reset survival timer.
        survivalTimer = 0f;

        // Make sure the game is not paused.
        Time.timeScale = 1f;

        AudioListener.pause = false;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(Mathf.RoundToInt(maxHealth));
        }

        if (losePanel != null)
            losePanel.SetActive(false);

        if (loseText != null)
            loseText.text = "YOU LOSE!";

        if (timeSurvivedText != null)
            timeSurvivedText.text = "TIME SURVIVED: 00:00";

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Update()
    {
        if (!isDead)
        {
            survivalTimer += Time.deltaTime;

            if (timeSurvivedText != null)
            {
                timeSurvivedText.text =
                    "TIME SURVIVED: " + GetSurvivalTime();
            }
        }
    }

    private string GetSurvivalTime()
    {
        int minutes = Mathf.FloorToInt(survivalTimer / 60f);
        int seconds = Mathf.FloorToInt(survivalTimer % 60f);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void TakeDamage(float damage, bool causesInsanity = true)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (healthBar != null)
        {
            healthBar.SetHealth(Mathf.RoundToInt(currentHealth));
        }

        if (causesInsanity && sanity != null)
            sanity.LowerSanity(damage);

        if (currentHealth <= 0)
            GameOver();
    }

    public void GameOver()
    {
        if (pauseUI != null)
            pauseUI.GameEnded();

        if (isDead)
            return;

        isDead = true;

        if (losePanel != null)
            losePanel.SetActive(true);

        // Stop the player's movement.
        PlayerMovement movement =
            GetComponent<PlayerMovement>();

        if (movement != null)
            movement.enabled = false;

        // Stop the zombie AI scripts.
        ZombieAI[] zombies =
            FindObjectsByType<ZombieAI>(FindObjectsSortMode.None);

        foreach (ZombieAI zombie in zombies)
        {
            zombie.enabled = false;
        }

        // Show the cursor.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause the game.
        Time.timeScale = 0f;

        AudioListener.pause = true;
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Min(
            currentHealth + amount,
            maxHealth
        );

        if (healthBar != null)
        {
            healthBar.SetHealth(
                Mathf.RoundToInt(currentHealth)
            );
        }

        Debug.Log("Player healed! Current HP: " + currentHealth);
    }

    public void WinGame()
    {
        if (pauseUI != null)
            pauseUI.GameEnded();

        if (isDead)
            return;

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winTimeSurvivedText != null)
        {
            winTimeSurvivedText.text =
                "TIME SURVIVED: " + GetSurvivalTime();
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement != null)
            movement.enabled = false;

        ZombieAI[] zombies =
            FindObjectsByType<ZombieAI>(FindObjectsSortMode.None);

        foreach (ZombieAI zombie in zombies)
        {
            zombie.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        AudioListener.pause = true;
    }
}
