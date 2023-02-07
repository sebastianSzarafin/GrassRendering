#version 330 core

in float distanceFromCamera;
in vec2 texCoord;

uniform float fogDensity;
uniform vec4 skyColor;
uniform sampler2D texTerrain;

out vec4 outColor;

/* FUNCTIONS */
float getVisibility(float, float);

void main()
{
    outColor = mix(texture(texTerrain, texCoord), vec4(0), 0.5);
    outColor = mix(skyColor, outColor, getVisibility(distanceFromCamera, fogDensity));
}