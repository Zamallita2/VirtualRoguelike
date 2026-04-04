// DiagnosticGroundPlane.cs
// Script de diagnóstico para detectar problemas con Vuforia Ground Plane

using UnityEngine;
using Vuforia;

public class DiagnosticGroundPlane : MonoBehaviour
{
    [Header("Referencias a revisar")]
    public GameObject menuContainer;
    public MenuManager menuManager;

    private PlaneFinderBehaviour planeFinder;
    private bool diagnosticComplete = false;

    void Start()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════╗");
        Debug.Log("║   🔍 DIAGNÓSTICO DE VUFORIA GROUND PLANE              ║");
        Debug.Log("╚═══════════════════════════════════════════════════════╝");

        RunDiagnostics();
    }

    void RunDiagnostics()
    {
        int issues = 0;

        // 1. Verificar PlaneFinderBehaviour
        Debug.Log("\n[1/6] Buscando PlaneFinderBehaviour...");
        planeFinder = FindFirstObjectByType<PlaneFinderBehaviour>();

        if (planeFinder != null)
        {
            Debug.Log($"   ✅ PlaneFinderBehaviour encontrado en: {planeFinder.gameObject.name}");
        }
        else
        {
            Debug.LogError("   ❌ NO se encontró PlaneFinderBehaviour en la escena");
            Debug.LogError("   → SOLUCIÓN: Agrega un GameObject con PlaneFinderBehaviour");
            issues++;
        }

        // 2. Verificar MenuContainer
        Debug.Log("\n[2/6] Verificando MenuContainer...");
        if (menuContainer != null)
        {
            Debug.Log($"   ✅ MenuContainer asignado: {menuContainer.name}");
            Debug.Log($"   Estado: {(menuContainer.activeSelf ? "ACTIVO" : "INACTIVO")}");
        }
        else
        {
            Debug.LogError("   ❌ MenuContainer NO asignado");
            Debug.LogError("   → SOLUCIÓN: Arrastra el GameObject del menú al Inspector");
            issues++;
        }

        // 3. Verificar MenuManager
        Debug.Log("\n[3/6] Verificando MenuManager...");
        if (menuManager != null)
        {
            Debug.Log($"   ✅ MenuManager asignado");
        }
        else
        {
            Debug.LogError("   ❌ MenuManager NO asignado");
            Debug.LogError("   → SOLUCIÓN: Arrastra el MenuManager al Inspector");
            issues++;
        }

        // 4. Verificar VuforiaConfiguration
        Debug.Log("\n[4/6] Verificando configuración de Vuforia...");
        var vuforiaConfig = Resources.Load<VuforiaConfiguration>("VuforiaConfiguration");
        if (vuforiaConfig != null)
        {
            Debug.Log("   ✅ VuforiaConfiguration encontrado");
        }
        else
        {
            Debug.LogWarning("   ⚠️ No se pudo cargar VuforiaConfiguration");
        }

        // 5. Verificar cámara AR
        Debug.Log("\n[5/6] Verificando cámara AR...");
        Camera arCam = Camera.main;
        if (arCam != null)
        {
            Debug.Log($"   ✅ Cámara principal encontrada: {arCam.gameObject.name}");

            var vuforiaCamera = arCam.GetComponent<VuforiaBehaviour>();
            if (vuforiaCamera != null)
            {
                Debug.Log("   ✅ VuforiaBehaviour en la cámara");
            }
            else
            {
                Debug.LogWarning("   ⚠️ NO hay VuforiaBehaviour en la cámara");
            }
        }
        else
        {
            Debug.LogError("   ❌ NO se encontró cámara principal");
            issues++;
        }

        // 6. Verificar scripts necesarios
        Debug.Log("\n[6/6] Verificando scripts en MenuContainer...");
        if (menuContainer != null)
        {
            var bootstrap = menuContainer.GetComponent<MenuBootstrap>();
            var uiSetup = menuContainer.GetComponent<MenuUISetup>();
            var manager = menuContainer.GetComponent<MenuManager>();

            if (bootstrap != null) Debug.Log("   ✅ MenuBootstrap presente");
            else { Debug.LogError("   ❌ FALTA MenuBootstrap"); issues++; }

            if (uiSetup != null) Debug.Log("   ✅ MenuUISetup presente");
            else { Debug.LogError("   ❌ FALTA MenuUISetup"); issues++; }

            if (manager != null) Debug.Log("   ✅ MenuManager presente");
            else { Debug.LogError("   ❌ FALTA MenuManager"); issues++; }
        }

        // Resumen final
        Debug.Log("\n╔═══════════════════════════════════════════════════════╗");
        if (issues == 0)
        {
            Debug.Log("║   ✅ DIAGNÓSTICO COMPLETO - TODO OK                   ║");
            Debug.Log("║   Esperando detección de plano...                     ║");
        }
        else
        {
            Debug.LogError($"║   ❌ DIAGNÓSTICO COMPLETO - {issues} PROBLEMAS ENCONTRADOS    ║");
            Debug.LogError("║   Revisa los errores arriba para solucionarlos        ║");
        }
        Debug.Log("╚═══════════════════════════════════════════════════════╝\n");

        diagnosticComplete = true;
    }

    void Update()
    {
        // Mostrar cuando se detecte touch (útil para debug en dispositivo)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log($"[Diagnostic] 👆 Touch detectado en ({Input.GetTouch(0).position.x}, {Input.GetTouch(0).position.y})");
        }

        // Mostrar estado de Vuforia cada 2 segundos
        if (diagnosticComplete && Time.frameCount % 120 == 0 && planeFinder != null)
        {
            Debug.Log($"[Diagnostic] 🔄 Frame {Time.frameCount} - PlaneFinder activo: {planeFinder.enabled}");
        }
    }

    // Método de prueba manual
    [ContextMenu("Activar menú manualmente")]
    public void ActivateMenuManually()
    {
        Debug.Log("[Diagnostic] 🔨 ACTIVACIÓN MANUAL DEL MENÚ");

        if (menuContainer != null)
        {
            menuContainer.SetActive(true);
            Debug.Log("   ✅ MenuContainer activado");
        }

        if (menuManager != null)
        {
            menuManager.OnSurfaceDetected();
            Debug.Log("   ✅ OnSurfaceDetected llamado");
        }
    }
}