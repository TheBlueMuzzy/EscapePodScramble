// HazardType.cs
// Defines the kinds of tokens you have in the game.
public enum HazardType
{
    Resource,      // Food, Medicine, Tools, Weapons
    Unstable,      // Up/Down, Left/Right, Diagonal
    Debris,        // Blocks movement
    Teleportal,    // Teleport within a section
    Turret,        // Kills adjacent
    Terminal,      // Disables turrets in section
    Creature       // Escaped lab creatures
}
