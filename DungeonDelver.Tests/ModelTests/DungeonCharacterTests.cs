using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests;

public class DungeonCharacterTests
{
    [Fact]
    public void TestDungeonCharacter_AttackHit()
    {
        var character =  new HitCharacterMaxHealth();

        var damage = character.Attack();

        Assert.Equal(10, damage);
    }

    [Fact]
    public void TestDungeonCharacter_AttackMiss()
    {
        
        var character =  new MissCharacterLowHealth();

        var damage = character.Attack();

        Assert.Equal(0, damage);
    }

    [Fact]
    public void ChangeHealth_IncreaseHealth()
    {
        
        var character =  new MissCharacterLowHealth();
        
        character.ChangeHealth(-10);

        Assert.Equal(20, character.HitPoints);

    }

    [Fact]
    public void ChangeHealth_DecreaseHealth()
    {
        var character =  new HitCharacterMaxHealth();
        
        character.ChangeHealth(10);

        Assert.Equal(90,  character.HitPoints);
        
    }

    [Fact]
    public void ChangeHealth_IncreaseHealthNotOverMax()
    {
        var character =  new HitCharacterMaxHealth();
        
        
        character.ChangeHealth(-100);

        Assert.Equal( character.MaxHitPoints, character.HitPoints);
        
    }

    [Fact]
    public void ChangeHealth_DecreaseHealthNotUnderMin()
    {
        var character =  new MissCharacterLowHealth();
        
        character.ChangeHealth(100);

        Assert.Equal(0, character.HitPoints);
    }

    [Fact]
    public void PositiveHealth_IsAlive()
    {
        var character =  new HitCharacterMaxHealth();
        Assert.True(character.IsAlive);
    }

    [Fact]
    public void ZeroHealthIsAliveFalse()
    {
        var character =  new DeadCharacter();
        
        Assert.False(character.IsAlive);
    }

    private class HitCharacterMaxHealth : DungeonCharacter
    {
        public HitCharacterMaxHealth()
        {
            Name = "Hit";
            HitPoints = 100;
            MaxHitPoints = 100;
            ChanceToHit = 1;
            MaxDamage = 10;
            MinDamage = 10;
        }
    }
    
    private class MissCharacterLowHealth : DungeonCharacter
    {
        public MissCharacterLowHealth()
        {
            Name = "Miss";
            HitPoints = 10;
            MaxHitPoints = 100;
            ChanceToHit = 0;
            MaxDamage = 10;
            MinDamage = 10;
        }
    }

    private class DeadCharacter : DungeonCharacter
    {
        public DeadCharacter()
        {
            Name = "Dead";
            HitPoints = 0;
            MaxHitPoints = 100;
            ChanceToHit = 0;
            MaxDamage = 10;
            MinDamage = 10;
        }
    }

}