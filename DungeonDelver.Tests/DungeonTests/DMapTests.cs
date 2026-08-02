// Project: TCSS 360 Dungeon Adventure
// File: DMapTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge


using DungeonDelver.Dungeon;


namespace DungeonDelver.Tests.Dungeon
{
    /// <summary>
    /// Test suite for the DungeonMap class, verifying dungeon creation,
    /// room grid initialization, entrance and exit placement, and edge cases.
    /// </summary>
    public class DungeonMapTests
    {
        /// <summary>
        /// Verifies that a 10x10 dungeon contains exactly 100 rooms.
        /// </summary>
        [Fact]
        public void DungeonMap_10x10_Contains100Rooms()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);

            int roomCount = 0;
            for (int x = 0; x < testDungeon.Width; x++)
            {
                for (int y = 0; y < testDungeon.Height; y++)
                {
                    Assert.NotNull(testDungeon.GetRoom(x, y));
                    roomCount++;
                }
            }

            Assert.Equal(100, roomCount);
        }

        /// <summary>
        /// Verifies that GetRooms returns an array with the correct dimensions.
        /// </summary>
        [Fact]
        public void GetRooms_ReturnsArrayWithCorrectDimensions()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);
            Room[,] rooms = testDungeon.GetRooms();

            Assert.Equal(10, rooms.GetLength(0));
            Assert.Equal(10, rooms.GetLength(1));
        }

        /// <summary>
        /// Verifies that GetRoom returns the room at the specified coordinates.
        /// </summary>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(3, 3)]
        [InlineData(9, 0)]
        [InlineData(0, 9)]
        [InlineData(9, 9)]
        public void GetRoom_ReturnsRoomAtCorrectLocation(int theX, int theY)
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);
            Room retrievedRoom = testDungeon.GetRoom(theX, theY);

            Assert.Equal(theX, retrievedRoom.X);
            Assert.Equal(theY, retrievedRoom.Y);
        }

        /// <summary>
        /// Verifies that the entrance is at (0, 0) with correct type.
        /// </summary>
        [Fact]
        public void Entrance_IsAtOriginAndTypedCorrectly()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);

            Assert.Equal(0, testDungeon.Entrance.X);
            Assert.Equal(0, testDungeon.Entrance.Y);
            Assert.Equal(RoomType.Entrance, testDungeon.Entrance.Type);
        }

        /// <summary>
        /// Verifies that the exit is at the far corner with correct type.
        /// </summary>
        [Fact]
        public void Exit_IsAtFarCornerAndTypedCorrectly()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);

            Assert.Equal(9, testDungeon.Exit.X);
            Assert.Equal(9, testDungeon.Exit.Y);
            Assert.Equal(RoomType.Exit, testDungeon.Exit.Type);
        }

        /// <summary>
        /// Verifies that small or narrow dungeons build the correct number of rooms.
        /// </summary>
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 5)]
        [InlineData(5, 1)]
        public void SmallOrNarrowDungeons_StillBuildCorrectRoomCount(int theWidth, int theHeight)
        {
            DungeonMap testDungeon = new DungeonMap(theWidth, theHeight);
            Room[,] rooms = testDungeon.GetRooms();

            Assert.Equal(theWidth, rooms.GetLength(0));
            Assert.Equal(theHeight, rooms.GetLength(1));
        }

        /// <summary>
        /// Verifies that in a 1x1 dungeon, entrance and exit are the same room.
        /// The Exit assignment overwrites the Entrance type.
        /// </summary>
        [Fact]
        public void OneByOneDungeon_EntranceAndExitAreSameRoom()
        {
            DungeonMap testDungeon = new DungeonMap(1, 1);

            Assert.Same(testDungeon.Entrance, testDungeon.Exit);
            Assert.Equal(RoomType.Exit, testDungeon.Entrance.Type);
        }

        /// <summary>
        /// Verifies that a zero-sized dungeon throws an exception
        /// because there is no room at (0, 0) to assign as entrance.
        /// </summary>
        [Fact]
        public void ZeroSizedDungeon_ThrowsBecauseThereIsNoRoomZeroZero()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new DungeonMap(0, 0));
        }

        /// <summary>
        /// Verifies that negative dimensions cause an exception during construction.
        /// </summary>
        [Theory]
        [InlineData(-1, 10)]
        [InlineData(10, -1)]
        public void NegativeDimensions_ThrowOnConstruction(int theWidth, int theHeight)
        {
            Assert.ThrowsAny<Exception>(() => new DungeonMap(theWidth, theHeight));
        }

        /// <summary>
        /// Verifies that every room in the grid is a distinct instance, not shared references.
        /// </summary>
        [Fact]
        public void AllRooms_AreDistinctInstances()
        {
            DungeonMap testDungeon = new DungeonMap(5, 5);
            HashSet<Room> seenRooms = new HashSet<Room>();

            for (int x = 0; x < testDungeon.Width; x++)
            {
                for (int y = 0; y < testDungeon.Height; y++)
                {
                    Assert.True(seenRooms.Add(testDungeon.GetRoom(x, y)));
                }
            }
        }

        /// <summary>
        /// Verifies that entrance and exit are different rooms in any dungeon larger than 1x1.
        /// </summary>
        [Fact]
        public void EntranceAndExit_AreDifferentRoomsWhenDungeonIsLargerThanOneByOne()
        {
            DungeonMap testDungeon = new DungeonMap(10, 10);

            Assert.NotSame(testDungeon.Entrance, testDungeon.Exit);
        }

        /// <summary>
        /// Verifies that Width and Height properties reflect the constructor arguments.
        /// </summary>
        [Fact]
        public void WidthAndHeight_MatchConstructorArguments()
        {
            DungeonMap testDungeon = new DungeonMap(7, 12);

            Assert.Equal(7, testDungeon.Width);
            Assert.Equal(12, testDungeon.Height);
        }

        /// <summary>
        /// Verifies that a larger dungeon builds the correct total room count.
        /// </summary>
        [Fact]
        public void LargeDungeon_BuildsCorrectRoomCount()
        {
            DungeonMap testDungeon = new DungeonMap(25, 25);
            Room[,] rooms = testDungeon.GetRooms();

            Assert.Equal(625, rooms.Length);
        }

        /// <summary>
        /// Verifies that requesting a room outside the grid bounds throws.
        /// </summary>
        [Fact]
        public void GetRoom_OutOfBounds_Throws()
        {
            DungeonMap testDungeon = new DungeonMap(5, 5);

            Assert.Throws<IndexOutOfRangeException>(() => testDungeon.GetRoom(5, 5));
        }
    }
}