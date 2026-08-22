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
@onready var actions_grid = $VBoxContainer/ActionsGrid
@onready var victory_panel = $VBoxContainer/VictoryPanel
@onready var victory_label = $VBoxContainer/VictoryPanel/VictoryLabel

var combat_log_text = ""

var item_selection_panel: Panel = null

func _ready():
	# Add pause menu
	var pause_menu_scene = load("res://View/PauseMenu.tscn")
	var pause_menu = pause_menu_scene.instantiate()
	add_child(pause_menu)

	update_display()
	# Connect to game state signals for reactive updates
	GameManager.hp_changed.connect(_on_state_changed)

# CRITICAL: Disconnect signals when scene unloads to prevent memory leak
func _exit_tree():
	if GameManager.hp_changed.is_connected(_on_state_changed):
		GameManager.hp_changed.disconnect(_on_state_changed)

func _on_state_changed():
	update_display()

	if GameManager.is_hero_dead():
		get_tree().call_deferred("change_scene_to_file", "res://View/GameOverView.tscn")

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

	# Update monster display. Keyed on monster_name (not in_combat) so the
	# final blow still renders the real HP/name instead of falling back to
	# placeholder text right when the fight ends.
	if combat_state.has("monster_name") and combat_state.monster_name != "":
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

## Handles Attack button press.
func _on_attack_pressed():
	GameManager.combat_action("attack")
	_check_combat_end()

## Handles Special Skill button press.
func _on_special_pressed():
	GameManager.combat_action("special")
	_check_combat_end()

## Handles Use Item button press.
func _on_item_pressed():
	_show_item_selection()

## Handles Run button press.
func _on_run_pressed():
	GameManager.combat_action("run")
	_check_combat_end()

## Checks whether combat just ended and, if so, shows the appropriate
## outcome. Hero death is already handled by _on_state_changed. A monster
## at 0 HP means it was defeated (show the victory panel); otherwise the
## hero fled and the still-alive monster remains in the room, so just
## return to the room view directly.
func _check_combat_end():
	if GameManager.is_hero_dead():
		return

	if GameManager.is_in_combat():
		return

	var combat_state = GameManager.get_combat_state()
	if combat_state.get("monster_hp", 0) <= 0:
		_show_victory(combat_state.get("monster_name", "the monster"))
	else:
		end_combat()

## Shows the victory panel in place of the action buttons.
func _show_victory(monster_name: String):
	victory_label.text = "You defeated %s!" % monster_name
	actions_grid.visible = false
	victory_panel.visible = true

## Handles Continue button press on the victory panel.
func _on_continue_pressed():
	end_combat()

## Returns to room view (called when combat ends).
func end_combat():
	get_tree().call_deferred("change_scene_to_file", "res://View/RoomView.tscn")

## Shows item selection dialog
func _show_item_selection():
	var inventory = GameManager.get_inventory()
	var items = inventory.get("items", [])

	if items.size() == 0:
		return

	# Create item selection panel
	item_selection_panel = Panel.new()
	item_selection_panel.position = Vector2(200, 150)
	item_selection_panel.size = Vector2(400, 300)
	add_child(item_selection_panel)

	var vbox = VBoxContainer.new()
	vbox.anchor_right = 1.0
	vbox.anchor_bottom = 1.0
	item_selection_panel.add_child(vbox)

	var title = Label.new()
	title.text = "Select Item to Use"
	title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	vbox.add_child(title)

	# Add button for each item
	for i in range(items.size()):
		var button = Button.new()
		button.text = str(items[i])
		button.pressed.connect(_on_item_selected.bind(i))
		vbox.add_child(button)

	# Cancel button
	var cancel = Button.new()
	cancel.text = "Cancel"
	cancel.pressed.connect(_close_item_selection)
	vbox.add_child(cancel)

func _on_item_selected(item_index: int):
	_close_item_selection()
	GameManager.combat_action("item", item_index)
	_check_combat_end()

func _close_item_selection():
	if item_selection_panel != null:
		remove_child(item_selection_panel)
		item_selection_panel.queue_free()
		item_selection_panel = null
