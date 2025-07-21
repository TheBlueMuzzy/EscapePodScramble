// HazardDefinition.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Hazard Definition")]
public class HazardDefinition : ScriptableObject
{
    public HazardType type;         // Which hazard this is
    public GameObject prefab;       // The prefab to instantiate
    public int count;        // How many to place per game
    public bool faceDownInDark = true;
    // (for Dark compartments, e.g. only flip face-up on reveal)
}
