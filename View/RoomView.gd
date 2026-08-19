extends Control

## Main game view showing current room and allowing movement through the dungeon.

@onready var hero_info = $VBoxContainer/HeroInfo
@onready var hero_hp_bar = $VBoxContainer/HeroHPContainer/HeroHPBar
@onready var hero_hp_label = $VBoxContainer/HeroHPContainer/HeroHPLabel
@onready var hero_skill = $VBoxContainer/HeroSkill
@onready var room_info = $VBoxContainer/RoomInfo
@onready var north_button = $VBoxContainer/MovementGrid/NorthButton
@onready var south_button = $VBoxContainer/MovementGrid/SouthButton
@onready var east_button = $VBoxContainer/MovementGrid/EastButton
@onready var west_button = $VBoxContainer/MovementGrid/WestButton
@onready var message_label = $VBoxContainer/MessageLabel
@onready var room_contents_label = $VBoxContainer/RoomContentsLabel
@onready var inventory_button = $VBoxContainer/InventoryButton
@onready var map_display = $VBoxContainer/MapDisplay

func _ready():
	update_display()
	# Connect to game state signals for reactive updates
	GameManager.hp_changed.connect(_on_state_changed)
	GameManager.pillars_changed.connect(_on_state_changed)
	GameManager.room_changed.connect(_on_state_changed)

func _on_state_changed():
	update_display()

	# Check for death when HP changes
	if GameManager.is_hero_dead():
		get_tree().change_scene_to_file("res://View/GameOverView.tscn")

## Updates all display elements with current game state.
func update_display():
	var hero = GameManager.get_detailed_hero_stats()
	var room = GameManager.get_current_room_info()

	# Update hero info with detailed stats
	if hero.has("name"):
		hero_info.text = "%s | Speed: %d | Hit: %.0f%% | Block: %.0f%% | Pillars: %d/4" % [
			hero.name,
			hero.attack_speed,
			hero.hit_chance * 100,
			hero.block_chance * 100,
			hero.pillars_collected
		]

		# Update HP bar and label
		hero_hp_bar.max_value = hero.max_hp
		hero_hp_bar.value = hero.hp
		hero_hp_label.text = "HP: %d/%d" % [hero.hp, hero.max_hp]

		# Color code HP bar based on percentage
		var hp_percent = float(hero.hp) / float(hero.max_hp)
		if hp_percent > 0.5:
			hero_hp_bar.modulate = Color(0, 1, 0)  # Green
		elif hp_percent > 0.25:
			hero_hp_bar.modulate = Color(1, 1, 0)  # Yellow
		else:
			hero_hp_bar.modulate = Color(1, 0, 0)  # Red

		# Display special skill
		if hero.has("special_skill"):
			hero_skill.text = "Special: " + hero.special_skill
		else:
			hero_skill.text = ""
	else:
		hero_info.text = "Hero: Not loaded"
		hero_skill.text = ""
		hero_hp_bar.value = 0
		hero_hp_label.text = "HP: ?/?"

	# Update room info
	if room.has("x"):
		var room_type = room.type
		room_info.text = "Location: (%d, %d) | Type: %s" % [
			room.x,
			room.y,
			room_type
		]

		# Update movement buttons based on walls
		north_button.disabled = room.north_wall
		south_button.disabled = room.south_wall
		east_button.disabled = room.east_wall
		west_button.disabled = room.west_wall

		# Show special messages for special rooms
		if room_type == "Entrance":
			message_label.text = "You are at the dungeon entrance."
		elif room_type == "Exit":
			message_label.text = "You found the exit! Collect all 4 pillars to win!"
		else:
			message_label.text = ""
	else:
		room_info.text = "Location: Unknown"

	# Update room contents
	var contents = GameManager.get_room_contents()
	_update_room_contents_display(contents)

	# Update debug map
	var map_string = GameManager.get_map_debug_string()
	map_display.text = "[code]" + map_string + "[/code]"

## Attempts to move in the specified direction and updates display.
func move_direction(direction: String):
	if GameManager.move_player(direction):
		message_label.text = "You moved %s." % direction.to_lower()
		update_display()

		# Check death condition
		if GameManager.is_hero_dead():
			get_tree().change_scene_to_file("res://View/GameOverView.tscn")
			return

		# Check win condition
		if GameManager.check_win_condition():
			get_tree().change_scene_to_file("res://View/WinScreen.tscn")
			return

		# Check for a monster encounter
		if GameManager.is_in_combat():
			get_tree().change_scene_to_file("res://View/CombatView.tscn")
	else:
		message_label.text = "Cannot move %s - blocked by wall!" % direction.to_lower()

## Handles North button press.
func _on_north_pressed():
	move_direction("North")

## Handles South button press.
func _on_south_pressed():
	move_direction("South")

## Handles East button press.
func _on_east_pressed():
	move_direction("East")

## Handles West button press.
func _on_west_pressed():
	move_direction("West")

## Updates the room contents display based on what's in the room.
func _update_room_contents_display(contents: Dictionary):
	if not contents.has("has_content") or not contents.has_content:
		room_contents_label.text = ""
		return

	var content_text = "Room contains:\n"

	# Display items
	if contents.has("items") and contents.items.size() > 0:
		content_text += "  Items: "
		for item in contents.items:
			content_text += item + ", "
		content_text = content_text.substr(0, content_text.length() - 2) + "\n"

	# Display monsters
	if contents.has("monsters") and contents.monsters.size() > 0:
		content_text += "  Monsters: "
		for monster in contents.monsters:
			content_text += monster + ", "
		content_text = content_text.substr(0, content_text.length() - 2) + "\n"

	# Display pillars
	if contents.has("pillars") and contents.pillars.size() > 0:
		content_text += "  Pillars: "
		for pillar in contents.pillars:
			content_text += pillar + ", "
		content_text = content_text.substr(0, content_text.length() - 2) + "\n"

	# Display pit warning (no physical trigger yet, so just flag it)
	if contents.has("has_pit") and contents.has_pit:
		content_text += "  ⚠ There's a pit somewhere in this room!\n"

	room_contents_label.text = content_text

## Handles Inventory button press.
func _on_inventory_pressed():
	get_tree().change_scene_to_file("res://View/InventoryView.tscn")
