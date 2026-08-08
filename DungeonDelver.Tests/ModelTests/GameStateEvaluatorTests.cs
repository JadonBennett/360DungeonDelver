using System;
using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class GameStateEvaluatorTests
    {
        private static TestHero HeroWithAllPillars()
        {
            var hero = new TestHero(theBlockChance: 0.0);
            hero.CollectPillar(PillarType.Abstraction);
            hero.CollectPillar(PillarType.Encapsulation);
            hero.CollectPillar(PillarType.Inheritance);
            hero.CollectPillar(PillarType.Polymorphism);
            return hero;
        }

        [Fact]
        public void AllPillarsAtExitAndAlive_Wins()
        {
            var hero = HeroWithAllPillars();
            Assert.Equal(GameStatus.Won, GameStateEvaluator.Evaluate(hero, true));
        }

        [Fact]
        public void AllPillarsButNotAtExit_InProgress()
        {
            var hero = HeroWithAllPillars();
            Assert.Equal(GameStatus.InProgress, GameStateEvaluator.Evaluate(hero, false));
        }

        [Fact]
        public void AtExitButMissingPillars_InProgress()
        {
            var hero = new TestHero(theBlockChance: 0.0);
            hero.CollectPillar(PillarType.Abstraction);
            Assert.Equal(GameStatus.InProgress, GameStateEvaluator.Evaluate(hero, true));
        }

        [Fact]
        public void DeadHero_Loses()
        {
            var hero = new TestHero(theBlockChance: 0.0);
            hero.DebugSetHP(0);
            Assert.False(hero.IsAlive);
            Assert.Equal(GameStatus.Lost, GameStateEvaluator.Evaluate(hero, false));
        }

        [Fact]
        public void DeadHeroWithAllPillarsAtExit_StillLoses()
        {
            // Loss takes priority: a dead hero cannot win, even on the exit.
            var hero = HeroWithAllPillars();
            hero.DebugSetHP(0);
            Assert.Equal(GameStatus.Lost, GameStateEvaluator.Evaluate(hero, true));
        }

        [Fact]
        public void FreshHero_InProgress()
        {
            var hero = new TestHero(theBlockChance: 0.0);
            Assert.Equal(GameStatus.InProgress, GameStateEvaluator.Evaluate(hero, false));
        }

        [Fact]
        public void NullHero_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => GameStateEvaluator.Evaluate(null, true));
        }
    }
}
