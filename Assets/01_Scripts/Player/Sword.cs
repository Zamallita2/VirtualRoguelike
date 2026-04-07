using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 10;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    public void StartAttack()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (hitEnemies.Contains(other.gameObject))
            return;

        // 🔥 BOSS
        BossAI boss = other.GetComponent<BossAI>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Debug.Log("⚔️ Golpeaste al BOSS");
            hitEnemies.Add(other.gameObject);
            return;
        }

        // 🔥 ENEMIGOS NORMALES
        Dummy dummy = other.GetComponent<Dummy>();
        if (dummy != null)
        {
            dummy.TakeDamage(damage);
            Debug.Log("⚔️ Golpeaste enemigo");
            hitEnemies.Add(other.gameObject);
        }
    }
}