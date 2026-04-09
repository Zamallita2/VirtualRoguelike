using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerMovement player;

    [Header("UI - Vida")]
    public Image healthFill;
    public TextMeshProUGUI healthText; // 👈 TEXTO VIDA

    [Header("UI - Stats")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI speedText;

    [Header("UI - Monedas")]
    public TextMeshProUGUI coinsText;
    public int coins = 0;

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
        }
    }

    void Update()
    {
        if (player == null) return;

        UpdateHealth();
        UpdateStats();
        UpdateCoins();
    }

    void UpdateHealth()
    {
        if (healthFill != null)
        {
            float value = player.GetHealthNormalized();
            healthFill.fillAmount = value;
        }

        if (healthText != null)
        {
            float current = player.GetCurrentHealth();
            float max = player.GetMaxHealth();

            healthText.text = current.ToString("F0") + " / " + max.ToString("F0");
        }
    }

    void UpdateStats()
    {
        if (damageText != null)
        {
            int damage = GetPlayerDamage();
            damageText.text = damage.ToString();
        }

        if (speedText != null)
        {
            speedText.text = player.speed.ToString("F1");
        }
    }

    void UpdateCoins()
    {
        if (coinsText != null)
        {
            coinsText.text =  coins.ToString();
        }
    }

    int GetPlayerDamage()
    {
        if (player.swordCollider == null) return 0;

        Sword sword = player.swordCollider.GetComponent<Sword>();
        if (sword == null) return 0;

        return sword.damage;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }
}