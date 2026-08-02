// Project: TCSS 360 Dungeon Adventure
// File: Warrior.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Represents a Warrior hero class, characterized by high hit points
    /// and strong physical attacks. Specific stats and special abilities
    /// to be defined.
    /// </summary>
    public class Warrior : Hero
    {
        /// <summary>
        /// Initializes a new Warrior with default statistics.
        /// Implementation pending.
        /// </summary>
        /// <param name="theName">The player-chosen name for this warrior.</param>
        public Warrior(string theName)
            : base(theName, 0, 0, 0, 0, 0)
        {
            // TODO: Define Warrior stats and special ability (Crushing Blow?)
        }
    }
}
