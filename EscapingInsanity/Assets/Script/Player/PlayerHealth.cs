
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public HealthBar healthBar;
    public GameObject losePanel;
    public TMP_Text loseText;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(Mathf.RoundToInt(maxHealth));
        }

        if (losePanel != null)
            losePanel.SetActive(false);

        if (loseText != null)
            loseText.text = "YOU LOSE!";
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (healthBar != null)
        {
            healthBar.SetHealth(Mathf.RoundToInt(currentHealth));
        }

        if (currentHealth <= 0)
            GameOver();
    }

    void GameOver()
    {
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

        // Pause the game.
        Time.timeScale = 0f;
    }
}
