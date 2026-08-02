/*
 * TCSS 360 Dungeon Adventure
 * Item.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Abstract base class for all collectible items. Every item has a name and
    /// applies some effect to a hero when used.
    /// </summary>
    public abstract class Item
    {
        /// <summary>The display name of this item.</summary>
        private readonly string myName;

        /// <summary>
        /// Initializes a new Item with the given display name.
        /// </summary>
        /// <param name="theName">The display name of this item.</param>
        protected Item(string theName)
        {
            myName = theName;
        }

        /// <summary>The display name of this item.</summary>
        public string Name => myName;

        /// <summary>
        /// Applies this item's effect to the given hero.
        /// </summary>
        /// <param name="theHero">The hero using the item.</param>
        /// <returns>A description of the item's effect for display.</returns>
        public abstract string Use(Hero theHero);
    }
}