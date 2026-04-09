using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private float currentSpeed;
    public float rotationSpeed = 10f;

    [Header("Ataque")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float attackCooldown = 1f;
    public float attackDuration = 0.5f;
    public Collider swordCollider;
    private float lastAttackTime = -Mathf.Infinity;
    private bool isAttacking = false;

    [Header("Sonidos")]
    public AudioClip[] walkSounds; // múltiples pasos para variedad
    public AudioClip attackVoiceSound; // "wooah"
    public AudioClip swordSwingSound; // sonido de espada
    public AudioClip takeDamageSound; // cuando recibe daño
    public AudioClip deathSound; // cuando muere
    public float walkSoundInterval = 0.5f; // cada cuánto suena el paso

    private AudioSource audioSource;
    private float lastWalkSoundTime = 0f;

    private Animator anim;
    private CapsuleCollider col;
    private Rigidbody rb;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        // 🔊 Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        currentHealth = maxHealth;
        currentSpeed = speed;

        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (isDead || isAttacking) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v).normalized;
        Vector3 velocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        rb.linearVelocity = velocity;

        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }

        anim.SetFloat("Speed", move.magnitude);

        // 🚶 Sonidos de caminar
        if (move.magnitude > 0.1f && Time.time - lastWalkSoundTime >= walkSoundInterval)
        {
            PlayWalkSound();
            lastWalkSoundTime = Time.time;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Morir();
        }
    }

    System.Collections.IEnumerator Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack");

        // 🔊 Reproducir sonidos de ataque
        PlayAttackSounds();

        if (swordCollider != null)
        {
            Sword sword = swordCollider.GetComponent<Sword>();
            if (sword != null)
            {
                sword.StartAttack();
            }
            swordCollider.enabled = true;
        }

        yield return new WaitForSeconds(attackDuration);

        if (swordCollider != null)
            swordCollider.enabled = false;

        isAttacking = false;
    }

    void Morir()
    {
        isDead = true;
        anim.SetTrigger("Dead");
        anim.SetFloat("Speed", 0);
        rb.linearVelocity = Vector3.zero;

        if (col != null)
            col.enabled = false;

        rb.isKinematic = true;

        // 🔊 Sonido de muerte
        PlaySound(deathSound);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player recibió daño: " + damage + " | Vida restante: " + currentHealth);

        // 🔊 Sonido de daño
        PlaySound(takeDamageSound);

        if (currentHealth <= 0)
        {
            Morir();
        }
    }

    public void ApplySlow(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SlowCoroutine(duration));
    }

    System.Collections.IEnumerator SlowCoroutine(float duration)
    {
        currentSpeed = speed * 0.5f;

        yield return new WaitForSeconds(duration);

        currentSpeed = speed;
    }
    public float GetHealthNormalized()
    {
        return currentHealth / maxHealth;
    }
    public int GetDamage()
    {
        if (swordCollider == null) return 0;
        return swordCollider.GetComponent<Sword>().damage;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // 🔊 Métodos de sonido
    void PlayWalkSound()
    {
        if (walkSounds != null && walkSounds.Length > 0)
        {
            // Elegir sonido aleatorio para variedad
            AudioClip clip = walkSounds[Random.Range(0, walkSounds.Length)];
            audioSource.PlayOneShot(clip, 0.5f); // volumen 50%
        }
    }

    void PlayAttackSounds()
    {
        // Reproducir voz "wooah"
        if (attackVoiceSound != null)
        {
            audioSource.PlayOneShot(attackVoiceSound, 0.8f);
        }

        // Reproducir sonido de espada (con pequeño delay)
        if (swordSwingSound != null)
        {
            audioSource.PlayOneShot(swordSwingSound, 0.7f);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}