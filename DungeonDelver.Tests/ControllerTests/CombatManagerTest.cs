// Project: TCSS 360 Dungeon Adventure
// File: CombatManagerTest.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Xunit;
using DungeonDelver.Source.Controller;
using DungeonDelver.Source.Model;
using DungeonDelver.Source.Interface;

namespace DungeonDelver.Tests.ControllerTests
{
    /// <summary>
    /// Test suite for the CombatManager class, verifying combat state management,
    /// turn tracking, and combat actions. Many tests expect implementations
    /// that are currently pending (NotImplementedException).
    /// </summary>
    public class CombatManagerTest
    {
        /// <summary>
        /// Verifies that a new CombatManager starts with InCombat false and Turn 0.
        /// </summary>
        [Fact]
        public void Constructor_NotInCombatTurn0()
        {
            CombatManager testManager = new CombatManager();

            Assert.False(testManager.InCombat);
            Assert.Equal(0, testManager.Turn);
        }

        /// <summary>
        /// Verifies that StartCombat sets InCombat to true.
        /// </summary>
        [Fact]
        public void StartCombat_StartsCombat()
        {
            CombatManager testManager = new CombatManager();
            TestPlayer testPlayer = new TestPlayer();
            TestMonster testMonster = new TestMonster();

            testManager.StartCombat(testPlayer, testMonster);

            Assert.True(testManager.InCombat);
        }

        /// <summary>
        /// Verifies that StartCombat stores the player reference.
        /// </summary>
        [Fact]
        public void StartCombat_StoresPlayer()
        {
            CombatManager testManager = new CombatManager();
            TestPlayer testPlayer = new TestPlayer();
            TestMonster testMonster = new TestMonster();

            testManager.StartCombat(testPlayer, testMonster);

            Assert.Equal(testPlayer, testManager.Player);
        }

        /// <summary>
        /// Verifies that StartCombat stores the monster reference.
        /// </summary>
        [Fact]
        public void StartCombat_StoresMonster()
        {
            CombatManager testManager = new CombatManager();
            TestPlayer testPlayer = new TestPlayer();
            TestMonster testMonster = new TestMonster();

            testManager.StartCombat(testPlayer, testMonster);

            Assert.Equal(testMonster, testManager.Monster);
        }

        /// <summary>
        /// Verifies that EndCombat sets InCombat to false.
        /// </summary>
        [Fact]
        public void EndCombat_EndsCombat()
        {
            CombatManager testManager = new CombatManager();
            TestPlayer testPlayer = new TestPlayer();
            TestMonster testMonster = new TestMonster();

            testManager.StartCombat(testPlayer, testMonster);
            testManager.EndCombat();

            Assert.False(testManager.InCombat);
        }

        /// <summary>
        /// Verifies that starting a new combat after ending one resets turn count.
        /// </summary>
        [Fact]
        public void StartCombat_ResetsTurn()
        {
            CombatManager testManager = new CombatManager();
            TestPlayer testPlayer = new TestPlayer();
            TestMonster testMonster = new TestMonster();

            testManager.StartCombat(testPlayer, testMonster);
            testManager.EndCombat();
            testManager.StartCombat(testPlayer, testMonster);

            Assert.Equal(0, testManager.Turn);
        }

        /// <summary>
        /// Verifies that CheckLife ends combat when player reaches zero health.
        /// </summary>
        [Fact]
        public void CheckLife_0HealthPlayerEndsCombat()
        {
            CombatManager testManager = new CombatManager();
            TestPlayer testPlayer = new TestPlayer();
            TestMonster testMonster = new TestMonster();

            testManager.StartCombat(testPlayer, testMonster);
            testPlayer.ChangeHealth(999);
            testManager.CheckLife(testPlayer, testMonster);

            Assert.False(testManager.InCombat);
        }

        /// <summary>
        /// Verifies that CheckLife ends combat when monster reaches zero health.
        /// </summary>
        [Fact]
        public void CheckLife_0HealthMonsterEndsCombat()
        {
            CombatManager testManager = new CombatManager();
            TestPlayer testPlayer = new TestPlayer();
            TestMonster testMonster = new TestMonster();

            testManager.StartCombat(testPlayer, testMonster);
            testMonster.ChangeHealth(999);
            testManager.CheckLife(testPlayer, testMonster);

            Assert.False(testManager.InCombat);
        }

        /// <summary>
        /// Verifies that CheckLife continues combat when both combatants are alive.
        /// </summary>
        [Fact]
        public void CheckLife_NonZeroHealthContinueCombat()
        {
            CombatManager testManager = new CombatManager();
            TestPlayer testPlayer = new TestPlayer();
            TestMonster testMonster = new TestMonster();

            testManager.StartCombat(testPlayer, testMonster);
            testManager.CheckLife(testPlayer, testMonster);

            Assert.True(testManager.InCombat);
        }

        // NOTE: The following tests expect PerformMonsterTurn and PerformPlayerTurn
        // to be implemented. Currently they throw NotImplementedException.

        /// <summary>
        /// Test double: Monster with deterministic stats for testing.
        /// </summary>
        private class TestMonster : Monster
        {
            /// <summary>
            /// Initializes a test monster with fixed stats.
            /// </summary>
            public TestMonster()
                : base("TestMummy", 100, 1, 1.0, 25, 25, 1.0, 5, 5)
            {
            }
        }

        /// <summary>
        /// Test double: Character representing a player for testing.
        /// </summary>
        private class TestPlayer : DungeonCharacter
        {
            /// <summary>
            /// Initializes a test player with fixed stats.
            /// </summary>
            public TestPlayer()
                : base("TestDummy", 100, 1, 1.0, 25, 25)
            {
            }
        }
    }
}
