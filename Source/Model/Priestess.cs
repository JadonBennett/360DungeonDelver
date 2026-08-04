/*
 * TCSS 360 Dungeon Adventure
 * Priestess.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

using System;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// A balanced hero whose special skill, Heal, restores a random amount of
    /// her own hit points.
    /// </summary>
    public class Priestess : Hero
    {
        /// <summary>The default starting hit points for a Priestess.</summary>
        private const int PriestessHitPoints = 75;

        /// <summary>The default attack speed for a Priestess.</summary>
        private const int PriestessAttackSpeed = 5;

        /// <summary>The default chance to hit for a Priestess.</summary>
        private const double PriestessChanceToHit = 0.7;

        /// <summary>The minimum normal-attack damage for a Priestess.</summary>
        private const int PriestessMinDamage = 25;

        /// <summary>The maximum normal-attack damage for a Priestess.</summary>
        private const int PriestessMaxDamage = 45;

        /// <summary>The default block chance for a Priestess.</summary>
        private const double PriestessBlockChance = 0.3;

        /// <summary>The minimum hit points restored by Heal.</summary>
        private const int HealMinAmount = 25;

        /// <summary>The maximum hit points restored by Heal.</summary>
        private const int HealMaxAmount = 50;

        /// <summary>
        /// Initializes a new Priestess with default Priestess statistics.
        /// </summary>
        /// <param name="theName">The display name of this priestess.</param>
        public Priestess(string theName)
            : base(theName, PriestessHitPoints, PriestessAttackSpeed,
                PriestessChanceToHit, PriestessMinDamage, PriestessMaxDamage,
                PriestessBlockChance)
        {
        }

        /// <summary>
        /// The name of this hero's special skill.
        /// </summary>
        public override string SpecialSkillName => "Heal";

        /// <summary>
        /// Heal: restores a random amount of hit points. Passes a negative
        /// value to ChangeHealth because negative amounts heal in the current
        /// DungeonCharacter implementation.
        /// </summary>
        /// <returns>A description of the skill's outcome.</returns>
        public override string UseSpecialSkill()
        {
            int healAmount = Random.Shared.Next(HealMinAmount, HealMaxAmount + 1);
            ChangeHealth(-healAmount);
            return $"{Name} heals for {healAmount} hit points.";
        }
    }
}
