#version 330 core

/*CONSTANTS*/
const int lightsCount = 5;
const float ambientStrength = 0.1;
const float specularStrength = 0.5;
const float shininess = 32;

uniform vec3 lightPosition[lightsCount];
uniform vec3 lightColor[lightsCount];
uniform vec3 attenuation[lightsCount];
uniform vec3 cameraPos;

/*FUNCTIONS*/
vec4 getLightColor(vec4 color, vec3 surfaceNormal, vec3 worldPos)
{
    vec3 toLightVector[lightsCount];
    for(int i = 0; i < lightsCount; i++)
    {
        toLightVector[i] = lightPosition[i] - worldPos.xyz;
    }

	vec3 unitNormal = normalize(surfaceNormal);
    vec3 viewDir = normalize(cameraPos - worldPos);

    vec3 diffuse = vec3(0), specular = vec3(0);

    for(int i = 0; i < lightsCount; i++)
    {
        float dist = length(toLightVector[i]);
        float attFactor = attenuation[i].x + attenuation[i].y * dist + attenuation[i].z * pow(dist, 2); 

        // diffuse
        vec3 unitLightVector = normalize(toLightVector[i]);

        float nDotl = dot(unitNormal, unitLightVector);
        float brightness = max(nDotl, 0.0);
        diffuse += brightness * lightColor[i] / attFactor;
        //specular
        vec3 reflectDir = reflect(-unitLightVector, unitNormal);

        float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
        specular += specularStrength * spec * lightColor[i] / attFactor;
    }
    //ambient 
    diffuse = max(diffuse, 0.2);

    return vec4(diffuse + specular, 1.0) * color;
}