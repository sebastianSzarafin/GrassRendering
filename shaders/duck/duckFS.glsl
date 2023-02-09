#version 330 core

in VS_OUT {
    vec2 texCoord;
    vec3 surfaceNormal;
    vec3 worldPos;
    float distanceFromCamera;
} fs_in;

uniform sampler2D texture_diffuse0;
uniform vec3 cameraPos;
out vec4 outColor;

/*FUNCTIONS*/
vec4 getFogColor(float, vec4);
vec4 getLightColor(vec4, vec3, vec3);

void main()
{
    outColor = texture(texture_diffuse0, fs_in.texCoord);

    outColor = getLightColor(outColor, fs_in.surfaceNormal, fs_in.worldPos);

    outColor = getFogColor(fs_in.distanceFromCamera, outColor);
}