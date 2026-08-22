extends CanvasLayer

## Debug menu overlay for testing game features.

@onready var panel = $Panel
@onready var info_label = $Panel/ScrollContainer/VBoxContainer/InfoLabel

# HP Control buttons
@onready var hp_100_btn = $Panel/ScrollContainer/VBoxContainer/HPGrid1/HP100
@onready var hp_50_btn = $Panel/ScrollContainer/VBoxContainer/HPGrid1/HP50
@onready var hp_25_btn = $Panel/ScrollContainer/VBoxContainer/HPGrid1/HP25
@onready var hp_1_btn = $Panel/ScrollContainer/VBoxContainer/HPGrid2/HP1
@onready var hp_0_btn = $Panel/ScrollContainer/VBoxContainer/HPGrid2/HP0
@onready var damage_10_btn = $Panel/ScrollContainer/VBoxContainer/HPGrid3/Damage10
@onready var heal_10_btn = $Panel/ScrollContainer/VBoxContainer/HPGrid3/Heal10

# Pillar Control buttons
@onready var pillars_0_btn = $Panel/ScrollContainer/VBoxContainer/PillarGrid/Pillars0
@onready var pillars_2_btn = $Panel/ScrollContainer/VBoxContainer/PillarGrid/Pillars2
@onready var pillars_4_btn = $Panel/ScrollContainer/VBoxContainer/PillarGrid/Pillars4

# Item buttons
@onready var give_healing_potion_btn = $Panel/ScrollContainer/VBoxContainer/ItemGrid/GiveHealingPotion
@onready var give_vision_potion_btn = $Panel/ScrollContainer/VBoxContainer/ItemGrid/GiveVisionPotion

# Screen buttons that need a game
@onready var room_view_btn = $Panel/ScrollContainer/VBoxContainer/ScreenScrollContainer/ScreenList/GotoRoomView
@onready var inventory_btn = $Panel/ScrollContainer/VBoxContainer/ScreenScrollContainer/ScreenList/GotoInventory
@onready var combat_btn = $Panel/ScrollContainer/VBoxContainer/ScreenScrollContainer/ScreenList/GotoCombat

# Teleport buttons
@onready var tp_exit_btn = $Panel/ScrollContainer/VBoxContainer/TeleportGrid/TPExit
@onready var tp_entrance_btn = $Panel/ScrollContainer/VBoxContainer/TeleportGrid/TPEntrance

# Minimap toggle
var minimap_toggle: Button = null

var debug_menu_visible = false

func _ready():
	# Set to high layer so debug menu appears on top
	layer = 100

	# Create minimap toggle button programmatically
	minimap_toggle = Button.new()
	minimap_toggle.pressed.connect(_on_minimap_toggle_pressed)
	_update_minimap_button_text()
	# Insert after info label
	$Panel/ScrollContainer/VBoxContainer.add_child(minimap_toggle)
	$Panel/ScrollContainer/VBoxContainer.move_child(minimap_toggle, 1)

	hide_menu()
	# Connect to all game state signals (deferred to avoid physics callback issues)
	GameManager.game_created.connect(func(): call_deferred("_on_game_state_changed"))
	GameManager.hp_changed.connect(func(): call_deferred("_on_game_state_changed"))
	GameManager.pillars_changed.connect(func(): call_deferred("_on_game_state_changed"))
	GameManager.room_changed.connect(func(): call_deferred("_on_game_state_changed"))
	GameManager.game_state_changed.connect(func(): call_deferred("_on_game_state_changed"))

# CRITICAL: Disconnect signals when scene unloads to prevent memory leak
func _exit_tree():
	# Disconnect minimap toggle
	if minimap_toggle != null and minimap_toggle.pressed.is_connected(_on_minimap_toggle_pressed):
		minimap_toggle.pressed.disconnect(_on_minimap_toggle_pressed)

	if GameManager.game_created.is_connected(_on_game_state_changed):
		GameManager.game_created.disconnect(_on_game_state_changed)
	if GameManager.hp_changed.is_connected(_on_game_state_changed):
		GameManager.hp_changed.disconnect(_on_game_state_changed)
	if GameManager.pillars_changed.is_connected(_on_game_state_changed):
		GameManager.pillars_changed.disconnect(_on_game_state_changed)
	if GameManager.room_changed.is_connected(_on_game_state_changed):
		GameManager.room_changed.disconnect(_on_game_state_changed)
	if GameManager.game_state_changed.is_connected(_on_game_state_changed):
		GameManager.game_state_changed.disconnect(_on_game_state_changed)

func _on_game_state_changed():
	# Update menu when any game state changes
	if debug_menu_visible:
		update_info()

func _input(event):
	# Toggle debug menu with F12
	if event is InputEventKey and event.pressed and event.keycode == KEY_F12:
		toggle_menu()

