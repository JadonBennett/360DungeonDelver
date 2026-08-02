// Project: TCSS 360 Dungeon Adventure
// File: MazeGenerator.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Dungeon
{
    /// <summary>
    /// Generates dungeon mazes using recursive backtracking algorithm.
    /// Creates a DungeonMap and carves passages through the rooms
    /// to create a solvable maze layout.
    /// </summary>
    public class MazeGenerator
    {
        /// <summary>
        /// Generates a new dungeon maze with the specified dimensions.
        /// </summary>
        /// <param name="theWidth">The width of the dungeon grid.</param>
        /// <param name="theHeight">The height of the dungeon grid.</param>
        /// <returns>A fully generated DungeonMap with carved passages.</returns>
        public DungeonMap Generate(int theWidth, int theHeight)
        {
            DungeonMap newDungeon = new DungeonMap(theWidth, theHeight);

            CreateMaze(newDungeon);

            return newDungeon;
        }

        /// <summary>
        /// Creates the maze structure within the given dungeon by carving passages.
        /// Starts at the entrance room and recursively visits all reachable rooms.
        /// </summary>
        /// <param name="theDungeon">The dungeon map to generate the maze within.</param>
        private void CreateMaze(DungeonMap theDungeon)
        {
            Room startRoom = theDungeon.Entrance;

            VisitRoom(startRoom);
        }

        /// <summary>
        /// Marks the given room as visited. Recursive backtracking algorithm
        /// to carve passages will be implemented here.
        /// </summary>
        /// <param name="theRoom">The room to visit and process.</param>
        private void VisitRoom(Room theRoom)
        {
            theRoom.Visited = true;

            // TODO: Implement recursive backtracking algorithm
            // 1. Get list of unvisited neighbors
            // 2. While unvisited neighbors exist:
            //    a. Choose a random unvisited neighbor
            //    b. Remove wall between current room and chosen neighbor
            //    c. Recursively visit the chosen neighbor
        }
    }
}
