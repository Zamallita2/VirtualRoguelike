using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    private float currentSpeed;
    public float rotationSpeed = 10f;

    [Header("Ataque / Vida")]
    public float maxHealth = 100f;
    [SerializeField] public float currentHealth;
    public float attackCooldown = 1f;
    public float attackDuration = 0.5f;
    public Collider swordCollider;
    private float lastAttackTime = -Mathf.Infinity;
    private bool isAttacking = false;

    // ═══════════════════════════════════════════════
    // MONEDAS
    // ═══════════════════════════════════════════════
    [Header("Monedas")]
    public int coins = 0;

    // ═══════════════════════════════════════════════
    // MEJORAS DE TIENDA
    // ═══════════════════════════════════════════════
    [Header("Regeneración (Especial)")]
    public bool hasRegeneration = false;
    public float regenAmountPerTick = 2f;
    public float regenTickInterval = 3f;
    private float regenTimer = 0f;

    [Header("Segunda Oportunidad (Especial)")]
    public bool hasSecondChance = false;
    public bool secondChanceUsed = false;
    public float secondChanceRevivePercent = 0.3f;

    [Header("Cadencia de Ataque (Básica)")]
    public float attackCooldownReduction = 0f;

    // ═══════════════════════════════════════════════
    // SONIDOS
    // ═══════════════════════════════════════════════
    [Header("Sonidos")]
    public AudioClip[] walkSounds;
    public AudioClip attackVoiceSound;
    public AudioClip swordSwingSound;
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    public float walkSoundInterval = 0.5f;

    private AudioSource audioSource;
    private float lastWalkSoundTime = 0f;

    // ═══════════════════════════════════════════════
    // COMPONENTES
    // ═══════════════════════════════════════════════
    private Animator anim;
    private CapsuleCollider col;
    private Rigidbody rb;
    private bool isDead = false;

    // ═══════════════════════════════════════════════
    // INICIO
    // ═══════════════════════════════════════════════
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        currentHealth = maxHealth;
        currentSpeed = speed;

        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    // ═══════════════════════════════════════════════
    // UPDATE
    // ═══════════════════════════════════════════════
    void Update()
    {
        if (isDead) return;

        // Ataque
        if (Input.GetKeyDown(KeyCode.Space) &&
            Time.time >= lastAttackTime + GetCurrentCooldown())
        {
            StartCoroutine(Attack());
        }

        // Regeneración pasiva
        if (hasRegeneration && currentHealth < maxHealth)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenTickInterval)
            {
                regenTimer = 0f;
                Heal(regenAmountPerTick);
            }
        }
    }

    // ═══════════════════════════════════════════════
    // FIXED UPDATE
    // ═══════════════════════════════════════════════
    void FixedUpdate()
    {
        if (isDead || isAttacking) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v).normalized;
        Vector3 velocity = new Vector3(move.x * currentSpeed,
                                       rb.linearVelocity.y,
                                       move.z * currentSpeed);
        rb.linearVelocity = velocity;

        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }

        anim.SetFloat("Speed", move.magnitude);

        if (move.magnitude > 0.1f &&
            Time.time - lastWalkSoundTime >= walkSoundInterval)
        {
            PlayWalkSound();
            lastWalkSoundTime = Time.time;
        }
    }

    // ═══════════════════════════════════════════════
    // COMBATE
    // ═══════════════════════════════════════════════
    System.Collections.IEnumerator Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack");

        PlayAttackSounds();

        if (swordCollider != null)
        {
            Sword sword = swordCollider.GetComponent<Sword>();
            if (sword != null) sword.StartAttack();
            swordCollider.enabled = true;
        }

        yield return new WaitForSeconds(attackDuration);

        if (swordCollider != null)
            swordCollider.enabled = false;

        isAttacking = false;
    }

    void Morir()
    {
        // ── Segunda Oportunidad ──
        if (hasSecondChance && !secondChanceUsed)
        {
            secondChanceUsed = true;
            currentHealth = maxHealth * secondChanceRevivePercent;
            Debug.Log($"¡Segunda Oportunidad! Reviviendo con {currentHealth} HP");
            return;
        }

        isDead = true;
        anim.SetTrigger("Dead");
        anim.SetFloat("Speed", 0);
        rb.linearVelocity = Vector3.zero;

        if (col != null) col.enabled = false;
        rb.isKinematic = true;

        PlaySound(deathSound);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Daño recibido: {damage} | Vida: {currentHealth}");

        PlaySound(takeDamageSound);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Morir();
        }
    }

    public void ApplySlow(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SlowCoroutine(duration));
    }
    
    /*public int GetDamage()
    {
        if (swordCollider == null) return 0;
        return swordCollider.GetComponent<Sword>().damage;
    }*/

    System.Collections.IEnumerator SlowCoroutine(float duration)
    {
        currentSpeed = speed * 0.5f;
        yield return new WaitForSeconds(duration);
        currentSpeed = speed;
    }

    // ═══════════════════════════════════════════════
    // GETTERS
    // ═══════════════════════════════════════════════
    /*public float GetHealthNormalized() => currentHealth / maxHealth;*/
    /*public float GetCurrentHealth() => currentHealth;*/
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentCooldown() => Mathf.Max(0.2f, attackCooldown - attackCooldownReduction);

    /*public int GetDamage()
    {
        if (swordCollider == null) return 0;
        Sword sword = swordCollider.GetComponent<Sword>();
        return sword != null ? sword.damage : 0;
    }*/

    // ═══════════════════════════════════════════════
    // MÉTODOS DE TIENDA — BÁSICAS
    // ═══════════════════════════════════════════════

    /// <summary>Agrega monedas (llamar al matar enemigos)</summary>
    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log($"Monedas: +{amount} | Total: {coins}");
    }

    /// <summary>Cura sin superar la vida máxima</summary>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Curado: +{amount} HP | Vida: {currentHealth}/{maxHealth}");
    }

    /// <summary>Aumenta vida máxima y cura esa cantidad</summary>
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        Debug.Log($"Vida máxima: +{amount} | Nueva: {maxHealth}");
    }

    /// <summary>Aumenta el daño de la espada</summary>
    public void IncreaseDamage(float amount)
    {
        if (swordCollider == null) return;
        Sword sword = swordCollider.GetComponent<Sword>();
        if (sword != null)
        {
            sword.damage += (int)amount;
            Debug.Log($"Daño: +{amount} | Daño actual: {sword.damage}");
        }
    }

    /// <summary>Aumenta velocidad de movimiento permanentemente</summary>
    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        currentSpeed += amount;
        Debug.Log($"Velocidad: +{amount} | Actual: {speed}");
    }

    /// <summary>Reduce el cooldown entre ataques</summary>
    public void IncreaseAttackSpeed(float reductionAmount)
    {
        attackCooldownReduction += reductionAmount;
        Debug.Log($"Cadencia mejorada. Cooldown actual: {GetCurrentCooldown()}s");
    }

    // ═══════════════════════════════════════════════
    // MÉTODOS DE TIENDA — ESPECIALES
    // ═══════════════════════════════════════════════

    /// <summary>Activa regeneración pasiva de vida</summary>
    public void UnlockRegeneration()
    {
        hasRegeneration = true;
        regenTimer = 0f;
        Debug.Log("Regeneración activada");
    }

    /// <summary>Activa Segunda Oportunidad (revive 1 vez con 30% HP)</summary>
    public void UnlockSecondChance()
    {
        hasSecondChance = true;
        secondChanceUsed = false;
        Debug.Log("Segunda Oportunidad activada");
    }

    // ═══════════════════════════════════════════════
    // SONIDOS
    // ═══════════════════════════════════════════════
    void PlayWalkSound()
    {
        if (walkSounds != null && walkSounds.Length > 0)
        {
            AudioClip clip = walkSounds[Random.Range(0, walkSounds.Length)];
            audioSource.PlayOneShot(clip, 0.5f);
        }
    }

    void PlayAttackSounds()
    {
        if (attackVoiceSound != null) audioSource.PlayOneShot(attackVoiceSound, 0.8f);
        if (swordSwingSound != null) audioSource.PlayOneShot(swordSwingSound, 0.7f);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }


    public float GetHealthNormalized()
    {
        if (maxHealth <= 0) return 0;
        return currentHealth / maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /*public float GetMaxHealth()
    {
        return maxHealth;
    }*/

    public int GetDamage()
    {
        if (swordCollider == null) return 0;

        Sword sword = swordCollider.GetComponent<Sword>();
        if (sword == null) return 0;

        return sword.GetDamage();
    }

    public int GetCoins()
    {
        return coins;
    }

    /*public void AddCoins(int amount)
    {
        coins += amount;
        if (coins < 0) coins = 0;
    }*/

    public void AddSpeed(float amount)
    {
        speed += amount;
        if (speed < 0) speed = 0;
    }

    public void AddDamage(int amount)
    {
        if (swordCollider == null) return;

        Sword sword = swordCollider.GetComponent<Sword>();
        if (sword == null) return;

        sword.AddDamage(amount);
    }

    /*public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        if (maxHealth < 1) maxHealth = 1;
    }*/

    /*public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentHealth < 0)
            currentHealth = 0;
    }*/
}