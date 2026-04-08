using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public ConnectionPoint[] connections;

    [Header("Spawn")]
    public List<Transform> spawnPoints = new List<Transform>();
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Walls")]
    public GameObject wallPrefab;
    public Transform generatedRoot;

    private List<GameObject> spawnedWalls = new List<GameObject>();
    private List<GameObject> aliveEnemies = new List<GameObject>();

    private bool doorsClosed = false;

    private void Awake()
    {
        connections = GetComponentsInChildren<ConnectionPoint>();

        if (generatedRoot == null)
        {
            GameObject root = new GameObject("Generated");
            root.transform.SetParent(transform);
            generatedRoot = root.transform;
        }
    }

    public ConnectionPoint GetFreeConnection()
    {
        foreach (var c in connections)
        {
            if (!c.isOccupied)
                return c;
        }
        return null;
    }

    // 🧱 Cerrar conexiones
    public void CloseConnections()
    {
        if (doorsClosed) return;
        doorsClosed = true;

        foreach (var c in connections)
        {
            if (c == null) continue;

            Vector3 pos = c.transform.position;
            pos.y = -0.4f;

            Quaternion rot = c.transform.rotation * Quaternion.Euler(0, -90f, 0);

            GameObject wall = Instantiate(wallPrefab, pos, rot, generatedRoot);
            spawnedWalls.Add(wall);
        }
    }

    // 🚪 Abrir conexiones
    public void OpenConnections()
    {
        if (!doorsClosed) return;

        doorsClosed = false;

        foreach (var w in spawnedWalls)
        {
            if (w != null)
                Destroy(w);
        }

        spawnedWalls.Clear();
    }

    // 👾 Registrar enemigo
    public void RegisterEnemy(GameObject enemy)
    {
        if (!aliveEnemies.Contains(enemy))
            aliveEnemies.Add(enemy);
    }

    // 💀 Cuando muere un enemigo
    public void NotifyEnemyDead(GameObject enemy)
    {
        if (enemy != null)
            aliveEnemies.Remove(enemy);

        aliveEnemies.RemoveAll(e => e == null);

        if (doorsClosed && aliveEnemies.Count == 0)
        {
            OpenConnections();
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0) return null;
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public GameObject GetRandomEnemy()
    {
        if (enemyPrefabs.Count == 0) return null;
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    }
}