extends Control

## Main game view showing current room and allowing movement through the dungeon.

# DEBUG: File logging for crash diagnosis
var log_file: FileAccess = null
var rebuild_counter = 0
var exit_trigger_counter = 0

func _log(message: String):
	if log_file == null:
		log_file = FileAccess.open("user://crash_debug.log", FileAccess.WRITE)
	if log_file:
		log_file.store_line(str(Time.get_ticks_msec()) + ": " + message)
		log_file.flush()  # Force write immediately
	print(message)  # Also print to console

	# Detect infinite loops
	if "REBUILD_START" in message:
		rebuild_counter += 1
		if rebuild_counter > 100:
			_log("CRITICAL: INFINITE REBUILD LOOP DETECTED!")
			get_tree().quit()
	elif "EXIT_TRIGGER_START" in message:
		exit_trigger_counter += 1
		if exit_trigger_counter > 100:
			_log("CRITICAL: INFINITE EXIT TRIGGER LOOP DETECTED!")
			get_tree().quit()
## Display (HP, stats, room info, minimap, messages) is now handled independently
## by HUD.gd — this script only owns the world: walls, movement, and transitions.
##
## MOVEMENT SYSTEM:
## Currently supports DUAL movement modes (transitioning to real-time only):
##   1. Real-time (WASD) - HeroMovement.gd with CharacterBody2D collision
##      - Exit triggers (_on_exit_triggered) detect room transitions
##      - Checks: death, win, combat after each room entry
##   2. Button-based (N/S/E/W buttons) - move_direction() function
##      - Direct GameManager.move_player() calls
##      - Same checks: death, win, combat
## FUTURE: Phase out button movement, keep only real-time WASD movement

@onready var hero_movement = $HeroMovement

#Wall/room setup
var wall_container: Node2D = Node2D.new()
const ROOM_CENTER := Vector2(550, 335)
const ROOM_SIZE := Vector2(900, 380)
const WALL_THICKNESS := 20.0
const DOOR_WIDTH := 100.0

# Prevent rapid-fire room transitions (double-movement bug fix)
var transition_locked := false
const TRANSITION_LOCK_TIME := 0.3  # Seconds to lock after transition

# Prevent overlapping rebuilds (memory leak fix)
var rebuild_in_progress := false
var last_rebuild_time := 0.0

# Timer cleanup to prevent memory leak
var transition_timer: SceneTreeTimer = null

func _ready():
	add_child(wall_container)

	# Add pause menu
	var pause_menu_scene = load("res://View/PauseMenu.tscn")
	var pause_menu = pause_menu_scene.instantiate()
	add_child(pause_menu)

	_rebuild_room_walls()

	# Connect to game state signals for reactive updates
	# NOTE: Only connect to room_changed and pillars_changed, NOT hp_changed
	# hp_changed should not trigger player repositioning or wall rebuilds
	GameManager.pillars_changed.connect(_on_state_changed)
	GameManager.room_changed.connect(_on_state_changed)

# CRITICAL: Disconnect signals when scene unloads to prevent memory leak
func _exit_tree():
	_log("EXIT_TREE: Cleaning up RoomView")

	# Clean up timer connection (CRITICAL - prevents memory leak)
	if transition_timer != null and transition_timer.timeout.is_connected(_unlock_transition):
		transition_timer.timeout.disconnect(_unlock_transition)
		transition_timer = null

	# Close log file
	if log_file != null:
		log_file.close()
		log_file = null

	# Disconnect signals
	if GameManager.pillars_changed.is_connected(_on_state_changed):
		GameManager.pillars_changed.disconnect(_on_state_changed)
	if GameManager.room_changed.is_connected(_on_state_changed):
		GameManager.room_changed.disconnect(_on_state_changed)

	_log("EXIT_TREE: Complete")

func _input(event):
	# Open inventory with I key
	if event is InputEventKey and event.pressed and event.keycode == KEY_I:
		get_tree().change_scene_to_file("res://View/InventoryView.tscn")

