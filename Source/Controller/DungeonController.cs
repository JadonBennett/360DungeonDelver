// Project: TCSS 360 Dungeon Adventure
// File: DungeonController.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using DungeonDelver.Dungeon;
using DungeonDelver.Source.Model;
using Godot;


namespace DungeonDelver.Source.Controller
{
    /// <summary>
    /// Top-level MVC orchestrator for the dungeon adventure game.
    /// This is the only class that touches the Model layer directly from outside it,
    /// coordinating between game state, combat, dungeon navigation, and the view.
    /// </summary>
    public partial class DungeonController : GodotObject
    {
        /// <summary>
        /// The generated dungeon map for the current game.
        /// </summary>
        private DungeonMap myDungeon;

        /// <summary>
        /// The player's hero character.
        /// </summary>
        private Hero myHero;

        /// <summary>
        /// The room the hero is currently in.
        /// </summary>
        private Room myCurrentRoom;

        /// <summary>
        /// The maze generator used to create dungeons.
        /// </summary>
        private readonly MazeGenerator myMazeGenerator;

        /// <summary>
        /// Combat manager for handling battles.
        /// </summary>
        private readonly CombatManager myCombatManager;

        /// <summary>
        /// Initializes a new DungeonController.
        /// </summary>
        public DungeonController()
        {
            myMazeGenerator = new MazeGenerator();
            myCombatManager = new CombatManager();
        }

        /// <summary>
        /// Creates a new game with the specified hero and dungeon size.
        /// </summary>
        /// <param name="theHeroName">The name of the hero.</param>
        /// <param name="theHeroClass">The class (Warrior, Priestess, or Thief).</param>
        /// <param name="thePillarType">The pillar this dungeon grants.</param>
        /// <param name="theWidth">The dungeon width (default 5).</param>
        /// <param name="theHeight">The dungeon height (default 5).</param>
        public void CreateNewGame(string theHeroName, string theHeroClass, PillarType thePillarType,
            int theWidth = 5, int theHeight = 5)
        {
            // Create the hero based on class
            myHero = CreateHero(theHeroName, theHeroClass);

            // Generate the dungeon, guaranteeing one pillar of the given type
            myDungeon = myMazeGenerator.Generate(theWidth, theHeight, thePillarType);

            // Start at the entrance
            myCurrentRoom = myDungeon.Entrance;
        }

        /// <summary>
        /// Gets information about the current room.
        /// </summary>
        /// <returns>A dictionary with room info for Godot to display.</returns>
        public Godot.Collections.Dictionary GetCurrentRoomInfo()
        {
            var info = new Godot.Collections.Dictionary
            {
                { "x", myCurrentRoom.X },
                { "y", myCurrentRoom.Y },
                { "type", myCurrentRoom.Type.ToString() },
                { "north_wall", myCurrentRoom.NorthWall },
                { "south_wall", myCurrentRoom.SouthWall },
                { "east_wall", myCurrentRoom.EastWall },
                { "west_wall", myCurrentRoom.WestWall }
            };

            return info;
        }

        /// <summary>
        /// Gets the hero's current stats.
        /// </summary>
        /// <returns>A dictionary with hero info for Godot to display.</returns>
        public Godot.Collections.Dictionary GetHeroInfo()
        {
            if (myHero == null)
            {
                return new Godot.Collections.Dictionary();
            }

            var info = new Godot.Collections.Dictionary
            {
                { "name", myHero.Name },
                { "hp", myHero.HitPoints },
                { "max_hp", myHero.MaxHitPoints },
                { "pillars", myHero.PillarsCollected }
            };

            return info;
        }

        /// <summary>
        /// Gets detailed hero statistics including combat stats and special abilities.
        /// </summary>
        /// <returns>A dictionary with detailed hero stats for display.</returns>
        public Godot.Collections.Dictionary GetDetailedHeroStats()
        {
            if (myHero == null)
            {
                return new Godot.Collections.Dictionary();
            }

            var stats = new Godot.Collections.Dictionary
            {
                { "name", myHero.Name },
                { "hp", myHero.HitPoints },
                { "max_hp", myHero.MaxHitPoints },
                { "attack_speed", myHero.AttackSpeed },
                { "hit_chance", myHero.ChanceToHit },
                { "block_chance", myHero.BlockChance },
                { "special_skill", myHero.UseSpecialSkill() },
                { "pillars_collected", myHero.PillarsCollected }
            };

            return stats;
        }

        /// <summary>
        /// Gets the contents of the current room (items, monsters, pillars).
        /// </summary>
        /// <returns>A dictionary describing room contents.</returns>
        public Godot.Collections.Dictionary GetRoomContents()
        {
            if (myCurrentRoom == null)
            {
                return new Godot.Collections.Dictionary();
            }

            var items = new Godot.Collections.Array();
            var pillars = new Godot.Collections.Array();

            if (myCurrentRoom.Item is Pillar pillar)
            {
                pillars.Add(pillar.PillarType.ToString());
            }
            else if (myCurrentRoom.Item != null)
            {
                items.Add(myCurrentRoom.Item.Name);
            }

            var contents = new Godot.Collections.Dictionary
            {
                { "items", items },
                { "monsters", new Godot.Collections.Array() },
                { "pillars", pillars },
                { "has_content", myCurrentRoom.Item != null }
            };

            return contents;
        }

