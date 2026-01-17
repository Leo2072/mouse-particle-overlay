shader_type canvas_item;

uniform vec2 FrameSize = vec2(0, 0);


void vertex()
{
    UV += TEXTURE_PIXEL_SIZE * FrameSize * INSTANCE_CUSTOM.xy;
}

void fragment()
{
    COLOR.x *= sin(UV.x + TIME) * 0.5 + 0.5;
    COLOR.y *= sin(UV.y + TIME + PI * 0.25) * 0.5 + 0.5;
    COLOR.z *= sin((UV.x + UV.y) * 0.5 + TIME + PI * 0.5) * 0.5 + 0.5;
}
