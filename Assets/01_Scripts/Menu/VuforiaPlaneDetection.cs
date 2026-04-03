// Script: VuforiaPlaneDetection.cs
// Versión corregida para Vuforia 10.x+

using UnityEngine;
using Vuforia;

public class VuforiaPlaneDetection : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuContainer;
    public MenuManager menuManager;

    [Header("Configuración AR")]
    public float menuHeightOffset = 0f; // Altura sobre el plano detectado

    private PlaneFinderBehaviour planeFinder;
    private bool hasSpawned = false;
    private AnchorBehaviour anchorBehaviour;

    void Start()
    {
        // Obtener PlaneFinderBehaviour
        planeFinder = FindObjectOfType<PlaneFinderBehaviour>();

        if (planeFinder == null)
        {
            Debug.LogError("¡PlaneFinderBehaviour no encontrado en la escena!");
            Debug.LogError("Asegúrate de haber agregado: GameObject → Vuforia Engine → Ground Plane");
            return;
        }

        // Asegurar que el menú esté desactivado al inicio
        if (menuContainer != null)
        {
            menuContainer.SetActive(false);
        }
        else
        {
            Debug.LogError("¡MenuContainer no asignado en VuforiaPlaneDetection!");
        }

        // Suscribirse a eventos de Vuforia
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnDestroy()
    {
        // Desuscribirse de eventos
        if (VuforiaApplication.Instance != null)
        {
            VuforiaApplication.Instance.OnVuforiaStarted -= OnVuforiaStarted;
        }
    }

    void OnVuforiaStarted()
    {
        Debug.Log("Vuforia iniciado correctamente");

        // Configurar el plane finder
        if (planeFinder != null)
        {
            // Opcional: configurar el comportamiento del plane finder aquí
            Debug.Log("PlaneFinderBehaviour configurado");
        }
    }

    void Update()
    {
        if (hasSpawned) return;

        // Detectar tap/click en la pantalla
        bool userTapped = false;

#if UNITY_EDITOR
        // En el editor, usar click del mouse
        if (Input.GetMouseButtonDown(0))
        {
            userTapped = true;
        }
#else
        // En dispositivo, usar touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            userTapped = true;
        }
#endif

        if (userTapped)
        {
            TryPlaceMenu();
        }
    }

    void TryPlaceMenu()
    {
        if (menuContainer == null || hasSpawned)
        {
            Debug.LogWarning("No se puede colocar el menú: MenuContainer nulo o ya spawneado");
            return;
        }

        // Método simplificado: colocar el menú directamente
        SpawnMenuAtCurrentPosition();
    }

    void SpawnMenuAtCurrentPosition()
    {
        // Activar el contenedor del menú
        menuContainer.SetActive(true);

        // Configurar como hijo del PlaneFinderBehaviour para que se ancle al plano
        menuContainer.transform.SetParent(planeFinder.transform);

        // Posicionar en el origen local con offset de altura
        menuContainer.transform.localPosition = new Vector3(0, menuHeightOffset, 0);
        menuContainer.transform.localRotation = Quaternion.identity;

        // Escala apropiada para AR (ajusta según necesites)
        menuContainer.transform.localScale = Vector3.one * 0.3f; // Escala más pequeña para AR

        hasSpawned = true;

        // Notificar al MenuManager
        if (menuManager != null)
        {
            menuManager.OnSurfaceDetected();
            Debug.Log("✅ ¡Menú spawneado exitosamente en AR!");
        }
        else
        {
            Debug.LogWarning("MenuManager no asignado en VuforiaPlaneDetection");
        }
    }

    public void ResetPlacement()
    {
        hasSpawned = false;

        if (menuContainer != null)
        {
            menuContainer.SetActive(false);
            menuContainer.transform.SetParent(null);
        }

        Debug.Log("Placement reseteado - listo para colocar de nuevo");
    }
}