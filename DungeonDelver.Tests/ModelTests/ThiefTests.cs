using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class ThiefTests
    {
        [Fact]
        public void Constructor_SetsDefaultThiefStats()
        {
            var t = new Thief("Garrett");

            Assert.Equal("Garrett", t.Name);
            Assert.Equal(75, t.HitPoints);
            Assert.Equal(75, t.MaxHitPoints);
            Assert.Equal(6, t.AttackSpeed);
            Assert.Equal(0.8, t.ChanceToHit, 3);
            Assert.Equal(0.4, t.BlockChance, 3);
        }

        [Fact]
        public void SpecialSkillName_IsSurpriseAttack()
        {
            Assert.Equal("Surprise Attack", new Thief("Garrett").SpecialSkillName);
        }

        [Fact]
        public void UseSpecialSkill_ProducesAllThreeOutcomesOverManyRuns()
        {
            var t = new Thief("Garrett");
            bool sawCaught = false;
            bool sawSurprise = false;
            bool sawNormal = false;

            for (int i = 0; i < 5000; i++)
            {
                string result = t.UseSpecialSkill();
                Assert.Contains("Garrett", result);
                if (result.Contains("caught")) sawCaught = true;
                else if (result.Contains("Surprise Attack")) sawSurprise = true;
                else if (result.Contains("normally")) sawNormal = true;
            }

            Assert.True(sawCaught, "Never saw the 'caught' outcome.");
            Assert.True(sawSurprise, "Never saw the 'surprise attack' outcome.");
            Assert.True(sawNormal, "Never saw the 'normal attack' outcome.");
        }
    }
}