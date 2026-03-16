extends Camera3D

var lastDelta = 0
var moveSpeed = 2.0
var rotateSpeed = 1.5
var rotation_mode : bool = false
var rot_speed = Vector2(0,0)
var rot_new_pos = Vector2(0,0)
var rot_last_pos = Vector2(0,0)
var up_velocity = 0

func _ready() -> void:
	pass 
	
func _input(event: InputEvent) -> void:
	if event.is_action_pressed("wheel_down"):
		up_velocity += 0.1
		if (up_velocity < 0):
			up_velocity = 0
	elif event.is_action_pressed("wheel_up"):
		up_velocity -= 0.1
		if (up_velocity > 0):
			up_velocity = 0
		
	
func _process(delta: float) -> void:
	if Input.is_action_pressed("camera_move_right"):
		position += global_transform.basis.x.normalized() * delta * (moveSpeed + global_position.y) #Vector3(1.0 * delta, 0.0, 0.0)
	if Input.is_action_pressed("camera_backwards"):
		var bk : Vector3 = global_transform.basis.z.normalized() * delta * (moveSpeed + global_position.y) 
		bk.y = 0
		position += bk
	if Input.is_action_pressed("camera_move_left"):
		position += global_transform.basis.x.normalized() * delta * -1 * (moveSpeed + global_position.y)
	if Input.is_action_pressed("camera_forward"):
		var fwd : Vector3 = global_transform.basis.z.normalized() * delta * -1 * (moveSpeed + global_position.y)
		fwd.y = 0
		position += fwd
		
	if Input.is_action_pressed("camera_pan_left"):
		rotation += Vector3(0.0, delta*rotateSpeed, 0.0)
	elif Input.is_action_pressed("camera_pan_right"):
		rotation += Vector3(0.0, -1.0*delta*rotateSpeed, 0.0)
	if Input.is_action_pressed("camera_up"):
		if global_position.y < 3.0:
			position += global_transform.basis.y.normalized() * delta * moveSpeed
	elif Input.is_action_pressed("camera_down"):
		if global_position.y > 0.5:
			position += global_transform.basis.y.normalized() * delta * moveSpeed * -1.0
	
	# camera mouse 
	if (up_velocity <= -0.1):	
		if global_position.y < 3.0:
			position += global_transform.basis.y.normalized() * delta * moveSpeed
			up_velocity += delta * moveSpeed
	elif(up_velocity >= 0.1):
		if global_position.y > 0.5:
			position -= global_transform.basis.y.normalized() * delta * moveSpeed
			up_velocity -= delta * moveSpeed
	# ##
	# #
	if Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
		rotation_mode = true
		if rot_last_pos == null:
			get_viewport().get_mouse_position()
	else:
		rot_last_pos = null
		rotation_mode = false
	if rotation_mode: 
		rot_new_pos = get_viewport().get_mouse_position()
		if rot_last_pos != null:
			rot_speed = rot_new_pos - rot_last_pos
			rotation.y -= (rot_speed[0] * delta * 0.1)
			rotation.x -= (rot_speed[1] * delta * 0.05)
		rot_last_pos = rot_new_pos
	####
	
	pass
	
