using System.Collections.Generic;
using UnityEngine;

public class BossSword : MonoBehaviour
{
    public int damage = 20;

    private HashSet<GameObject> hitPlayers = new HashSet<GameObject>();

    public void StartAttack()
    {
        hitPlayers.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLISION CON: " + other.name);

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("⚔️ DAÑO HECHO");
            }
        }
    }
}
