namespace DungeonDelver.Dungeon;
using DungeonDelver.Dungeon;
//Collection of rooms + dungeon layout

public class DungeonMap
{
    private readonly Room[,] rooms;

    public int Width { get; }
    public int Height { get; }

    public Room Entrance { get; private set; }
    public Room Exit { get; private set; }


    public DungeonMap(int width, int height)
    {
        Width = width;
        Height = height;

        rooms = new Room[width, height];

        CreateRooms();
        ConnectRooms();

        Entrance = rooms[0, 0];
        Entrance.Type = RoomType.Entrance;

        Exit = rooms[width - 1, height - 1];
        Exit.Type = RoomType.Exit;
    }


    private void CreateRooms()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                rooms[x, y] = new Room(x, y);
            }
        }
    }


    private void ConnectRooms()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Room current = rooms[x, y];

                if (y > 0)
                    current.North = rooms[x, y - 1];

                if (y < Height - 1)
                    current.South = rooms[x, y + 1];

                if (x > 0)
                    current.West = rooms[x - 1, y];

                if (x < Width - 1)
                    current.East = rooms[x + 1, y];
            }
        }
    }


    public Room GetRoom(int x, int y)
    {
        return rooms[x, y];
    }


    public Room[,] GetRooms()
    {
        return rooms;
    }
}