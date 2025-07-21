// Compartment.cs
// Place this in Assets/Scripts—attach to your Compartment prefab root.

using UnityEngine;

public class Compartment : MonoBehaviour
{
    [Tooltip("What kind of compartment this is")]
    public CompartmentType type;

    [Tooltip("Local transforms for each of the 8 tile spots, left (index 0) → right (index 7)")]
    public Transform[] tileSpots;  // Drag-and-drop your 8 child empty GameObjects here
}
