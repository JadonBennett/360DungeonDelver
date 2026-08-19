// Project: TCSS 360 Dungeon Adventure
// File: IGameRepository.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System.Collections.Generic;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Source.Persistence
{
    /// <summary>
    /// Provides monster data used by MonsterFactory to determine which
    /// monster types are available within a given dungeon.
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// Returns the monster type names that may spawn in the dungeon
        /// associated with the given pillar.
        /// </summary>
        /// <param name="thePillarType">The pillar granted by the dungeon.</param>
        /// <returns>The pool of monster type names for that dungeon.</returns>
        IReadOnlyList<string> GetMonsterTypesForDungeon(PillarType thePillarType);

        /// <summary>
        /// Returns all monster type names known to the game.
        /// </summary>
        IReadOnlyList<string> GetAllMonsterTypes();
        
        /// <summary>
        /// Queries the persistent SQLite storage to retrieve stats for a specific monster type 
        /// and constructs its corresponding subclass entity.
        /// </summary>
        /// <param name="theMonsterType">The unique primary key name of the monster type (e.g., "Ogre").</param>
        /// <returns>A fully instantiated Monster entity loaded with database parameters.</returns>
        /// <exception cref="ArgumentException">Thrown when an unknown monster type string is passed.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the requested monster does not exist in the database table.</exception>
        Monster GetMonsterStats(string theMonsterType);

        // TODO: Define methods for:
        // - GetHeroStats(string heroClass)
        // - GetItemDefinition(string itemType)
        // - SaveGameState(...)
        // - LoadGameState(...)
    }
}