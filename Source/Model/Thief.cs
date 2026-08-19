/*
 * TCSS 360 Dungeon Adventure
 * Thief.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

using System;
using DungeonDelver.Source.Interface;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// A fast, evasive hero whose special skill, Surprise Attack, can grant an
    /// extra strike, do nothing, or occasionally get the thief caught.
    /// </summary>
    public class Thief : Hero
    {
        /// <summary>The default starting hit points for a Thief.</summary>
        private const int ThiefHitPoints = 75;

        /// <summary>The default attack speed for a Thief.</summary>
        private const int ThiefAttackSpeed = 6;

        /// <summary>The default chance to hit for a Thief.</summary>
        private const double ThiefChanceToHit = 0.8;

        /// <summary>The minimum normal-attack damage for a Thief.</summary>
        private const int ThiefMinDamage = 20;

        /// <summary>The maximum normal-attack damage for a Thief.</summary>
        private const int ThiefMaxDamage = 40;

        /// <summary>The default block chance for a Thief.</summary>
        private const double ThiefBlockChance = 0.4;

        /// <summary>The probability the Surprise Attack succeeds.</summary>
        private const double SurpriseSuccessChance = 0.4;

        /// <summary>The probability the thief is caught and loses the turn.</summary>
        private const double CaughtChance = 0.2;

        /// <summary>
        /// Initializes a new Thief with default Thief statistics.
        /// </summary>
        /// <param name="theName">The display name of this thief.</param>
        public Thief(string theName)
            : base(theName, ThiefHitPoints, ThiefAttackSpeed, ThiefChanceToHit,
                ThiefMinDamage, ThiefMaxDamage, ThiefBlockChance)
        {
        }

        /// <summary>
        /// The name of this hero's special skill.
        /// </summary>
        public override string SpecialSkillName => "Surprise Attack";

        /// <summary>
        /// Surprise Attack: usually grants a bonus strike, sometimes does
        /// nothing, and occasionally the thief is caught and forfeits the turn.
        /// </summary>
        /// <param name="theTarget">The opponent to strike, if any.</param>
        /// <returns>A description of the skill's outcome.</returns>
        public override string UseSpecialSkill(IDungeonCharacter theTarget = null)
        {
            double roll = Random.Shared.NextDouble();
            string result;
            if (roll < CaughtChance)
            {
                result = $"{Name} was caught and loses the turn!";
            }
            else if (roll < CaughtChance + SurpriseSuccessChance)
            {
                int damage = Attack();
                theTarget?.ChangeHealth(-damage);
                result = $"{Name} pulls off a Surprise Attack and gets an extra strike!";
            }
            else
            {
                int damage = Attack();
                theTarget?.ChangeHealth(-damage);
                result = $"{Name} attacks normally.";
            }

            return result;
        }
    }
}
