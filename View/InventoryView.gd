extends Control

## Inventory screen for viewing and using items.

@onready var item_list: VBoxContainer = null
@onready var back_button: Button = null
@onready var message_label: Label = null

var speed_label: Label = null
var hit_label: Label = null
var block_label: Label = null
var pillars_label: Label = null

func _ready():
	# Add pause menu
	var pause_menu_scene = load("res://View/PauseMenu.tscn")
	var pause_menu = pause_menu_scene.instantiate()
	add_child(pause_menu)

	# Create UI programmatically
	var panel = Panel.new()
	panel.anchor_right = 1.0
	panel.anchor_bottom = 1.0
	add_child(panel)

	var margin = MarginContainer.new()
	margin.anchor_right = 1.0
	margin.anchor_bottom = 1.0
	margin.add_theme_constant_override("margin_left", 50)
	margin.add_theme_constant_override("margin_right", 50)
	margin.add_theme_constant_override("margin_top", 50)
	margin.add_theme_constant_override("margin_bottom", 50)
	panel.add_child(margin)

	# Main horizontal container
	var hbox_main = HBoxContainer.new()
	hbox_main.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	hbox_main.size_flags_vertical = Control.SIZE_EXPAND_FILL
	margin.add_child(hbox_main)

	# Left side - inventory
	var vbox = VBoxContainer.new()
	vbox.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	vbox.size_flags_vertical = Control.SIZE_EXPAND_FILL
	hbox_main.add_child(vbox)

	# Title
	var title = Label.new()
	title.text = "Inventory"
	title.add_theme_font_size_override("font_size", 32)
	vbox.add_child(title)

	# Message label
	message_label = Label.new()
	message_label.text = ""
	message_label.add_theme_color_override("font_color", Color(0.8, 0.8, 0))
	vbox.add_child(message_label)

	# Item list container
	var scroll = ScrollContainer.new()
	scroll.size_flags_vertical = Control.SIZE_EXPAND_FILL
	vbox.add_child(scroll)

	item_list = VBoxContainer.new()
	scroll.add_child(item_list)

	# Back button
	back_button = Button.new()
	back_button.text = "Back to Game"
	back_button.pressed.connect(_on_back_pressed)
	vbox.add_child(back_button)

	# Right side - stats panel
	var stats_panel = PanelContainer.new()
	stats_panel.custom_minimum_size = Vector2(300, 0)
	hbox_main.add_child(stats_panel)

	var stats_margin = MarginContainer.new()
	stats_margin.add_theme_constant_override("margin_left", 20)
	stats_margin.add_theme_constant_override("margin_right", 20)
	stats_margin.add_theme_constant_override("margin_top", 20)
	stats_margin.add_theme_constant_override("margin_bottom", 20)
	stats_panel.add_child(stats_margin)

	var stats_vbox = VBoxContainer.new()
	stats_margin.add_child(stats_vbox)

	# Stats title
	var stats_title = Label.new()
	stats_title.text = "Hero Stats"
	stats_title.add_theme_font_size_override("font_size", 24)
	stats_title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	stats_vbox.add_child(stats_title)

	# Add spacer
	var spacer1 = Control.new()
	spacer1.custom_minimum_size = Vector2(0, 20)
	stats_vbox.add_child(spacer1)

	# Speed stat
	speed_label = Label.new()
	speed_label.add_theme_font_size_override("font_size", 18)
	stats_vbox.add_child(speed_label)

	# Hit stat
	hit_label = Label.new()
	hit_label.add_theme_font_size_override("font_size", 18)
	stats_vbox.add_child(hit_label)

	# Block stat
	block_label = Label.new()
	block_label.add_theme_font_size_override("font_size", 18)
	stats_vbox.add_child(block_label)

	# Add spacer
	var spacer2 = Control.new()
	spacer2.custom_minimum_size = Vector2(0, 20)
	stats_vbox.add_child(spacer2)

	# Pillars stat
	pillars_label = Label.new()
	pillars_label.add_theme_font_size_override("font_size", 18)
	stats_vbox.add_child(pillars_label)

	# Populate inventory
	update_inventory()

	# Connect to inventory changes
	GameManager.game_state_changed.connect(func(): call_deferred("update_inventory"))

func _exit_tree():
	if GameManager.game_state_changed.is_connected(update_inventory):
		GameManager.game_state_changed.disconnect(update_inventory)

func update_inventory():
	# Update stats
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name") and speed_label != null:
		speed_label.text = "Speed: %s" % str(hero.get("attack_speed", "-"))
		hit_label.text = "Hit Chance: %d%%" % int(hero.get("hit_chance", 0) * 100)
		block_label.text = "Block Chance: %d%%" % int(hero.get("block_chance", 0) * 100)
		pillars_label.text = "Pillars: %d/4" % hero.get("pillars_collected", 0)

	# Clear existing items
	for child in item_list.get_children():
		item_list.remove_child(child)
		child.queue_free()

	var inventory = GameManager.get_inventory()
	var items = inventory.get("items", [])

	if items.size() == 0:
		var empty_label = Label.new()
		empty_label.text = "No items in inventory"
		item_list.add_child(empty_label)
		return

	# Create a button for each item
	for i in range(items.size()):
		var item_container = HBoxContainer.new()
		item_container.custom_minimum_size = Vector2(0, 50)
		item_list.add_child(item_container)

		# Add icon
		var icon = TextureRect.new()
		icon.custom_minimum_size = Vector2(40, 40)
		icon.expand_mode = TextureRect.EXPAND_FIT_WIDTH_PROPORTIONAL
		icon.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED

		var item_name = str(items[i])
		if "Healing" in item_name:
			icon.texture = load("res://assets/Potions/healthPotion.svg")
		elif "Vision" in item_name:
			icon.texture = load("res://assets/Potions/vision_potion.svg")

		item_container.add_child(icon)

		# Add button
		var item_button = Button.new()
		item_button.text = "Use: " + item_name
		item_button.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		item_button.pressed.connect(_on_use_item.bind(i))
		item_container.add_child(item_button)

func _on_use_item(item_index: int):
	var result = GameManager.use_inventory_item(item_index)
	message_label.text = result
	update_inventory()

func _on_back_pressed():
	get_tree().change_scene_to_file("res://View/RoomView.tscn")

func _input(event):
	# Allow ESC or I key to close inventory
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ESCAPE or event.keycode == KEY_I:
			_on_back_pressed()
