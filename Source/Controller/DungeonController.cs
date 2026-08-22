// Project: TCSS 360 Dungeon Adventure
// File: DungeonController.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using DungeonDelver.Dungeon;
using DungeonDelver.Source.Interface;
using DungeonDelver.Source.Model;
using DungeonDelver.Source.Persistence;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


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
		/// Generates a dungeon containing all 4 Pillars of OOP.
		/// </summary>
		/// <param name="theHeroName">The name of the hero.</param>
		/// <param name="theHeroClass">The class (Warrior, Priestess, or Thief).</param>
		/// <param name="theWidth">The dungeon width (default 8).</param>
		/// <param name="theHeight">The dungeon height (default 8).</param>
		public void CreateNewGame(string theHeroName, string theHeroClass,
			int theWidth = 8, int theHeight = 8)
		{
			// Create the hero based on class
			myHero = CreateHero(theHeroName, theHeroClass);

			// Generate the dungeon with all 4 pillars and populate it with monsters
			myDungeon = myMazeGenerator.GenerateWithAllPillars(theWidth, theHeight,
				() => myMonsterFactory.CreateRandomMonster(PillarType.Abstraction));

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

			// Convert collected pillar types to an array of strings
			var collectedPillars = new Godot.Collections.Array();
			foreach (var pillar in myHero.CollectedPillarTypes)
			{
				collectedPillars.Add(pillar.ToString());
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
				{ "pillars_collected", myHero.PillarsCollected },
				{ "collected_pillar_types", collectedPillars }
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
				CollectItemIfPresent();

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
		/// Automatically collects the current room's item (if not a pillar),
		/// adding it to the hero's inventory and clearing it from the room.
		/// </summary>
		private void CollectItemIfPresent()
		{
			Room currentRoom = myNavigator.CurrentRoom;

			if (currentRoom.Item != null && currentRoom.Item is not Pillar)
			{
				bool added = myHero.AddItem(currentRoom.Item);
				if (added)
				{
					currentRoom.Item = null;
				}
				else
				{
					// Inventory full - item remains in room
					Console.WriteLine("Inventory full! Cannot pick up item.");
				}
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

		// Check if using a vision potion before it gets removed
		bool usingVisionPotion = false;
		if (theAction?.ToLowerInvariant() == "item" && theItemIndex >= 0
			&& theItemIndex < myHero.Inventory.Count)
		{
			usingVisionPotion = myHero.Inventory[theItemIndex] is VisionPotion;
		}

		string action = theAction?.ToLowerInvariant() == "item" ? "use item" : theAction;
		myCombatManager.PerformPlayerTurn(action, theItemIndex);

		// Apply vision potion effect after use
		if (usingVisionPotion && myNavigator != null)
		{
			myNavigator.RevealAdjacentRooms();
			EmitSignal(SignalName.RoomChanged);
		}

		if (myCombatManager.InCombat && myHero != null && myHero.IsAlive)
		{
			myCombatManager.PerformMonsterTurn();
		}
		}

		/// <summary>
		/// Uses an item from the hero's inventory outside of combat.
		/// </summary>
		/// <param name="theItemIndex">The index of the item to use.</param>
		/// <returns>A message describing the result of using the item.</returns>
		public string UseInventoryItem(int theItemIndex)
		{
			if (myHero == null)
			{
				return "No hero available.";
			}

			if (theItemIndex < 0 || theItemIndex >= myHero.Inventory.Count)
			{
				return "Invalid item index.";
			}

			Item item = myHero.Inventory[theItemIndex];

			// Special handling for VisionPotion - reveal adjacent rooms
			if (item is VisionPotion && myNavigator != null)
			{
				myNavigator.RevealAdjacentRooms();
			}

			string result = item.Use(myHero);
			myHero.RemoveItem(item);

			// Emit room changed signal to update minimap
			if (item is VisionPotion)
			{
				EmitSignal(SignalName.RoomChanged);
			}

			return result;
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
		/// Includes room coordinates containing monsters and items for debug visualization.
		/// </summary>
		/// <returns>A dictionary with grid dimensions and key room coordinates, or an empty dictionary if not yet initialized.</returns>
		public Godot.Collections.Dictionary GetMinimapData()
		{
			if (myDungeon == null || myNavigator == null || myNavigator.CurrentRoom == null)
			{
				return new Godot.Collections.Dictionary();
			}

			// Collect coordinates of rooms with monsters and items
			var monsterRooms = new Godot.Collections.Array();
			var itemRooms = new Godot.Collections.Array();
			var pillarRooms = new Godot.Collections.Array();
			var visitedRooms = new Godot.Collections.Array();

			// Add visited room coordinates
			foreach (Room visitedRoom in myNavigator.VisitedRooms)
			{
				visitedRooms.Add(new Godot.Collections.Dictionary
				{
					{ "x", visitedRoom.X },
					{ "y", visitedRoom.Y }
				});
			}

			// Collect wall data for each room
			var roomWalls = new Godot.Collections.Array();

			for (int y = 0; y < myDungeon.Height; y++)
			{
				for (int x = 0; x < myDungeon.Width; x++)
				{
					Room room = myDungeon.GetRoom(x, y);

					// Add room wall information
					roomWalls.Add(new Godot.Collections.Dictionary
					{
						{ "x", x },
						{ "y", y },
						{ "north_wall", room.NorthWall },
						{ "south_wall", room.SouthWall },
						{ "east_wall", room.EastWall },
						{ "west_wall", room.WestWall }
					});

					if (room.Monster != null)
					{
						monsterRooms.Add(new Godot.Collections.Dictionary
						{
							{ "x", x },
							{ "y", y }
						});
					}

					if (room.Item is Pillar)
					{
						pillarRooms.Add(new Godot.Collections.Dictionary
						{
							{ "x", x },
							{ "y", y }
						});
					}
					else if (room.Item != null)
					{
						itemRooms.Add(new Godot.Collections.Dictionary
						{
							{ "x", x },
							{ "y", y }
						});
					}
				}
			}

			return new Godot.Collections.Dictionary
			{
				{ "width", myDungeon.Width },
				{ "height", myDungeon.Height },
				{ "current_x", myNavigator.CurrentRoom.X },
				{ "current_y", myNavigator.CurrentRoom.Y },
				{ "exit_x", myDungeon.Exit.X },
				{ "exit_y", myDungeon.Exit.Y },
				{ "visited_rooms", visitedRooms },
				{ "monster_rooms", monsterRooms },
				{ "item_rooms", itemRooms },
				{ "pillar_rooms", pillarRooms },
				{ "room_walls", roomWalls }
			};
		}

		// ========== SAVE/LOAD METHODS ==========

		/// <summary>
		/// Saves the current game state to a file.
		/// </summary>
		/// <param name="theSaveName">The name for this save.</param>
		/// <returns>Success message or error.</returns>
		public string SaveGame(string theSaveName)
		{
			if (myHero == null || myDungeon == null || myNavigator == null)
			{
				return "No active game to save";
			}

			try
			{
				string saveDir = Path.Combine(OS.GetUserDataDir(), "saves");
				Directory.CreateDirectory(saveDir);

				int saveId = (int)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond);
				string savePath = Path.Combine(saveDir, $"save_{saveId}.json");

				// Serialize hero stats
				var heroStats = new
				{
					hero_class = myHero.GetType().Name,
					hero_name = myHero.Name,
					hero_hp = myHero.HitPoints,
					hero_max_hp = myHero.MaxHitPoints,
					attack_speed = myHero.AttackSpeed,
					chance_to_hit = myHero.ChanceToHit,
					block_chance = myHero.BlockChance,
					pillars_collected = myHero.PillarsCollected
				};

				// Serialize inventory with properties
				var inventory = myHero.Inventory.Select(item => SerializeItem(item)).ToArray();

				// Serialize visited rooms
				var visitedRooms = myNavigator.VisitedRooms.Select(r => new { x = r.X, y = r.Y }).ToArray();

				// Serialize all dungeon rooms
				var rooms = new List<object>();
				for (int y = 0; y < myDungeon.Height; y++)
				{
					for (int x = 0; x < myDungeon.Width; x++)
					{
						Room room = myDungeon.GetRoom(x, y);
						rooms.Add(new
						{
							x = room.X,
							y = room.Y,
							room_type = room.Type.ToString(),
							north_wall = room.NorthWall,
							south_wall = room.SouthWall,
							east_wall = room.EastWall,
							west_wall = room.WestWall,
							item = room.Item != null ? SerializeItem(room.Item) : null,
							monster = room.Monster != null ? SerializeMonster(room.Monster) : null
						});
					}
				}

				// Serialize combat state
				object combatState = null;
				if (myCombatManager.InCombat)
				{
					combatState = new
					{
						in_combat = true,
						turn = myCombatManager.Turn,
						monster = myCombatManager.Monster != null ? SerializeMonster(myCombatManager.Monster) : null
					};
				}

				var saveData = new
				{
					save_id = saveId,
					save_name = theSaveName,
					timestamp = DateTime.UtcNow.ToString("o"),
					hero = heroStats,
					inventory = inventory,
					current_x = myNavigator.CurrentRoom.X,
					current_y = myNavigator.CurrentRoom.Y,
					visited_rooms = visitedRooms,
					dungeon_width = myDungeon.Width,
					dungeon_height = myDungeon.Height,
					entrance_x = myDungeon.Entrance.X,
					entrance_y = myDungeon.Entrance.Y,
					exit_x = myDungeon.Exit.X,
					exit_y = myDungeon.Exit.Y,
					rooms = rooms,
					combat = combatState
				};

				string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(savePath, json);

				return $"Game saved: {theSaveName}";
			}
			catch (Exception ex)
			{
				return $"Save failed: {ex.Message}";
			}
		}

		/// <summary>
		/// Overwrites an existing save with new game state.
		/// </summary>
		/// <param name="theSaveId">The save ID to overwrite.</param>
		/// <param name="theSaveName">The new name for this save.</param>
		/// <returns>Success message or error.</returns>
		public string OverwriteSave(int theSaveId, string theSaveName)
		{
			// Delete the old save
			DeleteSavedGame(theSaveId);

			// Save with the same ID
			if (myHero == null || myDungeon == null || myNavigator == null)
			{
				return "No active game to save";
			}

			try
			{
				string saveDir = Path.Combine(OS.GetUserDataDir(), "saves");
				Directory.CreateDirectory(saveDir);
				string savePath = Path.Combine(saveDir, $"save_{theSaveId}.json");

				// Use same serialization as SaveGame
				var heroStats = new
				{
					hero_class = myHero.GetType().Name,
					hero_name = myHero.Name,
					hero_hp = myHero.HitPoints,
					hero_max_hp = myHero.MaxHitPoints,
					attack_speed = myHero.AttackSpeed,
					chance_to_hit = myHero.ChanceToHit,
					block_chance = myHero.BlockChance,
					pillars_collected = myHero.PillarsCollected
				};

				var inventory = myHero.Inventory.Select(item => SerializeItem(item)).ToArray();
				var visitedRooms = myNavigator.VisitedRooms.Select(r => new { x = r.X, y = r.Y }).ToArray();

				var rooms = new List<object>();
				for (int y = 0; y < myDungeon.Height; y++)
				{
					for (int x = 0; x < myDungeon.Width; x++)
					{
						Room room = myDungeon.GetRoom(x, y);
						rooms.Add(new
						{
							x = room.X,
							y = room.Y,
							room_type = room.Type.ToString(),
							north_wall = room.NorthWall,
							south_wall = room.SouthWall,
							east_wall = room.EastWall,
							west_wall = room.WestWall,
							item = room.Item != null ? SerializeItem(room.Item) : null,
							monster = room.Monster != null ? SerializeMonster(room.Monster) : null
						});
					}
				}

				object combatState = null;
				if (myCombatManager.InCombat)
				{
					combatState = new
					{
						in_combat = true,
						turn = myCombatManager.Turn,
						monster = myCombatManager.Monster != null ? SerializeMonster(myCombatManager.Monster) : null
					};
				}

				var saveData = new
				{
					save_id = theSaveId,
					save_name = theSaveName,
					timestamp = DateTime.UtcNow.ToString("o"),
					hero = heroStats,
					inventory = inventory,
					current_x = myNavigator.CurrentRoom.X,
					current_y = myNavigator.CurrentRoom.Y,
					visited_rooms = visitedRooms,
					dungeon_width = myDungeon.Width,
					dungeon_height = myDungeon.Height,
					entrance_x = myDungeon.Entrance.X,
					entrance_y = myDungeon.Entrance.Y,
					exit_x = myDungeon.Exit.X,
					exit_y = myDungeon.Exit.Y,
					rooms = rooms,
					combat = combatState
				};

				string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(savePath, json);

				return $"Save overwritten: {theSaveName}";
			}
			catch (Exception ex)
			{
				return $"Overwrite failed: {ex.Message}";
			}
		}

		private object SerializeItem(Item item)
		{
			if (item is HealingPotion potion)
			{
				// Use reflection to get private fields
				var minField = typeof(HealingPotion).GetField("myMinHeal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				var maxField = typeof(HealingPotion).GetField("myMaxHeal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

				return new
				{
					type = "HealingPotion",
					min_heal = minField?.GetValue(potion) ?? 25,
					max_heal = maxField?.GetValue(potion) ?? 50
				};
			}
			else if (item is VisionPotion)
			{
				return new { type = "VisionPotion" };
			}
			else if (item is Pillar pillar)
			{
				var typeField = typeof(Pillar).GetField("myType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				return new
				{
					type = "Pillar",
					pillar_type = typeField?.GetValue(pillar).ToString() ?? "Abstraction"
				};
			}

			return new { type = item.GetType().Name };
		}

		private object SerializeMonster(IDungeonCharacter monster)
		{
			return new
			{
				type = monster.GetType().Name,
				current_hp = monster.HitPoints,
				max_hp = monster.MaxHitPoints
			};
		}

		/// <summary>
		/// Loads a saved game from file.
		/// </summary>
		/// <param name="theSaveId">The save ID to load.</param>
		/// <returns>Success or error message.</returns>
		public string LoadGame(int theSaveId)
		{
			try
			{
				string saveDir = Path.Combine(OS.GetUserDataDir(), "saves");
				string savePath = Path.Combine(saveDir, $"save_{theSaveId}.json");

				if (!File.Exists(savePath))
				{
					return "Save file not found";
				}

				string json = File.ReadAllText(savePath);
				using JsonDocument doc = JsonDocument.Parse(json);
				JsonElement root = doc.RootElement;

				// Load hero
				JsonElement heroData = root.GetProperty("hero");
				string heroClass = heroData.GetProperty("hero_class").GetString();
				string heroName = heroData.GetProperty("hero_name").GetString();

				myHero = CreateHero(heroName, heroClass);
				myHero.DebugSetHP(heroData.GetProperty("hero_hp").GetInt32());

				// Restore pillars
				int pillarsCollected = heroData.GetProperty("pillars_collected").GetInt32();
				for (int i = 0; i < pillarsCollected && i < 4; i++)
				{
					myHero.CollectPillar((PillarType)i);
				}

				// Restore inventory
				if (root.TryGetProperty("inventory", out JsonElement inventoryData))
				{
					foreach (JsonElement itemData in inventoryData.EnumerateArray())
					{
						Item item = DeserializeItem(itemData);
						if (item != null)
						{
							myHero.AddItem(item);
						}
					}
				}

				// Reconstruct dungeon
				int width = root.GetProperty("dungeon_width").GetInt32();
				int height = root.GetProperty("dungeon_height").GetInt32();
				int entranceX = root.GetProperty("entrance_x").GetInt32();
				int entranceY = root.GetProperty("entrance_y").GetInt32();
				int exitX = root.GetProperty("exit_x").GetInt32();
				int exitY = root.GetProperty("exit_y").GetInt32();

				myDungeon = new DungeonMap(width, height);

				// Restore all rooms
				JsonElement roomsData = root.GetProperty("rooms");
				foreach (JsonElement roomData in roomsData.EnumerateArray())
				{
					int x = roomData.GetProperty("x").GetInt32();
					int y = roomData.GetProperty("y").GetInt32();
					Room room = myDungeon.GetRoom(x, y);

					// Set room type
					string roomType = roomData.GetProperty("room_type").GetString();
					if (roomType == "Entrance")
						room.Type = RoomType.Entrance;
					else if (roomType == "Exit")
						room.Type = RoomType.Exit;

					// Set walls
					room.NorthWall = roomData.GetProperty("north_wall").GetBoolean();
					room.SouthWall = roomData.GetProperty("south_wall").GetBoolean();
					room.EastWall = roomData.GetProperty("east_wall").GetBoolean();
					room.WestWall = roomData.GetProperty("west_wall").GetBoolean();

					// Restore item
					if (roomData.TryGetProperty("item", out JsonElement itemData) && itemData.ValueKind != JsonValueKind.Null)
					{
						room.Item = DeserializeItem(itemData);
					}

					// Restore monster
					if (roomData.TryGetProperty("monster", out JsonElement monsterData) && monsterData.ValueKind != JsonValueKind.Null)
					{
						room.Monster = DeserializeMonster(monsterData);
					}
				}

				// Reconnect room neighbors manually
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						Room currentRoom = myDungeon.GetRoom(x, y);

						// Set north neighbor
						if (y > 0)
						{
							currentRoom.North = myDungeon.GetRoom(x, y - 1);
						}

						// Set south neighbor
						if (y < height - 1)
						{
							currentRoom.South = myDungeon.GetRoom(x, y + 1);
						}

						// Set west neighbor
						if (x > 0)
						{
							currentRoom.West = myDungeon.GetRoom(x - 1, y);
						}

						// Set east neighbor
						if (x < width - 1)
						{
							currentRoom.East = myDungeon.GetRoom(x + 1, y);
						}
					}
				}

				// Create navigator
				myNavigator = new DungeonNavigator(myDungeon);

				// Restore visited rooms
				if (root.TryGetProperty("visited_rooms", out JsonElement visitedData))
				{
					foreach (JsonElement coord in visitedData.EnumerateArray())
					{
						int vx = coord.GetProperty("x").GetInt32();
						int vy = coord.GetProperty("y").GetInt32();
						Room visitedRoom = myDungeon.GetRoom(vx, vy);
						// Access the private field to add to visited set
						var visitedField = typeof(DungeonNavigator).GetField("myVisitedRooms", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
						var visitedSet = visitedField?.GetValue(myNavigator) as System.Collections.Generic.HashSet<Room>;
						visitedSet?.Add(visitedRoom);
					}
				}

				// Set current position
				int currentX = root.GetProperty("current_x").GetInt32();
				int currentY = root.GetProperty("current_y").GetInt32();
				myNavigator.TeleportTo(myDungeon.GetRoom(currentX, currentY));

				// Restore combat state
				if (root.TryGetProperty("combat", out JsonElement combatData) && combatData.ValueKind != JsonValueKind.Null)
				{
					if (combatData.GetProperty("in_combat").GetBoolean())
					{
						JsonElement monsterData = combatData.GetProperty("monster");
						Monster monster = DeserializeMonster(monsterData);

						if (monster != null)
						{
							myCombatManager.StartCombat(myHero, monster);
							// Restore turn number
							var turnField = typeof(CombatManager).GetField("myTurn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
							turnField?.SetValue(myCombatManager, combatData.GetProperty("turn").GetInt32());
						}
					}
				}

				return "Success";
			}
			catch (Exception ex)
			{
				return $"Load failed: {ex.Message}";
			}
		}

		private Item DeserializeItem(JsonElement itemData)
		{
			string type = itemData.GetProperty("type").GetString();

			if (type == "HealingPotion")
			{
				int minHeal = itemData.GetProperty("min_heal").GetInt32();
				int maxHeal = itemData.GetProperty("max_heal").GetInt32();
				return new HealingPotion(minHeal, maxHeal);
			}
			else if (type == "VisionPotion")
			{
				return new VisionPotion();
			}
			else if (type == "Pillar")
			{
				string pillarType = itemData.GetProperty("pillar_type").GetString();
				PillarType pType = Enum.Parse<PillarType>(pillarType);
				return new Pillar(pType);
			}

			return null;
		}

		private Monster DeserializeMonster(JsonElement monsterData)
		{
			string type = monsterData.GetProperty("type").GetString();
			int currentHp = monsterData.GetProperty("current_hp").GetInt32();

			// Create monster of the appropriate type
			Monster monster = type switch
			{
				"Ogre" => myMonsterFactory.CreateMonster("Ogre"),
				"Gremlin" => myMonsterFactory.CreateMonster("Gremlin"),
				"Skeleton" => myMonsterFactory.CreateMonster("Skeleton"),
				_ => null
			};

			// Set HP to saved value
			if (monster != null)
			{
				monster.DebugSetHP(currentHp);
			}

			return monster;
		}

		/// <summary>
		/// Gets all saved games.
		/// </summary>
		/// <returns>Array of save metadata.</returns>
		public Godot.Collections.Array GetAllSavedGames()
		{
			var saves = new Godot.Collections.Array();

			try
			{
				string saveDir = Path.Combine(OS.GetUserDataDir(), "saves");
				if (!Directory.Exists(saveDir))
				{
					return saves;
				}

				foreach (string file in Directory.GetFiles(saveDir, "save_*.json"))
				{
					string json = File.ReadAllText(file);
					using JsonDocument doc = JsonDocument.Parse(json);
					JsonElement root = doc.RootElement;

					JsonElement heroData = root.GetProperty("hero");
					saves.Add(new Godot.Collections.Dictionary
					{
						{ "save_id", root.GetProperty("save_id").GetInt32() },
						{ "save_name", root.GetProperty("save_name").GetString() },
						{ "timestamp", root.GetProperty("timestamp").GetString() },
						{ "hero_name", heroData.GetProperty("hero_name").GetString() }
					});
				}
			}
			catch
			{
				// Return empty array on error
			}

			return saves;
		}

		/// <summary>
		/// Deletes a saved game.
		/// </summary>
		/// <param name="theSaveId">The save ID to delete.</param>
		public void DeleteSavedGame(int theSaveId)
		{
			try
			{
				string saveDir = Path.Combine(OS.GetUserDataDir(), "saves");
				string savePath = Path.Combine(saveDir, $"save_{theSaveId}.json");

				if (File.Exists(savePath))
				{
					File.Delete(savePath);
				}
			}
			catch
			{
				// Ignore errors
			}
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
		/// DEBUG: Gives an item to the hero.
		/// </summary>
		/// <param name="theItemType">The type of item to give (HealingPotion or VisionPotion).</param>
		public void DebugGiveItem(string theItemType)
		{
			if (myHero == null) return;

			Item newItem = null;
			switch (theItemType)
			{
				case "HealingPotion":
					newItem = new HealingPotion();
					break;
				case "VisionPotion":
					newItem = new VisionPotion();
					break;
			}

			if (newItem != null)
			{
				myHero.AddItem(newItem);
			}
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
