// Project: TCSS 360 Dungeon Adventure
// File: MazeGenTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
using DungeonDelver.Source.Model;

using DungeonDelver.Dungeon;

namespace DungeonDelver.Tests
{
    /// <summary>
    /// Test suite for the MazeGenerator class, verifying dungeon generation,
    /// room connectivity, neighbor relationships, and edge cases.
    /// </summary>
    public class MazeGeneratorTests
    {
        /// <summary>
        /// Verifies that Generate returns a dungeon with the requested dimensions.
        /// </summary>
        [Fact]
        public void Generate_ReturnsDungeonWithCorrectDimensions()
        {
            MazeGenerator testGenerator = new MazeGenerator();
            DungeonMap generatedDungeon = testGenerator.Generate(10, 10, PillarType.Abstraction, 0);

            Assert.Equal(10, generatedDungeon.Width);
            Assert.Equal(10, generatedDungeon.Height);
        }

        /// <summary>
        /// Verifies that Generate marks the entrance room as visited.
        /// </summary>
        [Fact]
        public void Generate_MarksEntranceAsVisited()
        {
            MazeGenerator testGenerator = new MazeGenerator();
            DungeonMap generatedDungeon = testGenerator.Generate(10, 10, PillarType.Abstraction, 0);

            Assert.True(generatedDungeon.Entrance.Visited);
        }

        /// <summary>
        /// Verifies that the top-left corner room has only east and south neighbors.
        /// </summary>
        [Fact]
        public void CornerRoom_TopLeft_HasOnlyEastAndSouthNeighbors()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);
            Room cornerRoom = testDungeon.GetRoom(0, 0);

