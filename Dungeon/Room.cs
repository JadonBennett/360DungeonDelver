// Project: TCSS 360 Dungeon Adventure
// File: Room.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using DungeonDelver.Source.Model;
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
        /// True if this room has a north wall.
        /// </summary>
        private bool myNorthWall;

        /// <summary>
        /// True if this room has a south wall.
        /// </summary>
        private bool mySouthWall;

        /// <summary>
        /// True if this room has an east wall.
        /// </summary>
        private bool myEastWall;

        /// <summary>
        /// True if this room has a west wall.
        /// </summary>
        private bool myWestWall;

        /// <summary>
        /// True if this room has been visited by the maze generation algorithm.
        /// </summary>
        private bool myVisited;

        /// <summary>
        /// The room to the north of this one, or null if none exists.
        /// </summary>
        private Room myNorth;

        /// <summary>
        /// The room to the south of this one, or null if none exists.
        /// </summary>
        private Room mySouth;

        /// <summary>
        /// The room to the east of this one, or null if none exists.
        /// </summary>
        private Room myEast;

        /// <summary>
        /// The room to the west of this one, or null if none exists.
        /// </summary>
        private Room myWest;
        
        /// <summary>
        /// Item placed in room, null if none 
        /// </summary>
        private Item myItem;

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
            myNorthWall = true;
            mySouthWall = true;
            myEastWall = true;
            myWestWall = true;
            myVisited = false;
            myNorth = null;
            mySouth = null;
            myEast = null;
            myWest = null;
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
            get => myNorthWall;
            internal set => myNorthWall = value;
        }

        /// <summary>
        /// True if this room has a south wall.
        /// </summary>
        public bool SouthWall
        {
            get => mySouthWall;
            internal set => mySouthWall = value;
        }

        /// <summary>
        /// True if this room has an east wall.
        /// </summary>
        public bool EastWall
        {
            get => myEastWall;
            internal set => myEastWall = value;
        }

        /// <summary>
        /// True if this room has a west wall.
        /// </summary>
        public bool WestWall
        {
            get => myWestWall;
            internal set => myWestWall = value;
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
            get => myNorth;
            internal set => myNorth = value;
        }

        /// <summary>
        /// The room to the south, or null if none exists.
        /// </summary>
        public Room South
        {
            get => mySouth;
            internal set => mySouth = value;
        }

        /// <summary>
        /// The room to the east, or null if none exists.
        /// </summary>
        public Room East
        {
            get => myEast;
            internal set => myEast = value;
        }

        /// <summary>
        /// The room to the west, or null if none exists.
        /// </summary>
        public Room West
        {
            get => myWest;
            internal set => myWest = value;
        }
        
        /// <summary>
        /// The item placed in this room, if any (e.g. Pillar or Potion).
        /// Null if the room has no item.
        /// </summary>

        public Item Item
        {
            get => myItem;
            internal set => myItem = value;
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
