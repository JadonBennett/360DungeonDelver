// Project: TCSS 360 Dungeon Adventure
// File: MonsterTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests
{
    /// <summary>
    /// Test suite for the Monster abstract base class,
    /// verifying healing mechanics and health clamping.
    /// </summary>
    public class MonsterTests
    {
        /// <summary>
        /// Verifies that a monster with 100% heal chance successfully heals.
        /// </summary>
        [Fact]
        public void TestMonsterHeal()
        {
            Monster testMonster = new HealMonster();
            testMonster.Heal();

            Assert.Equal(60, testMonster.HitPoints);
        }

        /// <summary>
        /// Verifies that healing does not increase hit points above maximum.
        /// </summary>
        [Fact]
        public void TestMonsterHeal_NeverHealOverMax()
        {
            Monster testMonster = new HealMonster();
            testMonster.ChangeHealth(45);
            testMonster.Heal();

            Assert.Equal(100, testMonster.HitPoints);
        }

        /// <summary>
        /// Verifies that a monster with 0% heal chance does not heal.
        /// </summary>
        [Fact]
        public void TestMonsterNeverHeal()
        {
            Monster testMonster = new NoHealMonster();
            testMonster.Heal();

            Assert.Equal(50, testMonster.HitPoints);
        }

        /// <summary>
        /// Test double: Monster with 100% heal chance.
        /// </summary>
        private class HealMonster : Monster
        {
            /// <summary>
            /// Initializes a test monster that always heals.
            /// </summary>
            public HealMonster()
                : base("HealMonster", 50, 1, 0.5, 5, 10, 1.0, 10, 10)
            {
            }
        }

        /// <summary>
        /// Test double: Monster with 0% heal chance.
        /// </summary>
        private class NoHealMonster : Monster
        {
            /// <summary>
            /// Initializes a test monster that never heals.
            /// </summary>
            public NoHealMonster()
                : base("NoHealMonster", 50, 1, 0.5, 5, 10, 0.0, 10, 10)
            {
            }
        }
    }
}
