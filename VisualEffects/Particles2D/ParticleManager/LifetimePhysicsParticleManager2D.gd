extends LifetimeParticleManager2D
class_name LifetimePhysicsParticleManager2D


func validate_particle_data_layout(particle_data: ParticleState2D) -> bool:
    return particle_data is LifetimePhysicsParticleState2D



## Callback method to update a particle by advancing a certain amount (in seconds).
func _update_particle(particle_id: int, delta: float) -> void:
    super._update_particle(particle_id, delta)
    var particle: LifetimePhysicsParticleState2D = particles[particle_id]
    if particle.lifetime > 0:
        var particle_position = particle.transform.origin + particle.linear_velocity * delta
        particle.transform = particle.transform.rotated(particle.angular_velocity * delta)
        particle.transform.origin = particle_position

        particle.linear_velocity += particle.linear_accel * delta
        particle.angular_velocity += particle.angular_accel * delta

        var particle_speed: = particle.linear_velocity.length()
        particle.linear_velocity = (
            particle.linear_velocity / particle_speed *
            max(particle_speed - particle.linear_drag * delta, 0)
        )

        particle.angular_velocity = sign(
            particle.angular_velocity) * (abs(particle.angular_velocity) - particle.angular_drag * delta
        )
        pass
    pass
