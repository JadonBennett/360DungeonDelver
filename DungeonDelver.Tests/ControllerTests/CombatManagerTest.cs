using Xunit;
using DungeonDelver.Source.Controller;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ControllerTests;

public class CombatManagerTest
{

    [Fact]
    public void Constructor_NotInCombatTurn0()
    {
        var manager = new CombatManager();
        
        Assert.False(manager.InCombat);
        Assert.Equal(0, manager.Turn);
    }

    [Fact]
    public void StartCombat_StartsCombat()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        
        Assert.True(manager.InCombat);
    }

    [Fact]
    public void StartCombat_StoresPlayer()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        
        Assert.Equal(player, manager.Player);
    }

    [Fact]
    public void StartCombat_StoresMonster()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        
        Assert.Equal(monster, manager.Monster);
    }
    
    [Fact] 
    public void EndCombat_EndsCombat()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        manager.EndCombat();
        
        Assert.False(manager.InCombat);
    }

    [Fact]
    public void StartCombat_ResetsTurn()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        manager.EndCombat();
        manager.StartCombat(player,monster);
        Assert.Equal(0, manager.Turn);
    }

    [Fact]
    public void CheckLife_0HealthPlayerEndsCombat()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        
        player.ChangeHealth(999);
        manager.CheckLife(player, monster);
        
        Assert.False(manager.InCombat);
    }

    [Fact]
    public void CheckLife_0HealthMonsterEndsCombat()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        
        monster.ChangeHealth(999);
        manager.CheckLife(player, monster);
        
        Assert.False(manager.InCombat);
    }

    [Fact]
    public void CheckLife_NonZeroHealthContinueCombat()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        
        manager.CheckLife(player, monster);
        
        Assert.True(manager.InCombat);
    }

    [Fact]
    public void Turn_TurnIncrementsOneTurn()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        
        manager.PerformMonsterTurn();
        
        Assert.Equal(1, manager.Turn);
    }
    
    [Fact]
    public void Turn_TurnIncrementsTwoTurns()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);

        manager.PerformMonsterTurn();
        manager.PerformMonsterTurn();
        
        Assert.Equal(2, manager.Turn);
    }
    

    [Fact]
    public void PerformMonsterTurn_MonsterAttacks()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        manager.PerformMonsterTurn();
        
        Assert.True(player.HitPoints < player.MaxHitPoints);
    }
    
    [Fact]
    public void PerformPlayerTurn_Attack()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        manager.PerformPlayerTurn("attack");
        
        Assert.True(monster.HitPoints < monster.MaxHitPoints);
    }
    
    [Fact]
    public void PerformPlayerTurn_SpecialAttack()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        player.ChangeHealth(50);
        manager.PerformPlayerTurn("special");
        
        Assert.True(player.HitPoints > 50);
    }
    
    [Fact]
    public void PerformPlayerTurn_Run()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        manager.PerformPlayerTurn("run");
        
        Assert.Equal(1, manager.Turn);
    }
    
    [Fact]
    public void PerformPlayerTurn_UseItem()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.StartCombat(player,monster);
        player.ChangeHealth(50);
        manager.PerformPlayerTurn("use item");
        
        Assert.True(player.HitPoints > 50);
    }
    [Fact]
    public void PerformPlayerTurn_InvalidAction()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();

        manager.StartCombat(player, monster);

        manager.PerformPlayerTurn("banana");

        Assert.Equal(0, manager.Turn);
    }
    
    [Fact]
    public void PerformTurn_OutOfCombat_DoesNothing()
    {
        var manager = new CombatManager();
        var player = new TestPlayer();
        var monster = new TestMonster();
        
        manager.PerformMonsterTurn();
        
        Assert.Equal(0, manager.Turn);
        Assert.Equal(100, player.HitPoints);
        
    }

    
    private class TestMonster : Monster
    {
        public TestMonster()
        {
            Name  = "TestMummy";
            HitPoints = 100;
            MaxHitPoints = 100;
            AttackSpeed = 1;
            ChanceToHit = 1;
            MinDamage = 25;
            MaxDamage = 25;
            ChanceToHeal = 1;
            MinHeal = 5;
            MaxHeal = 5;
        }
    }
    
    private class TestPlayer : DungeonCharacter
    {
        public TestPlayer()
        {
            Name  = "TestDummy";
            HitPoints = 100;
            MaxHitPoints = 100;
            AttackSpeed = 1;
            ChanceToHit = 1;
            MinDamage = 25;
            MaxDamage = 25;
        }
        
        public int Attack()
        {
            return 25;
        }

        public void ChangeHealth(int amount)
        {
            HitPoints -= amount;
            if (HitPoints > MaxHitPoints)
                HitPoints = MaxHitPoints;

            if (HitPoints < 0)
                HitPoints = 0;
        }

        public void SpecialAttack()
        {
            ChangeHealth(-10);
        }
    }

}