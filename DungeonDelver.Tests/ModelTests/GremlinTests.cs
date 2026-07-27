using System.Diagnostics;
using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests;

public class GremlinTests
{
    [Fact]
    public void GremlinConstructorTest_SetsStats()
    {
        
        var gremlin = new TestGremlin();
        
        Assert.Equal("Grot", gremlin.Name);
        Assert.Equal(70, gremlin.HitPoints);
        Assert.Equal(70, gremlin.MaxHitPoints);
        Assert.Equal(5, gremlin.AttackSpeed);
        Assert.Equal(0.8, gremlin.ChanceToHit);
        Assert.Equal(15, gremlin.TestMinDamage);
        Assert.Equal(30, gremlin.TestMaxDamage);
        Assert.Equal(0.4, gremlin.TestChanceToHeal);
        Assert.Equal(20, gremlin.TestMinHeal);
        Assert.Equal(40, gremlin.TestMaxHeal);
        Assert.True(gremlin.IsAlive);
    }
    
    private class TestGremlin : Gremlin
    {
        public int TestMinDamage => MinDamage;
        public int TestMaxDamage => MaxDamage;
        public double TestChanceToHeal => ChanceToHeal;
        public double TestMinHeal => MinHeal;
        public double TestMaxHeal => MaxHeal;
    }
}