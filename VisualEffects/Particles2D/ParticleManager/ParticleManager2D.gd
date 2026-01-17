## Base Class for CPU-Based 2D Particle Simulators in Compatibility Rendering Projects.
##
## A base class for helpers that seek to
## imitate the features of [GPUParticles2D] in Forward+ Rendering Projects,
## and provide more complex simulation logic for multiple particles using the CPU.
## [br][br]
## E.g.: non-deterministically render particles
## using an arbitrary number of state variables.
## [br][br]
## This is only recommended for
## projects tailored towards Compatibility Rendering,
## and simulations with much lower particle counts.
extends MultiMeshInstance2D
class_name ParticleManager2D


## Set the local 2D transform of a particle
## as if it were a child [CanvasItem] of this node.
func set_particle_transform(instance_id: int, new_transform: Transform2D) -> void:
    # Transforms for multimeshes are represented in right-hand 3D coordinates,
    # while Godot uses a left-hand coordinate system for 2D instead.
    # This causes the local y-axis of the mesh to point in the opposite direction
    # of the local y-axis that a 2D node with the same 2D transform would have.

    # Thus, for consistency, the local y-axis of the mesh must be negated beforehand.
    # This has the same result as negating the y-basis of the final given transform.
    new_transform.y = -new_transform.y

    # Despite the method being named set_instance_transform_2d,
    # we are actually setting the 3D mesh transform
    # by directly expanding the given 2D transformation matrix.
    # As such, to get the accurate corresponding mesh transform,
    # the negation of the y-basis (mentioned above) must be done.
    multimesh.set_instance_transform_2d(instance_id, new_transform)
    pass


## The array version of [method set_particle_transform]
## for giving multiple particles the same transform.
func mass_set_particle_transform(instance_ids: PackedInt32Array, new_transform: Transform2D) -> void:
    new_transform.y = -new_transform.y
    for instance_id in instance_ids:
        multimesh.set_instance_transform_2d(instance_id, new_transform)
        pass
    pass


## Set the modulate color of a particle.
## [br][br]
## This method should only be called if the [member MultiMeshInstance2D.multimesh]
## has the flag [member MultiMesh.use_colors] enabled.
func set_particle_color(instance_id: int, new_color: Color) -> void:
    multimesh.set_instance_color(instance_id, new_color)
    pass


## Set the GPU custom data of a particle.
## [br][br]
## This method should only be called if the [member MultiMeshInstance2D.multimesh]
## has the flag [member MultiMesh.use_custom_data] enabled.
## [br][br]
## [b]Note:[/b]
## This is the value used internally by the shaders to render the particle,
## separate from any actual state variables used for simulating the particle.
func set_particle_custom_data(instance_id: int, new_custom_data: Color) -> void:
    multimesh.set_instance_custom_data(instance_id, new_custom_data)
    pass



## Helper virtual method to verify the correct data struct
## that will be passed to [method emit_particle].
## [br][br]
## [b]Note:[/b] This method is used for debugging purposes only.
func validate_particle_data(_particle_data: ParticleState2D) -> bool:
    return true


## Emit a particle with the state of the provided [param particle_data].
## The user and implementation must ensure that the correct data struct is passed.
## [br][br]
## [b]Note:[/b]
## A reference of the given [param particle_data] struct will be used the emitted particle.
func emit_particle(particle_data: ParticleState2D) -> void:
    # Check that the data struct is of the correct type.
    assert(
        validate_particle_data(particle_data),
        "Invalid Particle Data."
    )

    # Copy the particle state to the list.
    particles.append(particle_data)

    # Allocate more space for the particles if needed.
    if particles.size() > multimesh.instance_count:
        multimesh.instance_count = particles.size()
        pass
    
    multimesh.visible_instance_count = particles.size()
    pass


## Emit a list of particles, as defined by the [param particle_data] array.
## The user and implementation must ensure that the array contains only the correct data struct.
func mass_emit_particle(particle_data: Array[ParticleState2D]) -> void:
    for entry in particle_data:
        # Check that the data struct is of the correct type.
        assert(
            validate_particle_data(entry),
            "Invalid Particle Data."
        )

        particles.append(entry)
        pass

    # Adjust the instance count of the underlying MultiMesh.
    if particles.size() > multimesh.instance_count:
        multimesh.instance_count = particles.size()
        pass
    
    multimesh.visible_instance_count = particles.size()
    pass



## The array of particle states.
## The members of this array are determined by the derived class.
## [br][br]
## [b]Note:[/b] This array should not be directly resized.
var particles: Array


## Delete all particles this node is currently managing.
func clear_particles() -> void:
    particles.clear()
    multimesh.visible_instance_count = 0
    pass


## Remove the particle with the specific ID.
## [br][br]
## [b]Note:[/b]
## This method should only be called internally,
## as the ID of a particle change
## when the list of particles get resized/rearranged.
func remove_particle(particle_id: int) -> void:
    particles.remove_at(particle_id)
    multimesh.visible_instance_count = particles.size()
    pass


## Remove the particles with the specific IDs.
## [br][br]
## [b]Note:[/b]
## The [param particle_ids] list must be sorted in ascending order.
## [br][br]
## [b]Note:[/b]
## This method should only be called internally,
## as the ID of a particle change
## when the list of particles get resized/rearranged.
func mass_remove_particle(particle_ids: PackedInt32Array) -> void:
    for i in range(particle_ids.size() - 1, -1, -1):
        particles.remove_at(particle_ids[i])
        pass

    multimesh.visible_instance_count = particles.size()
    pass
