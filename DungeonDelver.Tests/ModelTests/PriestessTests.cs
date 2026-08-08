using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class PriestessTests
    {
        [Fact]
        public void Constructor_SetsDefaultPriestessStats()
        {
            var p = new Priestess("Aria");

            Assert.Equal("Aria", p.Name);
            Assert.Equal(75, p.HitPoints);
            Assert.Equal(75, p.MaxHitPoints);
            Assert.Equal(5, p.AttackSpeed);
            Assert.Equal(0.7, p.ChanceToHit, 3);
            Assert.Equal(0.3, p.BlockChance, 3);
        }

        [Fact]
        public void SpecialSkillName_IsHeal()
        {
            Assert.Equal("Heal", new Priestess("Aria").SpecialSkillName);
        }

        [Fact]
        public void UseSpecialSkill_RestoresHitPointsWithinRange()
        {
            // Start at 1 HP so a 25..50 heal never clamps against max (75).
            for (int i = 0; i < 3000; i++)
            {
                var p = new Priestess("Aria");
                p.DebugSetHP(1);

                p.UseSpecialSkill();

                Assert.InRange(p.HitPoints, 26, 51);
            }
        }

        [Fact]
        public void UseSpecialSkill_ReturnsHealMessage()
        {
            var p = new Priestess("Aria");
            p.DebugSetHP(10);

            string result = p.UseSpecialSkill();

            Assert.Contains("Aria", result);
            Assert.Contains("heals for", result);
        }
    }
}