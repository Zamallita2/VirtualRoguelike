using System.Collections.Generic;
using UnityEngine;

public class BossSword : MonoBehaviour
{
    public int damage = 20;
    public Collider swordCollider;

    private bool isAttacking = false;
    private HashSet<GameObject> hitPlayers = new HashSet<GameObject>();

    void Start()
    {
        if (swordCollider == null)
            swordCollider = GetComponent<Collider>();
        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    public void StartAttack()
    {
        isAttacking = true;
        hitPlayers.Clear();
        if (swordCollider != null)
            swordCollider.enabled = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;
        if (hitPlayers.Contains(other.gameObject)) return;

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        player.TakeDamage(damage);
        hitPlayers.Add(other.gameObject);
        Debug.Log($"⚔️ Boss golpea con {damage}");
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }
}