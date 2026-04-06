using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 10;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    // 🔥 Llamado desde el player al iniciar ataque
    public void StartAttack()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // ❌ ya fue golpeado en este ataque
            if (hitEnemies.Contains(other.gameObject))
                return;

            Dummy dummy = other.GetComponent<Dummy>();

            if (dummy != null)
            {
                dummy.TakeDamage(damage);
                hitEnemies.Add(other.gameObject); // 🐾 lo marcamos
            }
        }
        if (other.CompareTag("Book"))
        {
            // ❌ ya fue golpeado en este ataque
            if (hitEnemies.Contains(other.gameObject))
                return;

            Book book = other.GetComponent<Book>();

            if (book != null)
            {
                book.TakeDamage(damage);
                hitEnemies.Add(other.gameObject); // 🐾 lo marcamos
            }
        }
        if (other.CompareTag("Candelabro"))
        {
            // ❌ ya fue golpeado en este ataque
            if (hitEnemies.Contains(other.gameObject))
                return;

            Candelabro cande = other.GetComponent<Candelabro>();

            if (cande != null)
            {
                cande.TakeDamage(damage);
                hitEnemies.Add(other.gameObject); // 🐾 lo marcamos
            }
        }
    }
}