using Godot;
using System;
using System.IO;
using DungeonDelver.Source.Persistence;
using DungeonDelver.Source.Controller;

namespace DungeonDelver.Source.Controller
{
    public partial class GameBootstrapper : Node
    {
        /// <summary>
        /// The global game state manager coordinating core systems, combat, and navigation.
        /// </summary>
        public DungeonController GameController { get; private set; }

        /// <summary>
        /// Godot engine lifecycle entry point. Resolves operating system file paths, 
        /// initializes SQLite schema structures, and bootstraps core engine services.
        /// </summary>
        public override void _Ready()
        {
            string dbPath = Path.Combine(OS.GetUserDataDir(), "dungeon.db");

            var dbManager = new DatabaseManager(dbPath);
            dbManager.InitializeDatabase();

            var repository = new SqliteGameRepository(dbPath);

            GameController = new DungeonController(repository);

            GD.Print("[GameBootstrapper] DungeonController successfully instantiated with SQLite tables!");
        }
    }
}