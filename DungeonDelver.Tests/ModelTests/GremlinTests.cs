// Project: TCSS 360 Dungeon Adventure
// File: GremlinTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests
{
    /// <summary>
    /// Test suite for the Gremlin monster class,
    /// verifying that constructor properly initializes all statistics.
    /// </summary>
    public class GremlinTests
    {
        /// <summary>
        /// Verifies that the Gremlin constructor sets all stats correctly.
        /// </summary>
        [Fact]
        public void GremlinConstructorTest_SetsStats()
        {
            TestGremlin testGremlin = new TestGremlin();

            Assert.Equal("Grot", testGremlin.Name);
            Assert.Equal(70, testGremlin.HitPoints);
            Assert.Equal(70, testGremlin.MaxHitPoints);
            Assert.Equal(5, testGremlin.AttackSpeed);
            Assert.Equal(0.8, testGremlin.ChanceToHit);
            Assert.Equal(15, testGremlin.TestMinDamage);
            Assert.Equal(30, testGremlin.TestMaxDamage);
            Assert.Equal(0.4, testGremlin.TestChanceToHeal);
            Assert.Equal(20, testGremlin.TestMinHeal);
            Assert.Equal(40, testGremlin.TestMaxHeal);
            Assert.True(testGremlin.IsAlive);
        }

        /// <summary>
        /// Test wrapper for Gremlin that exposes protected properties for verification.
        /// </summary>
        private class TestGremlin : Gremlin
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
