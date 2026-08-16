// Project: TCSS 360 Dungeon Adventure
// File: ICombatManager.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

namespace DungeonDelver.Source.Interface
{
    /// <summary>
    /// Defines the contract for managing combat encounters between
    /// a player character and a monster, including turn management
    /// and combat state.
    /// </summary>
    public interface ICombatManager
    {
        /// <summary>
        /// The current turn number in this combat encounter.
        /// </summary>
        int Turn { get; }

        /// <summary>
        /// True if a combat encounter is currently in progress.
        /// </summary>
        bool InCombat { get; }

        /// <summary>
        /// The player character in this combat encounter.
        /// </summary>
        IDungeonCharacter Player { get; }

        /// <summary>
        /// The monster in this combat encounter.
        /// </summary>
        IDungeonCharacter Monster { get; }

        /// <summary>
        /// Initiates a combat encounter between the given player and monster.
        /// </summary>
        /// <param name="thePlayer">The player character.</param>
        /// <param name="theMonster">The monster opponent.</param>
        void StartCombat(IDungeonCharacter thePlayer, IDungeonCharacter theMonster);

        /// <summary>
        /// Executes the monster's turn, performing its attack or special action.
        /// </summary>
        void PerformMonsterTurn();

        /// <summary>
        /// Executes the player's turn based on the given action command.
        /// </summary>
        /// <param name="theAction">The action to perform (e.g., "attack", "special", "use item").</param>
        /// <param name="theItemIndex">
        /// The index into the player's inventory of the item to use. Required
        /// (and only used) when <paramref name="theAction"/> is "use item".
        void PerformPlayerTurn(string theAction, int theItemIndex = -1);

        /// <summary>
        /// Checks whether either combatant has been defeated and ends combat if so.
        /// </summary>
        /// <param name="thePlayer">The player character to check.</param>
        /// <param name="theMonster">The monster to check.</param>
        void CheckLife(IDungeonCharacter thePlayer, IDungeonCharacter theMonster);

        /// <summary>
        /// Ends the current combat encounter and resets combat state.
        /// </summary>
        void EndCombat();

        /// <summary>
        /// Resets the turn counter to zero.
        /// </summary>
        void ResetTurn();
    }
}
