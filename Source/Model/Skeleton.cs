// Project: TCSS 360 Dungeon Adventure
// File: Skeleton.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Represents a Skeleton monster, a fast and accurate enemy
    /// with moderate hit points and good healing ability.
    /// </summary>
    public class Skeleton : Monster
    {
        /// <summary>
        /// Initializes a new Skeleton with predefined statistics.
        /// </summary>
        public Skeleton(
        string theName, int theHitPoints, int theAttackSpeed, double theChanceToHit,
        int theMinDamage, int theMaxDamage, double theChanceToHeal, int theMinHeal, int theMaxHeal)
        : base(theName,
            theHitPoints,
            theAttackSpeed,
            theChanceToHit, 
            theMinDamage,
            theMaxDamage,
            theChanceToHeal,
            theMinHeal,
            theMaxHeal)
    {
    }
    }
}
