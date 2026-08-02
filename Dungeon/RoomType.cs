// Project: TCSS 360 Dungeon Adventure
// File: RoomType.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Dungeon
{
    /// <summary>
    /// Enumeration of room types in the dungeon.
    /// </summary>
    public enum RoomType
    {
        /// <summary>
        /// A normal room with no special significance.
        /// </summary>
        Normal,

        /// <summary>
        /// The entrance room where the hero begins the adventure.
        /// </summary>
        Entrance,

        /// <summary>
        /// The exit room that must be reached to complete the dungeon.
        /// </summary>
        Exit
    }
}
