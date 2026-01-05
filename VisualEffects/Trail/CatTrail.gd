extends Node2D


@export var left_print: Texture2D
@export var left_print_size: Vector2 = Vector2(32, 32)
@export var left_print_relative_pivot: Vector2 = Vector2(0.5, 0.5)
@export_range(-180, 180, 1, &"radians_as_degrees", &"degrees") var left_additional_rotation: float = 0

@export var right_print: Texture2D
@export var right_print_size: Vector2 = Vector2(32, 32)
@export var right_print_relative_pivot: Vector2 = Vector2(0.5, 0.5)
@export_range(-180, 180, 1, &"radians_as_degrees", &"degrees") var right_additional_rotation: float = 0

@export_range(0, 10, 0.1, &"or_greater") var print_initial_lifetime: float = 3

@export_range(1, 100, 1, &"or_greater") var per_step_distance: float = 26

@export var left_print_screen_relative_offset: Vector2 = Vector2(0, -10)

@export var right_print_screen_relative_offset: Vector2 = Vector2(0, 10)


@export_range(0, 10, 0.1, &"or_greater") var idle_delay: float = 1


var screen_print_isleft: PackedByteArray = []
var screen_print_positions: PackedVector2Array = []
var screen_print_rotations: PackedFloat32Array = []
var screen_print_initial_lifetime: PackedFloat32Array = []
var screen_print_lifetime: PackedFloat32Array = []

var time_to_idle: float = 0

var distance_to_next_step: float = 0


var prev_mouse_screen_pos: Vector2i


func _ready() -> void:
    prev_mouse_screen_pos = DisplayServer.mouse_get_position()
    pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
    var last_dirty_index: int = -1
    for i in range(0, screen_print_positions.size()):
        screen_print_lifetime[i] -= delta
        if screen_print_lifetime[i] <= 0:
            last_dirty_index = i
            pass
        pass

    if last_dirty_index >= 0:
        screen_print_isleft = screen_print_isleft.slice(last_dirty_index)
        screen_print_positions = screen_print_positions.slice(last_dirty_index)
        screen_print_rotations = screen_print_rotations.slice(last_dirty_index)
        screen_print_initial_lifetime = screen_print_initial_lifetime.slice(last_dirty_index)
        screen_print_lifetime = screen_print_lifetime.slice(last_dirty_index)
        pass

    var mouse_screen_pos: Vector2i = DisplayServer.mouse_get_position()
    if prev_mouse_screen_pos != mouse_screen_pos:
        time_to_idle = idle_delay

        if print_initial_lifetime > 0:
            var move_vector: Vector2 = mouse_screen_pos - prev_mouse_screen_pos
            var move_distance: float = move_vector.length()

            if move_distance >= distance_to_next_step:
                var move_direction: = move_vector / move_distance
                var move_direction_angle: float = move_vector.angle()
                var left_print_screen_offset: Vector2 = left_print_screen_relative_offset.rotated(move_direction_angle)
                var right_print_screen_offset: Vector2 = right_print_screen_relative_offset.rotated(move_direction_angle)

                var step_count: int = ceil(move_distance / per_step_distance)
                var step_delta: float = delta / step_count
                var current_step_lifetime: float = print_initial_lifetime - delta

                var left_step: bool = true
                var current_print_count: int = screen_print_isleft.size()
                if current_print_count > 0:
                    left_step = not screen_print_isleft[current_print_count - 1]
                    pass
                
                var current_pos: Vector2 = (
                    Vector2(prev_mouse_screen_pos)
                    + move_direction * distance_to_next_step
                )
                var per_step_move: = move_direction * per_step_distance

                for i in range(0, step_count):
                    screen_print_isleft.append(left_step)
                    if left_step:
                        screen_print_positions.append(current_pos + left_print_screen_offset)
                        pass
                    else:
                        screen_print_positions.append(current_pos + right_print_screen_offset)
                        pass
                    screen_print_rotations.append(move_direction_angle)
                    screen_print_initial_lifetime.append(print_initial_lifetime)
                    screen_print_lifetime.append(current_step_lifetime)

                    left_step = not left_step
                    current_step_lifetime += step_delta
                    current_pos += per_step_move
                    pass
                distance_to_next_step = (step_count + 1) * per_step_distance - move_distance
                pass
            else:
                distance_to_next_step -= move_distance
                pass
        prev_mouse_screen_pos = mouse_screen_pos
        pass
    elif time_to_idle > 0:
        if time_to_idle <= delta:
            time_to_idle = 0
            distance_to_next_step = 0
            pass
        else:
            time_to_idle -= delta
            pass
        pass
    else:
        distance_to_next_step = 0;
        pass
    
    queue_redraw()
    pass


func _draw() -> void:
    if screen_print_positions.size() == 0:
        return
    
    var local_to_screen: = (get_viewport_transform() * get_global_transform()).translated(-get_window().position)

    var left_print_rect: = Rect2(-left_print_size * left_print_relative_pivot, left_print_size)
    var right_print_rect: = Rect2(-right_print_size * right_print_relative_pivot, right_print_size)


    for i in range(0, screen_print_positions.size()):
        if screen_print_isleft[i]:
            var footprint_transform: = (
                local_to_screen
                .translated(screen_print_positions[i])
                .rotated_local(screen_print_rotations[i] + left_additional_rotation)
            )
            draw_set_transform_matrix(footprint_transform)

            draw_texture_rect(
                left_print, left_print_rect, false,
                Color(1, 1, 1, screen_print_lifetime[i] / screen_print_initial_lifetime[i])
            )
        else:
            var footprint_transform: = (
                local_to_screen
                .translated(screen_print_positions[i])
                .rotated_local(screen_print_rotations[i] + right_additional_rotation)
            )
            draw_set_transform_matrix(footprint_transform)

            draw_texture_rect(
                right_print, right_print_rect, false,
                Color(1, 1, 1, screen_print_lifetime[i] / screen_print_initial_lifetime[i])
            )
        pass
    pass
