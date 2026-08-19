extends Control

## Hero selection screen where player enters name and chooses class.

@onready var name_input = $VBoxContainer/NameInput
@onready var warrior_button = $VBoxContainer/WarriorButton
@onready var priestess_button = $VBoxContainer/PriestessButton
@onready var thief_button = $VBoxContainer/ThiefButton
@onready var start_button = $VBoxContainer/StartButton

var selected_class: String = ""

func _ready():
	start_button.disabled = true
	_update_button_states()

## Handles Warrior button press.
func _on_warrior_pressed():
	selected_class = "Warrior"
	_update_button_states()

## Handles Priestess button press.
func _on_priestess_pressed():
	selected_class = "Priestess"
	_update_button_states()

## Handles Thief button press.
func _on_thief_pressed():
	selected_class = "Thief"
	_update_button_states()

## Updates button visual states to show selection.
func _update_button_states():
	warrior_button.button_pressed = (selected_class == "Warrior")
	priestess_button.button_pressed = (selected_class == "Priestess")
	thief_button.button_pressed = (selected_class == "Thief")

	var has_name = name_input.text.strip_edges() != ""
	var has_class = selected_class != ""
	start_button.disabled = not (has_name and has_class)

## Handles Start Adventure button press.
func _on_start_pressed():
	var hero_name = name_input.text.strip_edges()

	if hero_name == "" or selected_class == "":
		return

	# Store hero info in GameManager (will create this next)
	GameManager.create_new_game(hero_name, selected_class, 0)

	# Transition to game view
	get_tree().change_scene_to_file("res://View/RoomView.tscn")

## Handles Back button press.
func _on_back_pressed():
	get_tree().change_scene_to_file("res://View/MainMenu.tscn")

## Update start button when name changes.
func _on_name_input_text_changed(_new_text):
	_update_button_states()
