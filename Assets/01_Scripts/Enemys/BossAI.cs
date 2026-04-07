using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    enum State { Idle, Talk, Combat, Dead }
    State state = State.Idle;

    [Header("Referencias")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;
    public AudioSource audioSource;
    public PlayerMovement playerMovement;

    [Header("ESPADA")]
    public Collider weaponCollider;
    public BossSword bossSword;

    [Header("Rangos")]
    public float detectRange = 15f;
    public float loseRange = 35f;
    public float attackRange = 2.5f;

    [Header("Cooldowns")]
    public float attackCooldown = 2f;
    public float roarInterval = 12f;

    [Header("Vida")]
    public float maxHealth = 500f;
    float currentHealth;

    [Header("Audios")]
    public AudioClip[] talkClips;
    public AudioClip[] roarClips;
    public AudioClip[] attackVoice;
    public AudioClip summonClip;
    public AudioClip deathClip;

    [Header("Invocación")]
    public GameObject summonPrefab;

    bool isBusy = false;
    bool isAttacking = false;
    bool isSummoning = false;
    bool hasSummoned = false;

    float lastAttack;
    float nextRoarTime;

    Vector3 startPos;

    void Start()
    {
        currentHealth = maxHealth;
        startPos = transform.position;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();

        nextRoarTime = Time.time + 5f;

        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    void Update()
    {
        if (state == State.Dead) return;

        float dist = Vector3.Distance(transform.position, player.position);

        anim.SetBool("IsBusy", isBusy);

        // DETECTAR PLAYER
        if (state == State.Idle && dist <= detectRange)
        {
            StartCoroutine(StartCombat());
            return;
        }

        // RESET
        if (state != State.Idle && dist > loseRange)
        {
            ResetBoss();
            return;
        }

        // 🔥 SUMMON PRIORIDAD
        if (!hasSummoned && currentHealth <= maxHealth * 0.5f && !isSummoning)
        {
            StopAllCoroutines();
            StartCoroutine(Summon());
            return;
        }

        // BLOQUEO
        if (isBusy)
        {
            ForceStop();
        }
        else if (state == State.Combat)
        {
            // ROAR AUTOMÁTICO
            if (Time.time >= nextRoarTime)
            {
                StartCoroutine(RoarAndFury());
                return;
            }

            // PERSEGUIR
            agent.isStopped = false;
            agent.updatePosition = true;
            agent.SetDestination(player.position);

            // ATAQUE
            if (dist <= attackRange && Time.time >= lastAttack + attackCooldown)
            {
                StartCoroutine(Attack());
            }
        }

        // SPEED
        if (isBusy)
            anim.SetFloat("Speed", 0f);
        else
            anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    // ================= BLOQUEO =================
    void ForceStop()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
            agent.updatePosition = false;
        }
    }

    // ================= COMBATE =================
    IEnumerator StartCombat()
    {
        state = State.Talk;
        isBusy = true;

        ForceStop();

        anim.SetTrigger("Talk");
        Debug.Log("🗣️ TALK");

        foreach (var clip in talkClips)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
        }

        yield return StartCoroutine(RoarAndFury());

        state = State.Combat;
        isBusy = false;
        agent.updatePosition = true;
    }

    // ================= ROAR + FURY =================
    IEnumerator RoarAndFury()
    {
        isBusy = true;
        ForceStop();

        anim.SetTrigger("DoRoar");
        PlayRandom(roarClips);
        Debug.Log("🐲 ROAR");

        yield return new WaitForSeconds(1f);

        anim.SetTrigger("DoFury");
        Debug.Log("💀 FURY");

        // 🔥 ACTIVAR ESPADA (FURY)
        if (weaponCollider != null)
        {
            bossSword.StartAttack();
            weaponCollider.enabled = true;
        }

        yield return new WaitForSeconds(1f);

        if (weaponCollider != null)
            weaponCollider.enabled = false;

        yield return new WaitForSeconds(0.3f);

        nextRoarTime = Time.time + roarInterval;

        isBusy = false;
        agent.updatePosition = true;
    }

    // ================= ATAQUE =================
    IEnumerator Attack()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        isBusy = true;

        ForceStop();

        if (Random.value < 0.5f)
            anim.SetTrigger("DoAttack1");
        else
            anim.SetTrigger("DoAttack2");

        Debug.Log("⚔️ ATTACK");

        if (Random.value < 0.2f)
            PlayRandom(attackVoice);

        // 🔥 ACTIVAR ESPADA
        if (weaponCollider != null)
        {
            bossSword.StartAttack();
            weaponCollider.enabled = true;
        }

        yield return new WaitForSeconds(0.6f);

        if (weaponCollider != null)
            weaponCollider.enabled = false;

        yield return new WaitForSeconds(0.6f);

        lastAttack = Time.time;

        isAttacking = false;
        isBusy = false;

        agent.updatePosition = true;
    }

    // ================= SUMMON =================
    IEnumerator Summon()
    {
        isSummoning = true;
        isBusy = true;
        hasSummoned = true;

        ForceStop();

        anim.SetTrigger("DoSummon");

        if (summonClip != null)
            audioSource.PlayOneShot(summonClip);

        Debug.Log("👹 SUMMON INMEDIATO");

        // 🔥 INVOCAR INMEDIATO
        for (int i = 0; i < 4; i++)
        {
            Instantiate(summonPrefab,
                transform.position + Random.insideUnitSphere * 3f,
                Quaternion.identity);
        }

        yield return new WaitForSeconds(1.2f);

        isBusy = false;
        isSummoning = false;

        agent.updatePosition = true;
    }

    // ================= VIDA =================
    public void TakeDamage(float dmg)
    {
        if (state == State.Talk || state == State.Idle)
        {
            Debug.Log("🛡️ Boss invulnerable");
            return;
        }

        currentHealth -= dmg;
        Debug.Log("💀 Boss recibió daño: " + dmg + " | Vida: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        state = State.Dead;

        ForceStop();

        anim.SetTrigger("DoDie");

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        Debug.Log("💀 DEAD");
    }

    void ResetBoss()
    {
        state = State.Idle;
        isBusy = false;
        isAttacking = false;
        isSummoning = false;
        hasSummoned = false;

        currentHealth = maxHealth;

        agent.Warp(startPos);
        agent.updatePosition = true;
        agent.isStopped = false;

        Debug.Log("🔄 RESET");
    }

    void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}