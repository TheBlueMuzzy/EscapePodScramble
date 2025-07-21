// HazardManager.cs
// Place this in Assets/Scripts and remove any other HazardManager definitions.
// Spawns tokens based on your HazardDefinition ScriptableObjects.

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
    [Header("All Hazard Definitions")]
    public List<HazardDefinition> hazardDefinitions;   // Populate in Inspector

    private BoardManager boardMgr;

    void Awake()
    {
        boardMgr = UnityEngine.Object.FindFirstObjectByType<BoardManager>();
        if (boardMgr == null)
            Debug.LogError("HazardManager: No BoardManager found in scene!");
    }

    void Start()
    {
        SpawnAllHazards();
    }

    void SpawnAllHazards()
    {
        // Find every Tile in the scene
        Tile[] allTiles = UnityEngine.Object.FindObjectsByType<Tile>(
            FindObjectsSortMode.None
        );

        foreach (var def in hazardDefinitions)
        {
            // Shuffle eligible tiles and take 'count' of them
            var candidates = allTiles
                .Where(t => IsEligibleFor(def.type, t))
                .OrderBy(_ => Random.value)
                .Take(def.count);

            foreach (var tile in candidates)
            {
                var go = Instantiate(
                    def.prefab,
                    tile.transform.position,
                    Quaternion.identity,
                    tile.transform
                );

                // Face-down in dark compartments?
                if (def.faceDownInDark && tile.Compartment == CompartmentType.Dark)
                    go.transform.rotation = Quaternion.Euler(0, 0, 180f);
            }
        }
    }

    private bool IsEligibleFor(HazardType type, Tile tile)
    {
        int c = tile.Col;
        int r = tile.Row;
        int w = boardMgr.width;
        int h = boardMgr.height;

        // 1) Never on side hulls
        if (c == 0 || c == w - 1) return false;

        // 2) Never on any bulkhead rows
        if (r == 0 || r == h - 1 || r == 10 || r == 20) return false;

        // 3) Per-type rules
        switch (type)
        {
            case HazardType.Resource:
            case HazardType.Debris:
                return true;

            case HazardType.Unstable:
                return true;

            case HazardType.Teleportal:
            case HazardType.Turret:
            case HazardType.Terminal:
            case HazardType.Creature:
                return tile.Compartment != CompartmentType.Dark;

            default:
                return false;
        }
    }
}