func toggle_menu():
	debug_menu_visible = !debug_menu_visible
	if debug_menu_visible:
		show_menu()
	else:
		hide_menu()

func show_menu():
	panel.visible = true
	update_info()

func hide_menu():
	panel.visible = false

func _update_button_states(game_active: bool):
	# HP controls
	hp_100_btn.disabled = !game_active
	hp_50_btn.disabled = !game_active
	hp_25_btn.disabled = !game_active
	hp_1_btn.disabled = !game_active
	hp_0_btn.disabled = !game_active
	damage_10_btn.disabled = !game_active
	heal_10_btn.disabled = !game_active

	# Pillar controls
	pillars_0_btn.disabled = !game_active
	pillars_2_btn.disabled = !game_active
	pillars_4_btn.disabled = !game_active

	# Item controls
	give_healing_potion_btn.disabled = !game_active
	give_vision_potion_btn.disabled = !game_active

	# Screen buttons that need a game
	room_view_btn.disabled = !game_active
	inventory_btn.disabled = !game_active
	combat_btn.disabled = !game_active

	# Teleport buttons
	tp_exit_btn.disabled = !game_active
	tp_entrance_btn.disabled = !game_active

func update_info():
	var hero = GameManager.get_detailed_hero_stats()
	var room = GameManager.get_current_room_info()
	var game_active = hero.has("name")

	# Update info display
	if hero.has("name") and room.has("x"):
		info_label.text = "Hero: %s | HP: %d/%d | Pillars: %d/4\nRoom: (%d, %d) | Type: %s" % [
			hero.name,
			hero.hp,
			hero.max_hp,
			hero.pillars_collected,
			room.get("x", 0),
			room.get("y", 0),
			room.get("type", "Unknown")
		]
	elif hero.has("name"):
		info_label.text = "Hero: %s | HP: %d/%d | Pillars: %d/4\nRoom: No room data" % [
			hero.name,
			hero.hp,
			hero.max_hp,
			hero.pillars_collected
		]
	else:
		info_label.text = "No active game - Start a new game first!"

	# Enable/disable buttons based on game state
	_update_button_states(game_active)

# HP Controls
# Note: Signals automatically update all views, no need to call update_display manually
func _on_hp_100_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("max_hp"):
		GameManager.debug_set_hero_hp(hero.max_hp)

func _on_hp_50_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("max_hp"):
		GameManager.debug_set_hero_hp(hero.max_hp / 2)

func _on_hp_25_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("max_hp"):
		GameManager.debug_set_hero_hp(hero.max_hp / 4)

func _on_hp_1_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_set_hero_hp(1)

func _on_hp_0_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_set_hero_hp(0)

func _on_damage_10_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_damage_hero(10)

func _on_heal_10_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_heal_hero(10)

# Pillar Controls
func _on_pillars_0_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_set_pillars(0)

func _on_pillars_2_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_set_pillars(2)

func _on_pillars_4_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_set_pillars(4)

# Item Controls
func _on_give_healing_potion_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_give_item("HealingPotion")

func _on_give_vision_potion_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_give_item("VisionPotion")

# Screen Navigation
func _on_goto_main_menu_pressed():
	get_tree().change_scene_to_file("res://View/MainMenu.tscn")

func _on_goto_hero_selection_pressed():
	get_tree().change_scene_to_file("res://View/HeroSelection.tscn")

func _on_goto_room_view_pressed():
	get_tree().change_scene_to_file("res://View/RoomView.tscn")

func _on_goto_inventory_pressed():
	get_tree().change_scene_to_file("res://View/InventoryView.tscn")

func _on_goto_combat_pressed():
	get_tree().change_scene_to_file("res://View/CombatView.tscn")

func _on_goto_death_pressed():
	get_tree().change_scene_to_file("res://View/GameOverView.tscn")

func _on_goto_win_pressed():
	get_tree().change_scene_to_file("res://View/WinScreen.tscn")

# Teleport
func _on_tp_exit_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_teleport_to_exit()

func _on_tp_entrance_pressed():
	var hero = GameManager.get_detailed_hero_stats()
	if hero.has("name"):
		GameManager.debug_teleport_to_entrance()

# Close button
func _on_close_pressed():
	hide_menu()

# Minimap toggle
func _on_minimap_toggle_pressed():
	GameManager.show_minimap_contents = !GameManager.show_minimap_contents
	_update_minimap_button_text()
	# Trigger minimap refresh
	GameManager.game_state_changed.emit()

func _update_minimap_button_text():
	if minimap_toggle != null:
		if GameManager.show_minimap_contents:
			minimap_toggle.text = "Hide Map Details"
		else:
			minimap_toggle.text = "Show Map Details"
