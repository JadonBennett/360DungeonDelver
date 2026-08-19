using System.Collections.Generic;
using System.Linq;
using DungeonDelver.Source.Model;


namespace DungeonDelver.Source.Persistence
{
    /// <summary>
    /// In-memory implementation of IGameRepository. Defines which monster
    /// types are available in each of the four pillar dungeons.
    /// </summary>
    public class GameRepository : IGameRepository
    {
        private readonly Dictionary<PillarType, List<string>> myDungeonMonsterPools = new()
        {
            [PillarType.Abstraction]   = new List<string> { "Gremlin" },
            [PillarType.Encapsulation] = new List<string> { "Skeleton" },
            [PillarType.Inheritance]   = new List<string> { "Ogre" },
            [PillarType.Polymorphism]  = new List<string> { "Gremlin", "Ogre", "Skeleton" }
        };

        public IReadOnlyList<string> GetMonsterTypesForDungeon(PillarType thePillarType)
        {
            return myDungeonMonsterPools.TryGetValue(thePillarType, out var pool)
                ? pool
                : new List<string>();
        }

        public IReadOnlyList<string> GetAllMonsterTypes()
        {
            return myDungeonMonsterPools.Values.SelectMany(pool => pool).Distinct().ToList();
        }
        
        public Monster GetMonsterStats(string theMonsterType)
        {
            // Fallback/stub implementation so the old file still compiles
            return theMonsterType switch
            {
                "Gremlin"    => new Gremlin("Gremlin", 70, 5, 0.8, 15, 30, 0.4, 20, 40),
                "Ogre"       => new Ogre("Ogre", 200, 2, 0.6, 30, 60, 0.1, 30, 60),
                "Skeleton" => new Skeleton("Skeleton", 100, 3, 0.8, 30, 50, 0.3, 30, 50),
                _ => throw new KeyNotFoundException($"Monster {theMonsterType} not found in memory.")
            };
        }
    }
}