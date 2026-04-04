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

    private List<ConnectionPoint> openConnections = new List<ConnectionPoint>();
    private List<Room> spawnedRooms = new List<Room>();

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        Room first = Instantiate(startRoom, Vector3.zero, Quaternion.identity);
        spawnedRooms.Add(first);
        openConnections.AddRange(first.connections);
        Debug.Log("Connections iniciales: " + first.connections.Length);

        Debug.Log("Iniciando generación...");

        while (openConnections.Count > 0 && spawnedRooms.Count < maxRooms)
        {
            Debug.Log("Puertas disponibles: " + openConnections.Count);

            ConnectionPoint current = openConnections[0];
            openConnections.RemoveAt(0);

            if (current.isOccupied)
            {
                Debug.Log("Puerta ya ocupada");
                continue;
            }

            Room prefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            Debug.Log("Intentando generar sala: " + prefab.name);

            Room newRoom = Instantiate(prefab);

            ConnectionPoint target = newRoom.GetFreeConnection();

            if (target == null)
            {
                Debug.Log("No hay conexiones libres en la nueva sala");
                Destroy(newRoom.gameObject);
                continue;
            }

            AlignRooms(current, target, newRoom);
            Room parentRoom = current.GetComponentInParent<Room>();

            if (CheckOverlap(newRoom))
            {
                Debug.Log("Overlap detectado ❌");
                Destroy(newRoom.gameObject);
                continue;
            }

            Debug.Log("Sala colocada correctamente ✅");

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

            if (!isBoss || dist > 2f) // boss lejos, shop cualquiera
            {
                candidates.Add(c);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.Log("No hay conexiones válidas para special room 😿");
            return;
        }

        for (int i = 0; i < 10; i++) // 🔥 intenta varias veces
        {
            ConnectionPoint current = candidates[Random.Range(0, candidates.Count)];

            Room newRoom = Instantiate(specialPrefab);
            ConnectionPoint target = newRoom.GetFreeConnection();

            AlignRooms(current, target, newRoom);

            if (CheckOverlap(newRoom))
            {
                Destroy(newRoom.gameObject);
                continue; // intenta otra
            }

            current.isOccupied = true;
            target.isOccupied = true;

            spawnedRooms.Add(newRoom);
            openConnections.Remove(current);

            Debug.Log(isBoss ? "Boss colocado 😈" : "Shop colocada 🛒");
            return;
        }

        Debug.Log("No se pudo colocar special room después de intentos 💀");
    }

    void AlignRooms(ConnectionPoint a, ConnectionPoint b, Room room)
    {

        Quaternion targetRotation = Quaternion.FromToRotation(b.transform.forward, -a.transform.forward) * room.transform.rotation;
        room.transform.rotation = targetRotation;

        Vector3 bOffset = room.transform.rotation * b.transform.localPosition;
        room.transform.position = a.transform.position - bOffset;

        Quaternion rot = Quaternion.FromToRotation(b.transform.forward, -a.transform.forward);
        room.transform.rotation = rot * room.transform.rotation;

        // 👇 esto mata el bug de “de cabeza”
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

            Instantiate(wallPrefab, pos, rot);

            c.isOccupied = true;
        }

        Debug.Log("Todas las conexiones abiertas fueron cerradas uwu 🧱✨");
    }
}