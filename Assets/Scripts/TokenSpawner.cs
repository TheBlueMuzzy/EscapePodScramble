using System.Linq;
using UnityEngine;

public class TokenSpawner : MonoBehaviour
{
    [Header("Definitions & Prefabs")]
    public CompartmentTokenDefinition[] definitions;  // Assign all your definition assets here
    public GameObject[] tokenPrefabs;                 // Parallel array matching TokenType enum order

    [Header("Spawn Settings")]
    public float zOffset = -0.1f;     // Lift tokens off the board
    public float stackYOffset = 0.15f; // Height between stacked tokens

    void Start()
    {
        // Build quick lookup by compartment type
        var defLookup = definitions.ToDictionary(d => d.compartmentType);

        // Find all tiles and spawn accordingly
        Tile[] allTiles = Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (var tile in allTiles)
        {
            // Skip hull columns entirely—no tokens there
            int col = tile.Col;
            if (col == 0 || col == BoardManager.Instance.width - 1)
                continue;

            var tileComp = tile;                     // your Tile holds Row & Col
            var compType = BoardManager.Instance.layout[tileComp.Row];

            // If no definition, skip
            if (!defLookup.TryGetValue(compType, out var def))
                continue;

            // For each entry in that def
            foreach (var entry in def.tokens)
            {
                // Determine the prefab for this TokenType
                var prefab = tokenPrefabs[(int)entry.tokenType];

                // For each slot index
                foreach (int slot in entry.slotIndices)
                {
                    // Reverse slot if you originally numbered right-to-left:
                    int realSlot = slot;
                    // If your definitions used reversed numbering, invert here: 
                    // realSlot = (5 - slot);

                    // Compute world position: reuse tile position, offset in X if needed
                    // But since each tile is a distinct GameObject, spawn at the tile’s position
                    Vector3 basePos = tile.transform.position;
                    Vector3 spawnBase = new Vector3(basePos.x, basePos.y, basePos.z + zOffset);

                    // Spawn countPerSlot stacked
                    for (int i = 0; i < entry.countPerSlot; i++)
                    {
                        Vector3 finalPos = spawnBase + Vector3.up * (i * stackYOffset);
                        Instantiate(prefab, finalPos, Quaternion.identity, tile.transform);
                    }
                }
            }
        }
    }
}