func _on_state_changed():
	_log("STATE_CHANGED: centering player")

	# Snap player to center (safe during physics callback)
	var hero_node = get_node_or_null("HeroMovement")
	if hero_node != null:
		var character = hero_node.get_node_or_null("CharacterBody2D")
		if character != null:
			character.position = ROOM_CENTER
			_log("STATE_CHANGED: player centered at " + str(ROOM_CENTER))
		else:
			_log("STATE_CHANGED: ERROR - CharacterBody2D not found")
	else:
		_log("STATE_CHANGED: ERROR - HeroMovement not found")

	# Defer wall rebuild to avoid physics callback conflicts
	_log("STATE_CHANGED: requesting deferred rebuild")
	call_deferred("_rebuild_room_walls")
	_log("STATE_CHANGED: deferred rebuild queued")

	# Check for death when HP changes
	_log("STATE_CHANGED: checking death")
	if GameManager.is_hero_dead():
		get_tree().call_deferred("change_scene_to_file", "res://View/GameOverView.tscn")
		return

	# Check win condition when pillars change
	_log("STATE_CHANGED: checking win")
	if GameManager.check_win_condition():
		get_tree().call_deferred("change_scene_to_file", "res://View/WinScreen.tscn")
		return

	_log("STATE_CHANGED: complete")

	# Note: Combat is checked in movement functions (_on_exit_triggered and move_direction)
	# not here, since combat only triggers on room entry via movement

#Clears and remakes walls for current room
func _rebuild_room_walls():
	_log("REBUILD_ENTRY: Called")

	# CRITICAL: Guard against deferred call on freed scene
	if not is_inside_tree():
		_log("REBUILD_ABORTED: Scene not in tree (freed during transition)")
		return

	# Throttle rebuilds to prevent event loop overload
	var current_time = Time.get_ticks_msec() / 1000.0
	if current_time - last_rebuild_time < 0.05:  # Min 50ms between rebuilds
		_log("REBUILD_ABORTED: too soon (throttled)")
		return
	last_rebuild_time = current_time

	# CRITICAL: Prevent overlapping rebuilds that cause memory leak
	if rebuild_in_progress:
		_log("REBUILD_ABORTED: rebuild already in progress")
		return

	rebuild_in_progress = true

	# CRITICAL: Properly cleanup old nodes to prevent signal leak crash
	# This function is called via call_deferred, so we can safely modify physics objects
	var child_count = wall_container.get_child_count()
	_log("REBUILD_START: cleaning " + str(child_count) + " nodes")

	for child in wall_container.get_children():
		# Disable Area2D monitoring to stop signals IMMEDIATELY (safe because deferred)
		if child is Area2D:
			child.monitoring = false
			child.monitorable = false
		# Remove from tree and FREE IMMEDIATELY (not queue) to prevent memory leak
		wall_container.remove_child(child)
		child.free()  # FREE NOW - we're in deferred context so it's safe

	_log("REBUILD_CLEANUP_DONE: building new walls")

	var room = GameManager.get_current_room_info()
	if not room.has("north_wall"):
		return

	var half = ROOM_SIZE / 2
	var h_segment_len = (ROOM_SIZE.x - DOOR_WIDTH) / 2.0
	var v_segment_len = (ROOM_SIZE.y - DOOR_WIDTH) / 2.0

	#NORTH
	if room.north_wall:
		_maybe_add_wall(true, ROOM_CENTER + Vector2(0, -half.y), Vector2(ROOM_SIZE.x, WALL_THICKNESS))
	else:
		var seg_offset = DOOR_WIDTH / 2.0 + h_segment_len / 2.0
		_maybe_add_wall(true, ROOM_CENTER + Vector2(-seg_offset, -half.y), Vector2(h_segment_len, WALL_THICKNESS))
		_maybe_add_wall(true, ROOM_CENTER + Vector2(seg_offset, -half.y), Vector2(h_segment_len, WALL_THICKNESS))
		_maybe_add_exit(true, "North", ROOM_CENTER + Vector2(0, -half.y - 20), Vector2(DOOR_WIDTH, 40))

	#SOUTH
	if room.south_wall:
		_maybe_add_wall(true, ROOM_CENTER + Vector2(0, half.y), Vector2(ROOM_SIZE.x, WALL_THICKNESS))
	else:
		var seg_offset = DOOR_WIDTH / 2.0 + h_segment_len / 2.0
		_maybe_add_wall(true, ROOM_CENTER + Vector2(-seg_offset, half.y), Vector2(h_segment_len, WALL_THICKNESS))
		_maybe_add_wall(true, ROOM_CENTER + Vector2(seg_offset, half.y), Vector2(h_segment_len, WALL_THICKNESS))
		_maybe_add_exit(true, "South", ROOM_CENTER + Vector2(0, half.y + 20), Vector2(DOOR_WIDTH, 40))

	#EAST
	if room.east_wall:
		_maybe_add_wall(true, ROOM_CENTER + Vector2(half.x, 0), Vector2(WALL_THICKNESS, ROOM_SIZE.y))
	else:
		var seg_offset = DOOR_WIDTH / 2.0 + v_segment_len / 2.0
		_maybe_add_wall(true, ROOM_CENTER + Vector2(half.x, -seg_offset), Vector2(WALL_THICKNESS, v_segment_len))
		_maybe_add_wall(true, ROOM_CENTER + Vector2(half.x, seg_offset), Vector2(WALL_THICKNESS, v_segment_len))
		_maybe_add_exit(true, "East", ROOM_CENTER + Vector2(half.x + 20, 0), Vector2(40, DOOR_WIDTH))

	#WEST
	if room.west_wall:
		_maybe_add_wall(true, ROOM_CENTER + Vector2(-half.x, 0), Vector2(WALL_THICKNESS, ROOM_SIZE.y))
	else:
		var seg_offset = DOOR_WIDTH / 2.0 + v_segment_len / 2.0
		_maybe_add_wall(true, ROOM_CENTER + Vector2(-half.x, -seg_offset), Vector2(WALL_THICKNESS, v_segment_len))
		_maybe_add_wall(true, ROOM_CENTER + Vector2(-half.x, seg_offset), Vector2(WALL_THICKNESS, v_segment_len))
		_maybe_add_exit(true, "West", ROOM_CENTER + Vector2(-half.x - 20, 0), Vector2(40, DOOR_WIDTH))

	rebuild_in_progress = false
	_log("REBUILD_COMPLETE: all walls built")

