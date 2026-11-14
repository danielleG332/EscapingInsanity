
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUpgradeUI : MonoBehaviour
{
    public enum UpgradeType
    {
        Health,
        Sanity,
        DropChance,
        Damage
    }

    [Header("UI")]
    public GameObject levelUpPanel;
    public Button upgradeButton1;
    public Button upgradeButton2;
    public TMP_Text upgradeName1;
    public TMP_Text upgradeName2;
    public TMP_Text upgradeDescription1;
    public TMP_Text upgradeDescription2;

    [Header("Player References")]
    public PlayerHealth playerHealth;
    public PlayerSanity playerSanity;
    public PlayerMeleeCombat meleeCombat;
    public PlayerProjectileCombat projectileCombat;

    private UpgradeType firstUpgrade;
    private UpgradeType secondUpgrade;

    private int pendingLevelUps = 0;

    void Start()
    {
        levelUpPanel.SetActive(false);

        upgradeButton1.onClick.AddListener(
            () => SelectUpgrade(firstUpgrade)
        );

        upgradeButton2.onClick.AddListener(
            () => SelectUpgrade(secondUpgrade)
        );
    }

    public void QueueLevelUp()
    {
        pendingLevelUps++;

        if (!levelUpPanel.activeSelf)
        {
            ShowUpgradeChoices();
        }
    }

    void ShowUpgradeChoices()
    {
        Time.timeScale = 0f;

        firstUpgrade = GetRandomUpgrade();

        do
        {
            secondUpgrade = GetRandomUpgrade();
        }
        while (secondUpgrade == firstUpgrade);

        DisplayUpgrade(
            firstUpgrade,
            upgradeName1,
            upgradeDescription1
        );

        DisplayUpgrade(
            secondUpgrade,
            upgradeName2,
            upgradeDescription2
        );

        levelUpPanel.SetActive(true);
    }

    UpgradeType GetRandomUpgrade()
    {
        return (UpgradeType)Random.Range(0, 4);
    }

    void DisplayUpgrade(
        UpgradeType upgrade,
        TMP_Text upgradeName,
        TMP_Text description)
    {
        switch (upgrade)
        {
            case UpgradeType.Health:
                upgradeName.text = "VITALITY";
                description.text =
                    "+20 Maximum Health\nRestore 20 Health";
                break;

            case UpgradeType.Sanity:
                upgradeName.text = "MENTAL STRENGTH";
                description.text =
                    "+20 Maximum Sanity\nRestore 20 Sanity";
                break;

            case UpgradeType.DropChance:
                upgradeName.text = "SCAVENGER'S LUCK";
                description.text =
                    "+10% Health Kit and Pill Drop Chance";
                break;

            case UpgradeType.Damage:
                upgradeName.text = "WEAPON DAMAGE";
                description.text =
                    "+1 Melee and Projectile Damage";
                break;
        }
    }

    void SelectUpgrade(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Health:
                if (playerHealth != null)
                {
                    playerHealth.maxHealth += 20;
                    playerHealth.Heal(20);
                }
                break;

            case UpgradeType.Sanity:
                if (playerSanity != null)
                {
                    playerSanity.maxSanity += 20;
                    playerSanity.RestoreSanity(20);
                }
                break;

            case UpgradeType.DropChance:
                ZombieHealth.bonusDropChance += 0.10f;
                break;

            case UpgradeType.Damage:
                if (meleeCombat != null)
                    meleeCombat.attackDamage += 1;

                if (projectileCombat != null)
                    projectileCombat.projectileDamage += 1;
                break;
        }

        pendingLevelUps--;

        if (pendingLevelUps > 0)
        {
            ShowUpgradeChoices();
        }
        else
        {
            levelUpPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        Debug.Log("Selected upgrade: " + upgrade);
    }
}
