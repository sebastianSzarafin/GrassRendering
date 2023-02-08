#version 330 core

//const int maxLights = 2;

in VS_OUT {
    vec2 texCoord;
    vec3 surfaceNormal;
    //vec3 toLightVector[maxLights];
    vec3 worldPos;
} fs_in;

uniform sampler2D texture_diffuse0;

//uniform vec3 lightColor[maxLights];
//uniform vec3 attenuation[maxLights];
uniform vec3 cameraPos;

out vec4 outColor;

const float ambientStrength = 0.1;
const float specularStrength = 0.5;
const float shininess = 32;

void main()
{
    outColor = texture(texture_diffuse0, fs_in.texCoord);

    //vec3 unitNormal = normalize(fs_in.surfaceNormal);
    //vec3 viewDir = normalize(cameraPos - fs_in.worldPos);

    //vec3 diffuse = vec3(0), specular = vec3(0);

    //for(int i = 0; i < maxLights; i++)
    //{
    //    float dist = length(fs_in.toLightVector[i]);
    //    float attFactor = attenuation[i].x + attenuation[i].y * dist + attenuation[i].z * pow(dist, 2); 

        // diffuse
    //    vec3 unitLightVector = normalize(fs_in.toLightVector[i]);

    //    float nDotl = dot(unitNormal, unitLightVector);
    //    float brightness = max(nDotl, 0.0);
    //    diffuse += brightness * lightColor[i] / attFactor;
        //specular
    //    vec3 reflectDir = reflect(-unitLightVector, unitNormal);

    //    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    //    specular += specularStrength * spec * lightColor[i] / attFactor;
    //}
    // ambient 
    //diffuse = max(diffuse, 0.2);

    //outColor = vec4(diffuse + specular, 1.0) * outColor;
}