#invisible trigger zone
func _maybe_add_exit(is_open: bool, direction_name: String, pos: Vector2, size_param: Vector2) -> void:
	if not is_open:
		return

	var area := Area2D.new()
	area.position = pos
	area.monitoring = false  # Start disabled to prevent immediate collision

	# CRITICAL FIX: Only detect player on layer 1, ignore walls/other bodies
	area.collision_layer = 0  # Don't be on any layer
	area.collision_mask = 1   # Only detect layer 1 (player)

	var shape := CollisionShape2D.new()
	var rect := RectangleShape2D.new()
	rect.size = size_param
	shape.shape = rect
	area.add_child(shape)

	area.body_entered.connect(_on_exit_triggered.bind(direction_name))

	# No need to defer - _rebuild_room_walls is already deferred
	wall_container.add_child(area)

	# Enable monitoring immediately - we're already in deferred context
	area.monitoring = true

#Called when player enters an exit trigger zone
func _on_exit_triggered(body: Node2D, direction_name: String) -> void:
	_log("EXIT_TRIGGER_START: " + direction_name)

	# Check if this is the player character
	var hero_node = get_node_or_null("HeroMovement")
	if hero_node == null:
		_log("EXIT_TRIGGER_IGNORED: HeroMovement not found")
		return

	var player_char = hero_node.get_node_or_null("CharacterBody2D")
	if player_char == null or body != player_char:
		_log("EXIT_TRIGGER_IGNORED: wrong body")
		return

	# Prevent rapid-fire transitions that cause room skipping
	if transition_locked:
		_log("EXIT_TRIGGER_LOCKED: transition in progress")
		return

	_log("EXIT_TRIGGER_ATTEMPTING_MOVE: " + direction_name)

	if GameManager.move_player(direction_name):
		# Lock transitions to prevent double-movement
		transition_locked = true
		_log("EXIT_TRIGGER_MOVE_SUCCESS: locked for " + str(TRANSITION_LOCK_TIME))

		# Cancel previous timer to prevent memory leak
		if transition_timer != null and transition_timer.timeout.is_connected(_unlock_transition):
			transition_timer.timeout.disconnect(_unlock_transition)

		transition_timer = get_tree().create_timer(TRANSITION_LOCK_TIME)
		transition_timer.timeout.connect(_unlock_transition)

		_log("EXIT_TRIGGER_CHECKING_DEATH")
		# Check death condition (same as button movement)
		if GameManager.is_hero_dead():
			_log("EXIT_TRIGGER_DEATH_DETECTED: changing to GameOver")
			# MUST defer scene change - we're in a physics callback!
			get_tree().call_deferred("change_scene_to_file", "res://View/GameOverView.tscn")
			return

		_log("EXIT_TRIGGER_CHECKING_WIN")
		# Check win condition (same as button movement)
		if GameManager.check_win_condition():
			_log("EXIT_TRIGGER_WIN_DETECTED: changing to WinScreen")
			# MUST defer scene change - we're in a physics callback!
			get_tree().call_deferred("change_scene_to_file", "res://View/WinScreen.tscn")
			return

		_log("EXIT_TRIGGER_CHECKING_COMBAT")
		# Check for combat encounter (was missing from real-time movement!)
		if GameManager.is_in_combat():
			_log("EXIT_TRIGGER_COMBAT_DETECTED: changing to Combat")
			# MUST defer scene change - we're in a physics callback!
			get_tree().call_deferred("change_scene_to_file", "res://View/CombatView.tscn")
			return

		_log("EXIT_TRIGGER_REPOSITIONING: " + direction_name)
		# move_player() emits room_changed, which already ran
		# _on_state_changed() and snapped us to ROOM_CENTER above.
		# Now refine that to the correct edge for a smooth doorway feel.
		_reposition_player_for_entry(direction_name)
		_log("EXIT_TRIGGER_COMPLETE")

