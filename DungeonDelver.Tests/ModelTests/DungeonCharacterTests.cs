// Project: TCSS 360 Dungeon Adventure
// File: DungeonCharacterTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests
{
    /// <summary>
    /// Test suite for the DungeonCharacter abstract base class,
    /// verifying attack mechanics, health changes, and alive status.
    /// </summary>
    public class DungeonCharacterTests
    {
        /// <summary>
        /// Verifies that a character with 100% hit chance deals expected damage.
        /// </summary>
        [Fact]
        public void TestDungeonCharacter_AttackHit()
        {
            DungeonCharacter testCharacter = new HitCharacterMaxHealth();
            int damageDealt = testCharacter.Attack();

            Assert.Equal(10, damageDealt);
        }

        /// <summary>
        /// Verifies that a character with 0% hit chance deals no damage.
        /// </summary>
        [Fact]
        public void TestDungeonCharacter_AttackMiss()
        {
            DungeonCharacter testCharacter = new MissCharacterLowHealth();
            int damageDealt = testCharacter.Attack();

            Assert.Equal(0, damageDealt);
        }

        /// <summary>
        /// Verifies that positive healing increases hit points.
        /// </summary>
        [Fact]
        public void ChangeHealth_IncreaseHealth()
        {
            DungeonCharacter testCharacter = new MissCharacterLowHealth();
            testCharacter.ChangeHealth(10);

            Assert.Equal(20, testCharacter.HitPoints);
        }

        /// <summary>
        /// Verifies that negative damage decreases hit points.
        /// </summary>
        [Fact]
        public void ChangeHealth_DecreaseHealth()
        {
            DungeonCharacter testCharacter = new HitCharacterMaxHealth();
            testCharacter.ChangeHealth(-10);

            Assert.Equal(90, testCharacter.HitPoints);
        }

        /// <summary>
        /// Verifies that healing does not increase hit points above maximum.
        /// </summary>
        [Fact]
        public void ChangeHealth_IncreaseHealthNotOverMax()
        {
            DungeonCharacter testCharacter = new HitCharacterMaxHealth();
            testCharacter.ChangeHealth(100);

            Assert.Equal(testCharacter.MaxHitPoints, testCharacter.HitPoints);
        }

        /// <summary>
        /// Verifies that damage does not decrease hit points below zero.
        /// </summary>
        [Fact]
        public void ChangeHealth_DecreaseHealthNotUnderMin()
        {
            DungeonCharacter testCharacter = new MissCharacterLowHealth();
            testCharacter.ChangeHealth(-100);

            Assert.Equal(0, testCharacter.HitPoints);
        }

        /// <summary>
        /// Verifies that a character with positive hit points is alive.
        /// </summary>
        [Fact]
        public void PositiveHealth_IsAlive()
        {
            DungeonCharacter testCharacter = new HitCharacterMaxHealth();

            Assert.True(testCharacter.IsAlive);
        }

        /// <summary>
        /// Verifies that a character with zero hit points is not alive.
        /// </summary>
        [Fact]
        public void ZeroHealthIsAliveFalse()
        {
            DungeonCharacter testCharacter = new DeadCharacter();

            Assert.False(testCharacter.IsAlive);
        }

        /// <summary>
        /// Test double: Character with 100% hit chance and maximum health.
        /// </summary>
        private class HitCharacterMaxHealth : DungeonCharacter
        {
            /// <summary>
            /// Initializes a test character that always hits.
            /// </summary>
            public HitCharacterMaxHealth()
                : base("Hit", 100, 1, 1.0, 10, 10)
            {
            }
        }

        /// <summary>
        /// Test double: Character with 0% hit chance and low health.
        /// </summary>
        private class MissCharacterLowHealth : DungeonCharacter
        {
            /// <summary>
            /// Initializes a test character that always misses.
            /// </summary>
            public MissCharacterLowHealth()
                : base("Miss", 10, 1, 0.0, 10, 10)
            {
            }
        }

        /// <summary>
        /// Test double: Character with zero hit points.
        /// </summary>
        private class DeadCharacter : DungeonCharacter
        {
            /// <summary>
            /// Initializes a test character with zero health.
            /// </summary>
            public DeadCharacter()
                : base("Dead", 0, 1, 0.0, 10, 10)
            {
            }
        }
    }
}
