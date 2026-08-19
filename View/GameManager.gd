extends Node

## Global game state manager that wraps the C# DungeonController.
## This singleton provides access to game state from any scene.

# MVC Signals - emitted when game state changes
signal game_created
signal hp_changed
signal pillars_changed
signal room_changed
signal game_state_changed  # General state change signal
signal combat_started
signal combat_ended

var controller = null

func _ready():
	_initialize_controller()

## Creates a new game with the specified hero and dungeon parameters.
func create_new_game(hero_name: String, hero_class: String, pillar_type: String = "Abstraction", width: int = 5, height: int = 5):
	if controller == null:
		_initialize_controller()
	controller.CreateNewGame(hero_name, hero_class, pillar_type, width, height)
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
		if controller.IsInCombat():
			combat_started.emit()
	return success

## Performs the hero's chosen combat action (e.g. "attack", "special", "item", "run").
func combat_action(action: String):
	if controller == null:
		return
	controller.PerformCombatAction(action)
	hp_changed.emit()
	game_state_changed.emit()
	if not controller.IsInCombat():
		combat_ended.emit()

## True if the hero is currently in combat.
func is_in_combat() -> bool:
	if controller == null:
		return false
	return controller.IsInCombat()

## DEBUG: Summarizes pit/monster/item/pillar counts across the whole dungeon.
func get_dungeon_summary() -> String:
	if controller == null:
		return ""
	return controller.DebugGetDungeonSummary()

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

## Initializes the C# controller.
func _initialize_controller():
	# Create instance of the C# DungeonController class
	var DungeonControllerClass = load("res://Source/Controller/DungeonController.cs")
	controller = DungeonControllerClass.new()
