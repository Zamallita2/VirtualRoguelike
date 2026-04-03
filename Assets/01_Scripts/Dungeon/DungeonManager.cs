using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public Room startRoom;
    public Room bossRoomPrefab;
    public Room shopRoomPrefab;
    public List<Room> roomPrefabs;
    public GameObject wallPrefab;

    public int maxRooms = 10;
    public int waveNum=0;

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
            Regenerate();
            waveNum++;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Regenerando dungeon... owo 🔄");
            Regenerate();
            waveNum=0;
        }
    }

    public void Regenerate()
    {
        StartCoroutine(RegenerateRoutine());
    }

    IEnumerator RegenerateRoutine()
    {
        ClearGenerated();
        yield return null; // deja que Destroy haga efecto en el frame siguiente
        Generate();
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

    void Generate()
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

        AttachSpecialRoom(bossRoomPrefab, true);
        AttachSpecialRoom(shopRoomPrefab, false);
        CloseOpenConnections();
    }

    void AttachSpecialRoom(Room specialPrefab, bool isBoss)
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
            Debug.Log("No hay conexiones válidas para special room 😿");
            Regenerate();
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            ConnectionPoint current = candidates[Random.Range(0, candidates.Count)];

            Room newRoom = Instantiate(specialPrefab, Vector3.zero, Quaternion.identity, generatedRoot);
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
            openConnections.Remove(current);

            return;
        }
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