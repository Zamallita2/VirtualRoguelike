using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    private Animator anim;
    private CapsuleCollider col;
    private Rigidbody rb;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v).normalized;

        // Movimiento usando Rigidbody (correcto)
        Vector3 velocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        rb.linearVelocity = velocity;

        // Rotación hacia donde se mueve
        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }

        // Animación de movimiento
        anim.SetFloat("Speed", move.magnitude);
    }

    void Update()
    {
        if (isDead) return;

        // Ataque
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Attack");
        }

        // Muerte
        if (Input.GetKeyDown(KeyCode.K))
        {
            Morir();
        }
    }

    void Morir()
    {
        isDead = true;

        // Animación de muerte
        anim.SetTrigger("Dead");
        anim.SetFloat("Speed", 0);

        // Detener movimiento
        rb.linearVelocity = Vector3.zero;

        // Desactivar collider
        if (col != null)
        {
            col.enabled = false;
        }

        // Opcional: congelar Rigidbody
        rb.isKinematic = true;
    }
}