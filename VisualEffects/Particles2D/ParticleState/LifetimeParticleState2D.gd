extends ParticleState2D
class_name LifetimeParticleState2D


var total_lifetime: float
var lifetime: float


func _init
(
    particle_transform: Transform2D = Transform2D.IDENTITY,
    particle_lifetime: float = 0,
    preheat_lifetime: float = 0
) -> void:
    super._init(particle_transform)
    total_lifetime = particle_lifetime
    lifetime = maxf(total_lifetime - maxf(preheat_lifetime, 0), 0)
    pass


func clone() -> ParticleState2D:
    return LifetimeParticleState2D.new(transform, total_lifetime, total_lifetime - lifetime)