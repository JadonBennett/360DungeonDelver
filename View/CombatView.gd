extends Control

## Combat screen for turn-based battles with monsters.

@onready var hero_name_label = $VBoxContainer/HeroPanel/HeroName
@onready var hero_hp_bar = $VBoxContainer/HeroPanel/HeroHPContainer/HeroHPBar
@onready var hero_hp_label = $VBoxContainer/HeroPanel/HeroHPContainer/HeroHPLabel
@onready var monster_name_label = $VBoxContainer/MonsterPanel/MonsterName
@onready var monster_hp_bar = $VBoxContainer/MonsterPanel/MonsterHPContainer/MonsterHPBar
@onready var monster_hp_label = $VBoxContainer/MonsterPanel/MonsterHPContainer/MonsterHPLabel
@onready var combat_log = $VBoxContainer/CombatLog
@onready var attack_button = $VBoxContainer/ActionsGrid/AttackButton
@onready var special_button = $VBoxContainer/ActionsGrid/SpecialButton
@onready var item_button = $VBoxContainer/ActionsGrid/ItemButton
@onready var run_button = $VBoxContainer/ActionsGrid/RunButton

var combat_log_text = ""

func _ready():
	update_display()
	# Connect to game state signals for reactive updates
	GameManager.hp_changed.connect(_on_state_changed)

func _on_state_changed():
	update_display()

## Updates all combat displays with current game state.
func update_display():
	var hero = GameManager.get_detailed_hero_stats()
	var combat_state = GameManager.get_combat_state()

	# Update hero display
	if hero.has("name"):
		hero_name_label.text = hero.name
		hero_hp_bar.max_value = hero.max_hp
		hero_hp_bar.value = hero.hp
		hero_hp_label.text = "HP: %d/%d" % [hero.hp, hero.max_hp]

		# Color code hero HP bar
		var hero_hp_percent = float(hero.hp) / float(hero.max_hp)
		if hero_hp_percent > 0.5:
			hero_hp_bar.modulate = Color(0, 1, 0)  # Green
		elif hero_hp_percent > 0.25:
			hero_hp_bar.modulate = Color(1, 1, 0)  # Yellow
		else:
			hero_hp_bar.modulate = Color(1, 0, 0)  # Red
	else:
		hero_name_label.text = "Hero"
		hero_hp_bar.value = 0
		hero_hp_label.text = "HP: ?/?"

	# Update monster display
	if combat_state.has("in_combat") and combat_state.in_combat:
		var monster_hp = combat_state.get("monster_hp", 0)
		var monster_max_hp = combat_state.get("monster_max_hp", 1)

		monster_name_label.text = combat_state.get("monster_name", "Unknown Monster")
		monster_hp_bar.max_value = monster_max_hp
		monster_hp_bar.value = monster_hp
		monster_hp_label.text = "HP: %d/%d" % [monster_hp, monster_max_hp]

		# Color code monster HP bar
		var monster_hp_percent = float(monster_hp) / float(monster_max_hp)
		if monster_hp_percent > 0.5:
			monster_hp_bar.modulate = Color(1, 0, 0)  # Red (enemy)
		elif monster_hp_percent > 0.25:
			monster_hp_bar.modulate = Color(1, 0.5, 0)  # Orange
		else:
			monster_hp_bar.modulate = Color(0.5, 0, 0)  # Dark red

		# Only update combat log from state if combat is actually active
		if combat_state.has("combat_log") and combat_state.combat_log.size() > 0:
			combat_log_text = ""
			for entry in combat_state.combat_log:
				combat_log_text += entry + "\n"
			combat_log.text = combat_log_text
		else:
			# Keep existing local combat log
			combat_log.text = combat_log_text
	else:
		monster_name_label.text = "Test Monster"
		monster_hp_bar.max_value = 50
		monster_hp_bar.value = 50
		monster_hp_label.text = "HP: 50/50"
		# Don't overwrite combat log - keep test messages
		combat_log.text = combat_log_text

## Adds a message to the combat log.
func add_to_log(message: String):
	combat_log_text += message + "\n"
	combat_log.text = combat_log_text

## Handles Attack button press.
func _on_attack_pressed():
	add_to_log("You attack the monster!")
	# TODO: Call GameManager.attack() when implemented
	update_display()

## Handles Special Skill button press.
func _on_special_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("special_skill"):
		add_to_log("You use " + hero.special_skill + "!")
	else:
		add_to_log("You use your special skill!")
	# TODO: Call GameManager.use_special_skill() when implemented
	update_display()

## Handles Use Item button press.
func _on_item_pressed():
	add_to_log("Item usage not yet implemented")
	# TODO: Show item selection and use item
	update_display()

## Handles Run button press.
func _on_run_pressed():
	add_to_log("You attempt to flee!")
	# TODO: Call GameManager.attempt_flee() when implemented
	# For now, just return to room
	await get_tree().create_timer(1.0).timeout
	get_tree().change_scene_to_file("res://View/RoomView.tscn")

## Returns to room view (called when combat ends).
func end_combat():
	get_tree().change_scene_to_file("res://View/RoomView.tscn")
