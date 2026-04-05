using UnityEngine;

public class Dummy : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(int damage)
    {
        health -= damage;

        Debug.Log(gameObject.name + " recibió daño: " + damage + " | Vida: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió unu");
        Destroy(gameObject);
    }
}