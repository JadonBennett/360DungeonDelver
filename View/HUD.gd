
extends Control

# HP display
# Inventory display
# Pillar count display


# Node references
@onready var hero_name_label: Label = %HeroName
@onready var hp_text_label: Label = %HPText
@onready var hp_bar: ProgressBar = %HPBar
@onready var minimap_grid: GridContainer = %MinimapGrid

# Pillar display references
@onready var pillar_abstraction: TextureRect = %PillarAbstraction
@onready var pillar_encapsulation: TextureRect = %PillarEncapsulation
@onready var pillar_inheritance: TextureRect = %PillarInheritance
@onready var pillar_polymorphism: TextureRect = %PillarPolymorphism

# Inventory display
@onready var inventory_icons: GridContainer = %InventoryIcons

@onready var message_lines: VBoxContainer = %MessageLines
@onready var inventory_button: Button = get_node_or_null("%InventoryButton")
const MESSAGE_LIFETIME := 4.0
const MESSAGE_FADE_DURATION := 1.0
const MAX_VISIBLE_MESSAGES := 5

# HP bar color thresholds
const HP_COLOR_GOOD := Color("5DCAA5")   # teal-400
const HP_COLOR_WARN := Color("EF9F27")   # amber-400
const HP_COLOR_DANGER := Color("E24B4A") # red-400

const MINIMAP_CELL_UNVISITED := Color("2c2c2a")  # dark gray
const MINIMAP_CELL_VISITED := Color("4a4a48")    # light gray
const MINIMAP_CELL_CURRENT := Color("185fa5")    # blue
const MINIMAP_CELL_EXIT := Color("F0997B")       # orange
const MINIMAP_CELL_MONSTER := Color("E24B4A")    # red-400 (danger)
const MINIMAP_CELL_ITEM := Color("5DCAA5")       # teal-400 (items)
const MINIMAP_CELL_PILLAR := Color("F9C74F")     # yellow-400 (pillars)
const MINIMAP_WALL_COLOR := Color("8a8a88")      # light gray for walls
const MINIMAP_WALL_THICKNESS := 2.0

# Reusable StyleBox to prevent memory leak
var hp_bar_style: StyleBoxFlat = null

# Track active message tweens for cleanup
var active_message_tweens: Array[Tween] = []


func _ready() -> void:

	if inventory_button != null:
		inventory_button.pressed.connect(_on_inventory_pressed)

	# Defer all refreshes to avoid creating nodes during physics callbacks
	GameManager.game_state_changed.connect(func(): call_deferred("refresh_all"))
	GameManager.game_created.connect(func(): call_deferred("refresh_all"))
	refresh_all()

# CRITICAL: Disconnect signals when scene unloads to prevent memory leak
func _exit_tree():
	# Kill all active message tweens
	for tween in active_message_tweens:
		if tween and tween.is_valid():
			tween.kill()
	active_message_tweens.clear()

	# Disconnect button signal
	if inventory_button != null and inventory_button.pressed.is_connected(_on_inventory_pressed):
		inventory_button.pressed.disconnect(_on_inventory_pressed)

	# Disconnect GameManager signals
	if GameManager.game_state_changed.is_connected(refresh_all):
		GameManager.game_state_changed.disconnect(refresh_all)
	if GameManager.game_created.is_connected(refresh_all):
		GameManager.game_created.disconnect(refresh_all)


func refresh_all() -> void:
	_refresh_hero_info()
	_refresh_room_info()
	_refresh_inventory_display()
	# Defer minimap to avoid await during signal callback
	call_deferred("_refresh_minimap")


func _refresh_hero_info() -> void:
	var info: Dictionary = GameManager.get_detailed_hero_stats()
	if info.is_empty():
		return

	hero_name_label.text = info.get("name", "")

	var hp: int = info.get("hp", 0)
	var max_hp: int = info.get("max_hp", 1)
	hp_text_label.text = "%d / %d HP" % [hp, max_hp]
	hp_bar.max_value = max_hp
	hp_bar.value = hp
	_style_hp_bar(hp, max_hp)

	# Update pillar display - show only collected pillars
	var collected_pillars = info.get("collected_pillar_types", [])
	pillar_abstraction.visible = "Abstraction" in collected_pillars
	pillar_encapsulation.visible = "Encapsulation" in collected_pillars
	pillar_inheritance.visible = "Inheritance" in collected_pillars
	pillar_polymorphism.visible = "Polymorphism" in collected_pillars


