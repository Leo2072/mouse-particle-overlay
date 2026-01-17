extends Node2D
class_name ParticleEmitter2D


@export var particle_manager: ParticleManager2D:
    set(value):
        if particle_manager != value:
            assert(validate_particle_manager(value), "Invalid Particle Mananger 2D for Emitter.")
            particle_manager = value
            pass
        pass


func validate_particle_manager(_value: ParticleManager2D) -> bool:
    return true


func emit() -> void:
    particle_manager.emit_particle(ParticleState2D.new(transform))
    pass