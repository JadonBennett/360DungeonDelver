// Project: TCSS 360 Dungeon Adventure
// File: MonsterFactory.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
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
        /// The monster type names this factory can construct.
        /// </summary>
        private static readonly string[] MonsterTypes = { "Ogre", "Skeleton", "Gremlin" };

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
        /// Creates a new Monster instance of the specified type.
        /// </summary>
        /// <param name="theMonsterType">The monster type name ("Ogre", "Skeleton", or "Gremlin").</param>
        /// <returns>A new Monster instance of the requested type.</returns>
        public Monster CreateMonster(string theMonsterType)
        {
            return theMonsterType switch
            {
                "Ogre" => new Ogre(),
                "Skeleton" => new Skeleton(),
                "Gremlin" => new Gremlin(),
                _ => throw new ArgumentException($"Unknown monster type: {theMonsterType}", nameof(theMonsterType))
            };
        }

        /// <summary>
        /// Creates a new Monster instance of a randomly chosen type.
        /// </summary>
        /// <returns>A new Monster instance of a random type.</returns>
        public Monster CreateRandomMonster()
        {
            string type = MonsterTypes[Random.Shared.Next(MonsterTypes.Length)];
            return CreateMonster(type);
        }
    }
}
