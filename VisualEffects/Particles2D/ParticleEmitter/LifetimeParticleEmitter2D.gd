extends ParticleEmitter2D
class_name LifetimeParticleEmitter2D


@export_range(0, 10, 0.1, &"or_greater") var particle_lifetime: float = 0
@export_range(0, 10, 0.1, &"or_greater") var particle_preheat_lifetime: float = 0


func validate_particle_manager(value: ParticleManager2D) -> bool:
    return value is LifetimeParticleManager2D


func emit() -> void:
    particle_manager.emit_particle(
        LifetimeParticleState2D.new(
            transform, particle_lifetime, particle_preheat_lifetime
        )
    )
    pass
