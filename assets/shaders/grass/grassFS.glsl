#version 330 core

out vec4 outColor;

in GS_OUT {
    vec2 texCoord;
	float colorVariation;
	float texIndex;
	float distanceFromCamera;
} fs_in;

uniform vec4 skyColor;
uniform float visibility;
uniform sampler2D texGrass1;
uniform sampler2D texGrass2;
uniform sampler2D texGrass3;
uniform sampler2D texGrass4;
uniform sampler2D texGrass5;

/*CONSTANTS*/
float texBorderDetail = 0.1;

/* USEFUL FUNCTIONS */
vec4 mapTexture();
bool isTexBorder(vec3);
float getVisibility(float, float);

void main(){
	vec4 color = mapTexture();
	color.xyz = mix(color.xyz, 0.5*color.xyz, fs_in.colorVariation);

	if(color.a < 0.05
	|| isTexBorder(color.xyz)) discard;

	outColor = mix(skyColor, color, getVisibility(fs_in.distanceFromCamera, visibility));
}

vec4 mapTexture()
{
	switch(int(fs_in.texIndex))
	{
		case 1:
			return texture(texGrass1, fs_in.texCoord);
		case 2:
			return texture(texGrass2, fs_in.texCoord);
		case 3:
			return texture(texGrass3, fs_in.texCoord);
		case 4:
			return texture(texGrass4, fs_in.texCoord);
		case 5:
			return texture(texGrass5, fs_in.texCoord);
	}
}

bool isTexBorder(vec3 color)
{
	return 
		color.x <= texBorderDetail && 
		color.y <= texBorderDetail && 
		color.z <= texBorderDetail;
}