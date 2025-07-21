// CrewSpawner.cs
// Place this in Assets/Scripts.
// Dynamically finds all tiles in row 0 (Start Bulkhead) after the board is generated,
// then spawns crew members at those positions at runtime.

using System.Linq;
using UnityEngine;

public class CrewSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public CrewMember crewPrefab;      // Assign your CrewMember prefab here
    public int numberToSpawn = 1;      // How many crew members to spawn

    private void Start()
    {
        // Grab the BoardManager so we know the true bottom row index
        var boardMgr = UnityEngine.Object.FindFirstObjectByType<BoardManager>();
        int bottomRow = boardMgr.height - 1;

        // Find all Tile components in the scene
        Tile[] allTiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);

        // Filter for those on the true bottom (start bulkhead), excluding hull columns 0 and width-1
        var startRowTiles = allTiles
            .Where(t => t.Row == bottomRow && t.Col > 0 && t.Col < boardMgr.width - 1)
            .ToList();


        if (startRowTiles.Count == 0)
        {
            Debug.LogWarning("CrewSpawner: No tiles found at row 0. Make sure BoardManager runs before this.");
            return;
        }

        // Clamp the spawn count
        int spawnCount = Mathf.Min(numberToSpawn, startRowTiles.Count);
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = startRowTiles[i].transform.position;
            Instantiate(crewPrefab, spawnPos, Quaternion.identity);
        }
    }
}
