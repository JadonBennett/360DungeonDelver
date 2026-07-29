using DungeonDelver.Source.Model;

namespace DungeonDelver.Source.Interface;

public interface ICombatManager
{
    // Properties
    int Turn { get; }
    bool InCombat { get; }
    DungeonCharacter Player { get; }
    DungeonCharacter Monster { get; }
    
    // Methods
    void StartCombat(IDungeonCharacter player, IDungeonCharacter monster);

    void PerformMonsterTurn();

    void PerformPlayerTurn(string action);

    void CheckLife(IDungeonCharacter player, IDungeonCharacter monster);

    void EndCombat();
    
    void ResetTurn();
}