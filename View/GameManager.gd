extends Node

## Global game state manager that wraps the C# DungeonController.
## This singleton provides access to game state from any scene.

# MVC Signals - emitted when game state changes
signal game_created
signal hp_changed
signal pillars_changed
signal room_changed
signal game_state_changed  # General state change signal

var controller = null

# Debug settings
var show_minimap_contents := false  # Toggle for showing monsters/items on minimap

func _ready():
	# Initialize database once at startup
	@warning_ignore("unused_variable")
	var DatabaseInitializer = load("res://Source/Persistence/DatabaseInitializer.cs")


	_initialize_controller()

	if not get_current_room_info().has("x"):
		create_new_game("DebugHero", "Warrior", 8, 8)

## Creates a new game with the specified hero and dungeon parameters.
## Generates a dungeon containing all 4 Pillars of OOP.
func create_new_game(hero_name: String, hero_class: String, width: int = 8, height: int = 8):
	if controller == null:
		_initialize_controller()
	controller.CreateNewGame(hero_name, hero_class, width, height)
	game_created.emit()

## Gets information about the current room.
func get_current_room_info() -> Dictionary:
	if controller == null:
		return {}
	return controller.GetCurrentRoomInfo()

## Gets the hero's current stats.
func get_hero_info() -> Dictionary:
	if controller == null:
		return {}
	return controller.GetHeroInfo()

## Attempts to move the player in the specified direction.
func move_player(direction: String) -> bool:
	if controller == null:
		return false
	var success = controller.MovePlayer(direction)
	if success:
		room_changed.emit()
		game_state_changed.emit()
	return success

## Checks if the win condition has been met.
func check_win_condition() -> bool:
	if controller == null:
		return false
	return controller.CheckWinCondition()

## Checks if the hero has died.
func is_hero_dead() -> bool:
	if controller == null:
		return false
	return controller.IsHeroDead()

## Gets a debug string showing the dungeon map.
func get_map_debug_string() -> String:
	if controller == null:
		return "No controller"
	return controller.GetMapDebugString()

## Gets minimap data for the current dungeon.
func get_minimap_data() -> Dictionary:
	if controller == null:
		return {}
	return controller.GetMinimapData()
	
	
## Gets detailed hero statistics.
func get_detailed_hero_stats() -> Dictionary:
	if controller == null:
		return {}
	return controller.GetDetailedHeroStats()

## Gets the contents of the current room.
func get_room_contents() -> Dictionary:
	if controller == null:
		return {}
	return controller.GetRoomContents()

## Gets the hero's inventory.
func get_inventory() -> Dictionary:
	if controller == null:
		return {}
	return controller.GetInventory()

## Gets the current combat state.
func get_combat_state() -> Dictionary:
	if controller == null:
		return {}
	return controller.GetCombatState()
	
	
## Checks whether the hero is currently in combat.
func is_in_combat() -> bool:
	if controller == null:
		return false
	var combat_state = get_combat_state()
	return combat_state.get("in_combat", false)

## Performs a combat action (player turn + monster turn).
func combat_action(action: String, item_index: int = -1):
	if controller == null:
		return
	controller.PerformCombatAction(action, item_index)
	hp_changed.emit()
	game_state_changed.emit()

## Uses an item from inventory outside of combat.
func use_inventory_item(item_index: int) -> String:
	if controller == null:
		return "No controller"
	var result = controller.UseInventoryItem(item_index)
	hp_changed.emit()
	game_state_changed.emit()
	return result
# ========== DEBUG METHODS ==========

## DEBUG: Sets hero HP to specific value.
func debug_set_hero_hp(hp: int):
	if controller == null:
		return
	controller.DebugSetHeroHP(hp)
	hp_changed.emit()
	game_state_changed.emit()

## DEBUG: Damages the hero.
func debug_damage_hero(damage: int):
	if controller == null:
		return
	controller.DebugDamageHero(damage)
	hp_changed.emit()
	game_state_changed.emit()

## DEBUG: Heals the hero.
func debug_heal_hero(amount: int):
	if controller == null:
		return
	controller.DebugHealHero(amount)
	hp_changed.emit()
	game_state_changed.emit()

## DEBUG: Sets pillar count.
func debug_set_pillars(count: int):
	if controller == null:
		return
	controller.DebugSetPillars(count)
	pillars_changed.emit()
	game_state_changed.emit()

## DEBUG: Teleports to exit.
func debug_teleport_to_exit():
	if controller == null:
		return
	controller.DebugTeleportToExit()
	room_changed.emit()
	game_state_changed.emit()

## DEBUG: Teleports to entrance.
func debug_teleport_to_entrance():
	if controller == null:
		return
	controller.DebugTeleportToEntrance()
	room_changed.emit()
	game_state_changed.emit()

## DEBUG: Gives an item to the hero.
func debug_give_item(item_type: String):
	if controller == null:
		return
	controller.DebugGiveItem(item_type)
	game_state_changed.emit()

# ========== SAVE/LOAD METHODS ==========

## Saves the current game state
func save_game(save_name: String) -> String:
	if controller == null:
		return "No active game to save"

	var result = controller.SaveGame(save_name)
	return result

## Overwrites an existing save
func overwrite_save(save_id: int, save_name: String) -> String:
	if controller == null:
		return "No active game to save"

	var result = controller.OverwriteSave(save_id, save_name)
	return result

## Loads a saved game
func load_game(save_id: int) -> String:
	if controller == null:
		_initialize_controller()

	var result = controller.LoadGame(save_id)
	if result == "Success":
		game_created.emit()
		game_state_changed.emit()
	return result

## Gets all saved games
func get_all_saves() -> Array:
	if controller == null:
		_initialize_controller()

	var saves = controller.GetAllSavedGames()
	return saves if saves != null else []

## Deletes a saved game
func delete_save(save_id: int):
	if controller == null:
		return
	controller.DeleteSavedGame(save_id)

## Initializes the C# controller.
func _initialize_controller():
	# Create instance of the C# DungeonController class
	var DungeonControllerClass = load("res://Source/Controller/DungeonController.cs")
	controller = DungeonControllerClass.new()
