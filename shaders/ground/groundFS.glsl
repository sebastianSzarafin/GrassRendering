#version 330 core

in VS_OUT {
    vec3 normal;
    vec3 worldPos;
	vec2 texCoord;
	float distanceFromCamera; 
} fs_in;

uniform sampler2D texTerrain;

out vec4 outColor;

/*FUNCTIONS*/
vec4 getFogColor(float, vec4);
vec4 getLightColor(vec4, vec3, vec3);

void main()
{
    outColor = mix(texture(texTerrain, fs_in.texCoord), vec4(0), 0.5);

    outColor = getLightColor(outColor, fs_in.normal, fs_in.worldPos);

	outColor = getFogColor(fs_in.distanceFromCamera, outColor);
}