        /// <summary>
        /// Gets the hero's inventory of items and collected pillars.
        /// </summary>
        /// <returns>A dictionary with inventory information.</returns>
        public Godot.Collections.Dictionary GetInventory()
        {
            if (myHero == null)
            {
                return new Godot.Collections.Dictionary();
            }

            var itemNames = new Godot.Collections.Array();
            foreach (Item item in myHero.Inventory)
            {
                itemNames.Add(item.Name);
            }

            var inventory = new Godot.Collections.Dictionary
            {
                { "items", itemNames },
                { "pillars_collected", myHero.PillarsCollected }
            };

            return inventory;
        }

        /// <summary>
        /// Gets the current combat state if in combat.
        /// </summary>
        /// <returns>A dictionary with combat information, or empty if not in combat.</returns>
        public Godot.Collections.Dictionary GetCombatState()
        {
            // Placeholder - will be implemented when combat system is active
            var combatState = new Godot.Collections.Dictionary
            {
                { "in_combat", false },
                { "monster_name", "" },
                { "monster_hp", 0 },
                { "monster_max_hp", 0 },
                { "combat_log", new Godot.Collections.Array() }
            };

            return combatState;
        }

        /// <summary>
        /// Attempts to move the hero in the specified direction.
        /// On success, automatically collects any pillar in the destination room.
        /// </summary>
        /// <param name="theDirection">The direction to move (North, South, East, West).</param>
        /// <returns>True if movement was successful, false if blocked by wall.</returns>
        public bool MovePlayer(string theDirection)
        {
            Direction direction;

            switch (theDirection.ToLower())
            {
                case "north":
                    direction = Direction.North;
                    break;
                case "south":
                    direction = Direction.South;
                    break;
                case "east":
                    direction = Direction.East;
                    break;
                case "west":
                    direction = Direction.West;
                    break;
                default:
                    return false;
            }

            // Check if there's a wall blocking
            if (IsWallBlocking(direction))
            {
                return false;
            }

            // Get the neighbor room
            Room nextRoom = GetNeighborRoom(direction);

            if (nextRoom == null)
            {
                return false;
            }

            // Move to the new room
            myCurrentRoom = nextRoom;

            CollectPillarIfPresent();

            return true;
        }

        /// <summary>
        /// Automatically collects the current room's pillar, if one is present,
        /// recording it on the hero and clearing it from the room.
        /// </summary>
        private void CollectPillarIfPresent()
        {
            if (myCurrentRoom.Item is Pillar pillar)
            {
                pillar.Use(myHero);
                myCurrentRoom.Item = null;
            }
        }

        /// <summary>
        /// Checks if the win condition is met (at exit with all pillars).
        /// </summary>
        /// <returns>True if the hero has won the game.</returns>
        public bool CheckWinCondition()
        {
            if (myCurrentRoom == null || myHero == null)
            {
                return false;
            }

            return myCurrentRoom.Type == RoomType.Exit;
        }

        /// <summary>
        /// Gets a simple text-based map representation for debugging.
        /// </summary>
        /// <returns>ASCII art map of the dungeon.</returns>
        public string GetMapDebugString()
        {
            if (myDungeon == null)
            {
                return "No dungeon generated";
            }

            string result = "";

            for (int y = 0; y < myDungeon.Height; y++)
            {
                for (int x = 0; x < myDungeon.Width; x++)
                {
                    Room room = myDungeon.GetRoom(x, y);

                    // Mark current room with @
                    if (room == myCurrentRoom)
                    {
                        result += "[@]";
                    }
                    // Mark entrance with E
                    else if (room.Type == RoomType.Entrance)
                    {
                        result += "[E]";
                    }
                    // Mark exit with X
                    else if (room.Type == RoomType.Exit)
                    {
                        result += "[X]";
                    }
                    // Normal rooms
                    else
                    {
                        result += "[ ]";
                    }
                }
                result += "\n";
            }

            return result;
        }

        /// <summary>
        /// Checks if there's a wall blocking movement in the given direction.
        /// </summary>
        /// <param name="theDirection">The direction to check.</param>
        /// <returns>True if a wall is blocking.</returns>
        private bool IsWallBlocking(Direction theDirection)
        {
            switch (theDirection)
            {
                case Direction.North:
                    return myCurrentRoom.NorthWall;
                case Direction.South:
                    return myCurrentRoom.SouthWall;
                case Direction.East:
                    return myCurrentRoom.EastWall;
                case Direction.West:
                    return myCurrentRoom.WestWall;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Gets the neighbor room in the specified direction.
        /// </summary>
        /// <param name="theDirection">The direction to look.</param>
        /// <returns>The neighbor room, or null if none exists.</returns>
        private Room GetNeighborRoom(Direction theDirection)
        {
            switch (theDirection)
            {
                case Direction.North:
                    return myCurrentRoom.North;
                case Direction.South:
                    return myCurrentRoom.South;
                case Direction.East:
                    return myCurrentRoom.East;
                case Direction.West:
                    return myCurrentRoom.West;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates a hero of the specified class.
        /// </summary>
        /// <param name="theName">The hero's name.</param>
        /// <param name="theClass">The hero class.</param>
        /// <returns>A new hero instance.</returns>
        private Hero CreateHero(string theName, string theClass)
        {
            switch (theClass.ToLower())
            {
                case "warrior":
                    return new Warrior(theName);
                case "priestess":
                    return new Priestess(theName);
                case "thief":
                    return new Thief(theName);
                default:
                    return new Warrior(theName);
            }
        }
    }
}