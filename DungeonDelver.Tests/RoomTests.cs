// Project: TCSS 360 Dungeon Adventure
// File: RoomTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Xunit;
using DungeonDelver.Dungeon;

namespace DungeonDelver.Tests.Dungeon
{
    /// <summary>
    /// Test suite for the Room class, verifying position initialization,
    /// wall states, room types, and neighbor connections.
    /// </summary>
    public class RoomTests
    {
        /// <summary>
        /// Verifies that the Room constructor correctly sets the position coordinates.
        /// </summary>
        /// <param name="theX">The X coordinate to test.</param>
        /// <param name="theY">The Y coordinate to test.</param>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(4, 7)]
        [InlineData(9, 9)]
        public void Constructor_SetsCorrectPosition(int theX, int theY)
        {
            Room testRoom = new Room(theX, theY);

            Assert.Equal(theX, testRoom.X);
            Assert.Equal(theY, testRoom.Y);
        }

        /// <summary>
        /// Verifies that new rooms default to Normal type.
        /// </summary>
        [Fact]
        public void Constructor_DefaultsTypeToNormal()
        {
            Room testRoom = new Room(2, 2);

            Assert.Equal(RoomType.Normal, testRoom.Type);
        }

        /// <summary>
        /// Verifies that all walls start in the closed state.
        /// </summary>
        [Fact]
        public void Constructor_AllWallsStartClosed()
        {
            Room testRoom = new Room(0, 0);

            Assert.True(testRoom.NorthWall);
            Assert.True(testRoom.SouthWall);
            Assert.True(testRoom.EastWall);
            Assert.True(testRoom.WestWall);
        }

        /// <summary>
        /// Verifies that rooms start with Visited set to false.
        /// </summary>
        [Fact]
        public void Constructor_VisitedStartsFalse()
        {
            Room testRoom = new Room(0, 0);

            Assert.False(testRoom.Visited);
        }

        /// <summary>
        /// Verifies that neighbor references start as null.
        /// </summary>
        [Fact]
        public void Constructor_NeighborsStartNull()
        {
            Room testRoom = new Room(0, 0);

            Assert.Null(testRoom.North);
            Assert.Null(testRoom.South);
            Assert.Null(testRoom.East);
            Assert.Null(testRoom.West);
        }

        /// <summary>
        /// Verifies that room type can be changed after construction.
        /// </summary>
        [Fact]
        public void Type_CanBeChangedAfterConstruction()
        {
            Room testRoom = new Room(0, 0);
            testRoom.Type = RoomType.Entrance;

            Assert.Equal(RoomType.Entrance, testRoom.Type);
        }

        /// <summary>
        /// Verifies that individual walls can be opened independently.
        /// </summary>
        [Fact]
        public void Walls_CanBeOpenedIndividually()
        {
            Room testRoom = new Room(0, 0);
            testRoom.NorthWall = false;

            Assert.False(testRoom.NorthWall);
            Assert.True(testRoom.SouthWall);
            Assert.True(testRoom.EastWall);
            Assert.True(testRoom.WestWall);
        }

        /// <summary>
        /// Verifies that ToString includes position and type information.
        /// </summary>
        [Fact]
        public void ToString_IncludesPositionAndType()
        {
            Room testRoom = new Room(3, 4);
            testRoom.Type = RoomType.Exit;

            string result = testRoom.ToString();

            Assert.Contains("3", result);
            Assert.Contains("4", result);
            Assert.Contains("Exit", result);
        }

        /// <summary>
        /// Verifies that Room accepts boundary and negative coordinates.
        /// Coordinate validation is DungeonMap's responsibility, not Room's.
        /// </summary>
        /// <param name="theX">The X coordinate to test.</param>
        /// <param name="theY">The Y coordinate to test.</param>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        public void Constructor_AllowsBoundaryAndNegativeCoordinates(int theX, int theY)
        {
            Room testRoom = new Room(theX, theY);

            Assert.Equal(theX, testRoom.X);
            Assert.Equal(theY, testRoom.Y);
        }
    }
}
