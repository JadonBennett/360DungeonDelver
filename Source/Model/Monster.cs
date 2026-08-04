// Project: TCSS 360 Dungeon Adventure
// File: Monster.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Abstract base class for all monster types in the dungeon.
    /// Extends DungeonCharacter with self-healing capabilities.
    /// </summary>
    public abstract class Monster : DungeonCharacter
    {
        /// <summary>
        /// The probability (0.0 to 1.0) that this monster will heal itself.
        /// </summary>
        private readonly double myChanceToHeal;

        /// <summary>
        /// The minimum amount this monster can heal.
        /// </summary>
        private readonly int myMinHeal;

        /// <summary>
        /// The maximum amount this monster can heal.
        /// </summary>
        private readonly int myMaxHeal;

        /// <summary>
        /// Initializes a new Monster with the specified combat and healing statistics.
        /// </summary>
        /// <param name="theName">The display name of this monster.</param>
        /// <param name="theHitPoints">The starting and maximum hit points.</param>
        /// <param name="theAttackSpeed">The attack speed value.</param>
        /// <param name="theChanceToHit">The probability of landing an attack.</param>
        /// <param name="theMinDamage">The minimum damage on a hit.</param>
        /// <param name="theMaxDamage">The maximum damage on a hit.</param>
        /// <param name="theChanceToHeal">The probability of self-healing.</param>
        /// <param name="theMinHeal">The minimum amount healed.</param>
        /// <param name="theMaxHeal">The maximum amount healed.</param>
        protected Monster(
            string theName,
            int theHitPoints,
            int theAttackSpeed,
            double theChanceToHit,
            int theMinDamage,
            int theMaxDamage,
            double theChanceToHeal,
            int theMinHeal,
            int theMaxHeal)
            : base(theName, theHitPoints, theAttackSpeed, theChanceToHit, theMinDamage, theMaxDamage)
        {
            myChanceToHeal = theChanceToHeal;
            myMinHeal = theMinHeal;
            myMaxHeal = theMaxHeal;
        }

        /// <summary>
        /// The probability (0.0 to 1.0) that this monster will heal itself.
        /// </summary>
        protected double ChanceToHeal => myChanceToHeal;

        /// <summary>
        /// The minimum amount this monster can heal.
        /// </summary>
        protected int MinHeal => myMinHeal;

        /// <summary>
        /// The maximum amount this monster can heal.
        /// </summary>
        protected int MaxHeal => myMaxHeal;

        /// <summary>
        /// Attempts to heal this monster based on its ChanceToHeal.
        /// If successful, heals a random amount between MinHeal and MaxHeal inclusive.
        /// </summary>
        public virtual void Heal()
        {
            double healRoll = Random.Shared.NextDouble();

            if (healRoll < myChanceToHeal)
            {
                int healAmount = Random.Shared.Next(myMinHeal, myMaxHeal + 1);
                ChangeHealth(healAmount);
            }
        }
    }
}
