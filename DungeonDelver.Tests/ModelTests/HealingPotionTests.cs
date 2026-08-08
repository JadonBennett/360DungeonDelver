using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class HealingPotionTests
    {
        [Fact]
        public void Name_IsHealingPotion()
        {
            Assert.Equal("Healing Potion", new HealingPotion().Name);
        }

        [Fact]
        public void Use_WithFixedRange_HealsExactAmount()
        {
            // min == max removes randomness for a deterministic check.
            var hero = new TestHero(theHitPoints: 100, theBlockChance: 0.0);
            hero.DebugSetHP(40);
            var potion = new HealingPotion(30, 30);

            potion.Use(hero);

            Assert.Equal(70, hero.HitPoints);
        }

        [Fact]
        public void Use_DoesNotHealAboveMaxHitPoints()
        {
            var hero = new TestHero(theHitPoints: 100, theBlockChance: 0.0);
            hero.DebugSetHP(90);
            var potion = new HealingPotion(50, 50);

            potion.Use(hero);

            Assert.Equal(100, hero.HitPoints);
        }

        [Fact]
        public void Use_DefaultPotion_HealsWithinDefaultRange()
        {
            // Statistical invariant over many runs: default heal is 25..50.
            for (int i = 0; i < 2000; i++)
            {
                var hero = new TestHero(theHitPoints: 1000, theBlockChance: 0.0);
                hero.DebugSetHP(0);
                new HealingPotion().Use(hero);

                Assert.InRange(hero.HitPoints, 25, 50);
            }
        }

        [Fact]
        public void Use_ReturnsDescriptiveMessage()
        {
            var hero = new TestHero("Bex", theBlockChance: 0.0);
            hero.DebugSetHP(10);

            string result = new HealingPotion(20, 20).Use(hero);

            Assert.Contains("Bex", result);
            Assert.Contains("Healing Potion", result);
            Assert.Contains("20", result);
        }
    }
}