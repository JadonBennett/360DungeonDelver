/*
 * TCSS 360 Dungeon Adventure
 * TestDoubles.cs - concrete stubs for abstract classes under test.
 */
using DungeonDelver.Source.Interface;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests
{
    /// <summary>Concrete Hero used to test the abstract Hero base behavior.</summary>
    internal sealed class TestHero : Hero
    {
        public TestHero(string theName = "Tester",
            int theHitPoints = 100,
            int theAttackSpeed = 5,
            double theChanceToHit = 1.0,
            int theMinDamage = 10,
            int theMaxDamage = 10,
            double theBlockChance = 0.0)
            : base(theName, theHitPoints, theAttackSpeed, theChanceToHit,
                theMinDamage, theMaxDamage, theBlockChance)
        {
        }

        public override string SpecialSkillName => "Test Skill";
        public override string UseSpecialSkill(IDungeonCharacter? theTarget = null) => $"{Name} uses a test skill.";
    }

    /// <summary>Concrete Item used to test the abstract Item base behavior.</summary>
    internal sealed class TestItem : Item
    {
        public TestItem(string theName = "Test Item") : base(theName) { }
        public override string Use(Hero theHero) => $"{theHero.Name} used {Name}.";
    }
}