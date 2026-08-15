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
        
        
        // TODO: Define methods for:
        // - GetMonsterStats(string monsterType)
        // - GetHeroStats(string heroClass)
        // - GetItemDefinition(string itemType)
        // - SaveGameState(...)
        // - LoadGameState(...)
    }
}