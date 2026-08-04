extends Node

## Global game state manager that wraps the C# DungeonController.
## This singleton provides access to game state from any scene.

var controller = null

func _ready():
	_initialize_controller()

## Creates a new game with the specified hero and dungeon parameters.
func create_new_game(hero_name: String, hero_class: String, width: int = 5, height: int = 5):
	if controller == null:
		_initialize_controller()
	controller.CreateNewGame(hero_name, hero_class, width, height)

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
	return controller.MovePlayer(direction)

## Checks if the win condition has been met.
func check_win_condition() -> bool:
	if controller == null:
		return false
	return controller.CheckWinCondition()

## Gets a debug string showing the dungeon map.
func get_map_debug_string() -> String:
	if controller == null:
		return "No controller"
	return controller.GetMapDebugString()

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

## Initializes the C# controller.
func _initialize_controller():
	# Create instance of the C# DungeonController class
	var DungeonControllerClass = load("res://Source/Controller/DungeonController.cs")
	controller = DungeonControllerClass.new()
