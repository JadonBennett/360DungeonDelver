extends Control

## Game Over screen shown when the hero dies.

@onready var hero_name_label = $VBoxContainer/HeroNameLabel
@onready var stats_label = $VBoxContainer/StatsLabel
@onready var try_again_button = $VBoxContainer/TryAgainButton
@onready var menu_button = $VBoxContainer/MenuButton

func _ready():
	update_display()

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

## Handles Try Again button press - starts a new game.
func _on_try_again_pressed():
	get_tree().change_scene_to_file("res://View/HeroSelection.tscn")

## Handles Main Menu button press.
func _on_menu_pressed():
	get_tree().change_scene_to_file("res://View/MainMenu.tscn")
