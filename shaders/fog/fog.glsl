#version 330 core

/*CONSTANTS*/
const float gradient = 1.5; // how quickly visibility decreases with distance

/*FUNCTIONS*/
float getVisibility(float distanceFromCamera, float density)
{
	 return clamp(exp(-pow((distanceFromCamera * density), gradient)), 0.0, 1.0);
}


