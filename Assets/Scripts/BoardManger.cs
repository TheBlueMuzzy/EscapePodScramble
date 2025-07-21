// BoardManager.cs
// Place this in Assets/Scripts/BoardManager.cs (delete prior versions).
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject tilePrefab;

    [Header("All Strip Definitions")]
    [Tooltip("All 31 unique CompartmentDefinition SOs (any order)")]
    public List<CompartmentDefinition> compartmentDefs;

    public int width;    // inferred from first Compartment prefab
    public int height;   // == compartmentDefs.Count

    private Tile[,] tilesGrid;
    private Color[,] originalColors;
    private Transform artParent;

    void Awake()
    {
        // 1) Dimensions
        height = compartmentDefs.Count;
        if (height == 0)
        {
            Debug.LogError("BoardManager: No compartmentDefs!");
            return;
        }
        var firstComp = compartmentDefs[0].prefab.GetComponent<Compartment>();
        width = firstComp.tileSpots.Length;

        // 2) Build randomized 9+bulk+9+bulk+9+bulk+1 layout
        var sb = compartmentDefs.Where(d => d.type == CompartmentType.StartBulkhead).ToList();
        var l2 = compartmentDefs.Where(d => d.type == CompartmentType.LockedBulkhead2).ToList();
        var l3 = compartmentDefs.Where(d => d.type == CompartmentType.LockedBulkhead3).ToList();
        var eb = compartmentDefs.Where(d => d.type == CompartmentType.EndBulkhead).ToList();
        var nb = compartmentDefs
            .Where(d => d.type != CompartmentType.StartBulkhead
                     && d.type != CompartmentType.LockedBulkhead2
                     && d.type != CompartmentType.LockedBulkhead3
                     && d.type != CompartmentType.EndBulkhead)
            .OrderBy(_ => Random.value)
            .ToList();

        if (sb.Count != 1 || l2.Count != 1 || l3.Count != 1 || eb.Count != 1 || nb.Count != height - 4)
            Debug.LogError("BoardManager: bulkhead counts wrong");

        var sec1 = nb.Skip(0 * 9).Take(9);
        var sec2 = nb.Skip(1 * 9).Take(9);
        var sec3 = nb.Skip(2 * 9).Take(9);

        compartmentDefs = new List<CompartmentDefinition>();
        compartmentDefs.AddRange(sb);
        compartmentDefs.AddRange(sec1);
        compartmentDefs.AddRange(l2);
        compartmentDefs.AddRange(sec2);
        compartmentDefs.AddRange(l3);
        compartmentDefs.AddRange(sec3);
        compartmentDefs.AddRange(eb);

        // 3) Prepare storage & art parent
        tilesGrid = new Tile[width, height];
        originalColors = new Color[width, height];
        artParent = new GameObject("CompartmentArt").transform;
        artParent.parent = transform;
    }

    void Start()
    {
        BuildBoard();
    }

    void BuildBoard()
    {
        for (int row = 0; row < height; row++)
        {
            int defIdx = height - 1 - row;
            var def = compartmentDefs[defIdx];

            // Instantiate strip
            var compGO = Instantiate(
                def.prefab,
                new Vector3(0f, -row * 1f, 1f),
                Quaternion.identity,
                artParent
            );
            compGO.name = $"Compartment_{row}_{def.type}";

            // Swap in the assigned art texture
            var artRend = compGO.GetComponentInChildren<Renderer>();
            if (artRend != null && def.artTexture != null)
            {
                // This creates an instance of the material so only this compartment changes
                artRend.material.mainTexture = def.artTexture;
            }

            var comp = compGO.GetComponent<Compartment>();
            if (comp == null) continue;

            // Spawn interactive tiles as children of compGO
            for (int col = 0; col < width; col++)
            {
                Transform spot = comp.tileSpots[col];
                var tileGO = Instantiate(
                    tilePrefab,
                    spot.position,
                    Quaternion.identity,
                    compGO.transform
                );
                tileGO.name = $"Tile_{row}_{col}";

                var tileComp = tileGO.AddComponent<Tile>();
                tileComp.Row = row;
                tileComp.Col = col;
                tilesGrid[col, row] = tileComp;

                var rend = tileGO.GetComponent<Renderer>();
                originalColors[col, row] = rend.material.color;
                rend.enabled = false;  // hide until preview

                if (col == 0 || col == width - 1)
                {
                    var colld = tileGO.GetComponent<Collider>();
                    if (colld) colld.enabled = false;
                }
            }
        }
    }

    public void ClearHighlights()
    {
        for (int c = 0; c < width; c++)
            for (int r = 0; r < height; r++)
                tilesGrid[c, r].GetComponent<Renderer>().enabled = false;
    }

    public void HighlightRange(int startCol, int startRow, int range)
    {
        ClearHighlights();
        int[,] dirs = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };

        for (int d = 0; d < dirs.GetLength(0); d++)
        {
            int dx = dirs[d, 0], dy = dirs[d, 1];
            for (int step = 1; step <= range; step++)
            {
                int c = startCol + dx * step;
                int r = startRow + dy * step;
                if (c < 1 || c >= width - 1 || r < 0 || r >= height) break;

                var tile = tilesGrid[c, r];
                var rend = tile.GetComponent<Renderer>();
                rend.enabled = true;
                rend.material.color = Color.yellow;
            }
        }
    }

    public Tile GetTileAtPosition(Vector3 pos)
    {
        return tilesGrid.Cast<Tile>()
                        .FirstOrDefault(t => t.transform.position == pos);
    }
}
