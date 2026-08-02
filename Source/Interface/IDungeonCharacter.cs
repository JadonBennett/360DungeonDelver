// Project: TCSS 360 Dungeon Adventure
// File: IDungeonCharacter.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Interface
{
    /// <summary>
    /// Defines the contract for all dungeon characters, whether heroes or monsters.
    /// Provides access to combat statistics and health management.
    /// </summary>
    public interface IDungeonCharacter
    {
        /// <summary>
        /// The display name of this character.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The current hit points of this character.
        /// </summary>
        int HitPoints { get; }

        /// <summary>
        /// The maximum hit points this character can have.
        /// </summary>
        int MaxHitPoints { get; }

        /// <summary>
        /// The attack speed of this character, determining turn order.
        /// </summary>
        int AttackSpeed { get; }

        /// <summary>
        /// The probability (0.0 to 1.0) that this character's attack will hit.
        /// </summary>
        double ChanceToHit { get; }

        /// <summary>
        /// True if this character has more than zero hit points remaining.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Performs an attack, returning the amount of damage dealt.
        /// </summary>
        /// <returns>The damage dealt by this attack, or zero if the attack misses.</returns>
        int Attack();

        /// <summary>
        /// Adjusts this character's hit points by the given amount,
        /// clamping the result between zero and MaxHitPoints.
        /// </summary>
        /// <param name="theAmount">The signed change to apply to hit points.</param>
        void ChangeHealth(int theAmount);
    }
}
