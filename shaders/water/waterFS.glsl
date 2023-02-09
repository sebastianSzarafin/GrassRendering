#version 330 core

in VS_OUT {
    vec3 normal;
    vec2 texCoord;
    vec4 clipSpace;
    vec3 toCameraVector;
    float distanceFromCamera;
} fs_in;

out vec4 outColor;

uniform vec4 skyColor;
uniform sampler2D texReflect;
uniform sampler2D texRefract;
uniform sampler2D texWind;
uniform float time;

/*CONSTANTS*/
const float waveStrength = 0.001;

/*FUNCTIONS*/
vec4 getFogColor(float, vec4);

void main()
{
    vec2 ndc = (fs_in.clipSpace.xy / fs_in.clipSpace.w) / 2.0 + 0.5;

    vec2 reflectTexCoord = vec2(ndc.x, -ndc.y);
    vec2 refractTexCoord = ndc;

    float move = time / 12;
    vec2 distortion = 
        ((texture(texWind, vec2(fs_in.texCoord.x, fs_in.texCoord.y - move)).rg * 2.0 - 1.0) +
        (texture(texWind, vec2(-fs_in.texCoord.x - move, fs_in.texCoord.y - move)).rg * 2.0 - 1.0) +
        (texture(texWind, vec2(-fs_in.texCoord.x + move, fs_in.texCoord.y - move)).rg * 2.0 - 1.0))  * waveStrength;


    reflectTexCoord += distortion;
    reflectTexCoord.x = clamp(reflectTexCoord.x, 0.001, 0.999);
    reflectTexCoord.y = clamp(reflectTexCoord.y, -0.999, -0.001);


    refractTexCoord += distortion;
    refractTexCoord = clamp(refractTexCoord, 0.001, 0.999);

    vec4 reflectionColor = texture(texReflect, reflectTexCoord);
    vec4 refractionColor = texture(texRefract, refractTexCoord);

    float reflectiveFactor = dot(normalize(fs_in.toCameraVector), fs_in.normal);
    reflectiveFactor = pow(reflectiveFactor, 0.5);

    outColor = mix(reflectionColor, refractionColor, reflectiveFactor);
    outColor = mix(outColor, vec4(0.0, 0.3, 0.5, 1.0), 0.2);

	outColor = getFogColor(fs_in.distanceFromCamera, outColor);
}