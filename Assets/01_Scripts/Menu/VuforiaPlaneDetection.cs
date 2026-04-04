using UnityEngine;
using Vuforia;

public class VuforiaPlaneDetectionSimple : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuContainer;
    public MenuManager menuManager;

    private bool activated = false;
    private PlaneFinderBehaviour planeFinder;

    void Start()
    {
        Debug.Log("[Vuforia] Iniciando detector de planos...");

        if (menuContainer != null)
        {
            menuContainer.SetActive(false);
            Debug.Log("[Vuforia] MenuContainer desactivado al inicio");
        }

        // Buscar el PlaneFinderBehaviour
        planeFinder = GetComponent<PlaneFinderBehaviour>();

        if (planeFinder == null)
        {
            planeFinder = FindFirstObjectByType<PlaneFinderBehaviour>();
        }

        if (planeFinder != null)
        {
            planeFinder.OnAutomaticHitTest.AddListener(OnPlaneDetected);
            Debug.Log("[Vuforia] PlaneFinderBehaviour encontrado - Escuchando planos");
        }
        else
        {
            Debug.LogError("[Vuforia] No se encontró PlaneFinderBehaviour");
        }
    }

    void OnPlaneDetected(HitTestResult result)
    {
        if (activated) return;

        Debug.Log($"[Vuforia] ¡PLANO DETECTADO!");
        activated = true;

        if (menuContainer != null)
        {
            // Posicionar el menú en el plano detectado
            menuContainer.transform.position = result.Position;
            menuContainer.SetActive(true);
            Debug.Log($"[Vuforia] MenuContainer ACTIVADO en posición: {result.Position}");
        }

        if (menuManager != null)
        {
            menuManager.OnSurfaceDetected();
            Debug.Log("[Vuforia] OnSurfaceDetected llamado");
        }
    }
}