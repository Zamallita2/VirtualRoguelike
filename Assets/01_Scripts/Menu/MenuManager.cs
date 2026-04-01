// Script: MenuManager.cs
// ¡ESTE ES EL SCRIPT PRINCIPAL QUE CONTROLA TODO!

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("References")]
    public GameObject menuContainer;
    public GameObject castleModel;
    public CanvasGroup menuCanvasGroup;
    public Animator cameraAnimator;
    public DoorGlowParticles doorGlow;

    [Header("UI Elements")]
    public Button playButton;
    public Button multiplayerButton;
    public Button optionsButton;
    public TextMeshProUGUI titleText;

    [Header("Audio")]
    public AudioManager audioManager;

    [Header("Settings")]
    public float fadeInDuration = 2f;
    public float doorOpenDelay = 1f;

    private bool isTransitioning = false;

    void Start()
    {
        // Asegurar que el menú esté oculto al inicio
        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.alpha = 0f;
        }

        menuContainer.SetActive(false);

        // Configurar botones
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        if (multiplayerButton != null)
        {
            multiplayerButton.onClick.AddListener(OnMultiplayerButtonClicked);
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }
    }

    // Llamar esto cuando Vuforia detecte una superficie
    public void OnSurfaceDetected()
    {
        menuContainer.SetActive(true);
        StartCoroutine(FadeInMenu());

        if (audioManager != null)
        {
            audioManager.PlayAmbientMusic();
        }
    }

    IEnumerator FadeInMenu()
    {
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            if (menuCanvasGroup != null)
            {
                menuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            }
            yield return null;
        }

        menuCanvasGroup.alpha = 1f;
    }

    void OnPlayButtonClicked()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        StartCoroutine(PlayButtonSequence());
    }

    IEnumerator PlayButtonSequence()
    {
        // 1. Sonido de clic épico
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
        }

        // 2. Animación de escala del botón
        yield return StartCoroutine(ButtonPressAnimation(playButton.transform));

        // 3. Esperar un momento
        yield return new WaitForSeconds(0.3f);

        // 4. Sonido de zoom/whoosh
        if (audioManager != null)
        {
            audioManager.PlayZoomSound();
        }

        // 5. Activar partículas de la puerta
        if (doorGlow != null)
        {
            doorGlow.ActivateDoorGlow();
        }

        // 6. Fade out del menú
        yield return StartCoroutine(FadeOutMenu());

        // 7. Zoom de cámara hacia la puerta
        yield return StartCoroutine(ZoomToDoor());

        // 8. Sonido de puerta abriéndose
        if (audioManager != null)
        {
            audioManager.PlayDoorOpenSound();
        }

        // 9. Esperar un poco más
        yield return new WaitForSeconds(doorOpenDelay);

        // 10. Cargar la escena del juego
        LoadGameScene();
    }

    IEnumerator ButtonPressAnimation(Transform buttonTransform)
    {
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 pressedScale = originalScale * 0.9f;

        float duration = 0.1f;
        float elapsed = 0f;

        // Press down
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(originalScale, pressedScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;

        // Release
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(pressedScale, originalScale * 1.1f, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;

        // Return to normal
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, elapsed / duration);
            yield return null;
        }
    }

    IEnumerator FadeOutMenu()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (menuCanvasGroup != null)
            {
                menuCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            }
            yield return null;
        }

        menuCanvasGroup.alpha = 0f;
    }

    IEnumerator ZoomToDoor()
    {
        // Aquí puedes agregar una animación de cámara hacia la puerta
        // Por ahora un simple shake o movimiento

        Transform cameraTransform = Camera.main.transform;
        Vector3 originalPosition = cameraTransform.localPosition;
        Vector3 targetPosition = originalPosition + new Vector3(0, 0, 1.5f); // Acercar

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cameraTransform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsed / duration);
            yield return null;
        }
    }

    void LoadGameScene()
    {
        // Cambiar "GameScene" por el nombre de tu escena de juego
        SceneManager.LoadScene("GameScene");
    }

    void OnMultiplayerButtonClicked()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
        }

        // Aquí puedes agregar la lógica para multijugador
        Debug.Log("Multijugador presionado - Por implementar");
    }

    void OnOptionsButtonClicked()
    {
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
        }

        // Aquí puedes abrir un panel de opciones
        Debug.Log("Opciones presionado - Por implementar");
    }
}