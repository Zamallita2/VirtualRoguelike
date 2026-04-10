using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerMovement player;

    [Header("UI - Vida")]
    public Image healthFill;
    public TextMeshProUGUI healthText;

    [Header("UI - Stats")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI speedText;

    [Header("UI - Monedas")]
    public TextMeshProUGUI coinsText;

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
            healthFill.fillAmount = player.GetHealthNormalized();
        }

        if (healthText != null)
        {
            healthText.text = player.GetCurrentHealth().ToString("F0") + " / " + player.GetMaxHealth().ToString("F0");
        }
    }

    void UpdateStats()
    {
        if (damageText != null)
        {
            damageText.text = player.GetDamage().ToString();
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
            coinsText.text = player.GetCoins().ToString();
        }
    }
}