namespace DungeonDelver.Dungeon;
//Represents object Room


public class Room
{
    // Position in dun. grid
    public int X { get; }
    public int Y { get; }

    //Room classification
    public RoomType Type { get; set; }

    // Walls (start closed)
    public bool NorthWall { get; set; } = true;
    public bool SouthWall { get; set; } = true;
    public bool EastWall { get; set; } = true;
    public bool WestWall { get; set; } = true;

    // Used by maze generation algorithms
    public bool Visited { get; set; } = false;

    // Connected rooms
    public Room North { get; set; }
    public Room South { get; set; }
    public Room East { get; set; }
    public Room West { get; set; }


    public Room(int x, int y)
    {
        X = x;
        Y = y;
        Type = RoomType.Normal;
    }


    public override string ToString()
    {
        return $"Room ({X}, {Y}) - {Type}";
    }

}