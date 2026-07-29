using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests;

public class MonsterTests
{
    [Fact]
    public void TestMonsterHeal()
    {
        var monster = new HealMonster();

        monster.Heal();
        Assert.Equal(60, monster.HitPoints);
    }
    
    [Fact]
    public void TestMonsterHeal_NeverHealOverMax()
    {
        var monster = new HealMonster();

        monster.ChangeHealth(-45);
        monster.Heal();
        Assert.Equal(100, monster.HitPoints);
    }
    
    [Fact]
    public void TestMonsterNeverHeal()
    {
        var monster = new NoHealMonster();
        
        monster.Heal();
        Assert.Equal(50, monster.HitPoints);
    }

    private class HealMonster : Monster
    {
        public HealMonster()
        {
            Name = "HealMonster";
            HitPoints = 50;
            MaxHitPoints = 100;
            ChanceToHeal = 1;
            MinHeal = 10;
            MaxHeal = 10;
        }
    }
    
    private class NoHealMonster : Monster
    {
        public NoHealMonster()
        {
            Name = "NoHealMonster";
            HitPoints = 50;
            MaxHitPoints = 100;
            ChanceToHeal = 0;
            MinHeal = 10;
            MaxHeal = 10;
        }
    }
    
    
}