namespace DungeonDelver.Source.Model;

public abstract class Monster : DungeonCharacter
{
    // Properties
    protected double ChanceToHeal { get; init; }
    protected int MinHeal { get; init; }
    protected int MaxHeal { get; init; }
    
    // Methods
    public virtual void Heal()
    {
        var healRoll = Random.Shared.NextDouble();
        if (healRoll <= ChanceToHeal)
        {
            ChangeHealth(Random.Shared.Next(MinHeal, MaxHeal + 1));
        }
    }
}