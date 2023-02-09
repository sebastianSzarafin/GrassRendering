#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

/*CONSTANTS*/
const float tiling = 10.0;

out VS_OUT {
	float distanceFromCamera; 
	vec2 texCoord;
} vs_out;

uniform mat4 view;
uniform mat4 projection;
uniform vec3 cameraPos;
uniform vec4 plane;


void main() {
	gl_ClipDistance[0] = dot(vec4(aPosition, 1.0), plane);

	gl_Position = vec4(aPosition, 1.0) * view * projection;

	vs_out.distanceFromCamera = length(aPosition - cameraPos);
    vs_out.texCoord = aTexCoord * tiling;
}