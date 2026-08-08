/*
 * TCSS 360 Dungeon Adventure
 * GameStateEvaluator.cs
 * Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge
 */
using System;

namespace DungeonDelver.Source.Model
{
    /// <summary>
    /// Evaluates win/loss/in-progress state from the hero's condition and
    /// whether the hero is currently standing on the dungeon exit.
    ///
    /// The exit check is intentionally passed in as a bool so this class does
    /// not depend on the (not-yet-implemented) dungeon/map API. Once Joanna's
    /// map exists, the game loop should call this with dungeon.HeroIsAtExit.
    /// </summary>
    public static class GameStateEvaluator
    {
        /// <summary>
        /// Determines the current game status.
        /// Loss takes priority over win (a dead hero cannot win, even on the exit).
        /// </summary>
        /// <param name="theHero">The player's hero. Must not be null.</param>
        /// <param name="theHeroIsAtExit">True if the hero is on the exit tile.</param>
        /// <returns>The resulting <see cref="GameStatus"/>.</returns>
        public static GameStatus Evaluate(Hero theHero, bool theHeroIsAtExit)
        {
            ArgumentNullException.ThrowIfNull(theHero);

            if (!theHero.IsAlive)
            {
                return GameStatus.Lost;
            }

            if (theHero.HasAllPillars() && theHeroIsAtExit)
            {
                return GameStatus.Won;
            }

            return GameStatus.InProgress;
        }
    }
}