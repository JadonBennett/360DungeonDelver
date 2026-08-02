// Project: TCSS 360 Dungeon Adventure
// File: OgreTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests
{
    /// <summary>
    /// Test suite for the Ogre monster class,
    /// verifying that constructor properly initializes all statistics.
    /// </summary>
    public class OgreTests
    {
        /// <summary>
        /// Verifies that the Ogre constructor sets all stats correctly.
        /// </summary>
        [Fact]
        public void OgreConstructorTest_SetsStats()
        {
            TestOgre testOgre = new TestOgre();

            Assert.Equal("Shrek", testOgre.Name);
            Assert.Equal(200, testOgre.HitPoints);
            Assert.Equal(200, testOgre.MaxHitPoints);
            Assert.Equal(2, testOgre.AttackSpeed);
            Assert.Equal(0.6, testOgre.ChanceToHit);
            Assert.Equal(30, testOgre.TestMinDamage);
            Assert.Equal(60, testOgre.TestMaxDamage);
            Assert.Equal(0.1, testOgre.TestChanceToHeal);
            Assert.Equal(30, testOgre.TestMinHeal);
            Assert.Equal(60, testOgre.TestMaxHeal);
            Assert.True(testOgre.IsAlive);
        }

        /// <summary>
        /// Test wrapper for Ogre that exposes protected properties for verification.
        /// </summary>
        private class TestOgre : Ogre
        {
            /// <summary>
            /// Exposes MinDamage for testing.
            /// </summary>
            public int TestMinDamage => MinDamage;

            /// <summary>
            /// Exposes MaxDamage for testing.
            /// </summary>
            public int TestMaxDamage => MaxDamage;

            /// <summary>
            /// Exposes ChanceToHeal for testing.
            /// </summary>
            public double TestChanceToHeal => ChanceToHeal;

            /// <summary>
            /// Exposes MinHeal for testing.
            /// </summary>
            public double TestMinHeal => MinHeal;

            /// <summary>
            /// Exposes MaxHeal for testing.
            /// </summary>
            public double TestMaxHeal => MaxHeal;
        }
    }
}
