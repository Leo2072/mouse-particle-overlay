extends RefCounted
class_name ParticleState2D


var transform: Transform2D


func _init
(
    particle_transform: Transform2D = Transform2D.IDENTITY
) -> void:
    transform = particle_transform
    pass


func clone() -> ParticleState2D:
    return ParticleState2D.new(transform)