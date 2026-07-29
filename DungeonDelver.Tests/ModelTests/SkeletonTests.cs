using Xunit;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Tests.ModelTests;

public class SkeletonTests
{
    [Fact]
    public void SkeletonConstructorTest_SetsStats()
    {
        
        var skeleton = new TestSkeleton();
        
        Assert.Equal("Skellington", skeleton.Name);
        Assert.Equal(100, skeleton.HitPoints);
        Assert.Equal(100, skeleton.MaxHitPoints);
        Assert.Equal(3, skeleton.AttackSpeed);
        Assert.Equal(0.8, skeleton.ChanceToHit);
        Assert.Equal(30, skeleton.TestMinDamage);
        Assert.Equal(50, skeleton.TestMaxDamage);
        Assert.Equal(0.3, skeleton.TestChanceToHeal);
        Assert.Equal(30, skeleton.TestMinHeal);
        Assert.Equal(50, skeleton.TestMaxHeal);
        Assert.True(skeleton.IsAlive);
    }
    
    private class TestSkeleton : Skeleton
    {
        public int TestMinDamage => MinDamage;
        public int TestMaxDamage => MaxDamage;
        public double TestChanceToHeal => ChanceToHeal;
        public double TestMinHeal => MinHeal;
        public double TestMaxHeal => MaxHeal;
    }
    
}