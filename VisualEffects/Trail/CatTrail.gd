extends Node2D


## A [ParticleManager2D] for emitting left prints.
@export var left_print_renderer: LifetimeParticleManager2D

## The offset from the centre of the trail line for left prints.
## [br][br]
## The x and y components are tangential and orthogonal to the trail line, respectively.
@export var left_print_trail_offset: Vector2 = Vector2(0, -10)

## Additional rotation from the direction of the trail line for left prints.
@export_range(-180, 180, 1, &"radians_as_degrees", &"degrees") var left_additional_rotation: float = 0


## A [ParticleManager2D] for emitting right prints.
@export var right_print_renderer: LifetimeParticleManager2D

## The offset from the centre of the trail line for right prints.
## [br][br]
## The x and y components are tangential and orthogonal to the trail line, respectively.
@export var right_print_trail_offset: Vector2 = Vector2(0, 10)

## Additional rotation from the direction of the trail line for right prints.
@export_range(-180, 180, 1, &"radians_as_degrees", &"degrees") var right_additional_rotation: float = 0


## Flag to check if the next print should be a left or a right print.
var left_step: bool = true



## The amount of time (in seconds) a footprint will remain for.
@export_range(0, 10, 0.1, &"or_greater") var print_lifetime: float = 1

## The distance along the trail line between 2 successive footprints.
@export_range(1, 100, 1, &"or_greater") var per_step_distance: float = 26
## Variable to keep track of the additional distance to travel along the trail line
## before emitting an additional footprint.
var distance_to_next_step: float = 0


## The amount of time the mouse must not be moved for
## before the trail enters an idle state.
## [br][br]
## When the trail is in an idle state,
## the next movement will always emit a print starting from the old position.
@export_range(0, 10, 0.1, &"or_greater") var idle_delay: float = 1

## The amount of time the mouse must remain stationary for before it starts to idle.
## [br][br]
## When the trail is in an idle state,
## the next movement will always emit a print starting from the old position.
var time_to_idle: float = 0


## The previous position of the mouse in this node's local space.
var prev_local_mouse_pos: Vector2


func _ready() -> void:
    prev_local_mouse_pos = get_local_mouse_position()
    pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
    var local_mouse_pos: = get_local_mouse_position()
    if prev_local_mouse_pos != local_mouse_pos:
        time_to_idle = idle_delay

        if print_lifetime > 0:
            var move_vector: Vector2 = local_mouse_pos - prev_local_mouse_pos
            var move_distance: float = move_vector.length()

            if move_distance >= distance_to_next_step:
                var move_direction: = move_vector / move_distance
                var move_direction_angle: float = move_vector.angle()

                var left_print_transform: = Transform2D(
                    move_direction_angle + left_additional_rotation, Vector2.ZERO
                )
                var right_print_transform: = Transform2D(
                    move_direction_angle + right_additional_rotation, Vector2.ZERO
                )

                var left_print_offset: Vector2 = left_print_trail_offset.rotated(move_direction_angle)
                var right_print_offset: Vector2 = right_print_trail_offset.rotated(move_direction_angle)

                var step_count: int = ceil(move_distance / per_step_distance)
                var step_delta: float = delta / step_count
                var step_preheat: float = delta

                var current_pos: Vector2 = (
                    Vector2(prev_local_mouse_pos)
                    + move_direction * distance_to_next_step
                )
                var per_step_move: = move_direction * per_step_distance


                for i in range(0, step_count):
                    if left_step:
                        left_print_transform.origin = current_pos + left_print_offset
                        left_print_renderer.emit_particle(LifetimeParticleState2D.new(
                            left_print_transform, print_lifetime, step_preheat
                        ))
                        pass
                    else:
                        right_print_transform.origin = current_pos + right_print_offset
                        right_print_renderer.emit_particle(LifetimeParticleState2D.new(
                            right_print_transform, print_lifetime, step_preheat
                        ))
                        pass

                    left_step = not left_step
                    step_preheat -= step_delta
                    current_pos += per_step_move
                    pass
                distance_to_next_step = (step_count + 1) * per_step_distance - move_distance
                pass
            else:
                distance_to_next_step -= move_distance
                pass
        prev_local_mouse_pos = local_mouse_pos
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
    pass
