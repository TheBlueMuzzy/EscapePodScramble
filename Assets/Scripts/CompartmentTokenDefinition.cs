using UnityEngine;

public enum TokenType
{
    LabCreature,
    Turret,
    ResourceA,
    ResourceB,
    // … add as needed …
}

[System.Serializable]
public struct TokenEntry
{
    public TokenType tokenType;    // Which kind of token prefab to spawn
    public int[] slotIndices;      // Interior slots 0–5 (0 = left, 5 = right)
    public int countPerSlot;       // How many tokens in each of those slots
}

[CreateAssetMenu(menuName = "EscapePod/CompartmentTokenDefinition")]
public class CompartmentTokenDefinition : ScriptableObject
{
    public CompartmentType compartmentType;  // Which Compartment this applies to
    public TokenEntry[] tokens;             // The tokens to spawn in that Compartment
}
