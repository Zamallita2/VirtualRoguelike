using System.Collections;
using UnityEngine;
using Vuforia;

public class PlacementController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject dungeonRoot;
    public PlaneFinderBehaviour planeFinder;
    public DungeonManager dungeonManager;

    [Header("Escala del dungeon en AR")]
    public float dungeonScale = 0.1f;

    private bool placed = false;

    void Start()
    {
        Debug.Log("[Placement] Start OK");
    }

    public void OnDungeonPlaced(GameObject placedObject)
    {
        if (placed) return;
        placed = true;

        Debug.Log("[Placement] ✅ OnDungeonPlaced llamado");

        if (dungeonRoot != null)
        {
            dungeonRoot.SetActive(true);
            dungeonRoot.transform.localScale = Vector3.one * dungeonScale;
        }

        StartCoroutine(GenerateNextFrame());
    }

    private IEnumerator GenerateNextFrame()
    {
        yield return null;

        if (dungeonManager != null)
        {
            dungeonManager.Regenerate();
            Debug.Log("[Placement] ✅ Regenerate llamado");
        }
        else
        {
            Debug.LogError("[Placement] ❌ DungeonManager es null — asígnalo en el Inspector");
        }

        if (planeFinder != null)
            planeFinder.gameObject.SetActive(false);
    }

    public void ResetPlacement()
    {
        placed = false;
        dungeonRoot.transform.localScale = Vector3.one * dungeonScale;

        if (dungeonRoot != null)
            dungeonRoot.SetActive(false);

        if (planeFinder != null)
            planeFinder.gameObject.SetActive(true);
    }
}