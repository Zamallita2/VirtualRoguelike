using UnityEngine;

/// <summary>
/// Colocar en el objeto de la mesa de tienda.
/// Requiere un Collider con Is Trigger activado.
/// </summary>
public class ShopTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject shopUICanvas;
    public string playerTag = "Player";

    private ShopUI shopUI;

    private void Start()
    {
        shopUI = FindObjectOfType<ShopUI>();

        if (shopUI != null)
        {
            shopUICanvas = shopUI.gameObject;
            shopUICanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("ShopTrigger: No se encontró ningún ShopUI en la escena.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return;

        if (shopUI != null) shopUI.SetPlayer(player);
        shopUICanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        shopUICanvas.SetActive(false);
    }

    public void CloseShop() => shopUICanvas.SetActive(false);
}