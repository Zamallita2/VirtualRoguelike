using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject shopUICanvas;
    public string playerTag = "Player";
    private ShopUI shopUI;

    private void Start()
    {
        shopUI = FindFirstObjectByType<ShopUI>();

        if (shopUI != null)
        {
            shopUICanvas = shopUI.gameObject;
            shopUICanvas.SetActive(false); // ✅ siempre oculto al inicio
        }
        else
        {
            // ✅ si no hay ShopUI, ocultar el canvas directamente
            if (shopUICanvas != null)
                shopUICanvas.SetActive(false);

            Debug.LogWarning("ShopTrigger: No se encontró ShopUI.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return;
        if (shopUI != null) shopUI.SetPlayer(player);
        if (shopUICanvas != null) shopUICanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (shopUICanvas != null) shopUICanvas.SetActive(false);
    }

    public void CloseShop()
    {
        if (shopUICanvas != null) shopUICanvas.SetActive(false);
    }
}