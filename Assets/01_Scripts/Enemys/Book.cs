using UnityEngine;
using System.Collections;

public class Book : MonoBehaviour
{
    public float health = 20f;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float rotationSpeed = 5f;

    [Header("Shooting")]
    public float fireRate = 2f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Bones (opcional)")]
    public Transform leftBone;
    public Transform rightBone;

    [Header("Sonidos")]
    public AudioClip detectSound; // cuando detecta al jugador
    public AudioClip pageFlipSound; // al abrir/cerrar
    public AudioClip shootSound; // disparo
    public AudioClip hitSound; // cuando recibe daño
    public AudioClip deathSound; // al morir
    public AudioClip ambientWhisperSound; // susurros místicos (loop)

    private Transform player;
    private float fireTimer;
    private bool isAnimating = false;
    private bool hasDetected = false;
    private AudioSource audioSource;
    private AudioSource ambientAudioSource;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            player = obj.transform;

        // 🔊 Configurar AudioSources
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 1f; // 3D
        audioSource.maxDistance = 12f;

        // AudioSource para ambiente loop
        ambientAudioSource = gameObject.AddComponent<AudioSource>();
        ambientAudioSource.loop = true;
        ambientAudioSource.volume = 0.2f;
        ambientAudioSource.spatialBlend = 1f;
        ambientAudioSource.maxDistance = 15f;

        if (ambientWhisperSound != null)
        {
            ambientAudioSource.clip = ambientWhisperSound;
            ambientAudioSource.Play();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            // 🔊 Sonido de detección
            if (!hasDetected)
            {
                PlaySound(detectSound, 0.5f);
                hasDetected = true;
            }

            LookAtPlayer();
            HandleShooting();
        }
        else
        {
            hasDetected = false;
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 euler = targetRotation.eulerAngles;
        euler.z = 0f;
        Quaternion finalRotation = Quaternion.Euler(euler);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            finalRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void HandleShooting()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        // 🔊 Sonido de disparo
        PlaySound(shootSound, 0.7f);

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (!isAnimating && leftBone != null && rightBone != null)
            StartCoroutine(BoneAttackAnimation());
    }

    IEnumerator BoneAttackAnimation()
    {
        isAnimating = true;
        float duration = 0.35f;
        float time = 0f;

        // 🔊 Sonido de páginas al abrir
        PlaySound(pageFlipSound, 0.5f);

        Vector3 leftStartEuler = leftBone.localEulerAngles;
        Vector3 rightStartEuler = rightBone.localEulerAngles;
        leftStartEuler.x = 0f;
        leftStartEuler.z = 0f;
        rightStartEuler.x = 0f;
        rightStartEuler.z = 0f;

        Vector3 leftOpenEuler = new Vector3(0f, leftStartEuler.y + 35f, 134.49f);
        Vector3 rightOpenEuler = new Vector3(0f, rightStartEuler.y - 35f, -122.546f);

        // Abrir
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float leftY = Mathf.LerpAngle(leftStartEuler.y, leftOpenEuler.y, t);
            float rightY = Mathf.LerpAngle(rightStartEuler.y, rightOpenEuler.y, t);

            leftBone.localRotation = Quaternion.Euler(0f, leftY, 134.49f);
            rightBone.localRotation = Quaternion.Euler(0f, rightY, -122.546f);
            yield return null;
        }

        time = 0f;

        // 🔊 Sonido de páginas al cerrar
        PlaySound(pageFlipSound, 0.4f);

        // Cerrar
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float leftY = Mathf.LerpAngle(leftOpenEuler.y, leftStartEuler.y, t);
            float rightY = Mathf.LerpAngle(rightOpenEuler.y, rightStartEuler.y, t);

            leftBone.localRotation = Quaternion.Euler(0f, leftY, 134.49f);
            rightBone.localRotation = Quaternion.Euler(0f, rightY, -122.546f);
            yield return null;
        }

        isAnimating = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " recibió daño: " + damage + " | Vida: " + health);

        // 🔊 Sonido de golpe
        PlaySound(hitSound, 0.7f);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió unu");

        // 🔊 Sonido de muerte
        PlaySound(deathSound, 1f);

        // Detener susurros
        if (ambientAudioSource != null)
            ambientAudioSource.Stop();

        Destroy(gameObject, 1f);
    }

    void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}