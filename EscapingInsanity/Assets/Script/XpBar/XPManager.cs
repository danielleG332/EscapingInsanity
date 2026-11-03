using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPManager : MonoBehaviour
{
    [Header("XP Settings")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("XP Bar UI")]
    public Slider xpBar;
    public TMP_Text levelText;
    public TMP_Text xpText;

    void Start()
    {
        UpdateXPUI();
    }

    public void AddXP(int amount)
    {
        if (amount <= 0)
            return;

        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
    }

    void LevelUp()
    {
        currentLevel++;

        // Increase the XP required for the next level.
        xpToNextLevel += 50;
    }

    void UpdateXPUI()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }

        if (levelText != null)
            levelText.text = "Level " + currentLevel;

        if (xpText != null)
            xpText.text = currentXP + " / " + xpToNextLevel + " XP";
    }
}