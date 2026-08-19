// Project: TCSS 360 Dungeon Adventure
// File: MonsterFactory.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using DungeonDelver.Source.Model;
using DungeonDelver.Source.Interface;
using DungeonDelver.Source.Persistence;
using System;

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

        /// <summary>
        /// Creates a monster of the given type.
        /// </summary>
        /// <param name="theMonsterType">The monster type name (e.g. "Ogre").</param>
        /// <returns>A new instance of the requested monster.</returns>
        public Monster CreateMonster(string theMonsterType)
        {
            return myRepository.GetMonsterStats(theMonsterType);
        }

        /// <summary>
        /// Creates a random monster from the pool available in the dungeon
        /// associated with the given pillar.
        /// </summary>
        /// <param name="theDungeonPillar">The pillar of the dungeon to pull from.</param>
        /// <returns>A new instance of a randomly chosen monster from that dungeon's pool.</returns>
        public Monster CreateRandomMonster(PillarType theDungeonPillar)
        {
            var pool = myRepository.GetMonsterTypesForDungeon(theDungeonPillar);

            if (pool.Count == 0)
            {
                throw new InvalidOperationException($"No monsters configured for {theDungeonPillar}");
            }

            string chosen = pool[Random.Shared.Next(pool.Count)];
            return CreateMonster(chosen);
        }
    }
}