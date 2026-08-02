// Project: TCSS 360 Dungeon Adventure
// File: Pillar.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Represents one of the Pillars of Object-Oriented Programming
    /// that must be collected to win the game. Each pillar has a unique type.
    /// </summary>
    public class Pillar : Item
    {
        /// <summary>
        /// The type of this pillar (Abstraction, Encapsulation, Inheritance, or Polymorphism).
        /// </summary>
        private readonly PillarType myType;

        /// <summary>
        /// Initializes a new Pillar of the specified type.
        /// </summary>
        /// <param name="theType">The pillar type.</param>
        public Pillar(PillarType theType)
            : base($"Pillar of {theType}")
        {
            myType = theType;
        }

        /// <summary>
        /// The type of this pillar.
        /// </summary>
        public PillarType Type => myType;
    }
}
