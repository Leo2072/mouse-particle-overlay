shader_type canvas_item;


uniform bool UseColourCurve = false;
uniform sampler2D ColourCurve;
uniform bool UseOffsetCurve = false;
uniform sampler2D OffsetCurve;
uniform bool UseScaleRotationCurve = false;
uniform sampler2D ScaleRotationCurve;


const int SRO = 0;
const int SOR = 1;
const int RSO = 2;
const int ROS = 3;
const int OSR = 4;
const int ORS = 5;


uniform int transform_order:
    hint_enum("SRO", "SOR", "RSO", "ROS", "OSR", "ORS") = 0;


vec2 rotate(vec2 point, float rotation)
{
    float rot_cos = cos(rotation);
    float rot_sin = sin(rotation);
    return vec2(
        point.x * rot_cos - point.y * rot_sin,
        point.y * rot_cos + point.x * rot_sin
    );
}


void vertex()
{
    vec2 norm_lifetime = vec2(1.f - INSTANCE_CUSTOM.y / INSTANCE_CUSTOM.x);

    if (UseColourCurve)
    {
        COLOR *= texture(ColourCurve, norm_lifetime);
    }

    vec2 scale = vec2(1.f);
    float rotation = 0.f;
    vec2 offset = vec2(0.f);

    if (UseOffsetCurve)
    {
        offset = texture(OffsetCurve, norm_lifetime).xy;
    }
    if (UseScaleRotationCurve)
    {
        vec3 scale_rotation = texture(ScaleRotationCurve, norm_lifetime).xyz;
        scale = scale_rotation.xy;
        rotation = scale_rotation.z;
    }

    switch (transform_order)
    {
        case SRO:
            VERTEX = rotate(VERTEX * scale, rotation) + offset;
            break;
        case SOR:
            VERTEX = rotate(VERTEX * scale + offset, rotation);
            break;
        case RSO:
            VERTEX = rotate(VERTEX, rotation) * scale + offset;
            break;
        case ROS:
            VERTEX = (rotate(VERTEX, rotation) + offset) * scale;
            break;
        case OSR:
            VERTEX = rotate((VERTEX + offset) * scale, rotation);
            break;
        case ORS:
            VERTEX = rotate(VERTEX + offset, rotation) * scale;
            break;
    }
}
