using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class HeroTests
    {
        [Fact]
        public void TotalPillars_IsFour()
        {
            Assert.Equal(4, Hero.TotalPillars);
        }

        [Fact]
        public void NewHero_StartsWithEmptyInventoryAndNoPillars()
        {
            var hero = new TestHero();
            Assert.Empty(hero.Inventory);
            Assert.Equal(0, hero.PillarsCollected);
            Assert.False(hero.HasAllPillars());
        }

        [Fact]
        public void AddItem_AddsToInventory()
        {
            var hero = new TestHero();
            var item = new TestItem();

            hero.AddItem(item);

            Assert.Single(hero.Inventory);
            Assert.Same(item, hero.Inventory[0]);
        }

        [Fact]
        public void CollectPillar_IncrementsCountAndRaisesEvent()
        {
            var hero = new TestHero();
            PillarType? raised = null;
            hero.PillarCollected += (_, type) => raised = type;

            hero.CollectPillar(PillarType.Abstraction);

            Assert.Equal(1, hero.PillarsCollected);
            Assert.Equal(PillarType.Abstraction, raised);
        }

        [Fact]
        public void CollectPillar_DuplicateType_DoesNotDoubleCount()
        {
            var hero = new TestHero();

            hero.CollectPillar(PillarType.Inheritance);
            hero.CollectPillar(PillarType.Inheritance);

            Assert.Equal(1, hero.PillarsCollected);
        }

        [Fact]
        public void HasAllPillars_TrueOnlyAfterAllFourDistinct()
        {
            var hero = new TestHero();

            hero.CollectPillar(PillarType.Abstraction);
            hero.CollectPillar(PillarType.Encapsulation);
            hero.CollectPillar(PillarType.Inheritance);
            Assert.False(hero.HasAllPillars());

            hero.CollectPillar(PillarType.Polymorphism);
            Assert.True(hero.HasAllPillars());
        }

        [Fact]
        public void ChangeHealth_WhenBlockChanceZero_DamageAlwaysApplies()
        {
            var hero = new TestHero(theHitPoints: 100, theBlockChance: 0.0);

            hero.ChangeHealth(-40);

            Assert.Equal(60, hero.HitPoints);
        }

        [Fact]
        public void ChangeHealth_WhenBlockChanceOne_DamageAlwaysBlocked()
        {
            var hero = new TestHero(theHitPoints: 100, theBlockChance: 1.0);

            hero.ChangeHealth(-40);

            Assert.Equal(100, hero.HitPoints);
        }

        [Fact]
        public void ChangeHealth_Healing_IsNeverBlocked()
        {
            // Block chance 1.0 must not stop healing (only damage is blockable).
            var hero = new TestHero(theHitPoints: 100, theBlockChance: 1.0);
            hero.DebugSetHP(50);

            hero.ChangeHealth(+30);

            Assert.Equal(80, hero.HitPoints);
        }

        [Fact]
        public void ChangeHealth_DoesNotExceedMax()
        {
            var hero = new TestHero(theHitPoints: 100, theBlockChance: 0.0);
            hero.DebugSetHP(90);

            hero.ChangeHealth(+50);

            Assert.Equal(100, hero.HitPoints);
        }
    }
}
