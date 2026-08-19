using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class WarriorTests
    {
        [Fact]
        public void Constructor_SetsDefaultWarriorStats()
        {
            var w = new Warrior("Conan");

            Assert.Equal("Conan", w.Name);
            Assert.Equal(125, w.HitPoints);
            Assert.Equal(125, w.MaxHitPoints);
            Assert.Equal(4, w.AttackSpeed);
            Assert.Equal(0.8, w.ChanceToHit, 3);
            Assert.Equal(0.2, w.BlockChance, 3);
            Assert.True(w.IsAlive);
        }

        [Fact]
        public void SpecialSkillName_IsCrushingBlow()
        {
            Assert.Equal("Crushing Blow", new Warrior("Conan").SpecialSkillName);
        }

        [Fact]
        public void Attack_AlwaysZeroOrWithinNormalDamageRange()
        {
            var w = new Warrior("Conan");
            for (int i = 0; i < 5000; i++)
            {
                int dmg = w.Attack();
                Assert.True(dmg == 0 || (dmg >= 35 && dmg <= 60),
                    $"Unexpected damage value: {dmg}");
            }
        }

        [Fact]
        public void UseSpecialSkill_ProducesBothLandAndMissOverManyRuns()
        {
            var w = new Warrior("Conan");
            bool sawLand = false;
            bool sawMiss = false;

            for (int i = 0; i < 5000 && !(sawLand && sawMiss); i++)
            {
                string result = w.UseSpecialSkill();
                Assert.Contains("Crushing Blow", result);
                if (result.Contains("lands")) sawLand = true;
                if (result.Contains("missed")) sawMiss = true;
            }

            Assert.True(sawLand, "Crushing Blow never landed.");
            Assert.True(sawMiss, "Crushing Blow never missed.");
        }

        [Fact]
        public void UseSpecialSkill_WhenLandsAgainstTarget_AppliesDamageToTarget()
        {
            var w = new Warrior("Conan");
            bool verifiedALand = false;

            for (int i = 0; i < 5000 && !verifiedALand; i++)
            {
                var target = new TestHero(theHitPoints: 1000, theBlockChance: 0.0);
                string result = w.UseSpecialSkill(target);

                if (result.Contains("lands"))
                {
                    Assert.True(target.HitPoints < 1000);
                    verifiedALand = true;
                }
                else
                {
                    Assert.Equal(1000, target.HitPoints);
                }
            }

            Assert.True(verifiedALand, "Crushing Blow never landed against the target.");
        }
    }
}