#moves player to edge of new room
func _reposition_player_for_entry(exited_direction: String) -> void:
	_log("REPOSITION_START: " + exited_direction)

	# Robust node lookup - @onready might be stale
	var hero_node = get_node_or_null("HeroMovement")
	if hero_node == null:
		_log("REPOSITION_ERROR: HeroMovement node not found!")
		return

	var character = hero_node.get_node_or_null("CharacterBody2D")
	if character == null:
		_log("REPOSITION_ERROR: CharacterBody2D not found!")
		return

	var half = ROOM_SIZE / 2
	var new_pos: Vector2

	match exited_direction:
		"North":
			new_pos = ROOM_CENTER + Vector2(0, half.y - 40)
		"South":
			new_pos = ROOM_CENTER + Vector2(0, -half.y + 40)
		"East":
			new_pos = ROOM_CENTER + Vector2(-half.x + 40, 0)
		"West":
			new_pos = ROOM_CENTER + Vector2(half.x - 40, 0)
		_:
			new_pos = ROOM_CENTER

	character.position = new_pos
	_log("REPOSITION_COMPLETE: " + str(new_pos))

#Unlocks room transitions after cooldown
func _unlock_transition() -> void:
	transition_locked = false
	print("[RoomView] Transition unlocked")

#Creates one wall segment
func _maybe_add_wall(is_wall: bool, pos: Vector2, size_param: Vector2) -> void:
	if not is_wall:
		return

	var body := StaticBody2D.new()
	body.position = pos

	# Set collision layers: walls on layer 2, collide with layer 1 (player)
	body.collision_layer = 2
	body.collision_mask = 1

	var shape := CollisionShape2D.new()
	var rect := RectangleShape2D.new()
	rect.size = size_param
	shape.shape = rect
	body.add_child(shape)

	var visual := ColorRect.new()
	visual.size = size_param
	visual.position = -size_param / 2  # center on the body's position
	visual.color = Color(0.35, 0.22, 0.12)  # placeholder wall color
	body.add_child(visual)

	# No need to defer - _rebuild_room_walls is already deferred
	wall_container.add_child(body)
