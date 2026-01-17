extends Node2D


@export_flags(
    &"MOUSE_BUTTON_MASK_LEFT:1",
    &"MOUSE_BUTTON_MASK_RIGHT:2",
    &"MOUSE_BUTTON_MASK_MIDDLE:4",
    &"MOUSE_BUTTON_MASK_MB_XBUTTON1:128",
    &"MOUSE_BUTTON_MASK_MB_XBUTTON2:256"
) var tracked_mask: int = 3


@export var hold_icon: Node2D


@export var pressed_particles_emitter: ParticleEmitter2D
@export_range(0, 10, 1, &"or_greater") var particles_per_press: int = 10


@export var released_particles_emitter: ParticleEmitter2D
@export_range(0, 10, 1, &"or_greater") var particles_per_release: int = 10


var mouse_query: GlobalKeyStateCache = GlobalKeyStateCache.new()
var mouse_was_pressed: bool = false


func _process(_delta: float) -> void:
    mouse_query.UpdateButtonMaskCache(get_window())
    
    if mouse_query.GetMouseButtonMask() & tracked_mask:
        if !mouse_was_pressed:
            if pressed_particles_emitter:
                for i in range(0, particles_per_press):
                    pressed_particles_emitter.emit()
                    pass
                pass
            hold_icon.visible = true
            mouse_was_pressed = true
            pass
        pass
    else:
        if mouse_was_pressed:
            if released_particles_emitter:
                for i in range(0, particles_per_release):
                    released_particles_emitter.emit()
                    pass
                pass
            hold_icon.visible = false
            mouse_was_pressed = false
            pass
        pass
    pass
