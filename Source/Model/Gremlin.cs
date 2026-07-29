namespace DungeonDelver.Source.Model;

public class Gremlin : Monster
{
    public Gremlin()
    {
        Name =  "Grot";
        HitPoints = 70;
        MaxHitPoints = 70;

        AttackSpeed = 5;
        ChanceToHit = 0.8;
        MinDamage = 15;
        MaxDamage = 30;

        ChanceToHeal = 0.4;
        MinHeal = 20;
        MaxHeal = 40;
    }
}