// Project: TCSS 360 Dungeon Adventure
// File: CombatManager.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using System.Collections.Generic;
using DungeonDelver.Source.Interface;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Source.Controller
{
    /// <summary>
    /// Manages combat encounters between a player character and a monster,
    /// including turn tracking, action processing, and combat state.
    /// </summary>
    public class CombatManager : ICombatManager
    {
        
        /// <summary>
        /// The possible results of a combat encounter ending.
        /// </summary>
        public enum CombatOutcome
        {
            PlayerWon,
            PlayerDefeated,
            PlayerFled
        }

        /// <summary>
        /// Raised when combat ends, indicating the outcome.
        /// </summary>
        public event EventHandler<CombatOutcome> CombatEnded;

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
        /// Log of combat actions and outcomes for display.
        /// </summary>
        private readonly List<string> myCombatLog;

        /// <summary>
        /// Initializes a new CombatManager with no active combat.
        /// </summary>
        public CombatManager()
        {
            myTurn = 0;
            myInCombat = false;
            myPlayer = null;
            myMonster = null;
            myCombatLog = new List<string>();
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
        /// Gets a read-only copy of the combat log.
        /// </summary>
        public IReadOnlyList<string> CombatLog => myCombatLog.AsReadOnly();

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
            myCombatLog.Clear();
            myCombatLog.Add($"Combat started! {thePlayer.Name} vs {theMonster.Name}");
        }

        /// <summary>
        /// Executes the monster's turn, performing its attack or special action.
        /// </summary>
        public void PerformMonsterTurn()
        {
            if (!myInCombat || myMonster == null || myPlayer == null)
            {
                return;
            }

            int damage = myMonster.Attack();

            if (damage > 0)
            {
                // Check HP before and after to detect blocking
                int hpBefore = myPlayer.HitPoints;
                myPlayer.ChangeHealth(-damage);
                int hpAfter = myPlayer.HitPoints;
                int actualDamage = hpBefore - hpAfter;

                if (actualDamage == 0)
                {
                    myCombatLog.Add($"{myMonster.Name} attacks but {myPlayer.Name} blocked it!");
                }
                else
                {
                    myCombatLog.Add($"{myMonster.Name} attacks for {actualDamage} damage!");
                }
            }
            else
            {
                myCombatLog.Add($"{myMonster.Name} attacks but misses!");
            }

            myTurn++;
            CheckLife(myPlayer, myMonster);
        }

        /// <summary>
        /// Executes the player's turn based on the given action command.
        /// <param name="theAction">The action to perform (e.g., "attack", "special", "use item", "run").</param>
        /// <param name="theItemIndex">
        /// The index into the player's inventory of the item to use. Required
        /// when <paramref name="theAction"/> is "use item".
        /// </param>
        public void PerformPlayerTurn(string theAction, int theItemIndex = -1)
        {
            if (!myInCombat || myPlayer == null || myMonster == null)
            {
                return;
            }

            switch (theAction?.ToLowerInvariant())
            {
                case "attack":
                    Attack();
                    break;
                case "special":
                    UseSpecial();
                    break;
                case "use item":
                    UseItem(theItemIndex);
                    break;
                case "run":
                    Run();
                    break;
                default:
                    throw new ArgumentException($"Unknown action: {theAction}", nameof(theAction));
            }

            myTurn++;
            CheckLife(myPlayer, myMonster);
        }

        /// <summary>
        /// Checks whether either combatant has been defeated and ends combat if so.
        /// </summary>
        /// <param name="thePlayer">The player character to check.</param>
        /// <param name="theMonster">The monster to check.</param>
        public void CheckLife(IDungeonCharacter thePlayer, IDungeonCharacter theMonster)
        {
            if (!thePlayer.IsAlive)
            {
                myCombatLog.Add($"{thePlayer.Name} has been defeated!");
                myInCombat = false;
                CombatEnded?.Invoke(this, CombatOutcome.PlayerDefeated);
            }
            else if (!theMonster.IsAlive)
            {
                myCombatLog.Add($"{theMonster.Name} has been defeated!");
                myInCombat = false;
                CombatEnded?.Invoke(this, CombatOutcome.PlayerWon);
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
                myCombatLog.Add($"{myPlayer.Name} fled from combat!");
                EndCombat();
                CombatEnded?.Invoke(this, CombatOutcome.PlayerFled);
            }
            else
            {
                myCombatLog.Add($"{myPlayer.Name} tried to flee but failed!");
            }
        }

        /// <summary>
        /// Performs a basic attack action. Player attacks monster
        /// </summary>
        private void Attack()
        {
            int damage = myPlayer.Attack();
            myMonster.ChangeHealth(-damage);

            if (damage > 0)
            {
                myCombatLog.Add($"{myPlayer.Name} attacks for {damage} damage!");
            }
            else
            {
                myCombatLog.Add($"{myPlayer.Name} attacks but misses!");
            }
        }

        /// <summary>
        /// Uses an item from the player's inventory.
        /// </summary>
        ///<param name="theItemIndex">The index of the item to use.</param>
        private void UseItem(int theItemIndex)
        {
            if (myPlayer is not Hero hero)
            {
                throw new InvalidOperationException("Only a Hero can use items.");
            }

            if (theItemIndex < 0 || theItemIndex >= hero.Inventory.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(theItemIndex), "No item at that inventory index.");
            }

            Item item = hero.Inventory[theItemIndex];
            myCombatLog.Add($"{hero.Name} used {item.Name}!");
            item.Use(hero);
            hero.RemoveItem(item);
        }

        /// <summary>
        /// Uses the player character's special ability.
        /// </summary>
        private void UseSpecial()
        {
            if (myPlayer is not Hero hero)
            {
                throw new InvalidOperationException("Only a Hero has a special skill.");
            }

            // UseSpecialSkill returns a description of the outcome (hit/miss)
            string result = hero.UseSpecialSkill(myMonster);
            myCombatLog.Add(result);
        }
    }
}
