namespace DungeonDelver.Dungeon;

using DungeonDelver.Dungeon;
//Contains maze creation logic

public class MazeGenerator
{
    public DungeonMap Generate(int width, int height)
    {
        DungeonMap dungeon = new DungeonMap(width, height);

        CreateMaze(dungeon);

        return dungeon;
    }


    private void CreateMaze(DungeonMap dungeon)
    {
        Room start = dungeon.Entrance;

        VisitRoom(start);
    }


    private void VisitRoom(Room room)
    {
        room.Visited = true;

        // Recursive backtracking algorithm will be added here.
    }
}