// Project: TCSS 360 Dungeon Adventure
// File: MonsterFactoryTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using Xunit;
using DungeonDelver.Source.Controller;
using DungeonDelver.Source.Model;
using DungeonDelver.Source.Persistence;

namespace DungeonDelver.Tests.ControllerTests
{
    /// <summary>
    /// Test suite for the MonsterFactory class.
    /// </summary>
    public class MonsterFactoryTests
    {
        private static MonsterFactory CreateFactory()
        {
            return new MonsterFactory(new SqliteGameRepository(":memory:"));
        }

        [Theory]
        [InlineData("Ogre", typeof(Ogre))]
        [InlineData("Skeleton", typeof(Skeleton))]
        [InlineData("Gremlin", typeof(Gremlin))]
        public void CreateMonster_ReturnsExpectedType(string theMonsterType, Type theExpectedType)
        {
            MonsterFactory factory = CreateFactory();

            Monster monster = factory.CreateMonster(theMonsterType);

            Assert.IsType(theExpectedType, monster);
        }

        [Fact]
        public void CreateMonster_UnknownType_Throws()
        {
            MonsterFactory factory = CreateFactory();

            Assert.Throws<ArgumentException>(() => factory.CreateMonster("Dragon"));
        }

        [Fact]
        public void CreateRandomMonster_AlwaysReturnsKnownType()
        {
            MonsterFactory factory = CreateFactory();

            for (int i = 0; i < 100; i++)
            {
                Monster monster = factory.CreateRandomMonster();
                Assert.True(monster is Ogre or Skeleton or Gremlin);
            }
        }
    }
}
