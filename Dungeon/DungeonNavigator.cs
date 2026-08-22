// Project: TCSS 360 Dungeon Adventure
// File: DungeonNavigator.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System.Collections.Generic;

namespace DungeonDelver.Dungeon
{
    /// <summary>
    /// Handles movement and navigation logic within a dungeon.
    /// Manages the current room position and validates movement attempts.
    /// </summary>
    public class DungeonNavigator
    {
        /// <summary>
        /// The room the navigator is currently in.
        /// </summary>
        private Room myCurrentRoom;

        /// <summary>
        /// The dungeon map being navigated.
        /// </summary>
        private readonly DungeonMap myDungeon;

        /// <summary>
        /// Tracks which rooms the player has visited during gameplay.
        /// </summary>
        private readonly HashSet<Room> myVisitedRooms;

        /// <summary>
        /// Initializes a new DungeonNavigator at the dungeon entrance.
        /// </summary>
        /// <param name="theDungeon">The dungeon to navigate.</param>
        public DungeonNavigator(DungeonMap theDungeon)
        {
            myDungeon = theDungeon;
            myCurrentRoom = theDungeon.Entrance;
            myVisitedRooms = new HashSet<Room>();
            myVisitedRooms.Add(myCurrentRoom); // Mark entrance as visited
        }

        /// <summary>
        /// Gets the current room the navigator is in.
        /// </summary>
        public Room CurrentRoom => myCurrentRoom;

        /// <summary>
        /// Gets the set of rooms the player has visited.
        /// </summary>
        public IReadOnlyCollection<Room> VisitedRooms => myVisitedRooms;

        /// <summary>
        /// Attempts to move in the specified direction.
        /// </summary>
        /// <param name="theDirection">The direction to move.</param>
        /// <returns>True if movement was successful, false if blocked by wall or no room exists.</returns>
        public bool TryMove(Direction theDirection)
        {
            // Check if there's a wall blocking movement
            if (IsWallBlocking(theDirection))
            {
                return false;
            }

            // Get the neighbor room in that direction
            Room nextRoom = GetNeighborRoom(theDirection);

            if (nextRoom == null)
            {
                return false;
            }

            // Move to the new room
            myCurrentRoom = nextRoom;
            myVisitedRooms.Add(myCurrentRoom); // Mark as visited
            return true;
        }

        /// <summary>
        /// Teleports the navigator to a specific room.
        /// Used for testing and special game events.
        /// </summary>
        /// <param name="theRoom">The room to teleport to.</param>
        public void TeleportTo(Room theRoom)
        {
            if (theRoom != null)
            {
                myCurrentRoom = theRoom;
                myVisitedRooms.Add(myCurrentRoom); // Mark as visited
            }
        }

        /// <summary>
        /// Checks if there's a wall blocking movement in the given direction.
        /// </summary>
        /// <param name="theDirection">The direction to check.</param>
        /// <returns>True if a wall is blocking.</returns>
        private bool IsWallBlocking(Direction theDirection)
        {
            return myCurrentRoom.GetWall(theDirection);
        }

        /// <summary>
        /// Gets the neighbor room in the specified direction.
        /// </summary>
        /// <param name="theDirection">The direction to look.</param>
        /// <returns>The neighbor room, or null if none exists.</returns>
        private Room GetNeighborRoom(Direction theDirection)
        {
            return myCurrentRoom.GetNeighbor(theDirection);
        }
    }
}
