// ManualActivation.cs
using UnityEngine;

public class ManualActivation : MonoBehaviour
{
    public GameObject menuContainer;
    public MenuManager menuManager;

    void Update()
    {
        // Activar con toque en pantalla (para pruebas)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("[MANUAL] Activando menú por touch...");
            if (menuContainer != null) menuContainer.SetActive(true);
            if (menuManager != null) menuManager.OnSurfaceDetected();
        }
    }
}