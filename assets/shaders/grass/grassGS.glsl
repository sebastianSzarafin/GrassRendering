#version 330 core

layout (points) in;
layout (triangle_strip, max_vertices = 12) out;


in VS_OUT {
    float texIndex;  
	//float height;
} gs_in[];


out GS_OUT {
    vec2 texCoord;
    float colorVariation;
	float texIndex;  	
	float distanceFromCamera;
} gs_out;


uniform mat4 view;
uniform mat4 projection;
uniform vec3 cameraPos;
uniform float time;
uniform sampler2D texWind;

/*CONSTANTS*/
const float PI = 3.141592653589793;
const float grassMinSize = 0.4;
const float LOD1 = 5.0;  // High Level Of Details
const float LOD2 = 10.0; // Medium Level Of Details
const float LOD3 = 25.0; // Low Level Of Details
const vec2 windDirection = vec2(1.0, 1.0);
const float windStrength = 0.1;
const float deflectionFactor = 0.25;
const float leanFactor = 0.25;



/*PARAMETERS*/
float grassSize;

/* USEFUL FUNCTIONS */
mat4 rotationX(in float alpha);
mat4 rotationY(in float alpha);
mat4 rotationZ(in float alpha);
float random (vec2 st);
float noise (in vec2 st);
float fbm ( in vec2 _st); //Fractial Brownian Motion

/* MAIN FUNCTIONS */
void createQuad(vec4 basePosition, mat4 crossmodel, mat4 modelWind)
{
    vec2 texCoords[4];
    texCoords[0] = vec2(0.0, 0.0); // down left
    texCoords[1] = vec2(1.0, 0.0); // down right
    texCoords[2] = vec2(0.0, 1.0); // up left
    texCoords[3] = vec2(1.0, 1.0); // up right

    vec4 vertexPosition[4];
    vertexPosition[0] = vec4(-0.25, 0.0, 0.0, 0.0); 	// down left
	vertexPosition[1] = vec4( 0.25, 0.0, 0.0, 0.0);		// down right
	vertexPosition[2] = vec4(-0.25, 0.75, 0.0, 0.0);	// up left
	vertexPosition[3] = vec4( 0.25, 0.75, 0.0, 0.0);	// up right
    
    // random rotation on Y axis
	mat4 modelRandY = rotationY(random(basePosition.zx)*PI);

	// wind 
	mat4 modelWindApply = mat4(1);

	// loop of the grass clump creation
    for(int i = 0; i < 4; i++)
    {
		// apply the wind only to the top corners
		if(i == 2) modelWindApply = modelWind;

        gl_Position = 
            (basePosition + vertexPosition[i] * grassSize * crossmodel * modelRandY * modelWindApply) * 
            view * 
            projection;

        gs_out.texCoord = texCoords[i];
        gs_out.colorVariation = fbm(basePosition.xz);
		gs_out.texIndex = gs_in[0].texIndex;

        EmitVertex();
    }
    EndPrimitive();
}

void createGrass(vec4 basePosition, int numberOfQuads)
{
	// wind
	vec2 uv = basePosition.xz / 10.0 + windDirection * windStrength * time / 2;		// texture coordinates
	uv.x = mod(uv.x, 1.0);															// of the moving wind
	uv.y = mod(uv.y, 1.0);															//
	vec4 wind = texture(texWind, uv);
	mat4 modelWind = (rotationX(wind.x * PI * deflectionFactor - PI * leanFactor) * 
						rotationZ(wind.y * PI * deflectionFactor - PI * leanFactor));

	mat4 model0 = mat4(1.0);
	mat4 model45 = rotationY(radians(45));
	mat4 modelm45 = rotationY(-radians(45));
 
	switch(numberOfQuads)
	{
		case 1: // Low LOD
			createQuad(basePosition, model0, modelWind);
			break;
		case 2: // Medium LOD
			createQuad(basePosition, model45, modelWind);
			createQuad(basePosition, modelm45, modelWind);
			break;
		case 3: // High LOD
			createQuad(basePosition, model0, modelWind);
			createQuad(basePosition, model45, modelWind);
			createQuad(basePosition, modelm45, modelWind);
			break;
	}
}

