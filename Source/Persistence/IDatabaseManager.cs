// Project: TCSS 360 Dungeon Adventure
// File: IDatabaseManager.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Persistence
{
    /// <summary>
    /// Contract defining the infrastructure operations required to set up
    /// and prepare the SQLite database system for the game.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// Gets the absolute physical file system path to the database file.
        /// </summary>
        string DatabasePath { get; }

        /// <summary>
        /// Assembles all database tables, verifies structural integrity,
        /// and seeds the professor's required balance statistics.
        /// </summary>
        void InitializeDatabase();
    }
}