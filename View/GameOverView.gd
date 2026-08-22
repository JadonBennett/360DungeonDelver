extends Control

## Game Over screen shown when the hero dies.

@onready var hero_name_label = $VBoxContainer/HeroNameLabel
@onready var stats_label = $VBoxContainer/StatsLabel
@onready var try_again_button = $VBoxContainer/TryAgainButton
@onready var minimap_grid: GridContainer = %MinimapGrid

# Minimap colors (same as HUD)
const MINIMAP_CELL_VISITED := Color("4a4a48")
const MINIMAP_CELL_CURRENT := Color("185fa5")
const MINIMAP_CELL_EXIT := Color("F0997B")
const MINIMAP_CELL_MONSTER := Color("E24B4A")
const MINIMAP_CELL_ITEM := Color("5DCAA5")
const MINIMAP_CELL_PILLAR := Color("F9C74F")
const MINIMAP_WALL_COLOR := Color("8a8a88")
const MINIMAP_WALL_THICKNESS := 2.0

func _ready():
	update_display()
	# Render the full dungeon map
	_render_full_map()

## Updates the display with hero's final stats.
func update_display():
	var hero = GameManager.get_detailed_hero_stats()

	if hero.has("name"):
		hero_name_label.text = hero.name + " has fallen..."

		# Show final stats
		var stats_text = "Final Statistics:\n\n"
		stats_text += "HP: 0/%d\n" % hero.max_hp
		stats_text += "Attack Speed: %d\n" % hero.attack_speed
		stats_text += "Hit Chance: %.0f%%\n" % (hero.hit_chance * 100)
		stats_text += "Block Chance: %.0f%%\n" % (hero.block_chance * 100)
		stats_text += "Pillars Collected: %d/4\n" % hero.pillars_collected

		stats_label.text = stats_text
	else:
		hero_name_label.text = "Game Over"
		stats_label.text = "No hero data available"

## Renders the complete dungeon map showing all rooms
func _render_full_map() -> void:
	# Clear existing cells
	for child in minimap_grid.get_children():
		minimap_grid.remove_child(child)
		child.queue_free()

	var map_data: Dictionary = GameManager.get_minimap_data()
	if map_data.is_empty():
		return

	var width: int = map_data.get("width", 5)
	var height: int = map_data.get("height", 5)
	var current_x: int = map_data.get("current_x", 0)
	var current_y: int = map_data.get("current_y", 0)
	var exit_x: int = map_data.get("exit_x", -1)
	var exit_y: int = map_data.get("exit_y", -1)
	var monster_rooms: Array = map_data.get("monster_rooms", [])
	var item_rooms: Array = map_data.get("item_rooms", [])
	var pillar_rooms: Array = map_data.get("pillar_rooms", [])
	var room_walls: Array = map_data.get("room_walls", [])

	# Build lookup sets
	var monster_coords := {}
	var item_coords := {}
	var pillar_coords := {}
	var wall_data := {}

	for room in monster_rooms:
		var key = str(room.x) + "," + str(room.y)
		monster_coords[key] = true

	for room in item_rooms:
		var key = str(room.x) + "," + str(room.y)
		item_coords[key] = true

	for room in pillar_rooms:
		var key = str(room.x) + "," + str(room.y)
		pillar_coords[key] = true

	for room in room_walls:
		var key = str(room.x) + "," + str(room.y)
		wall_data[key] = room

	minimap_grid.columns = width

	# Calculate cell size
	var available_size: Vector2 = minimap_grid.custom_minimum_size
	var cell_size: float = min(available_size.x / width, available_size.y / height) * 0.9

	# Render all rooms
	for y in range(height):
		for x in range(width):
			var cell := ColorRect.new()
			cell.custom_minimum_size = Vector2(cell_size, cell_size)

			var coord_key = str(x) + "," + str(y)

			# Priority: current > exit > pillar > monster > item > visited
			if x == current_x and y == current_y:
				cell.color = MINIMAP_CELL_CURRENT
			elif x == exit_x and y == exit_y:
				cell.color = MINIMAP_CELL_EXIT
			elif coord_key in pillar_coords:
				cell.color = MINIMAP_CELL_PILLAR
			elif coord_key in monster_coords:
				cell.color = MINIMAP_CELL_MONSTER
			elif coord_key in item_coords:
				cell.color = MINIMAP_CELL_ITEM
			else:
				cell.color = MINIMAP_CELL_VISITED

			# Add walls for all rooms (full map)
			if coord_key in wall_data:
				_add_walls_to_cell(cell, wall_data[coord_key], cell_size)

			minimap_grid.add_child(cell)

## Adds wall decorations to a minimap cell
func _add_walls_to_cell(cell: ColorRect, room_data: Dictionary, cell_size: float) -> void:
	var thickness := MINIMAP_WALL_THICKNESS

	# North wall
	if room_data.get("north_wall", false):
		var wall := ColorRect.new()
		wall.color = MINIMAP_WALL_COLOR
		wall.size = Vector2(cell_size, thickness)
		wall.position = Vector2(0, 0)
		cell.add_child(wall)

	# South wall
	if room_data.get("south_wall", false):
		var wall := ColorRect.new()
		wall.color = MINIMAP_WALL_COLOR
		wall.size = Vector2(cell_size, thickness)
		wall.position = Vector2(0, cell_size - thickness)
		cell.add_child(wall)

	# West wall
	if room_data.get("west_wall", false):
		var wall := ColorRect.new()
		wall.color = MINIMAP_WALL_COLOR
		wall.size = Vector2(thickness, cell_size)
		wall.position = Vector2(0, 0)
		cell.add_child(wall)

	# East wall
	if room_data.get("east_wall", false):
		var wall := ColorRect.new()
		wall.color = MINIMAP_WALL_COLOR
		wall.size = Vector2(thickness, cell_size)
		wall.position = Vector2(cell_size - thickness, 0)
		cell.add_child(wall)

## Handles Try Again button press - returns to main menu.
func _on_try_again_pressed():
	get_tree().change_scene_to_file("res://View/MainMenu.tscn")
