// Project: TCSS 360 Dungeon Adventure
// File: RoomContentGenerator.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Dungeon
{
    /// <summary>
    /// Populates a generated dungeon's normal rooms with pits, monsters, and
    /// items via independent per-room probability rolls. Guaranteed content
    /// (pillars) is placed separately by MazeGenerator before this runs.
    /// </summary>
    public class RoomContentGenerator
    {
        /// <summary>The probability that a normal room contains a pit.</summary>
        private const double PitChance = 0.10;

        /// <summary>The probability that a normal room contains a monster.</summary>
        private const double MonsterChance = 0.35;

        /// <summary>The probability that a normal room contains an item.</summary>
        private const double ItemChance = 0.20;

        /// <summary>The minimum damage a placed pit deals when triggered.</summary>
        private const int PitMinDamage = 10;

        /// <summary>The maximum damage a placed pit deals when triggered.</summary>
        private const int PitMaxDamage = 30;

        /// <summary>
        /// Supplies a newly created monster on demand, decoupling this class
        /// from any particular monster-creation mechanism (e.g. MonsterFactory).
        /// </summary>
        private readonly Func<Monster> myMonsterProvider;

        /// <summary>
        /// Random number generator used for all content rolls.
        /// </summary>
        private readonly Random myRandom = new Random();

        /// <summary>
        /// Initializes a new RoomContentGenerator with the given monster provider.
        /// </summary>
        /// <param name="theMonsterProvider">A function that creates a new monster on demand.</param>
        public RoomContentGenerator(Func<Monster> theMonsterProvider)
        {
            myMonsterProvider = theMonsterProvider;
        }

        /// <summary>
        /// Rolls independent probabilities for pit, monster, and item content
        /// in every normal room of the given dungeon. Entrance and exit rooms
        /// are left empty, and item rolls never overwrite an existing item
        /// (e.g. a pillar already placed).
        /// </summary>
        /// <param name="theDungeon">The dungeon map to populate.</param>
        public void Populate(DungeonMap theDungeon)
        {
            foreach (Room room in theDungeon.GetRooms())
            {
                if (room.Type != RoomType.Normal)
                {
                    continue;
                }

                if (myRandom.NextDouble() < PitChance)
                {
                    room.Pit = new Pit(PitMinDamage, PitMaxDamage);
                }

                if (myRandom.NextDouble() < MonsterChance)
                {
                    room.AddMonster(myMonsterProvider());
                }

                if (room.Item == null && myRandom.NextDouble() < ItemChance)
                {
                    room.Item = CreateRandomItem();
                }
            }
        }

        /// <summary>
        /// Creates a random item to place in a room (a healing or vision potion).
        /// </summary>
        /// <returns>A newly created item.</returns>
        private Item CreateRandomItem()
        {
            return myRandom.NextDouble() < 0.5 ? new HealingPotion() : new VisionPotion();
        }
    }
}
