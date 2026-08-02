// Project: TCSS 360 Dungeon Adventure
// File: Item.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Abstract base class for all items that can be collected or used
    /// in the dungeon, including potions and pillars.
    /// </summary>
    public abstract class Item
    {
        /// <summary>
        /// The display name of this item.
        /// </summary>
        private readonly string myName;

        /// <summary>
        /// Initializes a new Item with the specified name.
        /// </summary>
        /// <param name="theName">The display name of this item.</param>
        protected Item(string theName)
        {
            myName = theName;
        }

        /// <summary>
        /// The display name of this item.
        /// </summary>
        public string Name => myName;

        // Future: Use() method to be defined for consumable items
    }
}
