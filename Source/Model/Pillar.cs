/*
 * TCSS 360 Dungeon Adventure
 * Pillar.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// A Pillar of OO. Using one records its type on the hero and contributes
    /// toward the win condition.
    /// </summary>
    public class Pillar : Item
    {
        /// <summary>Which pillar of OO this item represents.</summary>
        private readonly PillarType myPillarType;

        /// <summary>
        /// Initializes a new Pillar of the given type.
        /// </summary>
        /// <param name="thePillarType">The pillar of OO this item represents.</param>
        public Pillar(PillarType thePillarType)
            : base($"Pillar of {thePillarType}")
        {
            myPillarType = thePillarType;
        }

        /// <summary>Which pillar of OO this item represents.</summary>
        public PillarType PillarType => myPillarType;

        /// <summary>
        /// Records this pillar on the hero.
        /// </summary>
        /// <param name="theHero">The hero collecting the pillar.</param>
        /// <returns>A description of the pillar collected.</returns>
        public override string Use(Hero theHero)
        {
            theHero.CollectPillar(myPillarType);
            return $"{theHero.Name} collected the {Name}!";
        }
    }
}