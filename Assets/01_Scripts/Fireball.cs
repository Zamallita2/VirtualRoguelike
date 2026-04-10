using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 10f;
    public float damage = 10f;
    public float slowDuration = 2f;
    public float lifeTime = 5f;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    public void Activate()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.TakeDamage(damage);
                player.ApplySlow(slowDuration);
            }
        }
    }
}