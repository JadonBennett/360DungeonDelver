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
        /// The log of messages describing actions taken during the active
        /// combat encounter, in chronological order.
        /// </summary>
        private readonly List<string> myCombatLog = new();

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
        /// The log of messages describing actions taken during the active
        /// combat encounter, in chronological order.
        /// </summary>
        public IReadOnlyList<string> CombatLog => myCombatLog;

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
        }

        /// <summary>
        /// Executes the monster's turn: it may heal itself, then attacks the player.
        /// </summary>
        public void PerformMonsterTurn()
        {
            if (!myInCombat)
            {
                return;
            }

            if (myMonster is Monster monster)
            {
                monster.Heal();
            }

            int damage = myMonster.Attack();
            myPlayer.ChangeHealth(-damage);
            myCombatLog.Add(damage > 0
                ? $"{myMonster.Name} hits {myPlayer.Name} for {damage} damage."
                : $"{myMonster.Name}'s attack misses.");

            CheckLife(myPlayer, myMonster);
        }

        /// <summary>
        /// Executes the player's turn based on the given action command,
        /// then resolves the monster's turn if combat is still ongoing.
        /// </summary>
        /// <param name="theAction">The action to perform (e.g., "attack", "special", "use item", "run").</param>
        public void PerformPlayerTurn(string theAction)
        {
            if (!myInCombat)
            {
                return;
            }

            switch (theAction?.ToLower())
            {
                case "attack":
                    Attack();
                    break;
                case "special":
                    UseSpecial();
                    break;
                case "use item":
                case "item":
                    UseItem();
                    break;
                case "run":
                    Run();
                    break;
                default:
                    return;
            }

            CheckLife(myPlayer, myMonster);

            if (myInCombat)
            {
                PerformMonsterTurn();
            }

            myTurn++;
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
                myCombatLog.Add($"{myPlayer.Name} flees from combat!");
                EndCombat();
            }
            else
            {
                myCombatLog.Add($"{myPlayer.Name} failed to flee!");
            }
        }

        /// <summary>
        /// Performs a basic attack action against the monster.
        /// </summary>
        private void Attack()
        {
            int damage = myPlayer.Attack();
            myMonster.ChangeHealth(-damage);
            myCombatLog.Add(damage > 0
                ? $"{myPlayer.Name} hits {myMonster.Name} for {damage} damage."
                : $"{myPlayer.Name}'s attack misses.");
        }

        /// <summary>
        /// Uses the first healing potion found in the player's inventory, if any.
        /// </summary>
        private void UseItem()
        {
            if (myPlayer is Hero hero)
            {
                HealingPotion potion = null;

                foreach (Item item in hero.Inventory)
                {
                    if (item is HealingPotion healingPotion)
                    {
                        potion = healingPotion;
                        break;
                    }
                }

                if (potion != null)
                {
                    string result = potion.Use(hero);
                    hero.RemoveItem(potion);
                    myCombatLog.Add(result);
                    return;
                }
            }

            myCombatLog.Add($"{myPlayer.Name} has no items to use.");
        }

        /// <summary>
        /// Uses the player character's special ability against the monster.
        /// </summary>
        private void UseSpecial()
        {
            if (myPlayer is Hero hero)
            {
                string result = hero.UseSpecialSkill(myMonster);
                myCombatLog.Add(result);
            }
        }
    }
}
