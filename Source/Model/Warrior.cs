/*
 * TCSS 360 Dungeon Adventure
 * Warrior.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

using System;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// A high-health, high-damage hero whose special skill, Crushing Blow,
    /// deals heavy damage but only lands part of the time.
    /// </summary>
    public class Warrior : Hero
    {
        /// <summary>The default starting hit points for a Warrior.</summary>
        private const int WarriorHitPoints = 125;

        /// <summary>The default attack speed for a Warrior.</summary>
        private const int WarriorAttackSpeed = 4;

        /// <summary>The default chance to hit for a Warrior.</summary>
        private const double WarriorChanceToHit = 0.8;

        /// <summary>The minimum normal-attack damage for a Warrior.</summary>
        private const int WarriorMinDamage = 35;

        /// <summary>The maximum normal-attack damage for a Warrior.</summary>
        private const int WarriorMaxDamage = 60;

        /// <summary>The default block chance for a Warrior.</summary>
        private const double WarriorBlockChance = 0.2;

        /// <summary>The probability that a Crushing Blow lands.</summary>
        private const double CrushingBlowChance = 0.4;

        /// <summary>The minimum Crushing Blow damage.</summary>
        private const int CrushingBlowMinDamage = 75;

        /// <summary>The maximum Crushing Blow damage.</summary>
        private const int CrushingBlowMaxDamage = 175;

        /// <summary>
        /// Initializes a new Warrior with default Warrior statistics.
        /// </summary>
        /// <param name="theName">The display name of this warrior.</param>
        public Warrior(string theName)
            : base(theName, WarriorHitPoints, WarriorAttackSpeed,
                WarriorChanceToHit, WarriorMinDamage, WarriorMaxDamage,
                WarriorBlockChance)
        {
        }

        /// <summary>
        /// Crushing Blow: a chance-based heavy strike. Returns the amount of
        /// damage dealt, or zero if the blow misses.
        /// </summary>
        /// <returns>The damage dealt by the Crushing Blow, or zero on a miss.</returns>
        public override string UseSpecialSkill()
        {
            string result;
            if (Random.Shared.NextDouble() < CrushingBlowChance)
            {
                int damage = Random.Shared.Next(CrushingBlowMinDamage,
                    CrushingBlowMaxDamage + 1);
                result = $"{Name} lands a Crushing Blow for {damage} damage!";
            }
            else
            {
                result = $"{Name}'s Crushing Blow missed!";
            }

            return result;
        }
    }
}
