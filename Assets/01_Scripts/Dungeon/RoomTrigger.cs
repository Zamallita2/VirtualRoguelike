using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private Room room;
    [SerializeField] private int enemiesToSpawn = -1;
    [SerializeField] private float activationChance = 0.5f;

    private bool activated = false;
    private Collider[] triggers;

    private void Awake()
    {
        if (room == null)
            room = GetComponentInParent<Room>();

        triggers = GetComponents<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        DisableTriggers();

        // 🎲 50%
        if (Random.value > activationChance)
            return;

        room.CloseConnections();
        SpawnEnemies();
    }

    private void DisableTriggers()
    {
        foreach (var col in triggers)
            col.enabled = false;
    }

    private void SpawnEnemies()
    {
        if (room == null) return;

        int count = enemiesToSpawn;

        if (count < 0)
            count = room.spawnPoints.Count;

        count = Mathf.Clamp(count, 0, room.spawnPoints.Count);

        List<int> used = new List<int>();

        for (int i = 0; i < count; i++)
        {
            int index;

            do
            {
                index = Random.Range(0, room.spawnPoints.Count);
            }
            while (used.Contains(index));

            used.Add(index);

            Transform point = room.spawnPoints[index];
            GameObject prefab = room.GetRandomEnemy();

            if (point == null || prefab == null) continue;

            GameObject enemy = Instantiate(prefab, point.position, point.rotation);

            room.RegisterEnemy(enemy);

            var notifier = enemy.AddComponent<RoomEnemyNotifier>();
            notifier.Init(room);
        }
    }
}