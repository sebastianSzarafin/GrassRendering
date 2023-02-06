#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in float aTexIndex;

out VS_OUT {
	float texIndex;    
	//float height;
} vs_out;

void main() {
	gl_Position = vec4(aPosition, 1.0);

	vs_out.texIndex = aTexIndex;
	//vs_out.height = aPosition.y;
}