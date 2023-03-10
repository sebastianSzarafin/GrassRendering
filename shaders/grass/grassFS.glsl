#version 330 core


in GS_OUT {
	vec3 normal;
    vec3 worldPos;
    vec2 texCoord;
	float colorVariation;
	float texIndex;
	float distanceFromCamera;
} fs_in;

out vec4 outColor;

uniform sampler2D texGrass1;
uniform sampler2D texGrass2;
uniform sampler2D texGrass3;
uniform sampler2D texGrass4;
uniform sampler2D texGrass5;

/*CONSTANTS*/
float texBorderDetail = 0.1;

/*FUNCTIONS*/
vec4 mapTexture();
bool isTexBorder(vec3);
vec4 getFogColor(float, vec4);
vec4 getLightColor(vec4, vec3, vec3);

void main(){
	vec4 color = mapTexture();
	color.xyz = mix(color.xyz, 0.5*color.xyz, fs_in.colorVariation);

	if(color.a < 0.05
	|| isTexBorder(color.xyz)) discard;

    color = getLightColor(color, fs_in.normal, fs_in.worldPos);

	outColor = getFogColor(fs_in.distanceFromCamera, color);
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