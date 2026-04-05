using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI")]
    public Button playButton;
    public Button multiplayerButton;
    public Button optionsButton;
    public CanvasGroup menuCanvasGroup;

    [Header("Referencias")]
    public Transform castleTransform;
    public Camera arCamera;

    [Header("Zoom a la puerta")]
    public Transform doorTransform;
    public float zoomDistance = 0.15f;
    public float zoomDuration = 2.5f;

    [Header("Door Glow")]
    public DoorGlowParticles doorGlow;

    [Header("Audio (Opcional - se busca automáticamente)")]
    public AudioManager audioManager; // ✅ AHORA ES PÚBLICO para asignarlo en el Inspector

    void Start()
    {
        // ✅ Buscar AudioManager si no está asignado
        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();

        if (audioManager == null)
            Debug.LogError("[MenuManager] ❌ AudioManager no encontrado en Start()");
        else
            Debug.Log("[MenuManager] ✅ AudioManager encontrado en Start()");

        if (arCamera == null) arCamera = Camera.main;

        // Ajustar escala del castillo si es demasiado pequeño
        if (castleTransform != null)
        {
            if (castleTransform.localScale.magnitude < 1f)
                castleTransform.localScale = Vector3.one * 5f;
            Debug.Log("[MenuManager] Escala del castillo: " + castleTransform.localScale);
        }
    }

    public void RegisterButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayClicked);
        }
        if (multiplayerButton != null)
        {
            multiplayerButton.onClick.RemoveAllListeners();
            multiplayerButton.onClick.AddListener(OnMultiClicked);
        }
        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(OnOptionsClicked);
        }
    }

    public void OnSurfaceDetected()
    {
        Debug.Log("[MenuManager] Superficie detectada - Mostrando menú");
        StartCoroutine(ShowMenuSequence());
    }

    IEnumerator ShowMenuSequence()
    {
        // ✅ BUSCAR AudioManager JUSTO ANTES DE USARLO (por si Start() no se ejecutó aún)
        if (audioManager == null)
        {
            Debug.Log("[MenuManager] 🔍 Buscando AudioManager en ShowMenuSequence...");
            audioManager = FindFirstObjectByType<AudioManager>();

            // Segundo intento esperando un frame
            if (audioManager == null)
            {
                yield return null;
                audioManager = FindFirstObjectByType<AudioManager>();
            }
        }

        if (audioManager != null)
        {
            Debug.Log("[MenuManager] 🎵 Llamando a PlayAmbientMusic()...");
            audioManager.PlayAmbientMusic();

            // ✅ VERIFICAR si realmente está sonando
            yield return new WaitForSeconds(0.1f);

            AudioSource[] sources = audioManager.GetComponents<AudioSource>();
            foreach (var src in sources)
            {
                if (src.isPlaying)
                {
                    Debug.Log($"[MenuManager] ✅ AudioSource está reproduciendo: {src.clip?.name ?? "null"} - Volume: {src.volume}");
                }
                else
                {
                    Debug.LogWarning($"[MenuManager] ⚠️ AudioSource NO está reproduciendo. Clip: {src.clip?.name ?? "null"}");
                }
            }
        }
        else
        {
            Debug.LogError("[MenuManager] ❌❌❌ NO SE ENCONTRÓ AudioManager - LA MÚSICA NO PUEDE SONAR");
            Debug.LogError("[MenuManager] Asegúrate de que existe un GameObject con el script AudioManager en la escena");
        }

        yield return new WaitForSeconds(0.5f);

        if (menuCanvasGroup != null)
        {
            float elapsed = 0f;
            float duration = 1f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                menuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            menuCanvasGroup.alpha = 1f;
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
        }
    }

    void OnPlayClicked()
    {
        if (audioManager != null) audioManager.PlayButtonClick();
        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        }
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        yield return StartCoroutine(FadeMenu(1f, 0f, 0.6f));

        if (doorTransform != null && arCamera != null)
            yield return StartCoroutine(ZoomToDoor());
        else
            yield return new WaitForSeconds(0.5f);

        if (doorGlow != null)
            doorGlow.ActivateDoorGlow();

        if (audioManager != null)
            audioManager.PlayDoorOpenSound();

        Debug.Log("[MenuManager] Secuencia Play completada");
    }

    IEnumerator FadeMenu(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (menuCanvasGroup != null)
                menuCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        if (menuCanvasGroup != null)
            menuCanvasGroup.alpha = to;
    }

    IEnumerator ZoomToDoor()
    {
        Vector3 startPos = arCamera.transform.position;
        Quaternion startRot = arCamera.transform.rotation;
        Vector3 doorPos = doorTransform.position;
        Vector3 dirToDoor = (doorPos - startPos).normalized;
        Vector3 targetPos = doorPos - dirToDoor * zoomDistance;
        Quaternion targetRot = Quaternion.LookRotation(doorPos - targetPos);

        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / zoomDuration);
            arCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            arCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        arCamera.transform.position = targetPos;
        arCamera.transform.rotation = targetRot;
    }

    void OnMultiClicked()
    {
        if (audioManager != null) audioManager.PlayButtonClick();
    }

    void OnOptionsClicked()
    {
        if (audioManager != null) audioManager.PlayButtonClick();
    }
}