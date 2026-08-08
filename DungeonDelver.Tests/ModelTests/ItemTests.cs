using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class ItemTests
    {
        [Fact]
        public void Name_ReturnsValueGivenToConstructor()
        {
            var item = new TestItem("Magic Rock");
            Assert.Equal("Magic Rock", item.Name);
        }

        [Fact]
        public void Use_ReturnsEffectDescription()
        {
            var hero = new TestHero("Ari");
            var item = new TestItem("Magic Rock");

            string result = item.Use(hero);

            Assert.Contains("Ari", result);
            Assert.Contains("Magic Rock", result);
        }
    }
}