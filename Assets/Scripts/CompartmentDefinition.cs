// CompartmentDefinition.cs
// Place this in Assets/Scripts (delete any old copies).

using UnityEngine;

[CreateAssetMenu(menuName = "Game/Compartment Definition")]
public class CompartmentDefinition : ScriptableObject
{
    public CompartmentType type;        // e.g. Safe, Deadly, ShiftLeft, etc.
    public GameObject prefab;      // the 1×8 strip prefab
    [Tooltip("The PNG (imported as Texture) to display on this compartment")]
    public Texture2D artTexture;  // assign your 2000×250 PNG here
}
