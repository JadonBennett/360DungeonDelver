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
    }
}