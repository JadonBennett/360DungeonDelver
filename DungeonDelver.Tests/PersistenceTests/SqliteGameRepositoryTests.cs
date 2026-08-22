// Project: TCSS 360 Dungeon Adventure
// File: SqliteGameRepositoryTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using System.IO;
using System.Linq;
using Xunit;
using DungeonDelver.Source.Persistence;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.PersistenceTests
{
    /// <summary>
    /// Test suite for the SqliteGameRepository class.
    /// Tests database queries for monster data retrieval and pillar-based filtering.
    /// </summary>
    public class SqliteGameRepositoryTests : IDisposable
    {
        private readonly string testDbPath;
        private readonly DatabaseManager dbManager;
        private readonly SqliteGameRepository repository;

        /// <summary>
        /// Set up a test database for each test.
        /// </summary>
        public SqliteGameRepositoryTests()
        {
            // Create a unique test database file
            testDbPath = Path.Combine(Path.GetTempPath(), $"test_dungeon_{Guid.NewGuid()}.db");

            // Initialize database with test data
            dbManager = new DatabaseManager(testDbPath);
            dbManager.InitializeDatabase();

            // Create repository instance
            repository = new SqliteGameRepository(testDbPath);
        }

        /// <summary>
        /// Clean up test database after each test.
        /// </summary>
        public void Dispose()
        {
            if (File.Exists(testDbPath))
            {
                File.Delete(testDbPath);
            }
        }

        /// <summary>
        /// Verifies that GetMonsterStats retrieves Ogre with correct stats from database.
        /// </summary>
        [Fact]
        public void GetMonsterStats_Ogre_ReturnsCorrectStats()
        {
            var ogre = repository.GetMonsterStats("Ogre");

            Assert.NotNull(ogre);
            Assert.Equal("Ogre", ogre.Name);
            Assert.Equal(200, ogre.HitPoints);
            Assert.Equal(200, ogre.MaxHitPoints);
            Assert.Equal(2, ogre.AttackSpeed);
            Assert.Equal(0.6, ogre.ChanceToHit, 3);
            Assert.True(ogre.IsAlive);
        }

        /// <summary>
        /// Verifies that GetMonsterStats retrieves Gremlin with correct stats from database.
        /// </summary>
        [Fact]
        public void GetMonsterStats_Gremlin_ReturnsCorrectStats()
        {
            var gremlin = repository.GetMonsterStats("Gremlin");

            Assert.NotNull(gremlin);
            Assert.Equal("Gremlin", gremlin.Name);
            Assert.Equal(70, gremlin.HitPoints);
            Assert.Equal(70, gremlin.MaxHitPoints);
            Assert.Equal(5, gremlin.AttackSpeed);
            Assert.Equal(0.8, gremlin.ChanceToHit, 3);
            Assert.True(gremlin.IsAlive);
        }

        /// <summary>
        /// Verifies that GetMonsterStats retrieves Skeleton with correct stats from database.
        /// </summary>
        [Fact]
        public void GetMonsterStats_Skeleton_ReturnsCorrectStats()
        {
            var skeleton = repository.GetMonsterStats("Skeleton");

            Assert.NotNull(skeleton);
            Assert.Equal("Skeleton", skeleton.Name);
            Assert.Equal(100, skeleton.HitPoints);
            Assert.Equal(100, skeleton.MaxHitPoints);
            Assert.Equal(3, skeleton.AttackSpeed);
            Assert.Equal(0.8, skeleton.ChanceToHit, 3);
            Assert.True(skeleton.IsAlive);
        }

        /// <summary>
        /// Verifies that GetMonsterStats throws KeyNotFoundException for unknown monster.
        /// </summary>
        [Fact]
        public void GetMonsterStats_UnknownMonster_ThrowsKeyNotFoundException()
        {
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() =>
            {
                repository.GetMonsterStats("Dragon");
            });
        }

        /// <summary>
        /// Verifies that GetMonsterTypesForDungeon returns only Gremlin for Abstraction pillar.
        /// </summary>
        [Fact]
        public void GetMonsterTypesForDungeon_Abstraction_ReturnsOnlyGremlin()
        {
            var monsters = repository.GetMonsterTypesForDungeon(PillarType.Abstraction);

            Assert.Single(monsters);
            Assert.Contains("Gremlin", monsters);
        }

        /// <summary>
        /// Verifies that GetMonsterTypesForDungeon returns only Skeleton for Encapsulation pillar.
        /// </summary>
        [Fact]
        public void GetMonsterTypesForDungeon_Encapsulation_ReturnsOnlySkeleton()
        {
            var monsters = repository.GetMonsterTypesForDungeon(PillarType.Encapsulation);

            Assert.Single(monsters);
            Assert.Contains("Skeleton", monsters);
        }

        /// <summary>
        /// Verifies that GetMonsterTypesForDungeon returns only Ogre for Inheritance pillar.
        /// </summary>
        [Fact]
        public void GetMonsterTypesForDungeon_Inheritance_ReturnsOnlyOgre()
        {
            var monsters = repository.GetMonsterTypesForDungeon(PillarType.Inheritance);

            Assert.Single(monsters);
            Assert.Contains("Ogre", monsters);
        }

        /// <summary>
        /// Verifies that GetMonsterTypesForDungeon returns all monsters for Polymorphism pillar.
        /// </summary>
        [Fact]
        public void GetMonsterTypesForDungeon_Polymorphism_ReturnsAllMonsters()
        {
            var monsters = repository.GetMonsterTypesForDungeon(PillarType.Polymorphism);

            Assert.Equal(3, monsters.Count);
            Assert.Contains("Ogre", monsters);
            Assert.Contains("Gremlin", monsters);
            Assert.Contains("Skeleton", monsters);
        }

        /// <summary>
        /// Verifies that retrieved monsters have valid damage ranges.
        /// </summary>
        [Theory]
        [InlineData("Ogre", 30, 60)]
        [InlineData("Gremlin", 15, 30)]
        [InlineData("Skeleton", 30, 50)]
        public void GetMonsterStats_HasCorrectDamageRange(string monsterType, int expectedMin, int expectedMax)
        {
            var monster = repository.GetMonsterStats(monsterType);

            // Test actual attack damage is within range over multiple attempts
            bool foundMin = false;
            bool foundMax = false;

            for (int i = 0; i < 1000; i++)
            {
                int damage = monster.Attack();
                if (damage >= expectedMin && damage <= expectedMax)
                {
                    if (damage == expectedMin || damage == 0) foundMin = true;
                    if (damage == expectedMax) foundMax = true;
                }
                else if (damage != 0)
                {
                    Assert.Fail($"Damage {damage} outside expected range [{expectedMin}, {expectedMax}]");
                }
            }

            Assert.True(foundMin || foundMax, "Should produce damage values in expected range");
        }

        /// <summary>
        /// Verifies that monsters retrieved from database can heal.
        /// </summary>
        [Fact]
        public void GetMonsterStats_Gremlin_CanHeal()
        {
            var gremlin = repository.GetMonsterStats("Gremlin");

            // Damage the gremlin
            gremlin.ChangeHealth(-30);
            int damagedHP = gremlin.HitPoints;

            Assert.Equal(40, damagedHP);

            // Try healing multiple times (40% chance)
            bool healed = false;
            for (int i = 0; i < 50; i++)
            {
                int hpBefore = gremlin.HitPoints;
                gremlin.TryHeal();
                if (gremlin.HitPoints > hpBefore)
                {
                    healed = true;
                    break;
                }
            }

            Assert.True(healed, "Gremlin should heal at least once in 50 attempts with 40% chance");
        }

        /// <summary>
        /// Verifies that the database connection can be opened multiple times.
        /// </summary>
        [Fact]
        public void Repository_CanQueryMultipleTimes()
        {
            var monster1 = repository.GetMonsterStats("Ogre");
            var monster2 = repository.GetMonsterStats("Gremlin");
            var monster3 = repository.GetMonsterStats("Skeleton");

            Assert.NotNull(monster1);
            Assert.NotNull(monster2);
            Assert.NotNull(monster3);
            Assert.NotEqual(monster1.Name, monster2.Name);
        }
    }
}
