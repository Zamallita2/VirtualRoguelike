using UnityEngine;

public class RoomEnemyNotifier : MonoBehaviour
{
    private Room room;
    private bool notified = false;

    public void Init(Room r)
    {
        room = r;
    }

    private void OnDestroy()
    {
        if (notified) return;
        notified = true;

        if (room != null)
            room.NotifyEnemyDead(gameObject);
    }
    public void NotifyDeath()
    {
        if (notified) return;
        notified = true;

        if (room != null)
            room.NotifyEnemyDead(gameObject);
    }
}