extends LifetimeParticleState2D
class_name LifetimePhysicsParticleState2D


var linear_velocity: Vector2
var angular_velocity: float
var linear_accel: Vector2
var angular_accel: float
var linear_drag: float
var angular_drag: float


func _init
(
    particle_transform: Transform2D = Transform2D.IDENTITY,
    particle_lifetime: float = 0,
    preheat_lifetime: float = 0,
    particle_linear_velocity: Vector2 = Vector2.ZERO,
    particle_angular_velocity: float = 0,
    particle_linear_acceleration: Vector2 = Vector2.ZERO,
    particle_angular_acceleration: float = 0,
    particle_linear_drag: float = 0,
    particle_angular_drag: float = 0,

) -> void:
    super._init(particle_transform, particle_lifetime, preheat_lifetime)
    linear_velocity = particle_linear_velocity
    angular_velocity = particle_angular_velocity
    linear_accel = particle_linear_acceleration
    angular_accel = particle_angular_acceleration
    linear_drag = particle_linear_drag
    angular_drag = particle_angular_drag
    pass


func clone() -> ParticleState2D:
    return LifetimePhysicsParticleState2D.new(
        transform, total_lifetime, total_lifetime - lifetime,
        linear_velocity, angular_velocity,
        linear_accel, angular_accel,
        linear_drag, angular_drag
    )