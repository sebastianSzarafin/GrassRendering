#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in float aTexIndex;

out VS_OUT {
	vec3 position;
	vec3 normal;
	float texIndex;    
	//float height;
} vs_out;

void main() {
	gl_Position = vec4(aPosition, 1.0);

	vs_out.normal = aNormal;
	vs_out.texIndex = aTexIndex;
	//vs_out.height = aPosition.y;
}