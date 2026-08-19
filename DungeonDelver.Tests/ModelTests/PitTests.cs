// Project: TCSS 360 Dungeon Adventure
// File: PitTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    /// <summary>
    /// Test suite for the Pit class, verifying damage range and application.
    /// </summary>
    public class PitTests
    {
        [Fact]
        public void Trigger_DamageWithinConfiguredRange()
        {
            var pit = new Pit(10, 20);

            for (int i = 0; i < 500; i++)
            {
                var freshHero = new TestHero(theHitPoints: 1000, theBlockChance: 0.0);
                int damage = pit.Trigger(freshHero);
                Assert.InRange(damage, 10, 20);
            }
        }

        [Fact]
        public void Trigger_AppliesDamageToHero()
        {
            var pit = new Pit(15, 15);
            var hero = new TestHero(theHitPoints: 100, theBlockChance: 0.0);

            int damage = pit.Trigger(hero);

            Assert.Equal(15, damage);
            Assert.Equal(85, hero.HitPoints);
        }

        [Fact]
        public void Trigger_CanBeCalledRepeatedly()
        {
            var pit = new Pit(5, 5);
            var hero = new TestHero(theHitPoints: 100, theBlockChance: 0.0);

            pit.Trigger(hero);
            pit.Trigger(hero);

            Assert.Equal(90, hero.HitPoints);
        }
    }
}
