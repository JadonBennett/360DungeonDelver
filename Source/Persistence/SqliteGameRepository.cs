// Project: TCSS 360 Dungeon Adventure
// File: SqliteGameRepository.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Persistence
{
    /// <summary>
    /// Implementation of IGameRepository that uses SQLite for persistent storage.
    /// All raw SQL queries live in this class and nowhere else in the codebase.
    /// </summary>
    public class SqliteGameRepository : IGameRepository
    {
        /// <summary>
        /// The path to the SQLite database file.
        /// </summary>
        private readonly string myDatabasePath;

        /// <summary>
        /// Initializes a new SqliteGameRepository with the specified database file path.
        /// </summary>
        /// <param name="theDatabasePath">The path to the SQLite database file.</param>
        public SqliteGameRepository(string theDatabasePath)
        {
            myDatabasePath = theDatabasePath;
        }

        // TODO: Implement IGameRepository methods:
        // - Use SQLite connection to query monster/hero stats
        // - Execute INSERT/UPDATE for save game
        // - Execute SELECT for load game
        // - All SQL statements should be parameterized to prevent injection
    }
}
