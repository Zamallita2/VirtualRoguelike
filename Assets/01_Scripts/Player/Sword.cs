using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 10;

    private HashSet<int> hitEnemies = new HashSet<int>();

    public void StartAttack()
    {
        hitEnemies.Clear();
    }

    public void AddDamage(int amount)
    {
        damage += amount;
        if (damage < 0) damage = 0;
    }

    public int GetDamage()
    {
        return damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
        if (enemy != null)
        {
            int id = enemy.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) return;

            enemy.TakeDamage(damage);
            hitEnemies.Add(id);
            Debug.Log("⚔️ Golpeaste a " + other.name.ToString() + " con " + damage.ToString());
            return;
        }

        BossAI boss = other.GetComponentInParent<BossAI>();
        if (boss != null)
        {
            int id = boss.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) return;

            boss.TakeDamage(damage);
            hitEnemies.Add(id);
            Debug.Log("⚔️ Golpeaste a " + other.name.ToString() + " con " + damage.ToString());
            return;
        }

        Dummy dummy = other.GetComponentInParent<Dummy>();
        if (dummy != null)
        {
            int id = dummy.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) return;

            dummy.TakeDamage(damage);
            hitEnemies.Add(id);
            Debug.Log("⚔️ Golpeaste a " + other.name.ToString() + " con " + damage.ToString());
        }

        if (other.CompareTag("Book"))
        {
            int id = other.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) return;

            Book book = other.GetComponent<Book>();

            if (book != null)
            {
                book.TakeDamage(damage);
                hitEnemies.Add(id);
                Debug.Log("⚔️ Golpeaste a " + other.name.ToString() + " con " + damage.ToString());
            }
        }

        if (other.CompareTag("Candelabro"))
        {
            int id = other.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) return;

            Candelabro cande = other.GetComponent<Candelabro>();

            if (cande != null)
            {
                cande.TakeDamage(damage);
                hitEnemies.Add(id);
                Debug.Log("⚔️ Golpeaste a " + other.name.ToString() + " con " + damage.ToString());
            }
        }
        else {Debug.Log("Le diste a nada");}
    }
}