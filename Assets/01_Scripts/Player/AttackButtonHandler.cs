using UnityEngine;

public class AttackButtonHandler : MonoBehaviour
{
    private PlayerMovement player;

    void Start()
    {
        // Buscar el player automáticamente cuando aparezca
        player = FindObjectOfType<PlayerMovement>();
    }

    public void OnAttackPressed()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
            if (player == null) return;
        }

        player.AttackButton();
    }
}