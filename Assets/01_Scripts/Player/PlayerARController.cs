using UnityEngine;
using System.Collections;

public class PlayerARController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 10f;

    public Rigidbody rb;
    public Animator anim;

    public FixedJoystick joystick;

    public Collider attackCollider;
    public float attackDuration = 0.4f;

    bool isAttacking = false;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (anim == null)
            anim = GetComponent<Animator>();

        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        Vector3 move = new Vector3(h, 0, v);

        Vector3 velocity = new Vector3(
            move.x * moveSpeed,
            rb.linearVelocity.y,
            move.z * moveSpeed
        );

        rb.linearVelocity = velocity;

        if (move.magnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                rotationSpeed * Time.deltaTime
            );
        }

        anim.SetFloat("Speed", move.magnitude);
    }

    public void Attack()
    {
        if (!isAttacking)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        anim.SetTrigger("Attack");

        if (attackCollider != null)
            attackCollider.enabled = true;

        yield return new WaitForSeconds(attackDuration);

        if (attackCollider != null)
            attackCollider.enabled = false;

        isAttacking = false;
    }
}