// Project: TCSS 360 Dungeon Adventure
// File: Gremlin.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Represents a Gremlin monster, a very fast and fragile enemy
    /// with low hit points but excellent healing ability.
    /// </summary>
    public class Gremlin : Monster
    {
        /// <summary>
        /// Initializes a new Gremlin with predefined statistics.
        /// </summary>
        public Gremlin()
            : base(
                theName: "Grot",
                theHitPoints: 70,
                theAttackSpeed: 5,
                theChanceToHit: 0.8,
                theMinDamage: 15,
                theMaxDamage: 30,
                theChanceToHeal: 0.4,
                theMinHeal: 20,
                theMaxHeal: 40)
        {
        }
    }
}
