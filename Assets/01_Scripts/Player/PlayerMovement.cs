using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private float currentSpeed;
    public float rotationSpeed = 10f;

    [Header("Ataque")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float attackCooldown = 1f; // tiempo entre ataques
    public float attackDuration = 0.5f; // cuánto dura el ataque (para collider)
    public Collider swordCollider; // collider de la espada

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
        currentSpeed = speed;

        // Asegurarse que la espada empieza desactivada
        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (isDead || isAttacking) return; // 🚫 no se mueve mientras ataca

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v).normalized;

        Vector3 velocity = new Vector3(move.x * currentSpeed, rb.linearVelocity.y, move.z * currentSpeed); rb.linearVelocity = velocity;

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

        // 🗡️ Ataque con cooldown
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
            // 🔥 Obtener script de la espada
            Sword sword = swordCollider.GetComponent<Sword>();

            if (sword != null)
            {
                sword.StartAttack(); // resetear enemigos golpeados
            }

            swordCollider.enabled = true; // activar collider
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
}