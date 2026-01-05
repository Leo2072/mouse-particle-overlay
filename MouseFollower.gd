extends GPUParticles2D


func _ready() -> void:
    global_position = get_global_mouse_position()
    pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
    var new_mouse_position: = get_global_mouse_position()
    if new_mouse_position != global_position:
        global_position = new_mouse_position
        emitting = true
        pass
    else:
        emitting = false
        pass
    
    pass
