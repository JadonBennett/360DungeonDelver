// Project: TCSS 360 Dungeon Adventure
// File: SqliteGameRepository.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using System.Collections.Generic;
using DungeonDelver.Source.Model;
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

        /// <summary>
        /// Returns the monster type names that may spawn in the dungeon
        /// associated with the given pillar.
        /// </summary>
        /// <param name="thePillarType">The pillar granted by the dungeon.</param>
        /// <returns>The pool of monster type names for that dungeon.</returns>
        public IReadOnlyList<string> GetMonsterTypesForDungeon(PillarType thePillarType)
        {
            // TODO: SELECT monster_type FROM dungeon_monsters WHERE pillar_type = @pillarType
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all monster type names known to the game.
        /// </summary>
        public IReadOnlyList<string> GetAllMonsterTypes()
        {
            // TODO: SELECT DISTINCT monster_type FROM monsters
            throw new NotImplementedException();
        }

        // TODO: Implement IGameRepository methods:
        // - Use SQLite connection to query monster/hero stats
        // - Execute INSERT/UPDATE for save game
        // - Execute SELECT for load game
        // - All SQL statements should be parameterized to prevent injection
    }
}