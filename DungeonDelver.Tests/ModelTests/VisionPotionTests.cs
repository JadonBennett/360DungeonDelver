using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class VisionPotionTests
    {
        [Fact]
        public void Name_IsVisionPotion()
        {
            Assert.Equal("Vision Potion", new VisionPotion().Name);
        }

        [Fact]
        public void Use_ReturnsRevealMessageForHero()
        {
            var hero = new TestHero("Cade");

            string result = new VisionPotion().Use(hero);

            Assert.Contains("Cade", result);
            Assert.Contains("Vision Potion", result);
            Assert.Contains("revealed", result);
        }

        [Fact]
        public void Use_DoesNotChangeHeroHitPoints()
        {
            var hero = new TestHero(theHitPoints: 80, theBlockChance: 0.0);

            new VisionPotion().Use(hero);

            Assert.Equal(80, hero.HitPoints);
        }
    }
}