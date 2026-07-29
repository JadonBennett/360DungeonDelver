namespace DungeonDelver.Source.Interface;

public interface IDungeonCharacter
{
    
    // Properties
    string Name { get; }
    int HitPoints { get; }
    int MaxHitPoints { get; }
    int AttackSpeed { get; }
    double ChanceToHit { get; }
    bool IsAlive { get; }
    
    // Methods
    int Attack();

    void ChangeHealth(int amount);
    
}