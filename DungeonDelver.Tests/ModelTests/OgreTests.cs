using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests;

public class OgreTests
{
    [Fact]
    public void OgreConstructorTest_SetsStats()
    {
        
        var ogre = new TestOgre();
        
        Assert.Equal("Shrek", ogre.Name);
        Assert.Equal(200, ogre.HitPoints);
        Assert.Equal(200, ogre.MaxHitPoints);
        Assert.Equal(2, ogre.AttackSpeed);
        Assert.Equal(0.6, ogre.ChanceToHit);
        Assert.Equal(30, ogre.TestMinDamage);
        Assert.Equal(60, ogre.TestMaxDamage);
        Assert.Equal(0.1, ogre.TestChanceToHeal);
        Assert.Equal(30, ogre.TestMinHeal);
        Assert.Equal(60, ogre.TestMaxHeal);
        Assert.True(ogre.IsAlive);
    }
    
    private class TestOgre : Ogre
    {
        public int TestMinDamage => MinDamage;
        public int TestMaxDamage => MaxDamage;
        public double TestChanceToHeal => ChanceToHeal;
        public double TestMinHeal => MinHeal;
        public double TestMaxHeal => MaxHeal;
    }
    
}