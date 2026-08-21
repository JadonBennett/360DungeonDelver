
extends Control

# HP display
# Inventory display
# Pillar count display


# Node references
@onready var hero_name_label: Label = %HeroName
@onready var hp_text_label: Label = %HPText
@onready var hp_bar: ProgressBar = %HPBar
@onready var speed_label: Label = %SpeedStatLabel
@onready var hit_label: Label = %HPStatLabel
@onready var block_label: Label = %BlockStatLabel
@onready var pillars_label: Label = %PillarStatLabel
@onready var minimap_grid: GridContainer = %MinimapGrid

@onready var message_lines: VBoxContainer = %MessageLines
@onready var inventory_button: Button = get_node_or_null("%InventoryButton")
const MESSAGE_LIFETIME := 4.0
const MESSAGE_FADE_DURATION := 1.0
const MAX_VISIBLE_MESSAGES := 5

# HP bar color thresholds
const HP_COLOR_GOOD := Color("5DCAA5")   # teal-400
const HP_COLOR_WARN := Color("EF9F27")   # amber-400
const HP_COLOR_DANGER := Color("E24B4A") # red-400

const MINIMAP_CELL_UNVISITED := Color("2c2c2a")
const MINIMAP_CELL_CURRENT := Color("185fa5")
const MINIMAP_CELL_EXIT := Color("F0997B")


func _ready() -> void:

	if inventory_button != null:
		inventory_button.pressed.connect(_on_inventory_pressed)

	GameManager.game_state_changed.connect(refresh_all)
	GameManager.game_created.connect(refresh_all)
	refresh_all()


func refresh_all() -> void:
	_refresh_hero_info()
	_refresh_room_info()
	_refresh_minimap()


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

	speed_label.text = "Speed %s" % str(info.get("attack_speed", "-"))
	hit_label.text = "Hit %d%%" % int(info.get("hit_chance", 0) * 100)
	block_label.text = "Block %d%%" % int(info.get("block_chance", 0) * 100)
	pillars_label.text = "%d/4 pillars" % info.get("pillars_collected", 0)


func _style_hp_bar(hp: int, max_hp: int) -> void:
	var ratio := float(hp) / float(max(max_hp, 1))
	var color := HP_COLOR_GOOD
	if ratio <= 0.25:
		color = HP_COLOR_DANGER
	elif ratio <= 0.5:
		color = HP_COLOR_WARN

	var fill_style := StyleBoxFlat.new()
	fill_style.bg_color = color
	fill_style.corner_radius_top_left = 4
	fill_style.corner_radius_top_right = 4
	fill_style.corner_radius_bottom_left = 4
	fill_style.corner_radius_bottom_right = 4
	hp_bar.add_theme_stylebox_override("fill", fill_style)


func _refresh_room_info() -> void:
	var room_info: Dictionary = GameManager.get_current_room_info()
	if room_info.is_empty():
		return

	var room_type: String = room_info.get("type", "Normal")
	match room_type:
		"Entrance":
			add_message("You are at the dungeon entrance.")
		"Exit":
			add_message("You've reached the exit.")
		_:
			add_message("You entered a new room.")


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
		message_lines.get_child(0).queue_free()

	var tween := create_tween()
	tween.tween_interval(MESSAGE_LIFETIME)
	tween.tween_property(label, "modulate:a", 0.0, MESSAGE_FADE_DURATION)
	tween.tween_callback(label.queue_free)


func _refresh_minimap() -> void:
	# Wait for layout to settle so minimap_grid.size is accurate
	await get_tree().process_frame

	# Clear existing cells
	for child in minimap_grid.get_children():
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

	minimap_grid.columns = width

	var h_sep: int = minimap_grid.get_theme_constant("h_separation")
	var v_sep: int = minimap_grid.get_theme_constant("v_separation")
	var available_size: Vector2 = minimap_grid.size
	var cell_width: float = max((available_size.x - h_sep * (width - 1)) / width, 4.0)
	var cell_height: float = max((available_size.y - v_sep * (height - 1)) / height, 4.0)
	var cell_size: float = min(cell_width, cell_height)

	for y in range(height):
		for x in range(width):
			var cell := ColorRect.new()
			cell.custom_minimum_size = Vector2(cell_size, cell_size)

			if x == current_x and y == current_y:
				cell.color = MINIMAP_CELL_CURRENT
			elif x == exit_x and y == exit_y:
				cell.color = MINIMAP_CELL_EXIT
			else:
				cell.color = MINIMAP_CELL_UNVISITED

			minimap_grid.add_child(cell)



func _on_inventory_pressed() -> void:
	# Hook up to inventory panel / scene transition
	pass
