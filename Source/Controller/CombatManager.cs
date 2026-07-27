using DungeonDelver.Source.Interface;
using DungeonDelver.Source.Model;

namespace DungeonDelver.Source.Controller;

public class CombatManager : ICombatManager
{

    // Fields
    public int Turn { get; private set; }
    public bool InCombat { get; private set; }
    
    public DungeonCharacter Player { get; private set; }
    public DungeonCharacter Monster { get; private set; }


    // Methods
    public void StartCombat(IDungeonCharacter player, IDungeonCharacter target)
    {
        InCombat = true;
    }

    public void PerformMonsterTurn()
    {
        throw new NotImplementedException();
    }

    
    public void PerformPlayerTurn(string action)
    {
        throw new NotImplementedException();
    }

    public void CheckLife(IDungeonCharacter player, IDungeonCharacter target)
    {
        InCombat = false;
    }

    public void EndCombat()
    {
        InCombat = false;
    }
    

    private void Run()
    {
        var runRoll = Random.Shared.NextDouble();
        if (runRoll < 0.5)
        {
            EndCombat();
        }
    }

    private void Attack()
    {
    }

    public void UseItem()
    {
    }

    public void UseSpecial()
    {
    }

    public void ResetTurn()
    {
        Turn = 0;
    }

}
    