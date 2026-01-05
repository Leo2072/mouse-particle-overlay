extends Window



# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    FullPassthroughSetter.EnableFullPassthrough(self)
    close_requested.connect(shutdown)
    pass # Replace with function body.


func _process(_delta: float) -> void:
    var rect: = DisplayServer.screen_get_usable_rect()
    position = rect.position
    size = rect.size
    pass


func shutdown() -> void:
    queue_free()
    get_tree().quit()
    pass