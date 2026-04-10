using UnityEngine;
using System.Collections;

public class DarkMage : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Animator anim;
    public Transform firePoint;
    public GameObject fireballPrefab;
    private GameObject currentCharge;

    [Header("Stats")]
    public float detectRange = 8f;
    public float minDistance = 4f;
    public float speed = 2f;
    public float castTime = 5f;
    public float timeBetweenShots = 1.5f;
    public float variableRa=0.9f;
    [Header("Health")]
    public float maxHealth = 50f;
    public float currentHealth;

    private bool isCasting = false;
    void Start()
    {
        GameObject plagerGO = GameObject.FindGameObjectWithTag("Player");
        player=plagerGO.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectRange)
        {
            RotateTowardsPlayer();

            // 🔥 SI ESTÁ MUY CERCA → cancelar casteo y huir
            if (dist < minDistance)
            {
                StopAllCoroutines();

                if (currentCharge != null)
                {
                    Destroy(currentCharge);
                    currentCharge = null;
                }

                isCasting = false;
                anim.SetBool("IsCasting", false);

                Vector3 dir = transform.position - player.position;
                dir.y = 0f; // 💥 eliminar movimiento vertical
                dir = dir.normalized;

                transform.position += dir * speed * Time.deltaTime;

                anim.SetFloat("Speed", 1f);
            }
            else
            {
                // 🔥 distancia segura → atacar
                if (!isCasting)
                {
                    StartCoroutine(CastSpell());
                }

                anim.SetFloat("Speed", 0f);
            }
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;

        if (lookDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
    }

    IEnumerator CastSpell()
    {
        isCasting = true;
        anim.SetBool("IsCasting", true);

        currentCharge = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity, firePoint);
        GameObject charge = currentCharge;

        Collider col = charge.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        charge.transform.localScale = Vector3.one * 0.1f;

        float time = 0f;

        while (time < castTime)
        {
            if (currentCharge == null)
            {
                isCasting = false;
                yield break;
            }

            float dist = Vector3.Distance(transform.position, player.position);

            // 🔥 si se acerca demasiado → cancelar
            if (dist < minDistance)
            {
                Destroy(charge);
                anim.SetBool("IsCasting", false);
                isCasting = false;
                yield break;
            }

            // cancelar si se aleja
            if (dist > detectRange)
            {
                Destroy(charge);
                anim.SetBool("IsCasting", false);
                isCasting = false;
                yield break;
            }

            time += Time.deltaTime;

            // 🔥 NORMALIZADO (0 → 1)
            float t = time / castTime;

            // 🔥 CRECIMIENTO PRO (empieza lento, termina fuerte)
            float scale = Mathf.Lerp(0.1f, 0.6f, t * t);

            // 🔥 EFECTO PULSO (energía viva)
            float pulse = Mathf.Sin(Time.time * 10f) * 0.05f;

            charge.transform.localScale = Vector3.one * (scale + pulse);

            // 🔥 ROTACIÓN PRO (en 3 ejes)
            charge.transform.Rotate(
                120f * Time.deltaTime,
                240f * Time.deltaTime,
                80f * Time.deltaTime
            );

            yield return null;
        }

        // 🔥 activar collider ahora
        if (col != null)
            col.enabled = true;

        // 🔥 disparar
        charge.transform.parent = null;

        Vector3 targetPos = player.position + Vector3.up * variableRa;
        Vector3 dir = (targetPos - charge.transform.position).normalized;

        Fireball fb = charge.GetComponent<Fireball>();
        if (fb != null)
        {
            fb.SetDirection(dir);
            fb.Activate(); // 🔥 IMPORTANTE
        }

        anim.SetBool("IsCasting", false);

        yield return new WaitForSeconds(timeBetweenShots);

        isCasting = false;
        currentCharge = null;
    }

    // 🔥 DEBUG VISUAL (opcional)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{name} recibe {damage} de daño. Vida: {currentHealth}");
        if (currentHealth <= 0f) 
            Die();
    }
    private void Die()
    {
        var notifier = GetComponent<RoomEnemyNotifier>();
        if (notifier != null)
            notifier.NotifyDeath();

        // 🔥 DROPEAR LOOT
        EnemyLootDrop loot = GetComponent<EnemyLootDrop>();
        if (loot != null)
        {
            loot.DropLoot();
        }

        
        Destroy(gameObject);
    }
}