using UnityEngine;

public enum PowerUpType
{
    Damage,
    Speed,
    Coins,
    MaxHealth,
    Heal
}

[RequireComponent(typeof(Collider))]
public class PowerUpPickup : MonoBehaviour
{
    [Header("Tipo de PowerUp")]
    public PowerUpType type;

    [Header("Rotación")]
    public float rotationSpeed = 100f; // velocidad de giro

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    void Update()
    {
        // 🔄 Rotar sobre su propio eje (Y)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        switch (type)
        {
            case PowerUpType.Damage:
                player.AddDamage(10);
                Debug.Log("PowerUp: +10 daño");
                break;

            case PowerUpType.Speed:
                player.AddSpeed(1f);
                Debug.Log("PowerUp: +1 velocidad");
                break;

            case PowerUpType.Coins:
                player.AddCoins(5);
                Debug.Log("PowerUp: +5 coins");
                break;

            case PowerUpType.MaxHealth:
                player.IncreaseMaxHealth(50f);
                Debug.Log("PowerUp: +50 vida máxima");
                break;

            case PowerUpType.Heal:
                player.Heal(50f);
                Debug.Log("PowerUp: +50 vida");
                break;
        }

        Destroy(gameObject);
    }
}