

using Xunit;
using DungeonDelver.Dungeon;

namespace DungeonDelver.Tests;

public class MazeGeneratorTests
{
    [Fact]
    public void Generate_ReturnsDungeonWithCorrectDimensions()
    {
        var generator = new MazeGenerator();

        DungeonMap dungeon = generator.Generate(10, 10);

        Assert.Equal(10, dungeon.Width);
        Assert.Equal(10, dungeon.Height);
    }

    [Fact]
    public void Generate_MarksEntranceAsVisited()
    {
        var generator = new MazeGenerator();

        DungeonMap dungeon = generator.Generate(10, 10);

        Assert.True(dungeon.Entrance.Visited);
    }

    [Fact]
    public void CornerRoom_TopLeft_HasOnlyEastAndSouthNeighbors()
    {
        var dungeon = new DungeonMap(10, 10);
        Room corner = dungeon.GetRoom(0, 0);

        Assert.Null(corner.North);
        Assert.Null(corner.West);
        Assert.NotNull(corner.East);
        Assert.NotNull(corner.South);
    }

    [Fact]
    public void CornerRoom_BottomRight_HasOnlyNorthAndWestNeighbors()
    {
        var dungeon = new DungeonMap(10, 10);
        Room corner = dungeon.GetRoom(9, 9);

        Assert.Null(corner.South);
        Assert.Null(corner.East);
        Assert.NotNull(corner.North);
        Assert.NotNull(corner.West);
    }

    [Theory]
    [InlineData(0, 5)]  // left edge
    [InlineData(9, 5)]  // right edge
    [InlineData(5, 0)]  // top edge
    [InlineData(5, 9)]  // bottom edge
    public void EdgeRoom_HasExactlyThreeNeighbors(int x, int y)
    {
        var dungeon = new DungeonMap(10, 10);
        Room room = dungeon.GetRoom(x, y);

        int neighborCount = 0;
        if (room.North != null) neighborCount++;
        if (room.South != null) neighborCount++;
        if (room.East != null) neighborCount++;
        if (room.West != null) neighborCount++;

        Assert.Equal(3, neighborCount);
    }

    [Fact]
    public void InteriorRoom_HasAllFourNeighbors()
    {
        var dungeon = new DungeonMap(10, 10);
        Room room = dungeon.GetRoom(5, 5);

        Assert.NotNull(room.North);
        Assert.NotNull(room.South);
        Assert.NotNull(room.East);
        Assert.NotNull(room.West);
    }

    [Fact]
    public void Neighbors_AreCorrectAdjacentCoordinates()
    {
        var dungeon = new DungeonMap(10, 10);
        Room room = dungeon.GetRoom(5, 5);

        Assert.Equal((5, 4), (room.North.X, room.North.Y));
        Assert.Equal((5, 6), (room.South.X, room.South.Y));
        Assert.Equal((6, 5), (room.East.X, room.East.Y));
        Assert.Equal((4, 5), (room.West.X, room.West.Y));
    }

    [Fact]
    public void Neighbors_AreSymmetric()
    {
        // If A.East == B, then B.West should point back to A, and so on.
        var dungeon = new DungeonMap(10, 10);
        Room room = dungeon.GetRoom(5, 5);

        Assert.Same(room, room.East.West);
        Assert.Same(room, room.West.East);
        Assert.Same(room, room.North.South);
        Assert.Same(room, room.South.North);
    }

    [Fact]
    public void OneByOneDungeon_RoomHasNoNeighbors()
    {
        // Edge case: a single-room dungeon should have no connections at all.
        var dungeon = new DungeonMap(1, 1);
        Room room = dungeon.GetRoom(0, 0);

        Assert.Null(room.North);
        Assert.Null(room.South);
        Assert.Null(room.East);
        Assert.Null(room.West);
    }

    [Fact]
    public void SingleRowDungeon_EndRoomsHaveOnlyOneNeighbor()
    {
        // Edge case: 1-tall strip - top/bottom edge concepts collapse,
        // so end rooms should only connect East/West, not North/South.
        var dungeon = new DungeonMap(5, 1);

        Room left = dungeon.GetRoom(0, 0);
        Room right = dungeon.GetRoom(4, 0);

        Assert.Null(left.North);
        Assert.Null(left.South);
        Assert.Null(left.West);
        Assert.NotNull(left.East);

        Assert.Null(right.North);
        Assert.Null(right.South);
        Assert.Null(right.East);
        Assert.NotNull(right.West);
    }
}