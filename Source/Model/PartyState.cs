// Project: TCSS 360 Dungeon Adventure
// File: PartyState.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System.Collections.Generic;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Manages the state of the player's party, including active heroes,
    /// shared currency, and the shared inventory. This is not a field on Hero itself;
    /// the party state is maintained separately from individual characters.
    /// </summary>
    public class PartyState
    {
        /// <summary>
        /// The collection of heroes currently in the party.
        /// </summary>
        private readonly List<Hero> myHeroes;

        /// <summary>
        /// The amount of currency the party currently holds.
        /// </summary>
        private int myCurrency;

        /// <summary>
        /// The shared inventory of items available to all party members.
        /// </summary>
        private readonly List<Item> myInventory;

        /// <summary>
        /// Initializes a new PartyState with empty hero list and inventory.
        /// </summary>
        public PartyState()
        {
            myHeroes = new List<Hero>();
            myCurrency = 0;
            myInventory = new List<Item>();
        }

        /// <summary>
        /// The collection of heroes in the party.
        /// </summary>
        public IReadOnlyList<Hero> Heroes => myHeroes;

        /// <summary>
        /// The amount of currency the party holds.
        /// </summary>
        public int Currency => myCurrency;

        /// <summary>
        /// The shared inventory of items.
        /// </summary>
        public IReadOnlyList<Item> Inventory => myInventory;

        // Future methods: AddHero(), RemoveHero(), AddItem(), UseItem(), AddCurrency()
    }
}
