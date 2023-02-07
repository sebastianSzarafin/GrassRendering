#version 330 core

//in VS_OUT {
//    vec2 texCoord;
//} fs_in;

out vec4 outColor;

uniform sampler2D texReflect;
uniform sampler2D texRefract;

in vec4 clipSpace;

void main()
{
    vec2 ndc = (clipSpace.xy / clipSpace.w) / 2.0 + 0.5;

    vec2 reflectTexCoord = vec2(ndc.x, -ndc.y);
    vec2 refractTexCoord = ndc;

    vec4 reflectionColor = texture(texReflect, reflectTexCoord);
    vec4 refractionColor = texture(texRefract, refractTexCoord);

    outColor = mix(reflectionColor, refractionColor, 0.5);
}