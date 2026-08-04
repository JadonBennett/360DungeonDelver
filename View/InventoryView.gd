extends Control

## Inventory screen showing collected pillars, items, and hero stats.

@onready var hero_stats_label = $VBoxContainer/HeroStatsLabel
@onready var pillars_label = $VBoxContainer/PillarsSection/PillarsLabel
@onready var pillar_1 = $VBoxContainer/PillarsSection/PillarGrid/Pillar1
@onready var pillar_2 = $VBoxContainer/PillarsSection/PillarGrid/Pillar2
@onready var pillar_3 = $VBoxContainer/PillarsSection/PillarGrid/Pillar3
@onready var pillar_4 = $VBoxContainer/PillarsSection/PillarGrid/Pillar4
@onready var items_label = $VBoxContainer/ItemsSection/ItemsLabel
@onready var items_list = $VBoxContainer/ItemsSection/ItemsList

func _ready():
	update_display()

## Updates all inventory displays with current game state.
func update_display():
	var hero = GameManager.get_detailed_hero_stats()
	var inventory = GameManager.get_inventory()

	# Update hero stats
	if hero.has("name"):
		hero_stats_label.text = "%s | HP: %d/%d\nSpeed: %d | Hit: %.0f%% | Block: %.0f%%" % [
			hero.name,
			hero.hp,
			hero.max_hp,
			hero.attack_speed,
			hero.hit_chance * 100,
			hero.block_chance * 100
		]
	else:
		hero_stats_label.text = "Hero: Not loaded"

	# Update pillars collected
	var pillars_collected = inventory.get("pillars_collected", 0)
	pillars_label.text = "Pillars Collected: %d/4" % pillars_collected

	# Update pillar slots (for now just show collected count)
	pillar_1.text = "[ ]" if pillars_collected < 1 else "[✓]"
	pillar_2.text = "[ ]" if pillars_collected < 2 else "[✓]"
	pillar_3.text = "[ ]" if pillars_collected < 3 else "[✓]"
	pillar_4.text = "[ ]" if pillars_collected < 4 else "[✓]"

	# Update items list
	var items = inventory.get("items", [])
	if items.size() == 0:
		items_list.text = "No items in inventory"
	else:
		var items_text = ""
		for item in items:
			items_text += "- " + item + "\n"
		items_list.text = items_text

## Handles Back button press.
func _on_back_pressed():
	get_tree().change_scene_to_file("res://View/RoomView.tscn")
