/*
 * TCSS 360 Dungeon Adventure
 * HealingPotion.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

using System;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// A consumable that restores a random amount of the hero's hit points.
    /// </summary>
    public class HealingPotion : Item
    {
        /// <summary>The display name shown for a healing potion.</summary>
        private const string HealingPotionName = "Healing Potion";

        /// <summary>The default minimum hit points restored.</summary>
        private const int DefaultMinHeal = 25;

        /// <summary>The default maximum hit points restored.</summary>
        private const int DefaultMaxHeal = 50;

        /// <summary>The minimum hit points restored (inclusive).</summary>
        private readonly int myMinHeal;

        /// <summary>The maximum hit points restored (inclusive).</summary>
        private readonly int myMaxHeal;

        /// <summary>
        /// Initializes a new HealingPotion with the given healing range.
        /// </summary>
        /// <param name="theMinHeal">The minimum hit points restored.</param>
        /// <param name="theMaxHeal">The maximum hit points restored.</param>
        public HealingPotion(int theMinHeal = DefaultMinHeal,
            int theMaxHeal = DefaultMaxHeal)
            : base(HealingPotionName)
        {
            myMinHeal = theMinHeal;
            myMaxHeal = theMaxHeal;
        }

        /// <summary>
        /// Heals the hero by a random amount.
        /// </summary>
        /// <param name="theHero">The hero drinking the potion.</param>
        /// <returns>A description of the healing applied.</returns>
        public override string Use(Hero theHero)
        {
            int healAmount = Random.Shared.Next(myMinHeal, myMaxHeal + 1);
            theHero.ChangeHealth(healAmount);
            return $"{theHero.Name} drinks a {Name} and restores {healAmount} hit points.";
        }
    }
}
