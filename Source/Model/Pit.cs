/*
 * TCSS 360 Dungeon Adventure
 * Pit.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

using System;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// A hazard placed in a room's physical space. Dealing damage is
    /// triggered externally (by the real-time movement system) whenever the
    /// hero's position overlaps the pit's tile.
    /// </summary>
    public class Pit
    {
        /// <summary>The minimum damage this pit deals when triggered.</summary>
        private readonly int myMinDamage;

        /// <summary>The maximum damage this pit deals when triggered.</summary>
        private readonly int myMaxDamage;

        /// <summary>
        /// Initializes a new Pit with the given damage range.
        /// </summary>
        /// <param name="theMinDamage">The minimum damage dealt when triggered.</param>
        /// <param name="theMaxDamage">The maximum damage dealt when triggered.</param>
        public Pit(int theMinDamage, int theMaxDamage)
        {
            myMinDamage = theMinDamage;
            myMaxDamage = theMaxDamage;
        }

        /// <summary>The minimum damage this pit deals when triggered.</summary>
        public int MinDamage => myMinDamage;

        /// <summary>The maximum damage this pit deals when triggered.</summary>
        public int MaxDamage => myMaxDamage;

        /// <summary>
        /// Triggers the pit, dealing a random amount of damage to the hero.
        /// </summary>
        /// <param name="theHero">The hero who stepped into the pit.</param>
        /// <returns>The amount of damage dealt.</returns>
        public int Trigger(Hero theHero)
        {
            int damage = Random.Shared.Next(myMinDamage, myMaxDamage + 1);
            theHero.ChangeHealth(-damage);
            return damage;
        }
    }
}
