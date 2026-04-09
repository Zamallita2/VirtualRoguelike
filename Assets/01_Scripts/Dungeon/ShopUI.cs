using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Colocar en el ShopCanvas.
/// Maneja 3 pestañas: Básicas, Especiales y los paneles correspondientes.
/// </summary>
public class ShopUI : MonoBehaviour
{
    // ═══════════════════════════════════════════════
    // PRECIOS
    // ═══════════════════════════════════════════════
    [Header("Precios — Básicas")]
    public int maxHealthPrice = 100;
    public int damagePrice = 120;
    public int speedPrice = 100;
    public int attackSpeedPrice = 150;

    [Header("Precios — Especiales")]
    public int regenPrice = 200;
    public int secondChancePrice = 300;

    // ═══════════════════════════════════════════════
    // VALORES DE MEJORA
    // ═══════════════════════════════════════════════
    [Header("Valores — Básicas")]
    public float maxHealthIncrease = 20f;
    public float damageIncrease = 5f;
    public float speedIncrease = 1f;
    public float attackSpeedIncrease = 0.15f;

    // ═══════════════════════════════════════════════
    // PANELES DE PESTAÑAS
    // ═══════════════════════════════════════════════
    [Header("Paneles de Pestañas")]
    public GameObject panelBasicas;
    public GameObject panelEspeciales;

    // ═══════════════════════════════════════════════
    // BOTONES DE PESTAÑAS
    // ═══════════════════════════════════════════════
    [Header("Botones de Pestañas")]
    public Button tabBasicasButton;
    public Button tabEspecialesButton;

    // ═══════════════════════════════════════════════
    // UI — PANEL BÁSICAS
    // ═══════════════════════════════════════════════
    [Header("UI — Botones Básicas")]
    public Button maxHealthButton;
    public Button damageButton;
    public Button speedButton;
    public Button attackSpeedButton;

    [Header("UI — Textos Básicas")]
    public TextMeshProUGUI maxHealthPriceText;
    public TextMeshProUGUI damagePriceText;
    public TextMeshProUGUI speedPriceText;
    public TextMeshProUGUI attackSpeedPriceText;

    // ═══════════════════════════════════════════════
    // UI — PANEL ESPECIALES
    // ═══════════════════════════════════════════════
    [Header("UI — Botones Especiales")]
    public Button regenButton;
    public Button secondChanceButton;

    [Header("UI — Textos Especiales")]
    public TextMeshProUGUI regenPriceText;
    public TextMeshProUGUI secondChancePriceText;

    // ═══════════════════════════════════════════════
    // UI — GENERAL
    // ═══════════════════════════════════════════════
    [Header("UI — General")]
    public TextMeshProUGUI playerCoinsText;
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 2.5f;

    // ═══════════════════════════════════════════════
    // PRIVADOS
    // ═══════════════════════════════════════════════
    private PlayerMovement player;
    private float feedbackTimer = 0f;

    // ═══════════════════════════════════════════════
    // INICIALIZACIÓN
    // ═══════════════════════════════════════════════
    public void SetPlayer(PlayerMovement playerRef)
    {
        player = playerRef;
    }

    private void OnEnable()
    {
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.GetComponent<PlayerMovement>();
        }

        // Abrir en pestaña Básicas por defecto
        ShowTab("Basicas");
        RefreshAllPrices();
        RefreshUI();

