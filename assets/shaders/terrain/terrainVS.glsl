#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;


uniform vec3 cameraPos;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoord;
out float distanceFromCamera;

void main() {
	gl_Position = vec4(aPosition, 1.0) * view * projection;

	distanceFromCamera = length(aPosition - cameraPos);
    texCoord = aTexCoord * 10.0;
}