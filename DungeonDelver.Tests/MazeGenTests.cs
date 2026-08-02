// Project: TCSS 360 Dungeon Adventure
// File: MazeGenTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Xunit;
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
            DungeonMap generatedDungeon = testGenerator.Generate(10, 10);

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
            DungeonMap generatedDungeon = testGenerator.Generate(10, 10);

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
        /// <param name="theX">The X coordinate of the edge room to test.</param>
        /// <param name="theY">The Y coordinate of the edge room to test.</param>
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
            if (edgeRoom.North != null)
            {
                neighborCount++;
            }
            if (edgeRoom.South != null)
            {
                neighborCount++;
            }
            if (edgeRoom.East != null)
            {
                neighborCount++;
            }
            if (edgeRoom.West != null)
            {
                neighborCount++;
            }

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
        /// If room A's east neighbor is B, then B's west neighbor should be A.
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
    }
}
