extends Control

## Victory screen shown when the player wins the game.

@onready var hero_stats_label = $VBoxContainer/HeroStatsLabel

func _ready():
	var hero = GameManager.get_hero_info()

	if hero.has("name"):
		hero_stats_label.text = "%s finished with %d/%d HP" % [
			hero.name,
			hero.hp,
			hero.max_hp
		]
	else:
		hero_stats_label.text = "Hero Stats: N/A"

## Handles Menu button press - returns to main menu.
func _on_menu_pressed():
	get_tree().change_scene_to_file("res://View/MainMenu.tscn")
