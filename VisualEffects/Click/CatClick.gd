extends Node2D


@export_flags(
    &"MOUSE_BUTTON_MASK_LEFT:1",
    &"MOUSE_BUTTON_MASK_RIGHT:2",
    &"MOUSE_BUTTON_MASK_MIDDLE:4",
    &"MOUSE_BUTTON_MASK_MB_XBUTTON1:128",
    &"MOUSE_BUTTON_MASK_MB_XBUTTON2:256"
) var tracked_mask: int = 1


@export var animation_player: AnimationPlayer

@export var click_anim: StringName = &""

@export var release_anim: StringName = &""


var mouse_query: GlobalKeyStateCache = GlobalKeyStateCache.new()
var mouse_was_pressed: bool = false

var play_mouse_released: bool = false


func _process(_delta: float) -> void:
    global_position = get_global_mouse_position()

    mouse_query.UpdateButtonMaskCache(get_window())
    
    if mouse_query.GetMouseButtonMask() & tracked_mask:
        if !mouse_was_pressed and !click_anim.is_empty() and animation_player.current_animation != click_anim:
            animation_player.play(click_anim)
            pass
        mouse_was_pressed = true
        play_mouse_released = true
        pass
    else:
        if mouse_was_pressed:
            mouse_was_pressed = false
            play_mouse_released = true
            pass
        if play_mouse_released and !release_anim.is_empty() and animation_player.current_animation != release_anim:
            if !animation_player.is_playing():
                animation_player.play(release_anim)
                play_mouse_released = false
                pass
            pass
        pass
    pass
