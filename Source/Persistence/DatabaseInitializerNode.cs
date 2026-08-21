// Project: TCSS 360 Dungeon Adventure
// File: DatabaseInitializerNode.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using Godot;

namespace DungeonDelver.Source.Persistence
{
    /// <summary>
    /// Godot Node wrapper for DatabaseInitializer to allow GDScript to call initialization.
    /// </summary>
    public partial class DatabaseInitializerNode : Node
    {
        /// <summary>
        /// Initializes the database. Can be called from GDScript.
        /// </summary>
        public void Initialize()
        {
            DatabaseInitializer.EnsureInitialized();
        }
    }
}
