// Script: TitleGlowEffect.cs

using UnityEngine;
using TMPro;

public class TitleGlowEffect : MonoBehaviour
{
    private TextMeshProUGUI titleText;
    private Color originalColor;
    private Color glowColor;

    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float glowIntensity = 0.3f;

    void Start()
    {
        titleText = GetComponent<TextMeshProUGUI>();
        originalColor = titleText.color;
        glowColor = new Color(1f, 1f, 0.8f); // Amarillo claro
    }

    void Update()
    {
        float glow = Mathf.PingPong(Time.time * glowSpeed, glowIntensity);
        titleText.color = Color.Lerp(originalColor, glowColor, glow);
    }
}