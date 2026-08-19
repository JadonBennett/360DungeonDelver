// Project: TCSS 360 Dungeon Adventure
// File: DungeonController.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using DungeonDelver.Dungeon;
using DungeonDelver.Source.Model;
using DungeonDelver.Source.Persistence;
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
        /// Emitted when the hero moves to a new room.
        /// </summary>
        [Signal]
        public delegate void RoomChangedEventHandler();

        /// <summary>
        /// Emitted when the hero collects a pillar.
        /// </summary>
        [Signal]
        public delegate void PillarCollectedEventHandler(string thePillarType);

        /// <summary>
        /// Emitted when the hero's hit points change.
        /// </summary>
        [Signal]
        public delegate void HeroHealthChangedEventHandler(int theNewHp);

        /// <summary>
        /// Emitted when the hero reaches the exit and wins.
        /// </summary>
        [Signal]
        public delegate void GameWonEventHandler();

        /// <summary>
        /// Emitted when the hero encounters a monster and combat begins.
        /// </summary>
        [Signal]
        public delegate void CombatStartedEventHandler(string theMonsterName);

        /// <summary>
        /// Emitted when combat ends, indicating whether the hero survived.
        /// </summary>
        [Signal]
        public delegate void CombatEndedEventHandler(bool theVictory);

        /// <summary>
        /// The generated dungeon map for the current game.
        /// </summary>
        private DungeonMap myDungeon;

        /// <summary>
        /// The player's hero character.
        /// </summary>
        private Hero myHero;

        /// <summary>
        /// The navigator that handles dungeon movement.
        /// </summary>
        private DungeonNavigator myNavigator;

        /// <summary>
        /// The maze generator used to create dungeons.
        /// </summary>
        private readonly MazeGenerator myMazeGenerator;

        /// <summary>
        /// Combat manager for handling battles.
        /// </summary>
        private readonly CombatManager myCombatManager;

        /// <summary>
        /// Factory used to create monsters for room content generation and combat.
        /// </summary>
        private readonly MonsterFactory myMonsterFactory;

        /// <summary>
        /// Generator that populates rooms with pits, monsters, and items after
        /// the maze is carved.
        /// </summary>
        private readonly RoomContentGenerator myRoomContentGenerator;

        /// <summary>
        /// The monster currently engaged in combat, if any. Tracked separately
        /// from CombatManager.Monster so it can be removed from its room on defeat.
        /// </summary>
        private Monster myCombatMonster;

        /// <summary>
        /// Initializes a new DungeonController.
        /// </summary>
        public DungeonController()
        {
            myMazeGenerator = new MazeGenerator();
            myCombatManager = new CombatManager();
            myMonsterFactory = new MonsterFactory(new SqliteGameRepository("user://dungeondelver.db"));
            myRoomContentGenerator = new RoomContentGenerator(() => myMonsterFactory.CreateRandomMonster());
        }

        /// <summary>
        /// Creates a new game with the specified hero and dungeon size.
        /// </summary>
        /// <param name="theHeroName">The name of the hero.</param>
        /// <param name="theHeroClass">The class (Warrior, Priestess, or Thief).</param>
        /// <param name="thePillarType">The pillar this dungeon grants (e.g. "Abstraction").</param>
        /// <param name="theWidth">The dungeon width (default 5).</param>
        /// <param name="theHeight">The dungeon height (default 5).</param>
        public void CreateNewGame(string theHeroName, string theHeroClass, string thePillarType,
            int theWidth = 5, int theHeight = 5)
        {
            // Create the hero based on class
            myHero = CreateHero(theHeroName, theHeroClass);

            // Generate the dungeon, guaranteeing one pillar of the given type
            PillarType pillarType = Enum.Parse<PillarType>(thePillarType, true);
            myDungeon = myMazeGenerator.Generate(theWidth, theHeight, pillarType);

            // Populate rooms with pits, monsters, and items
            myRoomContentGenerator.Populate(myDungeon);

            // Create navigator starting at entrance
            myNavigator = new DungeonNavigator(myDungeon);
        }

        /// <summary>
        /// Gets information about the current room.
        /// </summary>
        /// <returns>A dictionary with room info for Godot to display.</returns>
        public Godot.Collections.Dictionary GetCurrentRoomInfo()
        {
            if (myNavigator == null || myNavigator.CurrentRoom == null)
            {
                return new Godot.Collections.Dictionary();
            }

            Room currentRoom = myNavigator.CurrentRoom;

            var info = new Godot.Collections.Dictionary
            {
                { "x", currentRoom.X },
                { "y", currentRoom.Y },
                { "type", currentRoom.Type.ToString() },
                { "north_wall", currentRoom.NorthWall },
                { "south_wall", currentRoom.SouthWall },
                { "east_wall", currentRoom.EastWall },
                { "west_wall", currentRoom.WestWall }
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
                { "special_skill", myHero.SpecialSkillName },
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
            if (myNavigator == null || myNavigator.CurrentRoom == null)
            {
                return new Godot.Collections.Dictionary();
            }

            Room currentRoom = myNavigator.CurrentRoom;

            var items = new Godot.Collections.Array();
            var pillars = new Godot.Collections.Array();
            var monsters = new Godot.Collections.Array();

            if (currentRoom.Item is Pillar pillar)
            {
                pillars.Add(pillar.PillarType.ToString());
            }
            else if (currentRoom.Item != null)
            {
                items.Add(currentRoom.Item.Name);
            }

            foreach (Monster monster in currentRoom.Monsters)
            {
                if (monster.IsAlive)
                {
                    monsters.Add(monster.Name);
                }
            }

            bool hasContent = currentRoom.Item != null || monsters.Count > 0 || currentRoom.Pit != null;

            var contents = new Godot.Collections.Dictionary
            {
                { "items", items },
                { "monsters", monsters },
                { "pillars", pillars },
                { "has_pit", currentRoom.Pit != null },
                { "has_content", hasContent }
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
            var log = new Godot.Collections.Array();
            foreach (string entry in myCombatManager.CombatLog)
            {
                log.Add(entry);
            }

            var combatState = new Godot.Collections.Dictionary
            {
                { "in_combat", myCombatManager.InCombat },
                { "monster_name", myCombatManager.Monster?.Name ?? "" },
                { "monster_hp", myCombatManager.Monster?.HitPoints ?? 0 },
                { "monster_max_hp", myCombatManager.Monster?.MaxHitPoints ?? 0 },
                { "combat_log", log }
            };

            return combatState;
        }

        /// <summary>
        /// Attempts to move the hero in the specified direction.
        /// On success, automatically collects any pillar in the destination room
        /// and emits signals for the room change and, if applicable, the win condition.
        /// </summary>
        /// <param name="theDirection">The direction to move (North, South, East, West).</param>
        /// <returns>True if movement was successful, false if blocked by wall.</returns>
        public bool MovePlayer(string theDirection)
        {
            if (myNavigator == null || myCombatManager.InCombat)
            {
                return false;
            }

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

            // Delegate movement to Navigator
            bool moved = myNavigator.TryMove(direction);

            if (moved)
            {
                CollectPillarIfPresent();

                EmitSignal(SignalName.RoomChanged);

                if (CheckWinCondition())
                {
                    EmitSignal(SignalName.GameWon);
                }
                else
                {
                    StartCombatIfMonsterPresent();
                }
            }

            return moved;
        }

        /// <summary>
        /// Starts combat with the first alive monster in the current room, if any.
        /// </summary>
        private void StartCombatIfMonsterPresent()
        {
            Monster monster = FindAliveMonsterInCurrentRoom();

            if (monster != null)
            {
                myCombatMonster = monster;
                myCombatManager.StartCombat(myHero, monster);
                EmitSignal(SignalName.CombatStarted, monster.Name);
            }
        }

        /// <summary>
        /// Finds the first living monster in the hero's current room.
        /// </summary>
        /// <returns>The first alive monster in the current room, or null if none.</returns>
        private Monster FindAliveMonsterInCurrentRoom()
        {
            foreach (Monster monster in myNavigator.CurrentRoom.Monsters)
            {
                if (monster.IsAlive)
                {
                    return monster;
                }
            }

            return null;
        }

        /// <summary>
        /// Performs the hero's chosen action during combat, then resolves the
        /// monster's response. If the monster is defeated and another alive
        /// monster remains in the room, the next fight begins immediately.
        /// Fleeing ends combat without removing the (still alive) monster
        /// from the room.
        /// </summary>
        /// <param name="theAction">The action to perform (e.g., "attack", "special", "use item", "run").</param>
        public void PerformCombatAction(string theAction)
        {
            if (!myCombatManager.InCombat)
            {
                return;
            }

            myCombatManager.PerformPlayerTurn(theAction);

            if (!myCombatManager.InCombat)
            {
                bool heroSurvived = myHero.IsAlive;
                bool monsterDefeated = heroSurvived && myCombatMonster != null && !myCombatMonster.IsAlive;

                if (monsterDefeated)
                {
                    myNavigator.CurrentRoom.RemoveMonster(myCombatMonster);
                    myCombatMonster = null;

                    Monster nextMonster = FindAliveMonsterInCurrentRoom();
                    if (nextMonster != null)
                    {
                        myCombatMonster = nextMonster;
                        myCombatManager.StartCombat(myHero, nextMonster);
                        EmitSignal(SignalName.CombatStarted, nextMonster.Name);
                        return;
                    }
                }
                else if (heroSurvived)
                {
                    myCombatMonster = null;
                }

                EmitSignal(SignalName.CombatEnded, heroSurvived);
            }
        }

        /// <summary>
        /// True if the hero is currently engaged in combat.
        /// </summary>
        /// <returns>True if combat is in progress.</returns>
        public bool IsInCombat()
        {
            return myCombatManager.InCombat;
        }

        /// <summary>
        /// Automatically collects the current room's pillar, if one is present,
        /// recording it on the hero and clearing it from the room. The hero's
        /// PillarCollected event, subscribed to in CreateHero, forwards this
        /// as a Godot signal.
        /// </summary>
        private void CollectPillarIfPresent()
        {
            Room currentRoom = myNavigator.CurrentRoom;

            if (currentRoom.Item is Pillar pillar)
            {
                pillar.Use(myHero);
                currentRoom.Item = null;
            }
        }

        /// <summary>
        /// Checks if the win condition is met (at exit with all pillars).
        /// </summary>
        /// <returns>True if the hero has won the game.</returns>
        public bool CheckWinCondition()
        {
            if (myNavigator == null || myNavigator.CurrentRoom == null || myHero == null)
            {
                return false;
            }

            return myNavigator.CurrentRoom.Type == RoomType.Exit && myHero.HasAllPillars();
        }

        /// <summary>
        /// Checks if the hero has died (HP at or below zero).
        /// </summary>
        /// <returns>True if the hero is dead.</returns>
        public bool IsHeroDead()
        {
            if (myHero == null)
            {
                return false;
            }

            return !myHero.IsAlive;
        }

        // ========== DEBUG METHODS ==========

        /// <summary>
        /// DEBUG: Summarizes the pits, monsters, items, and pillars placed
        /// across the entire generated dungeon, regardless of what the hero
        /// has explored so far.
        /// </summary>
        /// <returns>A human-readable summary string.</returns>
        public string DebugGetDungeonSummary()
        {
            if (myDungeon == null)
            {
                return "No dungeon generated";
            }

            int pitCount = 0;
            int monsterCount = 0;
            int itemCount = 0;
            int pillarCount = 0;

            for (int x = 0; x < myDungeon.Width; x++)
            {
                for (int y = 0; y < myDungeon.Height; y++)
                {
                    Room room = myDungeon.GetRoom(x, y);

                    if (room.Pit != null)
                    {
                        pitCount++;
                    }

                    foreach (Monster monster in room.Monsters)
                    {
                        if (monster.IsAlive)
                        {
                            monsterCount++;
                        }
                    }

                    if (room.Item is Pillar)
                    {
                        pillarCount++;
                    }
                    else if (room.Item != null)
                    {
                        itemCount++;
                    }
                }
            }

            return $"Dungeon {myDungeon.Width}x{myDungeon.Height} | Pits: {pitCount} | Monsters: {monsterCount} | Items: {itemCount} | Pillars: {pillarCount}";
        }

        /// <summary>
        /// DEBUG: Sets the hero's HP to a specific value.
        /// Bypasses blocking to ensure HP is set correctly.
        /// </summary>
        /// <param name="theHP">The HP value to set.</param>
        public void DebugSetHeroHP(int theHP)
        {
            if (myHero == null) return;

            // Use DebugSetHP to bypass blocking
            myHero.DebugSetHP(theHP);
        }

        /// <summary>
        /// DEBUG: Damages the hero by a specific amount.
        /// </summary>
        /// <param name="theDamage">The amount of damage to deal.</param>
        public void DebugDamageHero(int theDamage)
        {
            if (myHero == null) return;
            myHero.ChangeHealth(-theDamage);
        }

        /// <summary>
        /// DEBUG: Heals the hero by a specific amount.
        /// </summary>
        /// <param name="theAmount">The amount to heal.</param>
        public void DebugHealHero(int theAmount)
        {
            if (myHero == null) return;
            myHero.ChangeHealth(theAmount);
        }

        /// <summary>
        /// DEBUG: Sets the number of pillars collected to exactly the specified count.
        /// </summary>
        /// <param name="theCount">The number of pillars (0-4).</param>
        public void DebugSetPillars(int theCount)
        {
            if (myHero == null) return;

            // Clear existing pillars first
            myHero.DebugClearPillars();

            // Add the requested number of pillars
            for (int i = 0; i < theCount && i < 4; i++)
            {
                myHero.CollectPillar((PillarType)i);
            }
        }

        /// <summary>
        /// DEBUG: Teleports the hero to the exit room.
        /// </summary>
        public void DebugTeleportToExit()
        {
            if (myNavigator == null || myDungeon == null) return;
            myNavigator.TeleportTo(myDungeon.Exit);
        }

        /// <summary>
        /// DEBUG: Teleports the hero to the entrance room.
        /// </summary>
        public void DebugTeleportToEntrance()
        {
            if (myNavigator == null || myDungeon == null) return;
            myNavigator.TeleportTo(myDungeon.Entrance);
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
                    if (room == myNavigator.CurrentRoom)
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
        /// Creates a hero of the specified class.
        /// </summary>
        /// <param name="theName">The hero's name.</param>
        /// <param name="theClass">The hero class.</param>
        /// <returns>A new hero instance.</returns>
        private Hero CreateHero(string theName, string theClass)
        {
            Hero hero = theClass.ToLower() switch
            {
                "warrior" => new Warrior(theName),
                "priestess" => new Priestess(theName),
                "thief" => new Thief(theName),
                _ => new Warrior(theName)
            };

            hero.HealthChanged += (sender, newHp) => EmitSignal(SignalName.HeroHealthChanged, newHp);
            hero.PillarCollected += (sender, pillarType) => EmitSignal(SignalName.PillarCollected, pillarType.ToString());

            return hero;
        }
    }
}
