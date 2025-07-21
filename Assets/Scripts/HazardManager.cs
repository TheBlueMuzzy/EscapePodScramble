// HazardManager.cs
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
    [Header("All Token Definitions (type + prefab + count)")]
    public List<HazardDefinition> hazardDefinitions;

    void Start() => SpawnAllHazards();

    void SpawnAllHazards()
    {
        // 1) Gather all allowed spawn spots, per‐compartment
        var spawnSpots = new List<Transform>();
        foreach (var comp in Object.FindObjectsByType<Compartment>(FindObjectsSortMode.None))
        {
            // take the array from the SO; if empty, default to indices 1..(tileSpots.Length-2)
            var indices = comp.definition.tokenSpawnIndices;
            if (indices == null || indices.Length == 0)
            {
                indices = Enumerable
                    .Range(1, comp.tileSpots.Length - 2)  // e.g. 1..6 for width=8
                    .ToArray();
                Debug.Log($"[HazardManager] '{comp.definition.name}' has no tokenSpawnIndices – defaulting to {indices.Length} spots");
            }

            foreach (int idx in indices)
            {
                if (idx >= 0 && idx < comp.tileSpots.Length)
                    spawnSpots.Add(comp.tileSpots[idx]);
                else
                    Debug.LogError($"[HazardManager] Bad index {idx} on {comp.definition.name}");
            }
        }

        // 2) Build & shuffle your token pool
        var tokenPool = new List<GameObject>();
        foreach (var def in hazardDefinitions)
            for (int i = 0; i < def.count; i++)
                tokenPool.Add(def.prefab);
        tokenPool = tokenPool.OrderBy(_ => Random.value).ToList();

        Debug.Log($"[HazardManager] spawnSpots={spawnSpots.Count}, tokenPool={tokenPool.Count}");

        // 3) Deal tokens one‐to‐one
        int N = Mathf.Min(spawnSpots.Count, tokenPool.Count);
        for (int i = 0; i < N; i++)
        {
            var spot = spawnSpots[i];
            var prefab = tokenPool[i];
            Debug.Log($"[HazardManager] Spawning '{prefab.name}' at spot '{spot.parent.name}/{spot.name}'");

            var go = Instantiate(prefab, spot.position, Quaternion.identity, spot);

            // Face‐down for dark compartments?
            var hzDef = hazardDefinitions.First(h => h.prefab == prefab);
            var comp = spot.GetComponentInParent<Compartment>();
            if (hzDef.faceDownInDark && comp.definition.type == CompartmentType.Dark)
                go.transform.Rotate(0, 0, 180f);
        }
    }
}
