using DungeonDelver.Source.Interface;

namespace DungeonDelver.Source.Model;

public abstract class DungeonCharacter : IDungeonCharacter
{
    // Properties
    public string Name { get; protected init; }
    public int HitPoints { get; protected set; }
    public int MaxHitPoints { get; protected init; }
    public int AttackSpeed { get; protected init; }
    public double ChanceToHit { get; protected init; }
    protected int MinDamage { get; init; }
    protected int MaxDamage { get; init; }
    public bool IsAlive => HitPoints >= 0;
    
    
    // Methods
    public virtual int Attack()
    {
        var attackRoll = Random.Shared.NextDouble();
        int dealt;
        if (attackRoll < ChanceToHit)
        {
            dealt = Random.Shared.Next(MinDamage, MaxDamage + 1);
        }
        else
        {
            dealt = 0;
        }

        return dealt;
        
    }

    public virtual void ChangeHealth(int amount)
    {
        HitPoints -= amount;

        if (HitPoints > MaxHitPoints)
        {
            HitPoints = MaxHitPoints;
        }

        if (HitPoints < 0)
        {
            HitPoints = 0;
        }
    }

}