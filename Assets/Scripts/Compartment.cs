// Compartment.cs
using UnityEngine;

public class Compartment : MonoBehaviour
{
    public CompartmentType type;
    public Transform[] tileSpots;    // assign your 8 child spots here

    [HideInInspector]
    public CompartmentDefinition definition;   // set by BoardManager at runtime
}
