#version 330 core

in VS_OUT {
    float distanceFromCamera;
    vec2 texCoord;
} fs_in;

uniform sampler2D texTerrain;

out vec4 outColor;

/*FUNCTIONS*/
vec4 getFogColor(float, vec4);

void main()
{
    outColor = mix(texture(texTerrain, fs_in.texCoord), vec4(0), 0.5);
	outColor = getFogColor(fs_in.distanceFromCamera, outColor);
}