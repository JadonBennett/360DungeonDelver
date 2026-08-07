// Project: TCSS 360 Dungeon Adventure
// File: DungeonCharacter.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using DungeonDelver.Source.Interface;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Abstract base class representing any character in the dungeon,
    /// whether hero or monster. Provides common combat and health mechanics.
    /// </summary>
    public abstract class DungeonCharacter : IDungeonCharacter
    {
        /// <summary>
        /// The display name of this character.
        /// </summary>
        private readonly string myName;

        /// <summary>
        /// The current hit points of this character.
        /// </summary>
        private int myHitPoints;

        /// <summary>
        /// The maximum hit points this character can have.
        /// </summary>
        private readonly int myMaxHitPoints;

        /// <summary>
        /// The attack speed of this character, determining turn order.
        /// </summary>
        private readonly int myAttackSpeed;

        /// <summary>
        /// The probability (0.0 to 1.0) that this character's attack will hit.
        /// </summary>
        private readonly double myChanceToHit;

        /// <summary>
        /// The minimum damage this character can deal on a successful attack.
        /// </summary>
        private readonly int myMinDamage;

        /// <summary>
        /// The maximum damage this character can deal on a successful attack.
        /// </summary>
        private readonly int myMaxDamage;

        /// <summary>
        /// Initializes a new DungeonCharacter with the specified combat statistics.
        /// </summary>
        /// <param name="theName">The display name of this character.</param>
        /// <param name="theHitPoints">The starting and maximum hit points.</param>
        /// <param name="theAttackSpeed">The attack speed value.</param>
        /// <param name="theChanceToHit">The probability of landing an attack.</param>
        /// <param name="theMinDamage">The minimum damage on a hit.</param>
        /// <param name="theMaxDamage">The maximum damage on a hit.</param>
        protected DungeonCharacter(
            string theName,
            int theHitPoints,
            int theAttackSpeed,
            double theChanceToHit,
            int theMinDamage,
            int theMaxDamage)
        {
            myName = theName;
            myHitPoints = theHitPoints;
            myMaxHitPoints = theHitPoints;
            myAttackSpeed = theAttackSpeed;
            myChanceToHit = theChanceToHit;
            myMinDamage = theMinDamage;
            myMaxDamage = theMaxDamage;
        }

        /// <summary>
        /// The display name of this character.
        /// </summary>
        public string Name => myName;

        /// <summary>
        /// The current hit points of this character.
        /// </summary>
        public int HitPoints => myHitPoints;

        /// <summary>
        /// The maximum hit points this character can have.
        /// </summary>
        public int MaxHitPoints => myMaxHitPoints;

        /// <summary>
        /// The attack speed of this character, determining turn order.
        /// </summary>
        public int AttackSpeed => myAttackSpeed;

        /// <summary>
        /// The probability (0.0 to 1.0) that this character's attack will hit.
        /// </summary>
        public double ChanceToHit => myChanceToHit;

        /// <summary>
        /// The minimum damage this character can deal on a successful attack.
        /// </summary>
        protected int MinDamage => myMinDamage;

        /// <summary>
        /// The maximum damage this character can deal on a successful attack.
        /// </summary>
        protected int MaxDamage => myMaxDamage;

        /// <summary>
        /// True if this character has more than zero hit points remaining.
        /// </summary>
        public bool IsAlive => myHitPoints > 0;

        /// <summary>
        /// Performs an attack, returning the amount of damage dealt.
        /// Damage is zero if the attack misses based on ChanceToHit,
        /// otherwise a random value between MinDamage and MaxDamage inclusive.
        /// </summary>
        /// <returns>The damage dealt by this attack.</returns>
        public virtual int Attack()
        {
            double hitRoll = Random.Shared.NextDouble();
            int damageDealt;

            if (hitRoll < myChanceToHit)
            {
                damageDealt = Random.Shared.Next(myMinDamage, myMaxDamage + 1);
            }
            else
            {
                damageDealt = 0;
            }

            return damageDealt;
        }

        
        /// <summary>
        /// Raised when this character's hit points change.
        /// </summary>
        public event EventHandler<int> HealthChanged;
        /// <summary>
        /// Adjusts this character's hit points by the given amount,
        /// clamping the result between zero and MaxHitPoints.
        /// Positive amounts heal, negative amounts deal damage.
        /// </summary>
        /// <param name="theAmount">The signed change to apply to hit points.</param>
        public virtual void ChangeHealth(int theAmount)
        {
            myHitPoints -= theAmount;

            if (myHitPoints > myMaxHitPoints)
            {
                myHitPoints = myMaxHitPoints;
            }

            if (myHitPoints < 0)
            {
                myHitPoints = 0;
            }
            HealthChanged?.Invoke(this, myHitPoints);
        }
    }
}
