extends Control

## Inventory screen showing collected pillars, items, and hero stats.

@onready var hero_stats_label = $VBoxContainer/HeroStatsLabel
@onready var hero_hp_bar = $VBoxContainer/HeroHPContainer/HeroHPBar
@onready var hero_hp_label = $VBoxContainer/HeroHPContainer/HeroHPLabel
@onready var pillars_label = $VBoxContainer/PillarsSection/PillarsLabel
@onready var pillar_1 = $VBoxContainer/PillarsSection/PillarGrid/Pillar1
@onready var pillar_2 = $VBoxContainer/PillarsSection/PillarGrid/Pillar2
@onready var pillar_3 = $VBoxContainer/PillarsSection/PillarGrid/Pillar3
@onready var pillar_4 = $VBoxContainer/PillarsSection/PillarGrid/Pillar4
@onready var items_label = $VBoxContainer/ItemsSection/ItemsLabel
@onready var items_list = $VBoxContainer/ItemsSection/ItemsList

func _ready():
	update_display()
	# Connect to game state signals for reactive updates
	GameManager.hp_changed.connect(_on_state_changed)
	GameManager.pillars_changed.connect(_on_state_changed)

# CRITICAL: Disconnect signals when scene unloads to prevent memory leak
func _exit_tree():
	if GameManager.hp_changed.is_connected(_on_state_changed):
		GameManager.hp_changed.disconnect(_on_state_changed)
	if GameManager.pillars_changed.is_connected(_on_state_changed):
		GameManager.pillars_changed.disconnect(_on_state_changed)

func _on_state_changed():
	update_display()

## Updates all inventory displays with current game state.
func update_display():
	var hero = GameManager.get_detailed_hero_stats()
	var inventory = GameManager.get_inventory()

	# Update hero stats
	if hero.has("name"):
		hero_stats_label.text = "%s\nSpeed: %d | Hit: %.0f%% | Block: %.0f%%" % [
			hero.name,
			hero.attack_speed,
			hero.hit_chance * 100,
			hero.block_chance * 100
		]

		# Update HP bar and label
		hero_hp_bar.max_value = hero.max_hp
		hero_hp_bar.value = hero.hp
		hero_hp_label.text = "HP: %d/%d" % [hero.hp, hero.max_hp]

		# Color code HP bar
		var hp_percent = float(hero.hp) / float(hero.max_hp)
		if hp_percent > 0.5:
			hero_hp_bar.modulate = Color(0, 1, 0)  # Green
		elif hp_percent > 0.25:
			hero_hp_bar.modulate = Color(1, 1, 0)  # Yellow
		else:
			hero_hp_bar.modulate = Color(1, 0, 0)  # Red
	else:
		hero_stats_label.text = "Hero: Not loaded"
		hero_hp_bar.value = 0
		hero_hp_label.text = "HP: ?/?"

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
