using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    public class PillarTests
    {
        [Theory]
        [InlineData(PillarType.Abstraction, "Pillar of Abstraction")]
        [InlineData(PillarType.Encapsulation, "Pillar of Encapsulation")]
        [InlineData(PillarType.Inheritance, "Pillar of Inheritance")]
        [InlineData(PillarType.Polymorphism, "Pillar of Polymorphism")]
        public void Name_MatchesPillarType(PillarType type, string expectedName)
        {
            Assert.Equal(expectedName, new Pillar(type).Name);
        }

        [Fact]
        public void PillarType_ReturnsConstructorValue()
        {
            Assert.Equal(PillarType.Polymorphism, new Pillar(PillarType.Polymorphism).PillarType);
        }

        [Fact]
        public void Use_RecordsPillarOnHero()
        {
            var hero = new TestHero();
            var pillar = new Pillar(PillarType.Encapsulation);

            pillar.Use(hero);

            Assert.Equal(1, hero.PillarsCollected);
        }

        [Fact]
        public void Use_ReturnsCollectionMessage()
        {
            var hero = new TestHero("Dax");

            string result = new Pillar(PillarType.Inheritance).Use(hero);

            Assert.Contains("Dax", result);
            Assert.Contains("Pillar of Inheritance", result);
        }
    }
}