            Assert.Null(cornerRoom.North);
            Assert.Null(cornerRoom.West);
            Assert.NotNull(cornerRoom.East);
            Assert.NotNull(cornerRoom.South);
        }

        /// <summary>
        /// Verifies that the bottom-right corner room has only north and west neighbors.
        /// </summary>
        [Fact]
        public void CornerRoom_BottomRight_HasOnlyNorthAndWestNeighbors()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);
            Room cornerRoom = testDungeon.GetRoom(9, 9);

            Assert.Null(cornerRoom.South);
            Assert.Null(cornerRoom.East);
            Assert.NotNull(cornerRoom.North);
            Assert.NotNull(cornerRoom.West);
        }

        /// <summary>
        /// Verifies that rooms on the dungeon edge have exactly three neighbors.
        /// </summary>
        [Theory]
        [InlineData(0, 5)]  // left edge
        [InlineData(9, 5)]  // right edge
        [InlineData(5, 0)]  // top edge
        [InlineData(5, 9)]  // bottom edge
        public void EdgeRoom_HasExactlyThreeNeighbors(int theX, int theY)
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);
            Room edgeRoom = testDungeon.GetRoom(theX, theY);

            int neighborCount = 0;
            if (edgeRoom.North != null) neighborCount++;
            if (edgeRoom.South != null) neighborCount++;
            if (edgeRoom.East != null) neighborCount++;
            if (edgeRoom.West != null) neighborCount++;

            Assert.Equal(3, neighborCount);
        }

        /// <summary>
        /// Verifies that interior rooms have all four neighbors.
        /// </summary>
        [Fact]
        public void InteriorRoom_HasAllFourNeighbors()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);
            Room interiorRoom = testDungeon.GetRoom(5, 5);

            Assert.NotNull(interiorRoom.North);
            Assert.NotNull(interiorRoom.South);
            Assert.NotNull(interiorRoom.East);
            Assert.NotNull(interiorRoom.West);
        }

        /// <summary>
        /// Verifies that neighbor references point to the correct adjacent coordinates.
        /// </summary>
        [Fact]
        public void Neighbors_AreCorrectAdjacentCoordinates()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);
            Room centerRoom = testDungeon.GetRoom(5, 5);

            Assert.Equal((5, 4), (centerRoom.North.X, centerRoom.North.Y));
            Assert.Equal((5, 6), (centerRoom.South.X, centerRoom.South.Y));
            Assert.Equal((6, 5), (centerRoom.East.X, centerRoom.East.Y));
            Assert.Equal((4, 5), (centerRoom.West.X, centerRoom.West.Y));
        }

        /// <summary>
        /// Verifies that neighbor relationships are bidirectional.
        /// </summary>
        [Fact]
        public void Neighbors_AreSymmetric()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);
            Room centerRoom = testDungeon.GetRoom(5, 5);

            Assert.Same(centerRoom, centerRoom.East.West);
            Assert.Same(centerRoom, centerRoom.West.East);
            Assert.Same(centerRoom, centerRoom.North.South);
            Assert.Same(centerRoom, centerRoom.South.North);
        }

        /// <summary>
        /// Verifies that a 1x1 dungeon's single room has no neighbors.
        /// </summary>
        [Fact]
        public void OneByOneDungeon_RoomHasNoNeighbors()
        {
            DungeonMap testDungeon = new DungeonMap(1, 1);
            Room singleRoom = testDungeon.GetRoom(0, 0);

            Assert.Null(singleRoom.North);
            Assert.Null(singleRoom.South);
            Assert.Null(singleRoom.East);
            Assert.Null(singleRoom.West);
        }

        /// <summary>
        /// Verifies that in a single-row dungeon, end rooms have only one neighbor.
        /// </summary>
        [Fact]
        public void SingleRowDungeon_EndRoomsHaveOnlyOneNeighbor()
        {
            DungeonMap testDungeon = new DungeonMap(5, 1);

            Room leftEnd = testDungeon.GetRoom(0, 0);
            Room rightEnd = testDungeon.GetRoom(4, 0);

            Assert.Null(leftEnd.North);
            Assert.Null(leftEnd.South);
            Assert.Null(leftEnd.West);
            Assert.NotNull(leftEnd.East);

            Assert.Null(rightEnd.North);
            Assert.Null(rightEnd.South);
            Assert.Null(rightEnd.East);
            Assert.NotNull(rightEnd.West);
        }

        /// <summary>
        /// Verifies that every room in the generated dungeon is marked visited.
        /// </summary>
        [Fact]
        public void Generate_MarksEveryRoomAsVisited()
        {
            MazeGenerator testGenerator = new MazeGenerator();
            DungeonMap generatedDungeon = testGenerator.Generate(10, 10, PillarType.Abstraction, 0);

            for (int x = 0; x < generatedDungeon.Width; x++)
            {
                for (int y = 0; y < generatedDungeon.Height; y++)
                {
                    Assert.True(generatedDungeon.GetRoom(x, y).Visited);
                }
            }
        }

        /// <summary>
        /// Verifies that every room has at least one open wall (no room is fully sealed off).
        /// </summary>
        [Fact]
        public void Generate_NoRoomIsCompletelyWalledOff()
        {
            MazeGenerator testGenerator = new MazeGenerator();
            DungeonMap generatedDungeon = testGenerator.Generate(10, 10, PillarType.Abstraction, 0);

            for (int x = 0; x < generatedDungeon.Width; x++)
            {
                for (int y = 0; y < generatedDungeon.Height; y++)
                {
                    Room currentRoom = generatedDungeon.GetRoom(x, y);
                    bool hasOpenWall = !currentRoom.NorthWall || !currentRoom.SouthWall
                                        || !currentRoom.EastWall || !currentRoom.WestWall;

                    Assert.True(hasOpenWall);
                }
            }
        }

        /// <summary>
        /// Verifies that the exit room is reachable from the entrance via open passages,
        /// confirming the maze is fully solvable.
        /// </summary>
        [Fact]
        public void Generate_ExitIsReachableFromEntrance()
        {
            MazeGenerator testGenerator = new MazeGenerator();
            DungeonMap generatedDungeon = testGenerator.Generate(10, 10, PillarType.Abstraction, 0);

            HashSet<Room> visited = GetReachableRooms(generatedDungeon.Entrance);

            Assert.Contains(generatedDungeon.Exit, visited);
        }

        /// <summary>
        /// Verifies that every room in the dungeon is reachable from the entrance,
        /// confirming full connectivity.
        /// </summary>
        [Fact]
        public void Generate_EveryRoomIsReachableFromEntrance()
        {
            MazeGenerator testGenerator = new MazeGenerator();
            DungeonMap generatedDungeon = testGenerator.Generate(6, 6, PillarType.Abstraction, 0);

            HashSet<Room> visited = GetReachableRooms(generatedDungeon.Entrance);

            Assert.Equal(36, visited.Count);
        }

        /// <summary>
        /// Verifies that a wall being open on one room implies the paired wall
        /// on its neighbor is also open (carved passages are always bidirectional).
        /// </summary>
        [Fact]
        public void Generate_CarvedWallsAreBidirectional()
        {
            MazeGenerator testGenerator = new MazeGenerator();
            DungeonMap generatedDungeon = testGenerator.Generate(8, 8, PillarType.Abstraction, 0);

            for (int x = 0; x < generatedDungeon.Width; x++)
            {
                for (int y = 0; y < generatedDungeon.Height; y++)
                {
                    Room currentRoom = generatedDungeon.GetRoom(x, y);

                    if (!currentRoom.NorthWall)
                        Assert.False(currentRoom.North.SouthWall);

                    if (!currentRoom.EastWall)
                        Assert.False(currentRoom.East.WestWall);
                }
            }
        }

        /// <summary>
        /// Verifies that generating a 1x1 dungeon succeeds without needing any carved walls,
        /// since there are no neighbors to connect to.
        /// </summary>
        [Fact]
        public void Generate_OneByOneDungeon_Succeeds()
        {
            MazeGenerator testGenerator = new MazeGenerator();
            DungeonMap generatedDungeon = testGenerator.Generate(1, 1, PillarType.Abstraction, 0);

            Assert.True(generatedDungeon.Entrance.Visited);
            Assert.Same(generatedDungeon.Entrance, generatedDungeon.Exit);
        }

        /// <summary>
        /// Performs a breadth-first traversal from the given room, following only
        /// open (carved) passages, and returns every room reached.
        /// </summary>
        /// <param name="theStart">The room to begin traversal from.</param>
        /// <returns>The set of rooms reachable from theStart.</returns>
        private HashSet<Room> GetReachableRooms(Room theStart)
        {
            HashSet<Room> visited = new HashSet<Room>();
            Queue<Room> queue = new Queue<Room>();

            queue.Enqueue(theStart);
            visited.Add(theStart);

            while (queue.Count > 0)
            {
                Room current = queue.Dequeue();

                if (!current.NorthWall && current.North != null && visited.Add(current.North))
                    queue.Enqueue(current.North);
                if (!current.SouthWall && current.South != null && visited.Add(current.South))
                    queue.Enqueue(current.South);
                if (!current.EastWall && current.East != null && visited.Add(current.East))
                    queue.Enqueue(current.East);
                if (!current.WestWall && current.West != null && visited.Add(current.West))
                    queue.Enqueue(current.West);
            }

            return visited;
        }
    }
}