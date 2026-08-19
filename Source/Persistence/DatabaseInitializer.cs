// Project: TCSS 360 Dungeon Adventure
// File: DatabaseInitializer.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System.IO;
using Godot;

namespace DungeonDelver.Source.Persistence
{
    /// <summary>
    /// Static utility for one-time database initialization at application startup.
    /// Ensures the database schema is created before any game controllers are instantiated.
    /// </summary>
    public static class DatabaseInitializer
    {
        private static bool isInitialized = false;
        private static readonly object lockObject = new object();

        /// <summary>
        /// Initializes the database schema if not already initialized.
        /// Thread-safe and idempotent - safe to call multiple times.
        /// </summary>
        public static void EnsureInitialized()
        {
            lock (lockObject)
            {
                if (isInitialized)
                {
                    return;
                }

                string dbPath = Path.Combine(OS.GetUserDataDir(), "dungeon.db");
                var dbManager = new DatabaseManager(dbPath);
                dbManager.InitializeDatabase();

                isInitialized = true;
                GD.Print("[DatabaseInitializer] Database initialized at: " + dbPath);
            }
        }
    }
}
