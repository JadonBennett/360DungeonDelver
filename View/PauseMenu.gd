extends CanvasLayer

## Pause menu overlay - ESC to open, shows save/load options

@onready var panel: Panel = null
@onready var save_list: VBoxContainer = null
@onready var message_label: Label = null

var is_paused := false

func _ready():
	layer = 99  # High layer to appear on top
	
	# Create UI
	_create_ui()
	hide_menu()

func _create_ui():
	panel = Panel.new()
	panel.anchor_left = 0.25
	panel.anchor_top = 0.2
	panel.anchor_right = 0.75
	panel.anchor_bottom = 0.8
	add_child(panel)
	
	var margin = MarginContainer.new()
	margin.anchor_right = 1.0
	margin.anchor_bottom = 1.0
	margin.add_theme_constant_override("margin_left", 20)
	margin.add_theme_constant_override("margin_right", 20)
	margin.add_theme_constant_override("margin_top", 20)
	margin.add_theme_constant_override("margin_bottom", 20)
	panel.add_child(margin)
	
	var vbox = VBoxContainer.new()
	vbox.add_theme_constant_override("separation", 10)
	margin.add_child(vbox)
	
	# Title
	var title = Label.new()
	title.text = "PAUSED"
	title.add_theme_font_size_override("font_size", 32)
	title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	vbox.add_child(title)
	
	# Message label
	message_label = Label.new()
	message_label.text = ""
	message_label.add_theme_color_override("font_color", Color(1, 0.8, 0))
	message_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	vbox.add_child(message_label)
	
	# New Save button
	var new_save_btn = Button.new()
	new_save_btn.text = "Save Game"
	new_save_btn.pressed.connect(_on_new_save_pressed)
	vbox.add_child(new_save_btn)
	
	# Load section
	var load_label = Label.new()
	load_label.text = "Load Game:"
	load_label.add_theme_font_size_override("font_size", 20)
	vbox.add_child(load_label)
	
	var scroll = ScrollContainer.new()
	scroll.size_flags_vertical = Control.SIZE_EXPAND_FILL
	vbox.add_child(scroll)
	
	save_list = VBoxContainer.new()
	scroll.add_child(save_list)
	
	# Resume button
	var resume_btn = Button.new()
	resume_btn.text = "Resume"
	resume_btn.pressed.connect(_on_resume_pressed)
	vbox.add_child(resume_btn)
	
	# Quit button
	var quit_btn = Button.new()
	quit_btn.text = "Quit to Main Menu"
	quit_btn.pressed.connect(_on_quit_pressed)
	vbox.add_child(quit_btn)

func _input(event):
	if event is InputEventKey and event.pressed and event.keycode == KEY_ESCAPE:
		if is_paused:
			hide_menu()
		else:
			show_menu()

func show_menu():
	is_paused = true
	panel.visible = true
	get_tree().paused = true
	_refresh_save_list()

func hide_menu():
	is_paused = false
	panel.visible = false
	get_tree().paused = false
	message_label.text = ""

func _refresh_save_list():
	# Clear existing
	for child in save_list.get_children():
		save_list.remove_child(child)
		child.queue_free()
	
	var saves = GameManager.get_all_saves()
	
	if saves.size() == 0:
		var label = Label.new()
		label.text = "No saved games"
		save_list.add_child(label)
		return
	
	for save in saves:
		var hbox = HBoxContainer.new()
		
		var load_btn = Button.new()
		load_btn.text = "Load: " + save.save_name
		load_btn.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		load_btn.pressed.connect(_on_load_pressed.bind(save.save_id))
		hbox.add_child(load_btn)
		
		var delete_btn = Button.new()
		delete_btn.text = "Delete"
		delete_btn.pressed.connect(_on_delete_pressed.bind(save.save_id))
		hbox.add_child(delete_btn)
		
		save_list.add_child(hbox)

func _on_new_save_pressed():
	var saves = GameManager.get_all_saves()

	# Check if at max saves
	if saves.size() >= 3:
		message_label.text = "Max 3 saves! Overwrite an existing save by clicking Save below:"
		_show_overwrite_mode()
	else:
		var save_name = "Save " + Time.get_datetime_string_from_system()
		var result = GameManager.save_game(save_name)
		message_label.text = result
		_refresh_save_list()

func _show_overwrite_mode():
	# Clear and rebuild save list with overwrite buttons
	for child in save_list.get_children():
		save_list.remove_child(child)
		child.queue_free()

	var saves = GameManager.get_all_saves()

	for save in saves:
		var hbox = HBoxContainer.new()

		var save_btn = Button.new()
		save_btn.text = "Overwrite: " + save.save_name
		save_btn.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		save_btn.pressed.connect(_on_overwrite_pressed.bind(save.save_id))
		hbox.add_child(save_btn)

		var delete_btn = Button.new()
		delete_btn.text = "Delete"
		delete_btn.pressed.connect(_on_delete_pressed.bind(save.save_id))
		hbox.add_child(delete_btn)

		save_list.add_child(hbox)

func _on_overwrite_pressed(save_id: int):
	# Delete old save and create new one with same ID
	var save_name = "Save " + Time.get_datetime_string_from_system()
	var result = GameManager.overwrite_save(save_id, save_name)
	message_label.text = result
	_refresh_save_list()

func _on_load_pressed(save_id: int):
	var result = GameManager.load_game(save_id)
	if result == "Success":
		hide_menu()
		get_tree().reload_current_scene()
	else:
		message_label.text = result

func _on_delete_pressed(save_id: int):
	GameManager.delete_save(save_id)
	message_label.text = "Save deleted"
	_refresh_save_list()

func _on_resume_pressed():
	hide_menu()

func _on_quit_pressed():
	get_tree().paused = false
	get_tree().change_scene_to_file("res://View/MainMenu.tscn")
