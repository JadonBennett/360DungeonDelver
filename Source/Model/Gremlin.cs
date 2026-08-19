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
        public Gremlin(
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
