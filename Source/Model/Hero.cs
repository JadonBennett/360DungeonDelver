/*
 * TCSS 360 Dungeon Adventure
 * Hero.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */

using System;
using System.Collections.Generic;
using DungeonDelver.Source.Interface;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Abstract base class for all playable heroes. Adds a chance to block
    /// incoming damage, an inventory of items, and pillar tracking on top of
    /// DungeonCharacter.
    /// </summary>
    public abstract class Hero : DungeonCharacter
    {
        /// <summary>
        /// The number of pillars required to satisfy the win condition.
        /// </summary>
        public const int TotalPillars = 4;

        /// <summary>
        /// The probability (0.0 to 1.0) that this hero blocks incoming damage.
        /// </summary>
        private readonly double myBlockChance;

        /// <summary>
        /// The items this hero is currently carrying.
        /// </summary>
        private readonly List<Item> myInventory = new();

        /// <summary>
        /// The distinct pillar types this hero has collected so far.
        /// </summary>
        private readonly HashSet<PillarType> myPillarsCollected = new();

        /// <summary>
        /// Initializes a new Hero with the given statistics and block chance.
        /// </summary>
        /// <param name="theName">The display name of this hero.</param>
        /// <param name="theHitPoints">The starting and maximum hit points.</param>
        /// <param name="theAttackSpeed">The attack speed value.</param>
        /// <param name="theChanceToHit">The probability of landing an attack.</param>
        /// <param name="theMinDamage">The minimum damage on a hit.</param>
        /// <param name="theMaxDamage">The maximum damage on a hit.</param>
        /// <param name="theBlockChance">The probability of blocking damage.</param>
        protected Hero(string theName, int theHitPoints, int theAttackSpeed,
            double theChanceToHit, int theMinDamage, int theMaxDamage,
            double theBlockChance)
            : base(theName, theHitPoints, theAttackSpeed, theChanceToHit,
                theMinDamage, theMaxDamage)
        {
            myBlockChance = theBlockChance;
        }

        /// <summary>
        /// The probability (0.0 to 1.0) that this hero blocks incoming damage.
        /// </summary>
        public double BlockChance => myBlockChance;

        /// <summary>
        /// A read-only view of the items this hero is carrying.
        /// </summary>
        public IReadOnlyList<Item> Inventory => myInventory;

        /// <summary>
        /// The number of distinct pillars this hero has collected.
        /// </summary>
        public int PillarsCollected => myPillarsCollected.Count;

        /// <summary>
        /// The name of this hero's special skill.
        /// </summary>
        public abstract string SpecialSkillName { get; }

        /// <summary>
        /// Adds an item to this hero's inventory.
        /// </summary>
        /// <param name="theItem">The item to pick up.</param>
        public void AddItem(Item theItem)
        {
            myInventory.Add(theItem);
        }
        
        /// <summary>
        /// Removes an item from this hero's inventory.
        /// </summary>
        /// <param name="theItem">The item to remove.</param>
        public void RemoveItem(Item theItem)
        {
            myInventory.Remove(theItem);
        }

        /// <summary>
        /// Removes an item from this hero's inventory, e.g. after it is used.
        /// </summary>
        /// <param name="theItem">The item to remove.</param>
        public void RemoveItem(Item theItem)
        {
            myInventory.Remove(theItem);
        }

        
        /// <summary>
        /// Raised when this hero collects a pillar.
        /// </summary>
        public event EventHandler<PillarType> PillarCollected;
        
        /// <summary>
        /// Records that this hero has collected the given pillar.
        /// </summary>
        /// <param name="theType">The type of pillar collected.</param>
        public void CollectPillar(PillarType theType)
        {
            myPillarsCollected.Add(theType);
            PillarCollected?.Invoke(this, theType);
        }

        /// <summary>
        /// Determines whether this hero has collected all four pillars.
        /// </summary>
        /// <returns>True if every pillar type has been collected.</returns>
        public bool HasAllPillars()
        {
            return myPillarsCollected.Count == TotalPillars;
        }

        /// <summary>
        /// DEBUG ONLY: Clears all collected pillars.
        /// Used for testing and debugging purposes.
        /// </summary>
        internal void DebugClearPillars()
        {
            myPillarsCollected.Clear();
        }
        

        /// <summary>
        /// Applies a health change, giving the hero a chance to block damage.
        /// Positive amounts heal, negative amounts deal damage.
        /// Only damage can be blocked.
        /// </summary>
        /// <param name="theAmount">The signed change to apply to hit points.</param>
        public override void ChangeHealth(int theAmount)
        {
            bool isDamage = theAmount < 0;
            bool blocked = isDamage && Random.Shared.NextDouble() < myBlockChance;
            if (!blocked)
            {
                base.ChangeHealth(theAmount);
            }
        }

        /// <summary>
        /// Executes this hero's unique special skill and returns a description
        /// of the outcome for display in the console.
        /// </summary>
        /// <param name="theTarget">
        /// The opponent this skill is used against, if any. Skills that only
        /// affect the hero (e.g. self-healing) can ignore this.
        /// </param>
        /// <returns>A human-readable summary of the skill's effect.</returns>
        public abstract string UseSpecialSkill(IDungeonCharacter theTarget = null);
    }
    
}