extends Control

## Main menu scene with New Game and Quit options.

func _ready():
	pass

## Handles New Game button press - transitions to hero selection.
func _on_new_game_pressed():
	get_tree().change_scene_to_file("res://View/HeroSelection.tscn")

## Handles Quit button press - exits the game.
func _on_quit_pressed():
	get_tree().quit()
