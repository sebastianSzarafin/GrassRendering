#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

uniform mat4 view;
uniform mat4 projection;

uniform  vec4 plane;

//out VS_OUT {
//    vec2 texCoord;
//} vs_out;

out vec4 clipSpace;

void main() {
    gl_ClipDistance[0] = dot( vec4(aPosition, 1.0), plane);

    clipSpace = vec4(aPosition, 1.0) * view * projection;
    gl_Position = clipSpace;
}