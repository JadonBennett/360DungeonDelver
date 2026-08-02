// Project: TCSS 360 Dungeon Adventure
// File: RoomAnchor.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Dungeon
{
    /// <summary>
    /// Enumeration of the five fixed anchor positions within a room
    /// where items, monsters, or other content can be placed.
    /// </summary>
    public enum RoomAnchor
    {
        /// <summary>
        /// The center position of the room.
        /// </summary>
        Center,

        /// <summary>
        /// The north wall position.
        /// </summary>
        North,

        /// <summary>
        /// The south wall position.
        /// </summary>
        South,

        /// <summary>
        /// The east wall position.
        /// </summary>
        East,

        /// <summary>
        /// The west wall position.
        /// </summary>
        West
    }
}
