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
        public Ogre()
            : base(
                theName: "Shrek",
                theHitPoints: 200,
                theAttackSpeed: 2,
                theChanceToHit: 0.6,
                theMinDamage: 30,
                theMaxDamage: 60,
                theChanceToHeal: 0.1,
                theMinHeal: 30,
                theMaxHeal: 60)
        {
        }
    }
}
