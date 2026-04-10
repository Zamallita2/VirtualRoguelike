using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public void OnClickBoton()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    
    if (player != null)
    {
        player.GetComponent<PlayerMovement>().AttackButton();
    }
}
}
