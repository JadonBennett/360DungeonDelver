// Project: TCSS 360 Dungeon Adventure
// File: RoomContentGeneratorTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using DungeonDelver.Dungeon;
using DungeonDelver.Source.Model;
using Xunit;

namespace DungeonDelver.Tests
{
    /// <summary>
    /// Test suite for the RoomContentGenerator class, verifying that entrance
    /// and exit rooms stay empty and that content rolls behave as expected
    /// over a large sample of rooms.
    /// </summary>
    public class RoomContentGeneratorTests
    {
        /// <summary>
        /// Verifies that the entrance and exit rooms never receive a pit,
        /// monster, or item, regardless of roll outcomes.
        /// </summary>
        [Fact]
        public void Populate_NeverAddsContentToEntranceOrExit()
        {
            DungeonMap dungeon = new DungeonMap(10, 10);
            RoomContentGenerator generator = new RoomContentGenerator(() => new Gremlin());

            generator.Populate(dungeon);

            Assert.Null(dungeon.Entrance.Pit);
            Assert.Empty(dungeon.Entrance.Monsters);
            Assert.Null(dungeon.Entrance.Item);

            Assert.Null(dungeon.Exit.Pit);
            Assert.Empty(dungeon.Exit.Monsters);
            Assert.Null(dungeon.Exit.Item);
        }

        /// <summary>
        /// Verifies that, over a large dungeon, some normal rooms receive
        /// pits, some receive monsters, and some receive items -- confirming
        /// the independent probability rolls are actually wired up.
        /// </summary>
        [Fact]
        public void Populate_RollsProduceAllContentTypesOverLargeDungeon()
        {
            DungeonMap dungeon = new DungeonMap(20, 20);
            RoomContentGenerator generator = new RoomContentGenerator(() => new Gremlin());

            generator.Populate(dungeon);

            int pitCount = 0;
            int monsterCount = 0;
            int itemCount = 0;

            for (int x = 0; x < dungeon.Width; x++)
            {
                for (int y = 0; y < dungeon.Height; y++)
                {
                    Room room = dungeon.GetRoom(x, y);
                    if (room.Pit != null) pitCount++;
                    if (room.Monsters.Count > 0) monsterCount++;
                    if (room.Item != null) itemCount++;
                }
            }

            Assert.True(pitCount > 0, "Expected at least one pit across 400 rooms.");
            Assert.True(monsterCount > 0, "Expected at least one monster across 400 rooms.");
            Assert.True(itemCount > 0, "Expected at least one item across 400 rooms.");
        }

        /// <summary>
        /// Verifies that an item roll never overwrites a room's existing item
        /// (e.g. a pillar placed by MazeGenerator before content generation runs).
        /// </summary>
        [Fact]
        public void Populate_DoesNotOverwriteExistingItem()
        {
            DungeonMap dungeon = new DungeonMap(5, 5);
            Pillar existingPillar = new Pillar(PillarType.Abstraction);
            dungeon.GetRoom(2, 2).Item = existingPillar;

            RoomContentGenerator generator = new RoomContentGenerator(() => new Gremlin());
            generator.Populate(dungeon);

            Assert.Same(existingPillar, dungeon.GetRoom(2, 2).Item);
        }
    }
}
