using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Animator anim;
    private CapsuleCollider col;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        // Si está muerto, no hace nada más
        if (isDead) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v).normalized;

        // Movimiento
        transform.Translate(move * speed * Time.deltaTime, Space.World);

        // Rotación
        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        // Animación movimiento
        float velocidad = move.magnitude;
        anim.SetFloat("Speed", velocidad);

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

        // Activar animación de muerte
        anim.SetTrigger("Dead");

        // Detener movimiento
        anim.SetFloat("Speed", 0);

        // Desactivar collider para que no flote
        if (col != null)
        {
            col.enabled = false;
        }
    }
}