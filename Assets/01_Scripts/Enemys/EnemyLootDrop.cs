using UnityEngine;

public class EnemyLootDrop : MonoBehaviour
{
    [Header("PowerUps")]
    [Range(0f, 1f)]
    public float powerUpDropChance = 0.4f; // 40% por defecto

    public GameObject[] powerUpPrefabs; // lista de powerups posibles

    [Header("Monedas")]
    public GameObject coinPrefab;
    public int minCoins = 1;
    public int maxCoins = 5;

    [Header("Fuerza de dispersión")]
    public float dropForce = 3f;
    public float upwardForce = 2f;

    private bool hasDropped = false; // evitar duplicados

    // 👉 LLAMA A ESTE MÉTODO CUANDO EL ENEMIGO MUERA
    public void DropLoot()
    {
        if (hasDropped) return;
        hasDropped = true;

        Vector3 spawnPos = transform.position;

        // 💰 MONEDAS (SIEMPRE DROPEA)
        int coinAmount = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < coinAmount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            ApplyForce(coin);
        }

        // 🎁 POWER UP (PROBABILIDAD)
        float roll = Random.value;

        if (roll <= powerUpDropChance && powerUpPrefabs.Length > 0)
        {
            int index = Random.Range(0, powerUpPrefabs.Length);

            GameObject powerUp = Instantiate(powerUpPrefabs[index], spawnPos, Quaternion.identity);

            ApplyForce(powerUp);
        }
    }

    void ApplyForce(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                0,
                Random.Range(-1f, 1f)
            ).normalized;

            Vector3 force = (randomDir * dropForce) + (Vector3.up * upwardForce);

            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}