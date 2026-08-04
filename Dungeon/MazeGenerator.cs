// Project: TCSS 360 Dungeon Adventure
// File: MazeGenerator.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

//imports
using System;                    
using System.Collections.Generic; 

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
        /// Random number generator used for shuffling neighbor order.
        /// </summary>
        private readonly Random myRandom = new Random();

        /// <summary>
        /// Generates a new dungeon maze with the specified dimensions.
        /// Regenerates if the exit is somehow unreachable from the entrance.
        /// </summary>
        /// <param name="theWidth">The width of the dungeon grid.</param>
        /// <param name="theHeight">The height of the dungeon grid.</param>
        /// <returns>A fully generated DungeonMap with carved passages.</returns>
        public DungeonMap Generate(int theWidth, int theHeight)
        {
            DungeonMap newDungeon;

            do
            {
                newDungeon = new DungeonMap(theWidth, theHeight);
                CreateMaze(newDungeon);
            }
            while (!IsExitReachable(newDungeon));

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
        /// Marks the given room as visited, then recursively carves passages
        /// to each unvisited neighbor in random order, backtracking when
        /// no unvisited neighbors remain.
        /// </summary>
        /// <param name="theRoom">The room to visit and process.</param>
        private void VisitRoom(Room theRoom)
        {
            theRoom.Visited = true;

            foreach (Direction direction in GetShuffledDirections())
            {
                Room neighbor = GetNeighbor(theRoom, direction);

                if (neighbor == null || neighbor.Visited)
                {
                    continue;
                }

                CarveConnection(theRoom, neighbor, direction);
                VisitRoom(neighbor);
            }
        }

        /// <summary>
        /// Returns all four cardinal directions in a random order.
        /// </summary>
        /// <returns>A shuffled array of directions.</returns>
        private Direction[] GetShuffledDirections()
        {
            Direction[] directions =
            {
                Direction.North,
                Direction.South,
                Direction.East,
                Direction.West
            };

            for (int i = directions.Length - 1; i > 0; i--)
            {
                int swapIndex = myRandom.Next(i + 1);
                (directions[i], directions[swapIndex]) = (directions[swapIndex], directions[i]);
            }

            return directions;
        }

        /// <summary>
        /// Retrieves the neighboring room in the given direction, or null
        /// if no room exists in that direction.
        /// </summary>
        /// <param name="theRoom">The room to check from.</param>
        /// <param name="theDirection">The direction to look.</param>
        /// <returns>The neighboring room, or null.</returns>
        private Room GetNeighbor(Room theRoom, Direction theDirection)
        {
            return theRoom.GetNeighbor(theDirection);
        }

        /// <summary>
        /// Opens the walls between two adjacent rooms in the given direction,
        /// carving a passage connecting them.
        /// </summary>
        /// <param name="theFrom">The room being carved from.</param>
        /// <param name="theTo">The neighboring room being carved to.</param>
        /// <param name="theDirection">The direction from theFrom to theTo.</param>
        private void CarveConnection(Room theFrom, Room theTo, Direction theDirection)
        {
            theFrom.SetWall(theDirection, false);
            theTo.SetWall(Room.GetOppositeDirection(theDirection), false);
        }

        /// <summary>
        /// Confirms the exit room is reachable from the entrance by
        /// traversing only open (carved) passages. Acts as a safety
        /// check on the generation algorithm.
        /// </summary>
        /// <param name="theDungeon">The dungeon map to check.</param>
        /// <returns>True if the exit can be reached from the entrance.</returns>
        private bool IsExitReachable(DungeonMap theDungeon)
        {
            HashSet<Room> visited = new HashSet<Room>();
            Queue<Room> queue = new Queue<Room>();

            queue.Enqueue(theDungeon.Entrance);
            visited.Add(theDungeon.Entrance);

            while (queue.Count > 0)
            {
                Room current = queue.Dequeue();

                if (current == theDungeon.Exit)
                {
                    return true;
                }

                foreach (Room passableNeighbor in GetPassableNeighbors(current))
                {
                    if (passableNeighbor != null && visited.Add(passableNeighbor))
                    {
                        queue.Enqueue(passableNeighbor);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the neighbors of the given room that are reachable
        /// through an open (non-walled) passage.
        /// </summary>
        /// <param name="theRoom">The room to check neighbors of.</param>
        /// <returns>The set of passable neighboring rooms.</returns>
        private IEnumerable<Room> GetPassableNeighbors(Room theRoom)
        {
            Direction[] directions = { Direction.North, Direction.South, Direction.East, Direction.West };

            foreach (Direction direction in directions)
            {
                if (!theRoom.GetWall(direction))
                {
                    yield return theRoom.GetNeighbor(direction);
                }
            }
        }
    }
}