using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStatsData
    {
        public float maxHealth;
        public float currentHealth;
        public float speed;
        public int damage;
    }
    public Room startRoom;
    public Room lastRoomPrefab;
    public Room bossRoomPrefab;
    public Room shopRoomPrefab;
    public List<Room> roomPrefabs;
    public GameObject wallPrefab;

    public int maxRooms = 10;
    public int waveNum=1;

    [Header("Generated content")]
    public Transform generatedRoot; // aquí cuelga todo lo generado

    private List<ConnectionPoint> openConnections = new List<ConnectionPoint>();
    private List<Room> spawnedRooms = new List<Room>();

    void Start()
    {
        Regenerate();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Regenerando dungeon... owo 🔄");
            RegenerateRoom(true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Regenerando dungeon... owo 🔄");
            RegenerateRoom(false);
        }
    }
    public void RegenerateRoom(bool increaseWave)
    {
        Debug.Log("Regenerando dungeon... owo 🔄");

        // 1. Obtener player actual
        GameObject oldPlayerObj = GameObject.FindGameObjectWithTag("Player");

        PlayerStatsData savedStats = null;

        if (oldPlayerObj != null)
        {
            var oldPlayer = oldPlayerObj.GetComponent<PlayerMovement>();

            if (oldPlayer != null)
            {
                savedStats = new PlayerStatsData();

                savedStats.maxHealth = oldPlayer.maxHealth;
                savedStats.currentHealth = oldPlayer.currentHealth;
                savedStats.speed = oldPlayer.speed;

                var sword = oldPlayer.swordCollider.GetComponent<Sword>();
                if (sword != null)
                    savedStats.damage = sword.damage;
            }
        }

        // 2. Manejo wave
        if (increaseWave)
            waveNum++;
        else
            waveNum = 1;

        // 3. Regenerar dungeon
        Regenerate();

        // 4. Buscar nuevo player (IMPORTANTE: esperar 1 frame)
        if (savedStats != null)
            StartCoroutine(ApplyStatsNextFrame(savedStats));
    }
    private IEnumerator ApplyStatsNextFrame(PlayerStatsData stats)
    {
        yield return null; // esperar 1 frame

        GameObject newPlayerObj = GameObject.FindGameObjectWithTag("Player");

        if (newPlayerObj == null || stats == null)
        {
            Debug.LogWarning("No hay player nuevo o no había stats guardados unu");
            yield break;
        }

        var newPlayer = newPlayerObj.GetComponent<PlayerMovement>();

        if (newPlayer == null)
        {
            Debug.LogWarning("El nuevo player no tiene script unu");
            yield break;
        }

        // Aplicar stats
        newPlayer.maxHealth = stats.maxHealth;
        newPlayer.speed = stats.speed;
        newPlayer.currentHealth = stats.currentHealth;

        var sword = newPlayer.swordCollider.GetComponent<Sword>();
        if (sword != null)
            sword.damage = stats.damage;
    }

    public void Regenerate()
    {
        StartCoroutine(RegenerateRoutine());
    }

    IEnumerator RegenerateRoutine()
    {
        const int maxAttempts = 200;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            ClearGenerated();
            yield return null;

            bool ok = Generate();
            if (ok)
                yield break;

            Debug.LogWarning($"Falló la generación, reintentando... ({attempt + 1}/{maxAttempts})");
            yield return null;
        }

        Debug.LogError("No se pudo generar una dungeon válida.");
    }

    void ClearGenerated()
    {
        openConnections.Clear();
        spawnedRooms.Clear();

        if (generatedRoot == null) return;

        for (int i = generatedRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(generatedRoot.GetChild(i).gameObject);
        }
    }

    bool Generate()
    {
        Room first = Instantiate(startRoom, Vector3.zero, Quaternion.identity, generatedRoot);
        spawnedRooms.Add(first);
        openConnections.AddRange(first.connections);

        Debug.Log("Iniciando generación...");

        while (openConnections.Count > 0 && spawnedRooms.Count < maxRooms)
        {
            ConnectionPoint current = openConnections[0];
            openConnections.RemoveAt(0);

            if (current.isOccupied)
                continue;

            Room prefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            Room newRoom = Instantiate(prefab, Vector3.zero, Quaternion.identity, generatedRoot);

            ConnectionPoint target = newRoom.GetFreeConnection();
            if (target == null)
            {
                Destroy(newRoom.gameObject);
                continue;
            }

            AlignRooms(current, target, newRoom);

            if (CheckOverlap(newRoom))
            {
                Destroy(newRoom.gameObject);
                continue;
            }

            current.isOccupied = true;
            target.isOccupied = true;

            spawnedRooms.Add(newRoom);

            foreach (var c in newRoom.connections)
            {
                if (!c.isOccupied)
                    openConnections.Add(c);
            }
        }

        bool isBossWave = waveNum > 0 && waveNum % 5 == 0;
        Room special = isBossWave ? bossRoomPrefab : lastRoomPrefab;

        if (!AttachSpecialRoom(special, isBossWave))
            return false;

        if (!AttachSpecialRoom(shopRoomPrefab, false))
            return false;

        CloseOpenConnections();
        return true;
    }

    bool AttachSpecialRoom(Room specialPrefab, bool isBoss)
    {
        List<ConnectionPoint> candidates = new List<ConnectionPoint>();

        foreach (var c in openConnections)
        {
            float dist = Vector3.Distance(Vector3.zero, c.transform.position);

            if (!isBoss || dist > 2f)
                candidates.Add(c);
        }

        if (candidates.Count == 0)
        {
            Debug.Log("❌ No hay conexiones válidas para special room");
            return false;
        }

        // 🐱 Mezclar lista (Fisher-Yates shuffle)
        for (int i = 0; i < candidates.Count; i++)
        {
            int rnd = Random.Range(i, candidates.Count);
            var temp = candidates[i];
            candidates[i] = candidates[rnd];
            candidates[rnd] = temp;
        }

        int attemptsPerConnection = isBoss ? 3 : 1;

        foreach (var current in candidates)
        {
            for (int i = 0; i < attemptsPerConnection; i++)
            {
                Room newRoom = Instantiate(specialPrefab, Vector3.zero, Quaternion.identity, generatedRoot);
                ConnectionPoint target = newRoom.GetFreeConnection();

                if (target == null)
                {
                    Debug.Log("❌ Fallo: target null");
                    Destroy(newRoom.gameObject);
                    continue;
                }

                AlignRooms(current, target, newRoom);

                if (CheckOverlap(newRoom))
                {
                    Debug.Log("❌ Fallo: overlap en conexión " + current.name);
                    Destroy(newRoom.gameObject);
                    continue;
                }

                Debug.Log("✅ Special room colocada en " + current.name);

                current.isOccupied = true;
                target.isOccupied = true;

                spawnedRooms.Add(newRoom);
                openConnections.Remove(current);

                return true;
            }
        }

        Debug.Log("❌ Fallo total: ninguna conexión funcionó");
        return false;
    }

    void AlignRooms(ConnectionPoint a, ConnectionPoint b, Room room)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(b.transform.forward, -a.transform.forward) * room.transform.rotation;
        room.transform.rotation = targetRotation;

        Vector3 bOffset = room.transform.rotation * b.transform.localPosition;
        room.transform.position = a.transform.position - bOffset;

        Quaternion rot = Quaternion.FromToRotation(b.transform.forward, -a.transform.forward);
        room.transform.rotation = rot * room.transform.rotation;

        room.transform.rotation = Quaternion.Euler(0, room.transform.eulerAngles.y, 0);
    }

    bool CheckOverlap(Room room)
    {
        foreach (var r in spawnedRooms)
        {
            float dist = Vector3.Distance(r.transform.position, room.transform.position);
            if (dist < 0.9f)
                return true;
        }
        return false;
    }

    void CloseOpenConnections()
    {
        foreach (var c in openConnections)
        {
            if (c.isOccupied) continue;

            Vector3 pos = c.transform.position;
            pos.y = -0.4f;

            Quaternion rot = c.transform.rotation * Quaternion.Euler(0, -90f, 0);

            Instantiate(wallPrefab, pos, rot, generatedRoot);
            c.isOccupied = true;
        }
    }
}