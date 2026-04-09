using System.Collections;
using UnityEngine;

public class MenuBootstrap : MonoBehaviour
{
    public GameObject castleModel;
    public DoorGlowParticles doorGlow;
    public float editorDelay = 1.5f;

    private MenuUISetup uiSetup;
    private MenuManager manager;

    void Awake()
    {
        uiSetup = GetComponent<MenuUISetup>();
        if (uiSetup == null) uiSetup = gameObject.AddComponent<MenuUISetup>();

        manager = GetComponent<MenuManager>();
        if (manager == null) manager = gameObject.AddComponent<MenuManager>();

        // Forzar escala del castillo si es necesario
        if (castleModel != null && castleModel.transform.localScale.magnitude < 1f)
        {
            castleModel.transform.localScale = Vector3.one * 5f;
            Debug.Log("[Bootstrap] Escala del castillo ajustada a 5");
        }
    }

    IEnumerator Start()
    {
        yield return null;
        yield return null;

        if (uiSetup != null && manager != null)
        {
            manager.playButton = uiSetup.playButton;
            manager.optionsButton = uiSetup.optionsButton;
            manager.menuCanvasGroup = uiSetup.menuCanvasGroup;
            manager.castleTransform = castleModel != null ? castleModel.transform : null;
            manager.RegisterButtons();
        }

        if (uiSetup.menuCanvasGroup != null)
        {
            uiSetup.menuCanvasGroup.alpha = 0f;
            uiSetup.menuCanvasGroup.interactable = false;
            uiSetup.menuCanvasGroup.blocksRaycasts = false;
        }

#if UNITY_EDITOR
        yield return new WaitForSeconds(editorDelay);
        if (manager != null) manager.OnSurfaceDetected();
#endif
    }
}