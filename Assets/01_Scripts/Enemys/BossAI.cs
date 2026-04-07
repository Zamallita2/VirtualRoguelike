using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    enum State { Idle, Talk, Combat, Dead }
    State state = State.Idle;

    [Header("Referencias")]
    public Transform player;
    public Animator anim;
    public AudioSource audioSource;
    public PlayerMovement playerMovement;
    public Rigidbody rb;

    [Header("ESPADA")]
    public BossSword bossSword;   // Script que controla el daño

    [Header("Rangos")]
    public float detectionRange = 15f;
    public float loseRange = 35f;
    public float attackRange = 2.5f;

    [Header("Movimiento")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 8f;

    [Header("Cooldowns")]
    public float attackCooldown = 1.8f;    // Entre ataques normales (Attack1/Attack2)
    public float furyCooldown = 15f;       // Cada cuánto hace Fury
    public float talkDuration = 4f;        // Duración de la charla inicial (o ajusta con los clips)

    [Header("Vida")]
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("Audios")]
    public AudioClip[] talkClips;
    public AudioClip[] roarClips;
    public AudioClip[] attackVoice;
    public AudioClip summonClip;
    public AudioClip deathClip;

    [Header("Invocación")]
    public GameObject summonPrefab;   // El enemigo que se invoca (Ej: Armadura Viviente)
    public int summonCount = 4;
    public float summonRadius = 3f;

    // Estados internos
    private bool isBusy = false;
    private bool isAttacking = false;
    private bool isSummoning = false;
    private bool hasSummoned = false;

    private float lastAttackTime;
    private float lastFuryTime;

    private Vector3 startPos;

    void Start()
    {
        currentHealth = maxHealth;
        startPos = transform.position;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerMovement == null && player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.freezeRotation = true;
        }

        lastFuryTime = -furyCooldown; // Para que no haga fury al inicio inmediatamente
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (state == State.Dead) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Detectar jugador desde Idle
        if (state == State.Idle && dist <= detectionRange)
        {
            StartCoroutine(StartCombat());
            return;
        }

        // Reset si el jugador se aleja demasiado
        if (state != State.Idle && dist > loseRange)
        {
            ResetBoss();
            return;
        }

        // Invocación al 50% de vida (prioritaria)
        if (!hasSummoned && currentHealth <= maxHealth * 0.5f && !isSummoning && !isBusy)
        {
            StartCoroutine(Summon());
            return;
        }

        // Si está ocupado, no se mueve ni rota
        if (isBusy)
        {
            StopMovement();
            return;
        }

        if (state == State.Combat)
        {
            // Rotar hacia el jugador
            FacePlayer();

            // Verificar si debe hacer Fury (cada furyCooldown segundos)
            if (Time.time >= lastFuryTime + furyCooldown)
            {
                StartCoroutine(FuryRoutine());
                return;
            }

            // Moverse hacia el jugador
            MoveTowardsPlayer();

            // Ataque normal si está en rango y cooldown listo
            if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    // ================= MOVIMIENTO =================
    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        Vector3 targetVelocity = direction * moveSpeed;

        if (rb != null)
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        else
            transform.position += direction * moveSpeed * Time.deltaTime;

        // Actualizar velocidad en Animator
        if (anim != null)
            anim.SetFloat("Speed", direction.magnitude > 0.1f ? moveSpeed : 0f);
    }

    void StopMovement()
    {
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
        if (anim != null)
            anim.SetFloat("Speed", 0f);
    }

    // ================= INICIO DEL COMBATE (TALK) =================
    IEnumerator StartCombat()
    {
        state = State.Talk;
        isBusy = true;
        StopMovement();

        if (anim != null) anim.SetTrigger("Talk");
        Debug.Log("🗣️ TALK");

        // Reproducir todos los clips de diálogo (o usa talkDuration si no tienes clips)
        if (talkClips != null && talkClips.Length > 0)
        {
            foreach (var clip in talkClips)
            {
                if (clip != null && audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    yield return new WaitForSeconds(clip.length);
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(talkDuration);
        }

        yield return StartCoroutine(FuryRoutine()); // Primer rugido+Fury al empezar

        state = State.Combat;
        isBusy = false;
    }

    // ================= FURY (RUGIDO + ATAQUE GIRATORIO) =================
    IEnumerator FuryRoutine()
    {
        isBusy = true;
        StopMovement();

        // Rugido
        if (anim != null) anim.SetTrigger("DoRoar");
        PlayRandom(roarClips);
        Debug.Log("🐲 ROAR");
        yield return new WaitForSeconds(1f); // Ajusta según animación

        // Fury
        if (anim != null) anim.SetTrigger("DoFury");
        Debug.Log("💀 FURY");

        // Activar espada para daño
        if (bossSword != null)
        {
            bossSword.StartAttack();
        }

        yield return new WaitForSeconds(1.2f); // Duración del ataque Fury

        // Desactivar espada
        if (bossSword != null)
        {
            bossSword.EndAttack();
        }

        yield return new WaitForSeconds(0.3f);

        lastFuryTime = Time.time;
        isBusy = false;
    }

    // ================= ATAQUE NORMAL (ATTACK1 O ATTACK2) =================
    IEnumerator AttackRoutine()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        isBusy = true;
        StopMovement();

        // Elegir animación aleatoria
        if (Random.value < 0.5f)
            anim.SetTrigger("DoAttack1");
        else
            anim.SetTrigger("DoAttack2");

        Debug.Log("⚔️ ATTACK normal");

        if (Random.value < 0.2f)
            PlayRandom(attackVoice);

        // Activar espada
        if (bossSword != null)
            bossSword.StartAttack();

        // Tiempo de impacto (ajústalo según tu animación)
        yield return new WaitForSeconds(0.6f);

        // Desactivar espada
        if (bossSword != null)
            bossSword.EndAttack();

        yield return new WaitForSeconds(0.6f); // Terminar animación

        lastAttackTime = Time.time;
        isAttacking = false;
        isBusy = false;
    }

    // ================= INVOCACIÓN =================
    IEnumerator Summon()
    {
        isSummoning = true;
        isBusy = true;
        hasSummoned = true;
        StopMovement();

        if (anim != null) anim.SetTrigger("DoSummon");
        Debug.Log("👹 SUMMON");

        if (summonClip != null && audioSource != null)
            audioSource.PlayOneShot(summonClip);

        // Pequeña pausa para que se vea el gesto
        yield return new WaitForSeconds(0.8f);

        // Invocar enemigos
        if (summonPrefab != null)
        {
            for (int i = 0; i < summonCount; i++)
            {
                Vector3 offset = Random.insideUnitSphere * summonRadius;
                offset.y = 0;
                Vector3 spawnPos = transform.position + offset;
                Instantiate(summonPrefab, spawnPos, Quaternion.identity);
                Debug.Log($"📦 Invocado enemigo {i + 1}");
            }
        }

        yield return new WaitForSeconds(0.6f);

        isSummoning = false;
        isBusy = false;
    }

    // ================= DAÑO Y MUERTE =================
    public void TakeDamage(float dmg)
    {
        if (state == State.Talk || state == State.Idle)
        {
            Debug.Log("🛡️ Boss invulnerable");
            return;
        }

        currentHealth -= dmg;
        Debug.Log($"💀 Boss recibe {dmg} de daño. Vida: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (state == State.Dead) return;

        state = State.Dead;
        isBusy = true;
        StopMovement();

        if (anim != null) anim.SetTrigger("DoDie");

        if (deathClip != null && audioSource != null)
            audioSource.PlayOneShot(deathClip);

        if (bossSword != null)
            bossSword.EndAttack();

        Debug.Log("💀 BOSS MUERTO");
    }

    void ResetBoss()
    {
        StopAllCoroutines();

        state = State.Idle;
        isBusy = false;
        isAttacking = false;
        isSummoning = false;
        hasSummoned = false;
        currentHealth = maxHealth;

        transform.position = startPos;
        StopMovement();

        if (bossSword != null)
            bossSword.EndAttack();

        Debug.Log("🔄 BOSS RESETEADO");
    }

    void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}