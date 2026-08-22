// Project: TCSS 360 Dungeon Adventure
// File: DungeonController.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

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
		/// Emitted when a combat encounter ends, indicating the outcome.
		/// </summary>
		[Signal]
		public delegate void CombatEndedEventHandler(string theOutcome);

		/// <summary>
		/// Emitted when the hero's hit points reach zero.
		/// </summary>
		[Signal]
		public delegate void HeroDefeatedEventHandler();

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
		/// The monster factory used to generate enemy entities from database records.
		/// </summary>
		private readonly MonsterFactory myMonsterFactory;

		/// <summary>
		/// The game repository handling data access layer operations and SQLite queries.
		/// </summary>
		private readonly IGameRepository myRepository;


		/// <summary>
		/// REQUIRED BY GODOT: Parameterless constructor allowing the Godot Engine
		/// to safely instantiate this object internally.
		/// Note: Database must be initialized via DatabaseInitializer before creating this object.
		/// </summary>
		public DungeonController()
		{
			DatabaseInitializer.EnsureInitialized();
			
			string dbPath = System.IO.Path.Combine(Godot.OS.GetUserDataDir(), "dungeon.db");

			myRepository = new SqliteGameRepository(dbPath);
			myMonsterFactory = new MonsterFactory(myRepository);
			myMazeGenerator = new MazeGenerator();
			myCombatManager = new CombatManager();

			myCombatManager.CombatEnded += (sender, outcome) =>
			{
				EmitSignal(SignalName.CombatEnded, outcome.ToString());

				if (outcome == CombatManager.CombatOutcome.PlayerDefeated)
				{
					EmitSignal(SignalName.HeroDefeated);
				}
				else if (outcome == CombatManager.CombatOutcome.PlayerWon)
				{
					// Remove defeated monster from room
					if (myCombatManager.Monster is Monster defeatedMonster && myNavigator != null)
					{
						myNavigator.CurrentRoom.Monster = null;
					}
				}
			};
		}

		/// <summary>
		/// Initializes a new DungeonController with database repository infrastructure.
		/// </summary>
		/// <param name="theRepository">The persistent data storage repository.</param>
		public DungeonController(IGameRepository theRepository)
		{
			DatabaseInitializer.EnsureInitialized();
			myRepository = theRepository;
			myMonsterFactory = new MonsterFactory(myRepository);

			myMazeGenerator = new MazeGenerator();
			myCombatManager = new CombatManager();

			myCombatManager.CombatEnded += (sender, outcome) =>
			{
				EmitSignal(SignalName.CombatEnded, outcome.ToString());

				if (outcome == CombatManager.CombatOutcome.PlayerDefeated)
				{
					EmitSignal(SignalName.HeroDefeated);
				}
				else if (outcome == CombatManager.CombatOutcome.PlayerWon)
				{
					// Remove defeated monster from room
					if (myCombatManager.Monster is Monster defeatedMonster && myNavigator != null)
					{
						myNavigator.CurrentRoom.Monster = null;
					}
				}
			};
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
			// and populating it with monsters
			myDungeon = myMazeGenerator.Generate(theWidth, theHeight, thePillarType, 1,
				() => myMonsterFactory.CreateRandomMonster(thePillarType));

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
			
			if (currentRoom.Monster != null)
			{
				monsters.Add(currentRoom.Monster.Name);
			}

			var contents = new Godot.Collections.Dictionary
			{
				{ "items", items },
				{ "monsters", monsters},
				{ "pillars", pillars },
				{ "has_content", currentRoom.Item != null || currentRoom.Monster != null }
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
			// Convert combat log to Godot array
			var logArray = new Godot.Collections.Array();
			if (myCombatManager.CombatLog != null)
			{
				foreach (string logEntry in myCombatManager.CombatLog)
				{
					logArray.Add(logEntry);
				}
			}

			var combatState = new Godot.Collections.Dictionary
			{
				{ "in_combat", myCombatManager.InCombat },
				{ "monster_name", myCombatManager.Monster?.Name ?? "" },
				{ "monster_hp", myCombatManager.Monster?.HitPoints ?? 0 },
				{ "monster_max_hp", myCombatManager.Monster?.MaxHitPoints ?? 0 },
				{ "combat_log", logArray }
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
			if (myNavigator == null)
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
				Room currentRoom = myNavigator.CurrentRoom;

				// Start combat if monster is present
				if (currentRoom.Monster != null && !myCombatManager.InCombat)
				{
					myCombatManager.StartCombat(myHero, currentRoom.Monster);
				}

				CollectPillarIfPresent();

				EmitSignal(SignalName.RoomChanged);

				if (CheckWinCondition())
				{
					EmitSignal(SignalName.GameWon);
				}
			}

			return moved;
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

	public void PerformCombatAction(string theAction, int theItemIndex = -1)
	{
		if (myCombatManager == null || !myCombatManager.InCombat)
		{
			return;
		}

		string action = theAction?.ToLowerInvariant() == "item" ? "use item" : theAction;
		myCombatManager.PerformPlayerTurn(action, theItemIndex);

		if (myCombatManager.InCombat && myHero != null && myHero.IsAlive)
		{
			myCombatManager.PerformMonsterTurn();
		}
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

		
		/// <summary>
		/// Gets data describing the current dungeon map for minimap rendering.
		/// </summary>
		/// <returns>A dictionary with grid dimensions and key room coordinates, or an empty dictionary if not yet initialized.</returns>
		public Godot.Collections.Dictionary GetMinimapData()
		{
			if (myDungeon == null || myNavigator == null || myNavigator.CurrentRoom == null)
			{
				return new Godot.Collections.Dictionary();
			}

			return new Godot.Collections.Dictionary
			{
				{ "width", myDungeon.Width },
				{ "height", myDungeon.Height },
				{ "current_x", myNavigator.CurrentRoom.X },
				{ "current_y", myNavigator.CurrentRoom.Y },
				{ "exit_x", myDungeon.Exit.X },
				{ "exit_y", myDungeon.Exit.Y }
			};
		}
		
		
		// ========== DEBUG METHODS ==========

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