func _style_hp_bar(hp: int, max_hp: int) -> void:
	var ratio := float(hp) / float(max(max_hp, 1))
	var color := HP_COLOR_GOOD
	if ratio <= 0.25:
		color = HP_COLOR_DANGER
	elif ratio <= 0.5:
		color = HP_COLOR_WARN

	# Reuse StyleBox instead of creating new one each time (prevents memory leak)
	if hp_bar_style == null:
		hp_bar_style = StyleBoxFlat.new()
		hp_bar_style.corner_radius_top_left = 4
		hp_bar_style.corner_radius_top_right = 4
		hp_bar_style.corner_radius_bottom_left = 4
		hp_bar_style.corner_radius_bottom_right = 4
		hp_bar.add_theme_stylebox_override("fill", hp_bar_style)

	hp_bar_style.bg_color = color


func _refresh_room_info() -> void:
	var room_info: Dictionary = GameManager.get_current_room_info()
	if room_info.is_empty():
		return

	# Room info is now silent - no messages displayed
	# Messages can be added here in the future if needed


func _refresh_inventory_display() -> void:
	# Clear existing icons
	for child in inventory_icons.get_children():
		inventory_icons.remove_child(child)
		child.queue_free()

	var inventory = GameManager.get_inventory()
	var items = inventory.get("items", [])

	# Add clickable button for each item
	for i in range(items.size()):
		var item = items[i]
		var button = TextureButton.new()
		button.custom_minimum_size = Vector2(32, 32)
		button.ignore_texture_size = true
		button.stretch_mode = TextureButton.STRETCH_KEEP_ASPECT_CENTERED

		var item_name = str(item)
		if "Healing" in item_name:
			button.texture_normal = load("res://assets/Potions/healthPotion.svg")
		elif "Vision" in item_name:
			button.texture_normal = load("res://assets/Potions/vision_potion.svg")

		# Connect click to use item
		button.pressed.connect(_on_inventory_item_clicked.bind(i))
		inventory_icons.add_child(button)


## Adds a floating message line that fades out 
func add_message(text: String) -> void:
	var label := Label.new()
	label.text = text
	label.add_theme_color_override("font_color", Color("e8e8e6"))
	label.add_theme_font_size_override("font_size", 14)
	label.add_theme_constant_override("outline_size", 4)
	label.add_theme_color_override("font_outline_color", Color(0, 0, 0, 0.6))
	message_lines.add_child(label)

	# Trim oldest messages if over the cap
	while message_lines.get_child_count() > MAX_VISIBLE_MESSAGES:
		var old_label = message_lines.get_child(0)
		message_lines.remove_child(old_label)
		old_label.free()  # FREE NOW - we're in deferred context

	var tween := create_tween()
	tween.tween_interval(MESSAGE_LIFETIME)
	tween.tween_property(label, "modulate:a", 0.0, MESSAGE_FADE_DURATION)
	tween.tween_callback(label.queue_free)
	tween.tween_callback(func(): active_message_tweens.erase(tween))
	active_message_tweens.append(tween)


