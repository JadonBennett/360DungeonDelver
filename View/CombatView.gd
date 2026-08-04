extends Control

## Combat screen for turn-based battles with monsters.

@onready var hero_name_label = $VBoxContainer/HeroPanel/HeroName
@onready var hero_hp_label = $VBoxContainer/HeroPanel/HeroHP
@onready var monster_name_label = $VBoxContainer/MonsterPanel/MonsterName
@onready var monster_hp_label = $VBoxContainer/MonsterPanel/MonsterHP
@onready var combat_log = $VBoxContainer/CombatLog
@onready var attack_button = $VBoxContainer/ActionsGrid/AttackButton
@onready var special_button = $VBoxContainer/ActionsGrid/SpecialButton
@onready var item_button = $VBoxContainer/ActionsGrid/ItemButton
@onready var run_button = $VBoxContainer/ActionsGrid/RunButton

var combat_log_text = ""

func _ready():
	update_display()

## Updates all combat displays with current game state.
func update_display():
	var hero = GameManager.get_detailed_hero_stats()
	var combat_state = GameManager.get_combat_state()

	# Update hero display
	if hero.has("name"):
		hero_name_label.text = hero.name
		hero_hp_label.text = "HP: %d/%d" % [hero.hp, hero.max_hp]
	else:
		hero_name_label.text = "Hero"
		hero_hp_label.text = "HP: ?/?"

	# Update monster display
	if combat_state.has("in_combat") and combat_state.in_combat:
		monster_name_label.text = combat_state.get("monster_name", "Unknown Monster")
		monster_hp_label.text = "HP: %d/%d" % [
			combat_state.get("monster_hp", 0),
			combat_state.get("monster_max_hp", 0)
		]

		# Update combat log
		if combat_state.has("combat_log"):
			combat_log_text = ""
			for entry in combat_state.combat_log:
				combat_log_text += entry + "\n"
			combat_log.text = combat_log_text
	else:
		monster_name_label.text = "No Monster"
		monster_hp_label.text = "HP: 0/0"
		combat_log.text = "Combat not active"

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
