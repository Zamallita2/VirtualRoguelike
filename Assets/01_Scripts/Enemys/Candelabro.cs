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

    [Header("Sonidos")]
    public AudioClip detectSound; // cuando detecta al jugador
    public AudioClip shootSound; // disparo de las 5 balas
    public AudioClip hitSound; // cuando recibe daño
    public AudioClip deathSound; // al morir
    public AudioClip flameIdleSound; // fuego constante (loop)

    private Transform player;
    private float fireTimer;
    private bool isAttacking = false;
    private bool hasDetected = false;
    private Quaternion boneIdleRot;
    private Quaternion boneAttackRot;
    private AudioSource audioSource;
    private AudioSource flameAudioSource; // para el loop del fuego

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            player = obj.transform;

        if (bone != null)
        {
            boneIdleRot = bone.localRotation;
            boneAttackRot = Quaternion.Euler(90f, 0f, 0f);
        }

        // 🔊 Configurar AudioSources
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // AudioSource separado para el fuego loop
        flameAudioSource = gameObject.AddComponent<AudioSource>();
        flameAudioSource.loop = true;
        flameAudioSource.volume = 0.3f;
        flameAudioSource.spatialBlend = 1f; // 3D
        flameAudioSource.maxDistance = 15f;

        if (flameIdleSound != null)
        {
            flameAudioSource.clip = flameIdleSound;
            flameAudioSource.Play();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            // 🔊 Sonido de detección (solo una vez)
            if (!hasDetected)
            {
                PlaySound(detectSound, 0.6f);
                hasDetected = true;
            }

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
            hasDetected = false; // reset para cuando vuelva a detectar
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
        // 🔊 Sonido de disparo
        PlaySound(shootSound, 0.8f);

        // 5 balitas en abanico
        for (int i = -2; i <= 2; i++)
        {
            float angleOffset = i * 5f;
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0f, angleOffset, 0f);
            Instantiate(bulletPrefab, firePoint.position, rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " daño: " + damage + " | HP: " + health);

        // 🔊 Sonido de golpe
        PlaySound(hitSound, 0.7f);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió unu 💀");

        // 🔊 Sonido de muerte
        PlaySound(deathSound, 1f);

        // Detener el fuego loop
        if (flameAudioSource != null)
            flameAudioSource.Stop();

        Destroy(gameObject, 1f); // delay para que suene la muerte
    }

    void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}