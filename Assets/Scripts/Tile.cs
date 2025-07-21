// Tile.cs
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int Row { get; set; }
    public int Col { get; set; }

    // BoardManager sets this on instantiation
    public CompartmentType Compartment { get; set; }
}
