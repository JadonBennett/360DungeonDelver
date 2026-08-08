/*
 * TCSS 360 Dungeon Adventure
 * GameStatus.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */
namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// The overall outcome state of a play session.
    /// </summary>
    public enum GameStatus
    {
        /// <summary>The game is still being played.</summary>
        InProgress,

        /// <summary>The hero collected all pillars and reached the exit alive.</summary>
        Won,

        /// <summary>The hero died before completing the objective.</summary>
        Lost
    }
}