// Project: TCSS 360 Dungeon Adventure
// File: MonsterFactory.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using DungeonDelver.Source.Model;
using DungeonDelver.Source.Persistence;

namespace DungeonDelver.Source.Controller
{
    /// <summary>
    /// Factory class for creating Monster instances based on data
    /// retrieved from the game repository (database).
    /// </summary>
    public class MonsterFactory
    {
        /// <summary>
        /// The game repository used to retrieve monster statistics.
        /// </summary>
        private readonly IGameRepository myRepository;

        /// <summary>
        /// Initializes a new MonsterFactory with the specified repository.
        /// </summary>
        /// <param name="theRepository">The repository to use for monster data.</param>
        public MonsterFactory(IGameRepository theRepository)
        {
            myRepository = theRepository;
        }

        // TODO: Add method:
        // - CreateMonster(string monsterType)
        //   Queries repository for stats, instantiates appropriate Monster subclass
    }
}
