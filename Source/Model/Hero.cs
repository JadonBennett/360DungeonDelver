// Project: TCSS 360 Dungeon Adventure
// File: Hero.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Abstract base class for all playable hero types.
    /// Extends DungeonCharacter with hero-specific abilities to be defined in subclasses.
    /// </summary>
    public abstract class Hero : DungeonCharacter
    {
        /// <summary>
        /// Initializes a new Hero with the specified combat statistics.
        /// </summary>
        /// <param name="theName">The display name of this hero.</param>
        /// <param name="theHitPoints">The starting and maximum hit points.</param>
        /// <param name="theAttackSpeed">The attack speed value.</param>
        /// <param name="theChanceToHit">The probability of landing an attack.</param>
        /// <param name="theMinDamage">The minimum damage on a hit.</param>
        /// <param name="theMaxDamage">The maximum damage on a hit.</param>
        protected Hero(
            string theName,
            int theHitPoints,
            int theAttackSpeed,
            double theChanceToHit,
            int theMinDamage,
            int theMaxDamage)
            : base(theName, theHitPoints, theAttackSpeed, theChanceToHit, theMinDamage, theMaxDamage)
        {
        }

        // Future: Hero-specific special abilities will be defined here
    }
}
