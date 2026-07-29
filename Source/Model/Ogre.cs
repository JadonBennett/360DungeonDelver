namespace DungeonDelver.Source.Model;

public class Ogre : Monster
{
    public Ogre()
    {
        Name =  "Shrek";
        HitPoints = 200;
        MaxHitPoints = 200;

        AttackSpeed = 2;
        ChanceToHit = 0.6;
        MinDamage = 30;
        MaxDamage = 60;

        ChanceToHeal = 0.1;
        MinHeal = 30;
        MaxHeal = 60;
    }
}