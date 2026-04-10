using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float speed = 5f;
    private Transform player;

    void Update()
    {
        // Buscar al player si no existe
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
                player = obj.transform;

            return;
        }

        // Dirección hacia el player (sin afectar Y)
        Vector3 targetPos = player.position;
        targetPos.y = transform.position.y;

        // Moverse hacia el player
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }
}