func _refresh_minimap() -> void:
	# Clear existing cells (FREE NOW - we're in deferred context)
	for child in minimap_grid.get_children():
		minimap_grid.remove_child(child)
		child.free()

	var map_data: Dictionary = GameManager.get_minimap_data()
	if map_data.is_empty():
		return

	var width: int = map_data.get("width", 5)
	var height: int = map_data.get("height", 5)
	var current_x: int = map_data.get("current_x", 0)
	var current_y: int = map_data.get("current_y", 0)
	var exit_x: int = map_data.get("exit_x", -1)
	var exit_y: int = map_data.get("exit_y", -1)
	var visited_rooms: Array = map_data.get("visited_rooms", [])
	var monster_rooms: Array = map_data.get("monster_rooms", [])
	var item_rooms: Array = map_data.get("item_rooms", [])
	var pillar_rooms: Array = map_data.get("pillar_rooms", [])
	var room_walls: Array = map_data.get("room_walls", [])

	# Build lookup sets for faster checking
	var visited_coords := {}
	var monster_coords := {}
	var item_coords := {}
	var pillar_coords := {}
	var wall_data := {}

	for room in visited_rooms:
		var key = str(room.x) + "," + str(room.y)
		visited_coords[key] = true

	# Build wall data lookup
	for room in room_walls:
		var key = str(room.x) + "," + str(room.y)
		wall_data[key] = room

	# Only process monster/item data if debug display is enabled
	if GameManager.show_minimap_contents:
		for room in monster_rooms:
			var key = str(room.x) + "," + str(room.y)
			monster_coords[key] = true

		for room in item_rooms:
			var key = str(room.x) + "," + str(room.y)
			item_coords[key] = true

		for room in pillar_rooms:
			var key = str(room.x) + "," + str(room.y)
			pillar_coords[key] = true

	minimap_grid.columns = width

	# Set GridContainer to shrink to content size and center
	minimap_grid.size_flags_horizontal = Control.SIZE_SHRINK_CENTER
	minimap_grid.size_flags_vertical = Control.SIZE_SHRINK_CENTER

	# Keep spacing between cells, but calculate to use full width
	var h_sep: int = minimap_grid.get_theme_constant("h_separation")
	var v_sep: int = minimap_grid.get_theme_constant("v_separation")
	var available_size: Vector2 = minimap_grid.size

	# Calculate cell size accounting for separations, keep cells square
	var cell_width: float = (available_size.x - h_sep * (width - 1)) / width
	var cell_height: float = (available_size.y - v_sep * (height - 1)) / height
	var cell_size: float = min(cell_width, cell_height)

	for y in range(height):
		for x in range(width):
			var cell := ColorRect.new()
			cell.custom_minimum_size = Vector2(cell_size, cell_size)

			var coord_key = str(x) + "," + str(y)

			# Priority: current > exit > pillar > monster > item > visited > unvisited
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
			elif coord_key in visited_coords:
				cell.color = MINIMAP_CELL_VISITED
			else:
				cell.color = MINIMAP_CELL_UNVISITED

			# Add walls: always for visited rooms, or for all rooms when debug is enabled
			var should_show_walls = (coord_key in visited_coords) or GameManager.show_minimap_contents
			if should_show_walls and coord_key in wall_data:
				_add_walls_to_cell(cell, wall_data[coord_key], cell_size)

			minimap_grid.add_child(cell)


## Adds wall decorations to a minimap cell for visited rooms
func _add_walls_to_cell(cell: ColorRect, room_data: Dictionary, cell_size: float) -> void:
	var thickness := MINIMAP_WALL_THICKNESS

	# North wall (top edge)
	if room_data.get("north_wall", false):
		var wall := ColorRect.new()
		wall.color = MINIMAP_WALL_COLOR
		wall.size = Vector2(cell_size, thickness)
		wall.position = Vector2(0, 0)
		cell.add_child(wall)

	# South wall (bottom edge)
	if room_data.get("south_wall", false):
		var wall := ColorRect.new()
		wall.color = MINIMAP_WALL_COLOR
		wall.size = Vector2(cell_size, thickness)
		wall.position = Vector2(0, cell_size - thickness)
		cell.add_child(wall)

	# West wall (left edge)
	if room_data.get("west_wall", false):
		var wall := ColorRect.new()
		wall.color = MINIMAP_WALL_COLOR
		wall.size = Vector2(thickness, cell_size)
		wall.position = Vector2(0, 0)
		cell.add_child(wall)

	# East wall (right edge)
	if room_data.get("east_wall", false):
		var wall := ColorRect.new()
		wall.color = MINIMAP_WALL_COLOR
		wall.size = Vector2(thickness, cell_size)
		wall.position = Vector2(cell_size - thickness, 0)
		cell.add_child(wall)


func _on_inventory_item_clicked(item_index: int) -> void:
	var result = GameManager.use_inventory_item(item_index)
	if result != "":
		add_message(result)


func _on_inventory_pressed() -> void:
	# Hook up to inventory panel / scene transition
	pass
