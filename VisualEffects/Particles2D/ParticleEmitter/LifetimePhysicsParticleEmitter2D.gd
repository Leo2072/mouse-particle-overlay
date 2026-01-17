extends LifetimeParticleEmitter2D
class_name LifetimePhysicsParticleEmitter2D


@export_range(-360, 360, 0.1, &"radians_as_degrees", &"degrees") var min_emission_angle: float = 0
@export_range(-360, 360, 0.1, &"radians_as_degrees", &"degrees") var max_emission_angle: float = 0

@export var use_emission_angle_for_rotation: bool = true
@export_range(-180, 180, 0.1, &"radians_as_degrees", &"degrees") var emission_to_rotation_offset: float = PI/2

@export_range(-360, 360, 0.1, &"radians_as_degrees", &"degrees") var min_initial_angle: float = 0
@export_range(-360, 360, 0.1, &"radians_as_degrees", &"degrees") var max_initial_angle: float = 0


@export_range(0, 100, 0.1, &"or_greater") var min_initial_linear_speed: float = 0
@export_range(0, 100, 0.1, &"or_greater") var max_initial_linear_speed: float = 0


@export_range(-3600, 3600, 0.1, &"or_greater", &"or_less", &"radians_as_degrees", &"degrees")
var min_initial_angular_velocity: float = 0

@export_range(-3600, 3600, 0.1, &"or_greater", &"or_less", &"radians_as_degrees", &"degrees")
var max_initial_angular_velocity: float = 0


@export var linear_acceleration: Vector2 = Vector2.ZERO

@export_range(-PI * 20, PI * 20, 0.1, &"or_greater", &"radians_as_degrees", &"degrees")
var angular_acceleration: float = 0


@export_range(0, 10, 0.1, &"or_greater") var linear_drag: float = 0
@export_range(0, 10, 0.1, &"or_greater") var angular_drag: float = 0


func _ready() -> void:
    randomize()
    pass


func validate_particle_manager(value: ParticleManager2D) -> bool:
    return value is LifetimeParticleManager2D


func emit() -> void:
    var emission_angle: = randf_range(min_emission_angle, max_emission_angle)

    var initial_linear_velocity: = Vector2.from_angle(emission_angle) * (
        randf_range(min_initial_linear_speed, max_initial_linear_speed)
    )

    var initial_transform: Transform2D
    if use_emission_angle_for_rotation:
        initial_transform = transform.rotated(emission_angle + emission_to_rotation_offset)
        pass
    else:
        initial_transform = transform.rotated(randf_range(min_initial_angle, max_initial_angle))
        pass
    initial_transform.origin = transform.origin

    var initial_angular_velocity: = randf_range(min_initial_angular_velocity, max_initial_angular_velocity)

    particle_manager.emit_particle(
        LifetimePhysicsParticleState2D.new(
            initial_transform, particle_lifetime, particle_preheat_lifetime,
            initial_linear_velocity, initial_angular_velocity,
            linear_acceleration, angular_acceleration,
            linear_drag, angular_drag
        )
    )
    pass