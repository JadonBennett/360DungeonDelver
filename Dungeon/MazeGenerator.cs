// Project: TCSS 360 Dungeon Adventure
// File: MazeGenerator.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

//imports
using System;
using System.Collections.Generic;
using DungeonDelver.Source.Model;

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
        /// Places the given number of pillar items into random normal rooms
        /// once the maze is confirmed solvable.
        /// </summary>
        /// <param name="theWidth">The width of the dungeon grid.</param>
        /// <param name="theHeight">The height of the dungeon grid.</param>
        /// <param name="thePillarType">The pillar type this dungeon grants.</param>
        /// <param name="thePillarCount">The number of pillar items to place. Defaults to 1.</param>
        /// <returns>A fully generated DungeonMap with carved passages and placed pillars.</returns>
        public DungeonMap Generate(int theWidth, int theHeight, PillarType thePillarType, int thePillarCount = 1)
        {
            DungeonMap newDungeon;

            do
            {
                newDungeon = new DungeonMap(theWidth, theHeight);
                CreateMaze(newDungeon);
            }
            while (!IsExitReachable(newDungeon));

            PlacePillars(newDungeon, thePillarType, thePillarCount);

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
        /// Places the given number of pillar items into random normal rooms
        /// (excluding the entrance and exit) in the dungeon.
        /// </summary>
        /// <param name="theDungeon">The dungeon map to place pillars within.</param>
        /// <param name="thePillarType">The pillar type to place.</param>
        /// <param name="thePillarCount">The number of pillar items to place.</param>
        private void PlacePillars(DungeonMap theDungeon, PillarType thePillarType, int thePillarCount)
        {
            List<Room> eligibleRooms = new List<Room>();

            foreach (Room room in theDungeon.GetRooms())
            {
                if (room.Type == RoomType.Normal)
                {
                    eligibleRooms.Add(room);
                }
            }

            for (int i = eligibleRooms.Count - 1; i > 0; i--)
            {
                int swapIndex = myRandom.Next(i + 1);
                (eligibleRooms[i], eligibleRooms[swapIndex]) = (eligibleRooms[swapIndex], eligibleRooms[i]);
            }

            int placeCount = Math.Min(thePillarCount, eligibleRooms.Count);

            for (int i = 0; i < placeCount; i++)
            {
                eligibleRooms[i].Item = new Pillar(thePillarType);
            }
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
            switch (theDirection)
            {
                case Direction.North: return theRoom.North;
                case Direction.South: return theRoom.South;
                case Direction.East: return theRoom.East;
                case Direction.West: return theRoom.West;
                default: return null;
            }
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
            switch (theDirection)
            {
                case Direction.North:
                    theFrom.NorthWall = false;
                    theTo.SouthWall = false;
                    break;
                case Direction.South:
                    theFrom.SouthWall = false;
                    theTo.NorthWall = false;
                    break;
                case Direction.East:
                    theFrom.EastWall = false;
                    theTo.WestWall = false;
                    break;
                case Direction.West:
                    theFrom.WestWall = false;
                    theTo.EastWall = false;
                    break;
            }
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
            if (!theRoom.NorthWall)
            {
                yield return theRoom.North;
            }

            if (!theRoom.SouthWall)
            {
                yield return theRoom.South;
            }

            if (!theRoom.EastWall)
            {
                yield return theRoom.East;
            }

            if (!theRoom.WestWall)
            {
                yield return theRoom.West;
            }
        }
    }
}