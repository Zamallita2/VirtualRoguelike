// Script: ButtonHoverEffect.cs

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;

    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float animationSpeed = 10f;

    private Vector3 targetScale;
    private AudioManager audioManager;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;

        audioManager = FindObjectOfType<AudioManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleMultiplier;

        if (audioManager != null)
        {
            audioManager.PlayHoverSound();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    void Update()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }
}