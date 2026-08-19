// Project: TCSS 360 Dungeon Adventure
// File: Ogre.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Represents an Ogre monster, a slow but powerful enemy
    /// with high hit points and moderate healing ability.
    /// </summary>
    public class Ogre : Monster
    {
        /// <summary>
        /// Initializes a new Ogre with predefined statistics.
        /// </summary>
        public Ogre(
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
