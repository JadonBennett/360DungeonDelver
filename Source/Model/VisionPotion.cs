/*
 * TCSS 360 Dungeon Adventure
 * VisionPotion.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// A consumable that reveals the rooms surrounding the hero's location. The
    /// actual reveal is delegated to Joanna's map/room system once it exists.
    /// </summary>
    public class VisionPotion : Item
    {
        /// <summary>The display name shown for a vision potion.</summary>
        private const string VisionPotionName = "Vision Potion";

        /// <summary>
        /// Initializes a new VisionPotion.
        /// </summary>
        public VisionPotion()
            : base(VisionPotionName)
        {
        }

        /// <summary>
        /// Uses the vision potion. The reveal depends on the DungeonMap/Room
        /// API, so this returns a message the game loop can act on once that
        /// API is available.
        /// </summary>
        /// <param name="theHero">The hero using the potion.</param>
        /// <returns>A description of the potion's effect.</returns>
        public override string Use(Hero theHero)
        {
            return $"{theHero.Name} uses a {Name}; the surrounding rooms are revealed.";
        }
    }
}