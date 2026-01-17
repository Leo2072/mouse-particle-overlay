extends Node2D


func _ready() -> void:
    global_position = get_global_mouse_position()
    pass


func _process(_delta: float) -> void:
    var new_mouse_position: = get_global_mouse_position()
    if global_position != new_mouse_position:
        global_position = new_mouse_position
        pass
    pass
