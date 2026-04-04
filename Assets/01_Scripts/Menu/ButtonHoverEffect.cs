// ButtonHoverEffect.cs — CORREGIDO PARA UNITY 6
// Cambio: FindObjectOfType<AudioManager>() → FindFirstObjectByType<AudioManager>()

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Animacion")]
    [SerializeField] private float scaleOnHover = 1.06f;
    [SerializeField] private float scaleOnPress = 0.96f;
    [SerializeField] private float lerpSpeed = 14f;

    [Header("Tinte en Hover")]
    [SerializeField] private Color hoverTint = new Color(1f, 0.9f, 0.5f, 1f);

    private RectTransform rectTransform;
    private TextMeshProUGUI buttonText;
    private AudioManager audioManager;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Color originalTextColor;
    private bool isHovered;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Unity 6: FindFirstObjectByType en lugar del deprecado FindObjectOfType
#if UNITY_6000_0_OR_NEWER
        audioManager = FindFirstObjectByType<AudioManager>();
#else
        audioManager = FindObjectOfType<AudioManager>();
#endif

        originalScale = rectTransform.localScale;
        targetScale = originalScale;

        if (buttonText != null)
            originalTextColor = buttonText.color;
    }

    public void OnPointerEnter(PointerEventData _)
    {
        isHovered = true;
        targetScale = originalScale * scaleOnHover;
        if (audioManager != null) audioManager.PlayHoverSound();
        if (buttonText != null) buttonText.color = hoverTint;
    }

    public void OnPointerExit(PointerEventData _)
    {
        isHovered = false;
        targetScale = originalScale;
        if (buttonText != null) buttonText.color = originalTextColor;
    }

    public void OnPointerDown(PointerEventData _)
    {
        targetScale = originalScale * scaleOnPress;
    }

    public void OnPointerUp(PointerEventData _)
    {
        targetScale = isHovered ? originalScale * scaleOnHover : originalScale;
    }

    void Update()
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * lerpSpeed
        );
    }
}