void main()
{
	//if(gs_in[0].height < 0) return;

    grassSize = 
        random(gl_in[0].gl_Position.xz) * (1.0 - grassMinSize) + 
        grassMinSize;

	// distance of position to camera
	float distanceFromCamera = length(gl_in[0].gl_Position.xyz - cameraPos);
	gs_out.distanceFromCamera = distanceFromCamera;
	float t = 6.0; if (distanceFromCamera > LOD2) t *= 1.5;
	distanceFromCamera += (random(gl_in[0].gl_Position.xz) * t - t / 2.0);

	// change number of quad function of distance
	int lessDetails = 3;
	if (distanceFromCamera > LOD1) lessDetails = 2;
	if (distanceFromCamera > LOD2) lessDetails = 1;
	if (distanceFromCamera > LOD3) lessDetails = 0;
	
	// rendering grass clumps with inclusion of the detail level
    if (lessDetails != 1
		|| (lessDetails == 1 && (int(gl_in[0].gl_Position.x * 10) % 1) == 0
		|| (int(gl_in[0].gl_Position.z * 10) % 1) == 0)
		|| (lessDetails == 2 && (int(gl_in[0].gl_Position.x * 5) % 1) == 0 
		|| (int(gl_in[0].gl_Position.z * 5) % 1) == 0)
	)
		createGrass(gl_in[0].gl_Position, lessDetails);
} 

mat4 rotationX(float alpha) {
	return mat4(
		1.0,		0,			0,			0,
		0, 	cos(alpha),	-sin(alpha),		0,
		0, 	sin(alpha),	 cos(alpha),		0,
		0, 			0,			  0, 		1);
}

mat4 rotationY(float alpha)
{
    return mat4(
        cos(alpha),		0,	sin(alpha),		0,
        0,				1,  0,				0,
        -sin(alpha),	0,  cos(alpha),		0,
        0,				0,  0,				1
    );
}

mat4 rotationZ(float alpha) {
	return mat4(	
		cos(alpha),		-sin(alpha),	0,	0,
		sin(alpha),		cos(alpha),		0,	0,
		0,				0,				1,	0,
		0,				0,				0,	1);
}

float random (vec2 st) {
    return fract(sin(dot(st.xy,vec2(12.9898,78.233)))*43758.5453123);
}

// 2D Noise based on Morgan McGuire @morgan3d
// https://www.shadertoy.com/view/4dS3Wd
float noise (in vec2 st) {
	vec2 i = floor(st);
	vec2 f = fract(st);

	// Four corners in 2D of a tile
	float a = random(i);
	float b = random(i + vec2(1.0, 0.0));
	float c = random(i + vec2(0.0, 1.0));
	float d = random(i + vec2(1.0, 1.0));

	// Smooth Interpolation

	// Cubic Hermine Curve.  Same as SmoothStep()
	vec2 u = f*f*(3.0-2.0*f);
	// u = smoothstep(0.,1.,f);

	// Mix 4 coorners percentages
	return mix(a, b, u.x) +
	(c - a)* u.y * (1.0 - u.x) +
	(d - b) * u.x * u.y;
}
#define NUM_OCTAVES 5
float fbm ( in vec2 _st) {
	float v = 0.0;
	float a = 0.5;
	vec2 shift = vec2(100.0);
	// Rotate to reduce axial bias
	mat2 rot = mat2(cos(0.5), sin(0.5),
	-sin(0.5), cos(0.50));
	for (int i = 0; i < NUM_OCTAVES; ++i) {
		v += a * noise(_st);
		_st = rot * _st * 2.0 + shift;
		a *= 0.5;
	}
	return v;
}