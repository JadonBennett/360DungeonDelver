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
        public Skeleton()
            : base(
                theName: "Skellington",
                theHitPoints: 100,
                theAttackSpeed: 3,
                theChanceToHit: 0.8,
                theMinDamage: 30,
                theMaxDamage: 50,
                theChanceToHeal: 0.3,
                theMinHeal: 30,
                theMaxHeal: 50)
        {
        }
    }
}
