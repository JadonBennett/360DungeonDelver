

using Xunit;
using DungeonDelver.Dungeon;

namespace DungeonDelver.Tests.Dungeon;

public class RoomTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(4, 7)]
    [InlineData(9, 9)]
    public void Constructor_SetsCorrectPosition(int x, int y)
    {
        var room = new Room(x, y);

        Assert.Equal(x, room.X);
        Assert.Equal(y, room.Y);
    }

    [Fact]
    public void Constructor_DefaultsTypeToNormal()
    {
        var room = new Room(2, 2);

        Assert.Equal(RoomType.Normal, room.Type);
    }

    [Fact]
    public void Constructor_AllWallsStartClosed()
    {
        var room = new Room(0, 0);

        Assert.True(room.NorthWall);
        Assert.True(room.SouthWall);
        Assert.True(room.EastWall);
        Assert.True(room.WestWall);
    }

    [Fact]
    public void Constructor_VisitedStartsFalse()
    {
        var room = new Room(0, 0);

        Assert.False(room.Visited);
    }

    [Fact]
    public void Constructor_NeighborsStartNull()
    {
        var room = new Room(0, 0);

        Assert.Null(room.North);
        Assert.Null(room.South);
        Assert.Null(room.East);
        Assert.Null(room.West);
    }

    [Fact]
    public void Type_CanBeChangedAfterConstruction()
    {
        var room = new Room(0, 0) { Type = RoomType.Entrance };

        Assert.Equal(RoomType.Entrance, room.Type);
    }

    [Fact]
    public void Walls_CanBeOpenedIndividually()
    {
        var room = new Room(0, 0)
        {
            NorthWall = false
        };

        Assert.False(room.NorthWall);
        Assert.True(room.SouthWall);
        Assert.True(room.EastWall);
        Assert.True(room.WestWall);
    }

    [Fact]
    public void ToString_IncludesPositionAndType()
    {
        var room = new Room(3, 4) { Type = RoomType.Exit };

        string result = room.ToString();

        Assert.Contains("3", result);
        Assert.Contains("4", result);
        Assert.Contains("Exit", result);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    public void Constructor_AllowsBoundaryAndNegativeCoordinates(int x, int y)
    {
        // Edge case: Room itself does not validate coordinates -
        // that validation, if any, is DungeonMap's responsibility.
        var room = new Room(x, y);

        Assert.Equal(x, room.X);
        Assert.Equal(y, room.Y);
    }
}