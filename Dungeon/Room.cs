// Project: TCSS 360 Dungeon Adventure
// File: Room.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System.Collections.Generic;

namespace DungeonDelver.Dungeon
{
    /// <summary>
    /// Represents a single room in the dungeon grid, including its position,
    /// type, walls, visited status, and connections to neighboring rooms.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// The X coordinate of this room in the dungeon grid.
        /// </summary>
        private readonly int myX;

        /// <summary>
        /// The Y coordinate of this room in the dungeon grid.
        /// </summary>
        private readonly int myY;

        /// <summary>
        /// The classification of this room (Normal, Entrance, or Exit).
        /// </summary>
        private RoomType myType;

        /// <summary>
        /// True if this room has been visited by the maze generation algorithm.
        /// </summary>
        private bool myVisited;

        /// <summary>
        /// Stores whether this room has a wall in each direction.
        /// </summary>
        private readonly Dictionary<Direction, bool> myWalls;

        /// <summary>
        /// Stores the neighboring room in each direction, or null if none exists.
        /// </summary>
        private readonly Dictionary<Direction, Room> myNeighbors;

        /// <summary>
        /// Initializes a new Room at the specified grid coordinates.
        /// All walls start closed, type is Normal, and visited is false.
        /// </summary>
        /// <param name="theX">The X coordinate in the dungeon grid.</param>
        /// <param name="theY">The Y coordinate in the dungeon grid.</param>
        public Room(int theX, int theY)
        {
            myX = theX;
            myY = theY;
            myType = RoomType.Normal;
            myVisited = false;

            // Initialize all walls as closed
            myWalls = new Dictionary<Direction, bool>
            {
                { Direction.North, true },
                { Direction.South, true },
                { Direction.East, true },
                { Direction.West, true }
            };

            // Initialize all neighbors as null
            myNeighbors = new Dictionary<Direction, Room>
            {
                { Direction.North, null },
                { Direction.South, null },
                { Direction.East, null },
                { Direction.West, null }
            };
        }

        /// <summary>
        /// The X coordinate of this room in the dungeon grid.
        /// </summary>
        public int X => myX;

        /// <summary>
        /// The Y coordinate of this room in the dungeon grid.
        /// </summary>
        public int Y => myY;

        /// <summary>
        /// The classification of this room.
        /// </summary>
        public RoomType Type
        {
            get => myType;
            internal set => myType = value;
        }

        /// <summary>
        /// True if this room has a north wall.
        /// </summary>
        public bool NorthWall
        {
            get => myWalls[Direction.North];
            internal set => myWalls[Direction.North] = value;
        }

        /// <summary>
        /// True if this room has a south wall.
        /// </summary>
        public bool SouthWall
        {
            get => myWalls[Direction.South];
            internal set => myWalls[Direction.South] = value;
        }

        /// <summary>
        /// True if this room has an east wall.
        /// </summary>
        public bool EastWall
        {
            get => myWalls[Direction.East];
            internal set => myWalls[Direction.East] = value;
        }

        /// <summary>
        /// True if this room has a west wall.
        /// </summary>
        public bool WestWall
        {
            get => myWalls[Direction.West];
            internal set => myWalls[Direction.West] = value;
        }

        /// <summary>
        /// True if this room has been visited by the maze generation algorithm.
        /// </summary>
        public bool Visited
        {
            get => myVisited;
            internal set => myVisited = value;
        }

        /// <summary>
        /// The room to the north, or null if none exists.
        /// </summary>
        public Room North
        {
            get => myNeighbors[Direction.North];
            internal set => myNeighbors[Direction.North] = value;
        }

        /// <summary>
        /// The room to the south, or null if none exists.
        /// </summary>
        public Room South
        {
            get => myNeighbors[Direction.South];
            internal set => myNeighbors[Direction.South] = value;
        }

        /// <summary>
        /// The room to the east, or null if none exists.
        /// </summary>
        public Room East
        {
            get => myNeighbors[Direction.East];
            internal set => myNeighbors[Direction.East] = value;
        }

        /// <summary>
        /// The room to the west, or null if none exists.
        /// </summary>
        public Room West
        {
            get => myNeighbors[Direction.West];
            internal set => myNeighbors[Direction.West] = value;
        }

        /// <summary>
        /// Gets whether this room has a wall in the specified direction.
        /// </summary>
        /// <param name="theDirection">The direction to check.</param>
        /// <returns>True if there is a wall in that direction.</returns>
        public bool GetWall(Direction theDirection)
        {
            return myWalls[theDirection];
        }

        /// <summary>
        /// Sets whether this room has a wall in the specified direction.
        /// </summary>
        /// <param name="theDirection">The direction to set.</param>
        /// <param name="hasWall">True to create a wall, false to remove it.</param>
        internal void SetWall(Direction theDirection, bool hasWall)
        {
            myWalls[theDirection] = hasWall;
        }

        /// <summary>
        /// Gets the neighboring room in the specified direction.
        /// </summary>
        /// <param name="theDirection">The direction to check.</param>
        /// <returns>The neighbor in that direction, or null if none exists.</returns>
        public Room GetNeighbor(Direction theDirection)
        {
            return myNeighbors[theDirection];
        }

        /// <summary>
        /// Sets the neighboring room in the specified direction.
        /// </summary>
        /// <param name="theDirection">The direction to set.</param>
        /// <param name="theNeighbor">The room to set as neighbor.</param>
        internal void SetNeighbor(Direction theDirection, Room theNeighbor)
        {
            myNeighbors[theDirection] = theNeighbor;
        }

        /// <summary>
        /// Returns the opposite direction of the given direction.
        /// </summary>
        /// <param name="theDirection">The direction to reverse.</param>
        /// <returns>The opposite direction.</returns>
        public static Direction GetOppositeDirection(Direction theDirection)
        {
            return theDirection switch
            {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                Direction.West => Direction.East,
                _ => theDirection
            };
        }

        /// <summary>
        /// Returns a string representation of this room showing its position and type.
        /// </summary>
        /// <returns>A descriptive string for this room.</returns>
        public override string ToString()
        {
            return $"Room ({myX}, {myY}) - {myType}";
        }
    }
}
