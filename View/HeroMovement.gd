extends Node

# Real-time movement input and wall collision

@onready var body: CharacterBody2D = $CharacterBody2D
@export var speed: float = 250.0

func _ready():
	# Set collision layers to match wall system
	# Player on layer 1, collides with layer 2 (walls)
	body.collision_layer = 1
	body.collision_mask = 2

#Character starts at no speed, physics runs on time step
func _physics_process(_delta: float) -> void:
	var input_vector := Vector2.ZERO
	input_vector.x = Input.get_axis("ui_left", "ui_right")
	input_vector.y = Input.get_axis("ui_up", "ui_down")
	input_vector = input_vector.normalized()

#Sets velocity based on direction + speed 
	body.velocity = input_vector * speed
	body.move_and_slide()
