extends Control

## Main game view showing current room and allowing movement through the dungeon.
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

func _ready():
	add_child(wall_container)

	

	_rebuild_room_walls()

	# Connect to game state signals for reactive updates
	GameManager.hp_changed.connect(_on_state_changed)
	GameManager.pillars_changed.connect(_on_state_changed)
	GameManager.room_changed.connect(_on_state_changed)

func _on_state_changed():
	_rebuild_room_walls()

	# Always snap the player back inside the new room's bounds as a
	# safe default. If they entered through a doorway, _on_exit_triggered
	# will immediately override this with a more precise edge position.
	var character = hero_movement.get_node("CharacterBody2D")
	character.position = ROOM_CENTER

	# Check for death when HP changes
	if GameManager.is_hero_dead():
		get_tree().change_scene_to_file("res://View/GameOverView.tscn")
		return

	# Check win condition when pillars change
	if GameManager.check_win_condition():
		get_tree().change_scene_to_file("res://View/WinScreen.tscn")
		return

	# Note: Combat is checked in movement functions (_on_exit_triggered and move_direction)
	# not here, since combat only triggers on room entry via movement

#Clears and remakes walls for current room
func _rebuild_room_walls():
	# Disconnect signals and free children immediately to prevent memory leak
	for child in wall_container.get_children():
		# If it's an Area2D exit trigger, disconnect the signal first
		if child is Area2D and child.body_entered.is_connected(_on_exit_triggered):
			# Can't disconnect with bound parameters, so just free it
			# The signal will be automatically disconnected on free
			pass
		child.queue_free()  # Immediate queue, no deferral needed

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

#invisible trigger zone
func _maybe_add_exit(is_open: bool, direction_name: String, pos: Vector2, size: Vector2) -> void:
	if not is_open:
		return

	var area := Area2D.new()
	area.position = pos

	var shape := CollisionShape2D.new()
	var rect := RectangleShape2D.new()
	rect.size = size
	shape.shape = rect
	area.add_child(shape)

	area.body_entered.connect(_on_exit_triggered.bind(direction_name))

	wall_container.call_deferred("add_child", area)

#Called when player enters an exit trigger zone
func _on_exit_triggered(body: Node2D, direction_name: String) -> void:
	if body != hero_movement.get_node("CharacterBody2D"):
		return

	if GameManager.move_player(direction_name):
		# Check death condition (same as button movement)
		if GameManager.is_hero_dead():
			get_tree().change_scene_to_file("res://View/GameOverView.tscn")
			return

		# Check win condition (same as button movement)
		if GameManager.check_win_condition():
			get_tree().change_scene_to_file("res://View/WinScreen.tscn")
			return

		# Check for combat encounter (was missing from real-time movement!)
		if GameManager.is_in_combat():
			get_tree().change_scene_to_file("res://View/CombatView.tscn")
			return

		# move_player() emits room_changed, which already ran
		# _on_state_changed() and snapped us to ROOM_CENTER above.
		# Now refine that to the correct edge for a smooth doorway feel.
		_reposition_player_for_entry(direction_name)

#moves player to edge of new room
func _reposition_player_for_entry(exited_direction: String) -> void:
	var character = hero_movement.get_node("CharacterBody2D")
	var half = ROOM_SIZE / 2

	match exited_direction:
		"North":
			character.position = ROOM_CENTER + Vector2(0, half.y - 40)
		"South":
			character.position = ROOM_CENTER + Vector2(0, -half.y + 40)
		"East":
			character.position = ROOM_CENTER + Vector2(-half.x + 40, 0)
		"West":
			character.position = ROOM_CENTER + Vector2(half.x - 40, 0)

#Creates one wall segment
func _maybe_add_wall(is_wall: bool, pos: Vector2, size: Vector2) -> void:
	if not is_wall:
		return

	var body := StaticBody2D.new()
	body.position = pos

	var shape := CollisionShape2D.new()
	var rect := RectangleShape2D.new()
	rect.size = size
	shape.shape = rect
	body.add_child(shape)

	var visual := ColorRect.new()
	visual.size = size
	visual.position = -size / 2  # center on the body's position
	visual.color = Color(0.35, 0.22, 0.12)  # placeholder wall color
	body.add_child(visual)

	wall_container.call_deferred("add_child", body)