        if (feedbackText != null) feedbackText.text = "";
    }

    // ═══════════════════════════════════════════════
    // UPDATE
    // ═══════════════════════════════════════════════
    private void Update()
    {
        if (player == null) return;

        RefreshUI();

        if (feedbackTimer > 0f)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0f && feedbackText != null)
                feedbackText.text = "";
        }
    }

    // ═══════════════════════════════════════════════
    // PESTAÑAS
    // ═══════════════════════════════════════════════
    public void ShowTab(string tabName)
    {
        if (panelBasicas != null) panelBasicas.SetActive(tabName == "Basicas");
        if (panelEspeciales != null) panelEspeciales.SetActive(tabName == "Especiales");

        // Resaltar botón activo
        HighlightTab(tabBasicasButton, tabName == "Basicas");
        HighlightTab(tabEspecialesButton, tabName == "Especiales");
    }

    private void HighlightTab(Button btn, bool active)
    {
        if (btn == null) return;
        ColorBlock cb = btn.colors;
        cb.normalColor = active
            ? new Color(0.3f, 0.7f, 1f)    // Azul activo
            : new Color(0.2f, 0.2f, 0.2f); // Gris inactivo
        btn.colors = cb;
    }

    // ═══════════════════════════════════════════════
    // ACTUALIZACIÓN DE UI
    // ═══════════════════════════════════════════════
    private void RefreshAllPrices()
    {
        // Básicas
        SetPriceText(maxHealthPriceText, maxHealthPrice);
        SetPriceText(damagePriceText, damagePrice);
        SetPriceText(speedPriceText, speedPrice);
        SetPriceText(attackSpeedPriceText, attackSpeedPrice);

        // Especiales
        SetPriceText(regenPriceText, regenPrice);
        SetPriceText(secondChancePriceText, secondChancePrice);
    }

    private void SetPriceText(TextMeshProUGUI text, int price)
    {
        if (text != null) text.text = $"{price}";
    }

    private void RefreshUI()
    {
        // Monedas
        if (playerCoinsText != null && player != null)
            playerCoinsText.text = $" {player.coins}";

        RefreshButtonStates();
    }

    private void RefreshButtonStates()
    {
        if (player == null) return;

        // Básicas
        SetBtn(maxHealthButton, player.coins >= maxHealthPrice);
        SetBtn(damageButton, player.coins >= damagePrice);
        SetBtn(speedButton, player.coins >= speedPrice);
        SetBtn(attackSpeedButton, player.coins >= attackSpeedPrice);

        // Especiales — deshabilitar si ya están compradas
        SetBtn(regenButton,
               player.coins >= regenPrice && !player.hasRegeneration);
        SetBtn(secondChanceButton,
               player.coins >= secondChancePrice && !player.hasSecondChance);

        // Marcar como "YA COMPRADO" si aplica
        MarkIfOwned(regenButton, player.hasRegeneration, "Regeneración");
        MarkIfOwned(secondChanceButton, player.hasSecondChance, "Segunda Oportunidad");
    }

    private void SetBtn(Button btn, bool canAfford)
    {
        if (btn == null) return;
        btn.interactable = canAfford;
    }

    private void MarkIfOwned(Button btn, bool owned, string ownedLabel)
    {
        if (btn == null) return;
        TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (label == null) return;

        if (owned)
        {
            label.text = ownedLabel;
            label.color = Color.green;
            btn.interactable = false;
        }
    }

    // ═══════════════════════════════════════════════
    // FEEDBACK
    // ═══════════════════════════════════════════════
    private void ShowFeedback(string message, bool success)
    {
        if (feedbackText == null) return;
        feedbackText.text = message;
        feedbackText.color = success ? Color.green : Color.red;
        feedbackTimer = feedbackDuration;
    }

    // ═══════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════
    private bool TrySpend(int cost)
    {
        if (player.coins >= cost)
        {
            player.coins -= cost;
            return true;
        }
        ShowFeedback($"¡Necesitas {cost} 🪙!", false);
        return false;
    }

    private bool ValidatePlayer()
    {
        if (player != null) return true;
        ShowFeedback("Error: jugador no encontrado", false);
        return false;
    }

    // ═══════════════════════════════════════════════
    // COMPRAS — BÁSICAS
    // ═══════════════════════════════════════════════
    public void BuyMaxHealth()
    {
        if (!ValidatePlayer() || !TrySpend(maxHealthPrice)) return;
        player.IncreaseMaxHealth(maxHealthIncrease);
        ShowFeedback($"Vida máxima +{maxHealthIncrease}", true);
    }

    public void BuyDamage()
    {
        if (!ValidatePlayer() || !TrySpend(damagePrice)) return;
        player.IncreaseDamage(damageIncrease);
        ShowFeedback($"Daño +{damageIncrease}", true);
    }

    public void BuySpeed()
    {
        if (!ValidatePlayer() || !TrySpend(speedPrice)) return;
        player.IncreaseSpeed(speedIncrease);
        ShowFeedback($"Velocidad +{speedIncrease}", true);
    }

    public void BuyAttackSpeed()
    {
        if (!ValidatePlayer() || !TrySpend(attackSpeedPrice)) return;
        player.IncreaseAttackSpeed(attackSpeedIncrease);
        ShowFeedback($"Cadencia mejorada -{attackSpeedIncrease}s cooldown", true);
    }

    // ═══════════════════════════════════════════════
    // COMPRAS — ESPECIALES
    // ═══════════════════════════════════════════════
    public void BuyRegeneration()
    {
        if (!ValidatePlayer()) return;
        if (player.hasRegeneration) { ShowFeedback("Ya tienes Regeneración", false); return; }
        if (!TrySpend(regenPrice)) return;
        player.UnlockRegeneration();
        ShowFeedback("Regeneración activada", true);
    }

    public void BuySecondChance()
    {
        if (!ValidatePlayer()) return;
        if (player.hasSecondChance) { ShowFeedback("Ya tienes Segunda Oportunidad", false); return; }
        if (!TrySpend(secondChancePrice)) return;
        player.UnlockSecondChance();
        ShowFeedback("Segunda Oportunidad activada", true);
    }
}