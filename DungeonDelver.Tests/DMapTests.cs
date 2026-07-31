
using System;
using Xunit;
using DungeonDelver.Dungeon;

namespace DungeonDelver.Tests.Dungeon;

public class DungeonMapTests
{
    [Fact]
    public void DungeonMap_10x10_Contains100Rooms()
    {
        var dungeon = new DungeonMap(10, 10);

        int count = 0;
        for (int x = 0; x < dungeon.Width; x++)
        {
            for (int y = 0; y < dungeon.Height; y++)
            {
                Assert.NotNull(dungeon.GetRoom(x, y));
                count++;
            }
        }

        Assert.Equal(100, count);
    }

    [Fact]
    public void GetRooms_ReturnsArrayWithCorrectDimensions()
    {
        var dungeon = new DungeonMap(10, 10);

        Room[,] rooms = dungeon.GetRooms();

        Assert.Equal(10, rooms.GetLength(0));
        Assert.Equal(10, rooms.GetLength(1));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(3, 3)]
    [InlineData(9, 0)]
    [InlineData(0, 9)]
    [InlineData(9, 9)]
    public void GetRoom_ReturnsRoomAtCorrectLocation(int x, int y)
    {
        var dungeon = new DungeonMap(10, 10);

        Room room = dungeon.GetRoom(x, y);

        Assert.Equal(x, room.X);
        Assert.Equal(y, room.Y);
    }

    [Fact]
    public void Entrance_IsAtOriginAndTypedCorrectly()
    {
        var dungeon = new DungeonMap(10, 10);

        Assert.Equal(0, dungeon.Entrance.X);
        Assert.Equal(0, dungeon.Entrance.Y);
        Assert.Equal(RoomType.Entrance, dungeon.Entrance.Type);
    }

    [Fact]
    public void Exit_IsAtFarCornerAndTypedCorrectly()
    {
        var dungeon = new DungeonMap(10, 10);

        Assert.Equal(9, dungeon.Exit.X);
        Assert.Equal(9, dungeon.Exit.Y);
        Assert.Equal(RoomType.Exit, dungeon.Exit.Type);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 5)]
    [InlineData(5, 1)]
    public void SmallOrNarrowDungeons_StillBuildCorrectRoomCount(int width, int height)
    {
        var dungeon = new DungeonMap(width, height);

        Room[,] rooms = dungeon.GetRooms();

        Assert.Equal(width, rooms.GetLength(0));
        Assert.Equal(height, rooms.GetLength(1));
    }

    [Fact]
    public void OneByOneDungeon_EntranceAndExitAreSameRoom()
    {
        // Edge case: a 1x1 dungeon has only one room, so entrance and exit
        // both resolve to rooms[0,0] and end up sharing a location.
        // Whichever assignment runs last ("Exit") wins the Type on that room.
        var dungeon = new DungeonMap(1, 1);

        Assert.Same(dungeon.Entrance, dungeon.Exit);
        Assert.Equal(RoomType.Exit, dungeon.Entrance.Type);
    }

    [Fact]
    public void ZeroSizedDungeon_ThrowsBecauseThereIsNoRoomZeroZero()
    {
        // Edge case: with width or height of 0, no rooms are created,
        // so assigning Entrance/Exit to rooms[0,0] should fail.
        Assert.Throws<IndexOutOfRangeException>(() => new DungeonMap(0, 0));
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(10, -1)]
    public void NegativeDimensions_ThrowOnConstruction(int width, int height)
    {
        Assert.ThrowsAny<Exception>(() => new DungeonMap(width, height));
    }
}