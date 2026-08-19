// Project: TCSS 360 Dungeon Adventure
// File: SkeletonTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests
{
    /// <summary>
    /// Test suite for the Skeleton monster class,
    /// verifying that constructor properly initializes all statistics.
    /// </summary>
    public class SkeletonTests
    {
        /// <summary>
        /// Verifies that the Skeleton constructor sets all stats correctly.
        /// </summary>
        [Fact]
        public void SkeletonConstructorTest_SetsStats()
        {
            TestSkeleton testSkeleton = new TestSkeleton();

            Assert.Equal("Skellington", testSkeleton.Name);
            Assert.Equal(100, testSkeleton.HitPoints);
            Assert.Equal(100, testSkeleton.MaxHitPoints);
            Assert.Equal(3, testSkeleton.AttackSpeed);
            Assert.Equal(0.8, testSkeleton.ChanceToHit);
            Assert.Equal(30, testSkeleton.TestMinDamage);
            Assert.Equal(50, testSkeleton.TestMaxDamage);
            Assert.Equal(0.3, testSkeleton.TestChanceToHeal);
            Assert.Equal(30, testSkeleton.TestMinHeal);
            Assert.Equal(50, testSkeleton.TestMaxHeal);
            Assert.True(testSkeleton.IsAlive);
        }

        /// <summary>
        /// Test wrapper for Skeleton that exposes protected properties for verification.
        /// </summary>
        private class TestSkeleton : Skeleton
        {
            /// <summary>
            /// Creates a test skeleton with default stats.
            /// </summary>
            public TestSkeleton() : base("Test Skeleton", 100, 5, 0.8, 10, 20, 0.4, 5, 15)
            {
            }

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
