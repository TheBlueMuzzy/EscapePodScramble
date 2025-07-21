// CompartmentDefinition.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Compartment Definition")]
public class CompartmentDefinition : ScriptableObject
{
    public CompartmentType type;         // e.g. Safe, Deadly, ShiftLeft…
    public GameObject prefab;       // your 1×8 strip prefab

    [Tooltip("Indices (0–7) of child tileSpots where tokens may spawn")]
    public int[] tokenSpawnIndices;
}
