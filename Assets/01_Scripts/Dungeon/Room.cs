using UnityEngine;

public class Room : MonoBehaviour
{
    public ConnectionPoint[] connections;

    private void Awake()
    {
        connections = GetComponentsInChildren<ConnectionPoint>();
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
}