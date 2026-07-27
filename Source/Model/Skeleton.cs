namespace DungeonDelver.Source.Model;

public class Skeleton : Monster
{
    public Skeleton()
    {
        Name =  "Skellington";
        HitPoints = 100;
        MaxHitPoints = 100;

        AttackSpeed = 3;
        ChanceToHit = 0.8;
        MinDamage = 30;
        MaxDamage = 50;

        ChanceToHeal = 0.3;
        MinHeal = 30;
        MaxHeal = 50;
    }
}