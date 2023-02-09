#version 330 core
layout (location = 0) in vec3 aPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec4 plane;

void main()
{
    gl_ClipDistance[0] = dot(vec4(aPosition, 1.0), plane);

    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}