using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Roar, Chase, FirstAttack, NormalAttack, Dead }

    [Header("Target")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Detection / Attack")]
    public float detectionRange = 10f;
    public float firstAttackRange = 2f;
    public float normalAttackRange = 2.4f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 8f;

    [Header("Health")]
    public float maxHealth = 50f;
    public float currentHealth;

    [Header("Animator")]
    public Animator anim;
    public string runBool = "Run";
    public string roarTrigger = "Roar";
    public string firstAttackTrigger = "FirstAttack";
    public string normalAttackTrigger = "AttackNormal";
    public string dieTrigger = "Die";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip roarClip;
    public AudioClip firstAttackClip;
    public AudioClip normalAttackClip;
    public AudioClip dieClip;

    [Header("Attack Timing")]
    public float roarDuration = 1.2f;
    public float firstAttackDuration = 1.0f;
    public float normalAttackDuration = 0.9f;
    public float firstAttackHitDelay = 0.5f;
    public float normalAttackHitDelay = 0.3f;

    [Header("Enemy Weapon")]
    public EnemyWeapon enemyWeapon;
    public float normalDamage = 10f;
    public float criticalDamage = 30f;

    [Header("State")]
    public EnemyState currentState = EnemyState.Idle;

    private Rigidbody rb;
    private Collider mainCollider;       // <-- Collider principal (Capsule, Box, etc.)
    private bool isBusy = false;
    private bool isDead = false;
    private bool firstAttackDone = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();   // Obtiene el collider principal

        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (enemyWeapon == null) enemyWeapon = GetComponentInChildren<EnemyWeapon>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.freezeRotation = true;
        }
        if (enemyWeapon != null) enemyWeapon.SetDamageActive(false);
        if (anim != null) anim.SetBool(runBool, false);
    }

    private void Update()
    {
        if (isDead || player == null) return;
        float distance = Vector3.Distance(transform.position, player.position);

        if (currentState == EnemyState.Chase && distance > detectionRange && !isBusy)
            ReturnToIdle();

        switch (currentState)
        {
            case EnemyState.Idle:
                if (!isBusy && distance <= detectionRange) StartCoroutine(RoarRoutine());
                else if (anim != null) anim.SetBool(runBool, false);
                break;

            case EnemyState.Chase:
                FacePlayer();
                if (distance <= firstAttackRange && !firstAttackDone && !isBusy)
                    StartCoroutine(FirstAttackRoutine());
                else if (distance <= normalAttackRange && firstAttackDone && !isBusy)
                    StartCoroutine(NormalAttackRoutine());
                else
                    MoveTowardsPlayer();
                break;
        }
    }

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
        if (anim != null && !anim.GetBool(runBool)) anim.SetBool(runBool, true);
    }

    private void StopMovement()
    {
        if (rb != null) rb.linearVelocity = Vector3.zero;
        if (anim != null) anim.SetBool(runBool, false);
    }

    private void ReturnToIdle()
    {
        currentState = EnemyState.Idle;
        firstAttackDone = false;
        isBusy = false;
        StopMovement();
    }

    private IEnumerator RoarRoutine()
    {
        isBusy = true;
        currentState = EnemyState.Roar;
        StopMovement();
        if (anim != null) anim.SetTrigger(roarTrigger);
        PlayClip(roarClip);
        yield return new WaitForSeconds(roarDuration);
        if (!isDead) currentState = EnemyState.Chase;
        isBusy = false;
    }

    private IEnumerator FirstAttackRoutine()
    {
        isBusy = true;
        currentState = EnemyState.FirstAttack;
        StopMovement();
        if (anim != null) anim.SetTrigger(firstAttackTrigger);
        PlayClip(firstAttackClip);
        if (enemyWeapon != null)
            StartCoroutine(ActivateWeaponForDamage(firstAttackHitDelay, criticalDamage, firstAttackDuration));
        yield return new WaitForSeconds(firstAttackDuration);
        if (!isDead)
        {
            firstAttackDone = true;
            currentState = EnemyState.Chase;
        }
        yield return new WaitForSeconds(0.2f);
        isBusy = false;
    }

    private IEnumerator NormalAttackRoutine()
    {
        isBusy = true;
        currentState = EnemyState.NormalAttack;
        StopMovement();
        if (anim != null) anim.SetTrigger(normalAttackTrigger);
        PlayClip(normalAttackClip);
        if (enemyWeapon != null)
            StartCoroutine(ActivateWeaponForDamage(normalAttackHitDelay, normalDamage, normalAttackDuration));
        yield return new WaitForSeconds(normalAttackDuration);
        if (!isDead) currentState = EnemyState.Chase;
        isBusy = false;
    }

    private IEnumerator ActivateWeaponForDamage(float delay, float damageAmount, float duration)
    {
        yield return new WaitForSeconds(delay);
        if (enemyWeapon != null && !isDead)
        {
            enemyWeapon.SetDamage(damageAmount);
            enemyWeapon.SetDamageActive(true);
            Debug.Log($"🗡️ Arma activada con daño: {damageAmount} ({(damageAmount == criticalDamage ? "CRÍTICO" : "NORMAL")})");
            yield return new WaitForSeconds(duration-0.05f);
            enemyWeapon.SetDamageActive(false);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        Debug.Log($"{name} recibe {damage} de daño. Vida: {currentHealth}");
        if (currentHealth <= 0f) Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Destroy(gameObject,10);
        var notifier = GetComponent<RoomEnemyNotifier>();
        if (notifier != null)
            notifier.NotifyDeath();
        
        isBusy = true;
        currentState = EnemyState.Dead;
        StopMovement();

        // 🔥 DROPEAR LOOT
        EnemyLootDrop loot = GetComponent<EnemyLootDrop>();
        if (loot != null)
        {
            loot.DropLoot();
        }

        // Desactivar el arma
        if (enemyWeapon != null) enemyWeapon.SetDamageActive(false);

        if (anim != null) anim.SetTrigger(dieTrigger);
        PlayClip(dieClip);

        if (mainCollider != null)
            mainCollider.enabled = false;

        Collider[] allColliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in allColliders)
        {
            if (col != mainCollider)
                col.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Debug.Log($"{name} ha muerto. Colliders desactivados.");
    }
}