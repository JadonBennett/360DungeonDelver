// Project: TCSS 360 Dungeon Adventure
// File: CombatManager.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using DungeonDelver.Source.Interface;

namespace DungeonDelver.Source.Controller
{
    /// <summary>
    /// Manages combat encounters between a player character and a monster,
    /// including turn tracking, action processing, and combat state.
    /// </summary>
    public class CombatManager : ICombatManager
    {
        /// <summary>
        /// The probability that a run attempt will succeed.
        /// </summary>
        private const double RUN_SUCCESS_CHANCE = 0.5;

        /// <summary>
        /// The current turn number in the active combat encounter.
        /// </summary>
        private int myTurn;

        /// <summary>
        /// True if a combat encounter is currently in progress.
        /// </summary>
        private bool myInCombat;

        /// <summary>
        /// The player character in the active combat encounter.
        /// </summary>
        private IDungeonCharacter myPlayer;

        /// <summary>
        /// The monster in the active combat encounter.
        /// </summary>
        private IDungeonCharacter myMonster;

        /// <summary>
        /// Initializes a new CombatManager with no active combat.
        /// </summary>
        public CombatManager()
        {
            myTurn = 0;
            myInCombat = false;
            myPlayer = null;
            myMonster = null;
        }

        /// <summary>
        /// The current turn number in the active combat encounter.
        /// </summary>
        public int Turn => myTurn;

        /// <summary>
        /// True if a combat encounter is currently in progress.
        /// </summary>
        public bool InCombat => myInCombat;

        /// <summary>
        /// The player character in the active combat encounter.
        /// </summary>
        public IDungeonCharacter Player => myPlayer;

        /// <summary>
        /// The monster in the active combat encounter.
        /// </summary>
        public IDungeonCharacter Monster => myMonster;

        /// <summary>
        /// Initiates a combat encounter between the given player and monster.
        /// Sets combat state to active and stores references to the combatants.
        /// </summary>
        /// <param name="thePlayer">The player character.</param>
        /// <param name="theMonster">The monster opponent.</param>
        public void StartCombat(IDungeonCharacter thePlayer, IDungeonCharacter theMonster)
        {
            myPlayer = thePlayer;
            myMonster = theMonster;
            myInCombat = true;
            myTurn = 0;
        }

        /// <summary>
        /// Executes the monster's turn, performing its attack or special action.
        /// Implementation pending.
        /// </summary>
        public void PerformMonsterTurn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the player's turn based on the given action command.
        /// Implementation pending.
        /// </summary>
        /// <param name="theAction">The action to perform (e.g., "attack", "special", "use item", "run").</param>
        public void PerformPlayerTurn(string theAction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether either combatant has been defeated and ends combat if so.
        /// </summary>
        /// <param name="thePlayer">The player character to check.</param>
        /// <param name="theMonster">The monster to check.</param>
        public void CheckLife(IDungeonCharacter thePlayer, IDungeonCharacter theMonster)
        {
            if (!thePlayer.IsAlive || !theMonster.IsAlive)
            {
                myInCombat = false;
            }
        }

        /// <summary>
        /// Ends the current combat encounter and resets combat state.
        /// </summary>
        public void EndCombat()
        {
            myInCombat = false;
        }

        /// <summary>
        /// Resets the turn counter to zero.
        /// </summary>
        public void ResetTurn()
        {
            myTurn = 0;
        }

        /// <summary>
        /// Attempts to flee from combat with a 50% success chance.
        /// If successful, ends the combat encounter.
        /// </summary>
        private void Run()
        {
            double runRoll = Random.Shared.NextDouble();

            if (runRoll < RUN_SUCCESS_CHANCE)
            {
                EndCombat();
            }
        }

        /// <summary>
        /// Performs a basic attack action. Implementation pending.
        /// </summary>
        private void Attack()
        {
            // TODO: Implement attack logic
        }

        /// <summary>
        /// Uses an item from the player's inventory. Implementation pending.
        /// </summary>
        private void UseItem()
        {
            // TODO: Implement item usage logic
        }

        /// <summary>
        /// Uses the player character's special ability. Implementation pending.
        /// </summary>
        private void UseSpecial()
        {
            // TODO: Implement special ability logic
        }
    }
}
