#version 330 core

/*CONSTANTS*/
const float gradient = 1.5; // how quickly visibility decreases with distance

uniform vec4 skyColor;
uniform float fogDensity;

/*FUNCTIONS*/
vec4 getFogColor(float distanceFromCamera, vec4 color)
{
	float visibility =  clamp(exp(-pow((distanceFromCamera * fogDensity), gradient)), 0.0, 1.0);
	return mix(skyColor, color, visibility);	 
}

