extends Control

## Main menu scene with New Game, Load Game, and Quit options.

var load_dialog: Panel = null
var controls_dialog: Panel = null

func _ready():
	pass

## Handles New Game button press - transitions to hero selection.
func _on_new_game_pressed():
	get_tree().change_scene_to_file("res://View/HeroSelection.tscn")

## Handles Load Game button press - shows load dialog.
func _on_load_game_pressed():
	_show_load_dialog()

## Handles Controls button press - shows controls dialog.
func _on_controls_pressed():
	_show_controls_dialog()

## Handles Quit button press - exits the game.
func _on_quit_pressed():
	get_tree().quit()

## Shows the load game dialog
func _show_load_dialog():
	if load_dialog != null:
		return

	# Create load dialog
	load_dialog = Panel.new()
	load_dialog.position = Vector2(300, 150)
	load_dialog.size = Vector2(600, 400)
	add_child(load_dialog)

	var margin = MarginContainer.new()
	margin.anchor_right = 1.0
	margin.anchor_bottom = 1.0
	margin.add_theme_constant_override("margin_left", 20)
	margin.add_theme_constant_override("margin_right", 20)
	margin.add_theme_constant_override("margin_top", 20)
	margin.add_theme_constant_override("margin_bottom", 20)
	load_dialog.add_child(margin)

	var vbox = VBoxContainer.new()
	margin.add_child(vbox)

	var title = Label.new()
	title.text = "Load Game"
	title.add_theme_font_size_override("font_size", 24)
	title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	vbox.add_child(title)

	var scroll = ScrollContainer.new()
	scroll.size_flags_vertical = Control.SIZE_EXPAND_FILL
	vbox.add_child(scroll)

	var save_list = VBoxContainer.new()
	scroll.add_child(save_list)

	# Populate saves
	var saves = GameManager.get_all_saves()

	if saves.size() == 0:
		var label = Label.new()
		label.text = "No saved games found"
		save_list.add_child(label)
	else:
		for save in saves:
			var btn = Button.new()
			btn.text = save.save_name + " - " + save.timestamp
			btn.pressed.connect(_on_load_save.bind(save.save_id))
			save_list.add_child(btn)

	# Close button
	var close_btn = Button.new()
	close_btn.text = "Close"
	close_btn.pressed.connect(_close_load_dialog)
	vbox.add_child(close_btn)

func _on_load_save(save_id: int):
	var result = GameManager.load_game(save_id)
	if result == "Success":
		get_tree().change_scene_to_file("res://View/RoomView.tscn")
	else:
		print("Load failed: " + result)

func _close_load_dialog():
	if load_dialog != null:
		remove_child(load_dialog)
		load_dialog.queue_free()
		load_dialog = null

## Shows the controls dialog
func _show_controls_dialog():
	if controls_dialog != null:
		return

	# Create controls dialog
	controls_dialog = Panel.new()
	controls_dialog.position = Vector2(250, 100)
	controls_dialog.size = Vector2(700, 500)
	add_child(controls_dialog)

	var margin = MarginContainer.new()
	margin.anchor_right = 1.0
	margin.anchor_bottom = 1.0
	margin.add_theme_constant_override("margin_left", 30)
	margin.add_theme_constant_override("margin_right", 30)
	margin.add_theme_constant_override("margin_top", 20)
	margin.add_theme_constant_override("margin_bottom", 20)
	controls_dialog.add_child(margin)

	var vbox = VBoxContainer.new()
	vbox.add_theme_constant_override("separation", 10)
	margin.add_child(vbox)

	# Title
	var title = Label.new()
	title.text = "CONTROLS"
	title.add_theme_font_size_override("font_size", 28)
	title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	vbox.add_child(title)

	# Spacer
	var spacer1 = Control.new()
	spacer1.custom_minimum_size = Vector2(0, 10)
	vbox.add_child(spacer1)

	# Scroll container for controls list
	var scroll = ScrollContainer.new()
	scroll.size_flags_vertical = Control.SIZE_EXPAND_FILL
	vbox.add_child(scroll)

	var controls_list = VBoxContainer.new()
	controls_list.add_theme_constant_override("separation", 8)
	scroll.add_child(controls_list)

	# Add control entries
	_add_control_section(controls_list, "MOVEMENT", [
		"W / Up Arrow - Move North",
		"A / Left Arrow - Move West",
		"S / Down Arrow - Move South",
		"D / Right Arrow - Move East",
		"Hold Shift - Sprint (1.75x speed)"
	])

	_add_control_section(controls_list, "INVENTORY", [
		"I - Open Inventory",
		"Click Potion Icons - Use item from inventory"
	])

	_add_control_section(controls_list, "COMBAT", [
		"Attack - Standard attack",
		"Special - Use hero's special ability",
		"Use Item - Consume inventory item",
		"Run - 50% chance to flee combat"
	])

	_add_control_section(controls_list, "GAME", [
		"ESC - Pause Menu",
		"Save Game - Available in pause menu",
		"Load Game - Available from main menu"
	])

	_add_control_section(controls_list, "OBJECTIVE", [
		"Collect all 4 Pillars of OO:",
		"  • Abstraction",
		"  • Encapsulation",
		"  • Inheritance",
		"  • Polymorphism",
		"Reach the exit with all pillars to win!"
	])

	# Close button
	var spacer2 = Control.new()
	spacer2.custom_minimum_size = Vector2(0, 10)
	vbox.add_child(spacer2)

	var close_btn = Button.new()
	close_btn.text = "Close"
	close_btn.custom_minimum_size = Vector2(0, 40)
	close_btn.add_theme_font_size_override("font_size", 18)
	close_btn.pressed.connect(_close_controls_dialog)
	vbox.add_child(close_btn)

## Helper function to add a section of controls
func _add_control_section(parent: VBoxContainer, section_title: String, controls: Array):
	# Section title
	var title = Label.new()
	title.text = section_title
	title.add_theme_font_size_override("font_size", 18)
	title.add_theme_color_override("font_color", Color(0.8, 0.9, 1.0))
	parent.add_child(title)

	# Controls in section
	for control in controls:
		var label = Label.new()
		label.text = "  " + control
		label.add_theme_font_size_override("font_size", 14)
		parent.add_child(label)

	# Spacer between sections
	var spacer = Control.new()
	spacer.custom_minimum_size = Vector2(0, 5)
	parent.add_child(spacer)

## Closes the controls dialog
func _close_controls_dialog():
	if controls_dialog != null:
		remove_child(controls_dialog)
		controls_dialog.queue_free()
		controls_dialog = null
