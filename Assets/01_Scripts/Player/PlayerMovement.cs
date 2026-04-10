using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 10f;

    [Header("Ataque / Vida")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    public float attackCooldown = 1f;
    public float attackDuration = 0.5f;
    public Collider swordCollider;

    [Header("Monedas")]
    [SerializeField] private int coins = 0;

    private float lastAttackTime = -Mathf.Infinity;
    private bool isAttacking = false;

    private Animator anim;
    private CapsuleCollider col;
    private Rigidbody rb;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;

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
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log("Player recibió daño: " + damage + " | Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Morir();
        }
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

    public float GetMaxHealth()
    {
        return maxHealth;
    }

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

    public void AddCoins(int amount)
    {
        coins += amount;
        if (coins < 0) coins = 0;
    }

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

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        if (maxHealth < 1) maxHealth = 1;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentHealth < 0)
            currentHealth = 0;
    }
}