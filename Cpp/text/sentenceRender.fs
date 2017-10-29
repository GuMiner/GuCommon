#version 400 core

uniform sampler2D fontimage;

out vec4 color;

in VS_OUT
{
    vec4 color;
    vec2 texPos;
} fs_in;

// Render our text with a sharp boundary.
void main(void)
{
    color = fs_in.color * texture2D(fontimage, fs_in.texPos);
    if (color.a > 0.20)
    {
        color.a = 1.0f;
    }
    else
    {
        color.a = 0.0f;
    }
}