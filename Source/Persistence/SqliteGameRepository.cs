// Project: TCSS 360 Dungeon Adventure
// File: SqliteGameRepository.cs
// Team: Jadon Bennett, Joanna Duran, Nick Humeniuk-Sandberg, Sean Prigge

using System;
using System.Collections.Generic;
using DungeonDelver.Source.Model;
using Microsoft.Data.Sqlite;

namespace DungeonDelver.Source.Persistence
{
	/// <summary>
	/// Implementation of IGameRepository that uses SQLite for persistent storage.
	/// All raw SQL queries live in this class and nowhere else in the codebase.
	/// </summary>
	public class SqliteGameRepository : IGameRepository
	{
		/// <summary>
		/// The path to the SQLite database file.
		/// </summary>
		private readonly string myDatabasePath;

		/// <summary>
		/// Initializes a new SqliteGameRepository with the specified database file path.
		/// </summary>
		/// <param name="theDatabasePath">The path to the SQLite database file.</param>
		public SqliteGameRepository(string theDatabasePath)
		{
			myDatabasePath = theDatabasePath;
		}

		/// <summary>
		/// Returns the monster type names that may spawn in the dungeon
		/// associated with the given pillar.
		/// </summary>
		/// <param name="thePillarType">The pillar granted by the dungeon.</param>
		/// <returns>The pool of monster type names for that dungeon.</returns>
		public IReadOnlyList<string> GetMonsterTypesForDungeon(PillarType thePillarType)
		{
			var monsterList = new List<string>();

			// Open a secure connection stream using your database path property
			using var connection = new SqliteConnection($"Data Source={myDatabasePath}");
			connection.Open();

			// Query all distinct names available inside your seeded stats table
			string sql = "SELECT name FROM monster_stats;";
			
			using var cmd = new SqliteCommand(sql, connection);
			using var reader = cmd.ExecuteReader();
			
			while (reader.Read())
			{
				string monsterName = reader.GetString(0);
				
				// Route monsters to their appropriate pillar dungeons to match your pool rules
				if (thePillarType == PillarType.Abstraction && monsterName == "Gremlin") 
				{
					monsterList.Add(monsterName);
				}
				else if (thePillarType == PillarType.Encapsulation && monsterName == "Skeleton") 
				{
					monsterList.Add(monsterName);
				}
				else if (thePillarType == PillarType.Inheritance && monsterName == "Ogre") 
				{
					monsterList.Add(monsterName);
				}
				else if (thePillarType == PillarType.Polymorphism) 
				{
					monsterList.Add(monsterName); // All monsters can spawn in the Polymorphism dungeon
				}
			}

			return monsterList;
		}

		/// <summary>
		/// Returns all monster type names known to the game.
		/// </summary>
		public IReadOnlyList<string> GetAllMonsterTypes()
		{
			// TODO: SELECT DISTINCT monster_type FROM monsters
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries the persistent SQLite storage to retrieve stats for a specific monster type 
		/// and constructs its corresponding subclass entity.
		/// </summary>
		/// <param name="theMonsterType">The unique primary key name of the monster type (e.g., "Ogre").</param>
		/// <returns>A fully instantiated Monster entity loaded with database parameters.</returns>
		/// <exception cref="ArgumentException">Thrown when an unknown monster type string is passed.</exception>
		/// <exception cref="KeyNotFoundException">Thrown when the requested monster does not exist in the database table.</exception>
		public Monster GetMonsterStats(string theMonsterType)
		{
			// Use the ConnectionString helper we created earlier
			using var connection = new SqliteConnection($"Data Source={myDatabasePath}");
			connection.Open();

			// Query all stats fields using a safe parameterized query
			string sql = "SELECT name, hit_points, attack_speed, chance_to_hit, min_damage, max_damage, chance_to_heal, min_heal, max_heal FROM monster_stats WHERE name = @name;";
	
			using var cmd = new SqliteCommand(sql, connection);
			cmd.Parameters.AddWithValue("@name", theMonsterType);

			using var reader = cmd.ExecuteReader();
			if (reader.Read())
			{
				string name = reader.GetString(0);
				int hp = reader.GetInt32(1);
				int speed = reader.GetInt32(2);
				double chanceToHit = reader.GetDouble(3);
				int minDmg = reader.GetInt32(4);
				int maxDmg = reader.GetInt32(5);
				double chanceToHeal = reader.GetDouble(6);
				int minHeal = reader.GetInt32(7);
				int maxHeal = reader.GetInt32(8);

				// Map the string keys from your DB seed ('Gretchin', 'Shrek', 'Skellington')
				// to the matching class instances
				return theMonsterType switch
				{
					"Gremlin"    => new Gremlin(name, hp, speed, chanceToHit, minDmg, maxDmg, chanceToHeal, minHeal, maxHeal),
					"Ogre"       => new Ogre(name, hp, speed, chanceToHit, minDmg, maxDmg, chanceToHeal, minHeal, maxHeal),
					"Skeleton" => new Skeleton(name, hp, speed, chanceToHit, minDmg, maxDmg, chanceToHeal, minHeal, maxHeal),
					_ => throw new ArgumentException($"Unknown monster type: {theMonsterType}")
				};
			}
			throw new KeyNotFoundException($"Monster '{theMonsterType}' not found in database.");
		}
		
		
		
		// TODO: Implement IGameRepository methods:
		// - Use SQLite connection to query monster/hero stats
		// - Execute INSERT/UPDATE for save game
		// - Execute SELECT for load game
		// - All SQL statements should be parameterized to prevent injection
	}
}
