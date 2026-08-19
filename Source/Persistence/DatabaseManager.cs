// Project: TCSS 360 Dungeon Adventure
// File: DatabaseManager.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Microsoft.Data.Sqlite;

namespace DungeonDelver.Source.Persistence
{
    /// <summary>
    /// Pure C# implementation of IDatabaseManager. Handles file creation,
    /// table schema execution, and initial data seeding.
    /// </summary>
    public class DatabaseManager : IDatabaseManager
    {
        public string DatabasePath { get; }
        private readonly string myConnectionString;

        /// <summary>
        /// Initializes the manager with a pre-resolved operating system file path.
        /// </summary>
        /// <param name="theTargetDatabasePath">The absolute physical path to the DB file.</param>
        public DatabaseManager(string theTargetDatabasePath)
        {
            DatabasePath = theTargetDatabasePath;
            myConnectionString = $"Data Source={DatabasePath}";
        }

        /// <summary>
        /// Main orchestrator that opens a single connection stream and calls individual table builders.
        /// </summary>
        public void InitializeDatabase()
        {
            // Automatic memory guard: closes and unlocks the database file when this method finishes.
            using var connection = new SqliteConnection(myConnectionString);
            connection.Open();

            InitializeMonsterTable(connection);
            InitializeHeroTable(connection);
            InitializeItemTable(connection);
        }

        // =========================================================================
        // MONSTER TABLE INITIALIZATION
        // =========================================================================
        private void InitializeMonsterTable(SqliteConnection theConnection)
        {
            string tableSql = @"
                CREATE TABLE IF NOT EXISTS monster_stats (
                    name TEXT PRIMARY KEY,
                    hit_points INTEGER,
                    attack_speed INTEGER,
                    chance_to_hit REAL,
                    min_damage INTEGER,
                    max_damage INTEGER,
                    chance_to_heal REAL,
                    min_heal INTEGER,
                    max_heal INTEGER
                );";
            
            using (var cmd = new SqliteCommand(tableSql, theConnection)) cmd.ExecuteNonQuery();

            // Seed only if completely empty
            using var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM monster_stats;", theConnection);
            if ((long)checkCmd.ExecuteScalar() == 0)
            {
                string seedSql = @"
                    INSERT INTO monster_stats VALUES 
                    ('Ogre', 200, 2, 0.6, 30, 60, 0.1, 30, 60),
                    ('Gremlin', 70, 5, 0.8, 15, 30, 0.4, 20, 40),
                    ('Skeleton', 100, 3, 0.8, 30, 50, 0.3, 30, 50);";
                
                using var seedCmd = new SqliteCommand(seedSql, theConnection);
                seedCmd.ExecuteNonQuery();
            }
        }

        // =========================================================================
        // HERO TABLE INITIALIZATION
        // =========================================================================
        private void InitializeHeroTable(SqliteConnection theConnection)
        {
            string tableSql = @"
                CREATE TABLE IF NOT EXISTS hero_stats (
                    name TEXT PRIMARY KEY,
                    hit_points INTEGER,
                    attack_speed INTEGER,
                    chance_to_hit REAL,
                    min_damage INTEGER,
                    max_damage INTEGER,
                    chance_to_block REAL,
                    special_skill_name TEXT,
                    special_min REAL,
                    special_max REAL,
                    special_chance REAL
                );";
            
            using (var cmd = new SqliteCommand(tableSql, theConnection)) cmd.ExecuteNonQuery();

            // Seed only if completely empty
            using var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM hero_stats;", theConnection);
            if ((long)checkCmd.ExecuteScalar() == 0)
            {
                string seedSql = @"
                    INSERT INTO hero_stats VALUES 
                    ('Warrior', 125, 4, 0.8, 35, 60, 0.2, 'Crushing Blow', 75, 175, 0.4),
                    ('Priestess', 75, 5, 0.7, 25, 45, 0.3, 'Heal', 20, 50, 1.0),
                    ('Thief', 75, 6, 0.8, 20, 40, 0.4, 'Surprise Attack', 0, 0, 0.4);";
                
                using var seedCmd = new SqliteCommand(seedSql, theConnection);
                seedCmd.ExecuteNonQuery();
            }
        }

        // =========================================================================
        // ITEM TABLE INITIALIZATION
        // =========================================================================
        private void InitializeItemTable(SqliteConnection theConnection)
        {
            string tableSql = @"
                CREATE TABLE IF NOT EXISTS item_definitions (
                    name TEXT PRIMARY KEY,
                    type TEXT NOT NULL,
                    min_effect INTEGER,
                    max_effect INTEGER,
                    description TEXT
                );";
            
            using (var cmd = new SqliteCommand(tableSql, theConnection)) cmd.ExecuteNonQuery();

            // Seed only if completely empty
            using var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM item_definitions;", theConnection);
            if ((long)checkCmd.ExecuteScalar() == 0)
            {
                string seedSql = @"
                    INSERT INTO item_definitions VALUES 
                    ('Healing Potion', 'Potion', 15, 30, 'Heals a range of hit points.'),
                    ('Vision Potion', 'Vision', 0, 0, 'Reveals the surrounding eight rooms.');";
                
                using var seedCmd = new SqliteCommand(seedSql, theConnection);
                seedCmd.ExecuteNonQuery();
            }
        }
    }
}