#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 cameraPos;
uniform vec4 plane;

out VS_OUT {
    vec2 texCoord;
    vec3 surfaceNormal;
    vec3 worldPos;
    float distanceFromCamera;
} vs_out;

void main()
{
    gl_ClipDistance[0] = dot(vec4(aPosition, 1.0), plane);

    vec4 worldPos = vec4(aPosition, 1.0) * model; 
    vs_out.worldPos = worldPos.xyz;

    gl_Position = worldPos * view * projection;

    vs_out.texCoord = aTexCoord;
    vs_out.surfaceNormal = aNormal * mat3(transpose(inverse(model)));
    vs_out.distanceFromCamera = length(aPosition - cameraPos);
}
