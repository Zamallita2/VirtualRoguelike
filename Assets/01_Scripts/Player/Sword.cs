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

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
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
            Debug.Log("⚔️ Golpeaste al enemigo");
            return;
        }

        BossAI boss = other.GetComponentInParent<BossAI>();
        if (boss != null)
        {
            int id = boss.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) return;

            boss.TakeDamage(damage);
            hitEnemies.Add(id);
            Debug.Log("⚔️ Golpeaste al BOSS");
            return;
        }

        Dummy dummy = other.GetComponentInParent<Dummy>();
        if (dummy != null)
        {
            int id = dummy.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) return;

            dummy.TakeDamage(damage);
            hitEnemies.Add(id);
            Debug.Log("⚔️ Golpeaste enemigo");
        }
    }
}