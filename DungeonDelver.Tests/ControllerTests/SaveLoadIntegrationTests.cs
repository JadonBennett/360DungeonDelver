// Project: TCSS 360 Dungeon Adventure
// File: SaveLoadIntegrationTests.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using System.IO;
using Xunit;
using DungeonDelver.Source.Controller;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ControllerTests
{
    /// <summary>
    /// Integration tests for Save/Load game functionality.
    /// Tests full round-trip serialization and deserialization of game state.
    /// </summary>
    public class SaveLoadIntegrationTests : IDisposable
    {
        private readonly string testSaveDir;

        public SaveLoadIntegrationTests()
        {
            // Create unique test save directory
            testSaveDir = Path.Combine(Path.GetTempPath(), $"test_saves_{Guid.NewGuid()}");
            Directory.CreateDirectory(testSaveDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(testSaveDir))
            {
                Directory.Delete(testSaveDir, true);
            }
        }

        /// <summary>
        /// Verifies that SaveGame creates a save file.
        /// </summary>
        [Fact]
        public void SaveGame_CreatesFile()
        {
            var controller = new DungeonController();
            controller.CreateNewGame("TestHero", "Warrior", 5, 5);

            string result = controller.SaveGame("TestSave");

            Assert.Equal("Game saved successfully!", result);
        }

        /// <summary>
        /// Verifies that LoadGame restores hero name correctly.
        /// </summary>
        [Fact]
        public void LoadGame_RestoresHeroName()
        {
            var controller1 = new DungeonController();
            controller1.CreateNewGame("Gandalf", "Priestess", 5, 5);
            controller1.SaveGame("TestSave");

            var controller2 = new DungeonController();
            string loadResult = controller2.LoadGame(1);

            Assert.Equal("Success", loadResult);

            var heroInfo = controller2.GetHeroInfo();
            Assert.Equal("Gandalf", heroInfo["name"]);
        }

        /// <summary>
        /// Verifies that LoadGame restores hero class correctly.
        /// </summary>
        [Fact]
        public void LoadGame_RestoresHeroClass()
        {
            var controller1 = new DungeonController();
            controller1.CreateNewGame("Legolas", "Thief", 5, 5);
            controller1.SaveGame("TestSave");

            var controller2 = new DungeonController();
            controller2.LoadGame(1);

            var heroInfo = controller2.GetDetailedHeroStats();
            Assert.Equal("Thief", heroInfo["hero_class"]);
        }

        /// <summary>
        /// Verifies that LoadGame restores hero hit points after taking damage.
        /// </summary>
        [Fact]
        public void LoadGame_RestoresModifiedHitPoints()
        {
            var controller1 = new DungeonController();
            controller1.CreateNewGame("Aragorn", "Warrior", 5, 5);

            // Take some damage
            controller1.DebugDamageHero(50);
            var hpBefore = controller1.GetHeroInfo()["hp"];
            controller1.SaveGame("TestSave");

            var controller2 = new DungeonController();
            controller2.LoadGame(1);

            var heroInfo = controller2.GetHeroInfo();
            Assert.Equal(hpBefore, heroInfo["hp"]);
        }

        /// <summary>
        /// Verifies that LoadGame restores collected pillars.
        /// </summary>
        [Fact]
        public void LoadGame_RestoresPillars()
        {
            var controller1 = new DungeonController();
            controller1.CreateNewGame("Frodo", "Warrior", 5, 5);

            // Collect a pillar (using debug method if available)
            controller1.DebugCollectPillar(PillarType.Abstraction);
            controller1.SaveGame("TestSave");

            var controller2 = new DungeonController();
            controller2.LoadGame(1);

            var heroInfo = controller2.GetDetailedHeroStats();
            Assert.Equal(1, heroInfo["pillars_collected"]);
        }

        /// <summary>
        /// Verifies that LoadGame restores inventory items.
        /// </summary>
        [Fact]
        public void LoadGame_RestoresInventory()
        {
            var controller1 = new DungeonController();
            controller1.CreateNewGame("Samwise", "Warrior", 5, 5);

            // Add items to inventory
            controller1.DebugGiveItem("HealingPotion");
            controller1.DebugGiveItem("VisionPotion");
            controller1.SaveGame("TestSave");

            var controller2 = new DungeonController();
            controller2.LoadGame(1);

            var inventory = controller2.GetInventory();
            var items = (Godot.Collections.Array)inventory["items"];
            Assert.Equal(2, items.Count);
        }

        /// <summary>
        /// Verifies that LoadGame restores hero position in dungeon.
        /// </summary>
        [Fact]
        public void LoadGame_RestoresHeroPosition()
        {
            var controller1 = new DungeonController();
            controller1.CreateNewGame("Merry", "Thief", 5, 5);

            // Move to a different position
            var roomBefore = controller1.GetCurrentRoomInfo();
            controller1.MovePlayer("north");
            controller1.MovePlayer("east");
            controller1.SaveGame("TestSave");

            var roomAfterMove = controller1.GetCurrentRoomInfo();

            var controller2 = new DungeonController();
            controller2.LoadGame(1);

            var roomAfterLoad = controller2.GetCurrentRoomInfo();
            Assert.Equal(roomAfterMove["x"], roomAfterLoad["x"]);
            Assert.Equal(roomAfterMove["y"], roomAfterLoad["y"]);
        }

        /// <summary>
        /// Verifies that LoadGame restores dungeon dimensions.
        /// </summary>
        [Fact]
        public void LoadGame_RestoresDungeonSize()
        {
            var controller1 = new DungeonController();
            controller1.CreateNewGame("Pippin", "Warrior", 8, 6);
            controller1.SaveGame("TestSave");

            var controller2 = new DungeonController();
            controller2.LoadGame(1);

            var minimapData = controller2.GetMinimapData();
            Assert.Equal(8, minimapData["width"]);
            Assert.Equal(6, minimapData["height"]);
        }

        /// <summary>
        /// Verifies that LoadGame returns error for non-existent save.
        /// </summary>
        [Fact]
        public void LoadGame_NonExistentSave_ReturnsError()
        {
            var controller = new DungeonController();
            string result = controller.LoadGame(999);

            Assert.Equal("Save file not found", result);
        }

        /// <summary>
        /// Verifies that multiple saves can be created and loaded independently.
        /// </summary>
        [Fact]
        public void SaveLoad_MultipleSaves_WorkIndependently()
        {
            var controller1 = new DungeonController();
            controller1.CreateNewGame("Hero1", "Warrior", 5, 5);
            controller1.SaveGame("Save1");

            var controller2 = new DungeonController();
            controller2.CreateNewGame("Hero2", "Priestess", 6, 6);
            controller2.SaveGame("Save2");

            var controller3 = new DungeonController();
            controller3.LoadGame(1);
            var hero1Info = controller3.GetHeroInfo();

            var controller4 = new DungeonController();
            controller4.LoadGame(2);
            var hero2Info = controller4.GetHeroInfo();

            Assert.Equal("Hero1", hero1Info["name"]);
            Assert.Equal("Hero2", hero2Info["name"]);
        }
    }
}
