#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 3) in float aTexIndex;

out VS_OUT {
	float texIndex;    
} vs_out;

void main() {
	gl_Position = vec4(aPosition, 1.0);

	vs_out.texIndex = aTexIndex;
}