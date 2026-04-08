// =================== EnemyWeapon.cs ===================
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    private float currentDamage = 10f;
    public Collider weaponCollider;
    private bool activeDamage = false;
    private HashSet<GameObject> hit = new HashSet<GameObject>();

    void Start()
    {
        if (weaponCollider == null) weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false;
    }

    public void SetDamage(float damage) => currentDamage = damage;

    public void SetDamageActive(bool value)
    {
        activeDamage = value;
        if (!value) hit.Clear();
        if (weaponCollider != null) weaponCollider.enabled = value;
    }

    private void OnTriggerEnter(Collider other) => TryHit(other);
    private void OnTriggerStay(Collider other) => TryHit(other);

    private void TryHit(Collider other)
    {
        if (!activeDamage) return;
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;
        if (hit.Contains(player.gameObject)) return;

        player.TakeDamage(currentDamage);
        hit.Add(player.gameObject);
        Debug.Log($"⚔️ GOLPE con {currentDamage} de daño - {(currentDamage >= 20 ? "CRÍTICO" : "NORMAL")}");
    }
}