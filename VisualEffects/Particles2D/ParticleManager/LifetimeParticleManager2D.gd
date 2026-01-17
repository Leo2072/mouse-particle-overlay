extends ParticleManager2D
class_name LifetimeParticleManager2D


func validate_particle_data(particle_data: ParticleState2D) -> bool:
    return particle_data is LifetimeParticleState2D


"""
# Problem too big for the scope of this project.
enum UPDATE_MODE {NONE, PROCESS, PHYSICS}
@export var update_mode: UPDATE_MODE = UPDATE_MODE.NONE:
    set(value):
        if update_mode != value:
            update_mode = value
            match update_mode:
                UPDATE_MODE.NONE:
                    set_physics_process(false)
                    set_process(false)
                    pass
                UPDATE_MODE.PROCESS:
                    set_physics_process(false)
                    set_process(true)
                    pass
                UPDATE_MODE.PHYSICS:
                    set_physics_process(true)
                    set_process(false)
                    pass
            pass
        pass
"""


## Callback method to update a particle by advancing a certain amount (in seconds).
func _update_particle(particle_id: int, delta: float) -> void:
    particles[particle_id].lifetime -= delta
    pass


func _process(delta: float) -> void:
    if particles.size() > 0:
        var particle_id: = 0
        var particle_count: = particles.size()

        while particle_id < particle_count:
            _update_particle(particle_id, delta)

            if particles[particle_id].lifetime <= 0:
                remove_particle(particle_id)
                particle_count -= 1
                pass
            else:
                set_particle_transform(particle_id, particles[particle_id].transform)
                set_particle_custom_data(
                    particle_id,
                    Color(
                        particles[particle_id].total_lifetime,
                        particles[particle_id].lifetime, 0
                    )
                )
                particle_id += 1
                pass
            pass
        pass
    pass
