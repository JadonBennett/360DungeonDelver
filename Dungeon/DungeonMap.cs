// Project: TCSS 360 Dungeon Adventure
// File: DungeonMap.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Dungeon
{
    /// <summary>
    /// Represents the complete dungeon layout as a grid of rooms,
    /// including the entrance and exit. Manages room creation and connectivity.
    /// </summary>
    public class DungeonMap
    {
        /// <summary>
        /// The two-dimensional array of rooms comprising this dungeon.
        /// </summary>
        private readonly Room[,] myRooms;

        /// <summary>
        /// The width of the dungeon grid.
        /// </summary>
        private readonly int myWidth;

        /// <summary>
        /// The height of the dungeon grid.
        /// </summary>
        private readonly int myHeight;

        /// <summary>
        /// The entrance room where the adventure begins.
        /// </summary>
        private Room myEntrance;

        /// <summary>
        /// The exit room that must be reached to complete the dungeon.
        /// </summary>
        private Room myExit;

        /// <summary>
        /// Initializes a new DungeonMap with the specified dimensions,
        /// creating all rooms and establishing neighbor connections.
        /// The entrance is at (0, 0) and the exit at (width-1, height-1).
        /// </summary>
        /// <param name="theWidth">The width of the dungeon grid.</param>
        /// <param name="theHeight">The height of the dungeon grid.</param>
        public DungeonMap(int theWidth, int theHeight)
        {
            myWidth = theWidth;
            myHeight = theHeight;
            myRooms = new Room[theWidth, theHeight];

            CreateRooms();
            ConnectRooms();

            myEntrance = myRooms[0, 0];
            myEntrance.Type = RoomType.Entrance;

            myExit = myRooms[theWidth - 1, theHeight - 1];
            myExit.Type = RoomType.Exit;
        }

        /// <summary>
        /// The width of the dungeon grid.
        /// </summary>
        public int Width => myWidth;

        /// <summary>
        /// The height of the dungeon grid.
        /// </summary>
        public int Height => myHeight;

        /// <summary>
        /// The entrance room where the adventure begins.
        /// </summary>
        public Room Entrance => myEntrance;

        /// <summary>
        /// The exit room that must be reached to complete the dungeon.
        /// </summary>
        public Room Exit => myExit;

        /// <summary>
        /// Retrieves the room at the specified grid coordinates.
        /// </summary>
        /// <param name="theX">The X coordinate.</param>
        /// <param name="theY">The Y coordinate.</param>
        /// <returns>The room at the given position.</returns>
        public Room GetRoom(int theX, int theY)
        {
            return myRooms[theX, theY];
        }

        /// <summary>
        /// Returns the internal room array. Intended for testing and generation use only.
        /// External callers should use GetRoom() to access individual rooms.
        /// </summary>
        /// <returns>The two-dimensional array of rooms.</returns>
        internal Room[,] GetRooms()
        {
            return myRooms;
        }

        /// <summary>
        /// Creates all rooms in the dungeon grid with default settings.
        /// </summary>
        private void CreateRooms()
        {
            for (int x = 0; x < myWidth; x++)
            {
                for (int y = 0; y < myHeight; y++)
                {
                    myRooms[x, y] = new Room(x, y);
                }
            }
        }

        /// <summary>
        /// Establishes neighbor connections between adjacent rooms in the grid.
        /// Each room is linked to its north, south, east, and west neighbors where applicable.
        /// </summary>
        private void ConnectRooms()
        {
            for (int x = 0; x < myWidth; x++)
            {
                for (int y = 0; y < myHeight; y++)
                {
                    Room currentRoom = myRooms[x, y];

                    if (y > 0)
                    {
                        currentRoom.North = myRooms[x, y - 1];
                    }

                    if (y < myHeight - 1)
                    {
                        currentRoom.South = myRooms[x, y + 1];
                    }

                    if (x > 0)
                    {
                        currentRoom.West = myRooms[x - 1, y];
                    }

                    if (x < myWidth - 1)
                    {
                        currentRoom.East = myRooms[x + 1, y];
                    }
                }
            }
        }
    }
}
