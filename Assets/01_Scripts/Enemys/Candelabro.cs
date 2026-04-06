using UnityEngine;

public class Candelabro : MonoBehaviour
{
    [Header("Stats")]
    public float health = 20f;

    [Header("Ranges")]
    public float detectionRange = 12f;
    public float attackRange = 6f;

    [Header("Movement")]
    public float rotationSpeed = 5f;

    [Header("Shooting")]
    public float fireRate = 1.5f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Bone")]
    public Transform bone;

    private Transform player;
    private float fireTimer;
    private bool isAttacking = false;

    private Quaternion boneIdleRot;
    private Quaternion boneAttackRot;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            player = obj.transform;

        if (bone != null)
        {
            boneIdleRot = bone.localRotation;
            boneAttackRot = Quaternion.Euler(90f, 0f, 0f); // X negativo hasta 90
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            LookAtPlayer();

            if (distance <= attackRange)
            {
                isAttacking = true;
                AttackMode();
            }
            else
            {
                isAttacking = false;
                StopAttackMode();
            }
        }
        else
        {
            isAttacking = false;
            StopAttackMode();
        }
    }

    void LookAtPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    void AttackMode()
    {
        if (bone != null)
        {
            bone.localRotation = Quaternion.Slerp(
                bone.localRotation,
                boneAttackRot,
                Time.deltaTime * 6f
            );
        }

        HandleShooting();
    }

    void StopAttackMode()
    {
        if (bone != null)
        {
            bone.localRotation = Quaternion.Slerp(
                bone.localRotation,
                boneIdleRot,
                Time.deltaTime * 6f
            );
        }
    }

    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            ShootSpread();
            fireTimer = 0f;
        }
    }

    void ShootSpread()
    {
        // 5 balitas en abanico (Y variando)
        for (int i = -2; i <= 2; i++)
        {
            float angleOffset = i * 5f; // -10, -5, 0, 5, 10

            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0f, angleOffset, 0f);

            Instantiate(bulletPrefab, firePoint.position, rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        Debug.Log(gameObject.name + " daño: " + damage + " | HP: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió unu 💀");
        Destroy(gameObject);
    }
}
