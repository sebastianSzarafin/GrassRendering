#version 330 core

/*CONSTANTS*/
const int lightsCount = 5;
const float ambientStrength = 0.2;
const float specularStrength = 0.5;
const float shininess = 32;

struct Sun {
    float ambient;
    vec3 color;
    vec3 direction;
  
    //vec3 diffuse;
    //vec3 specular;
};

uniform Sun sun;
uniform float dayTime; // 0 - Morning, 1 - Afternoon, 2 - Evening, 3 - Night
uniform vec3 lightPosition[lightsCount];
uniform vec3 lightColor[lightsCount];
uniform vec3 attenuation[lightsCount];
uniform vec3 cameraPos;

/*FUNCTIONS*/
vec4 getLightColor(vec4 color, vec3 surfaceNormal, vec3 worldPos)
{
    vec3 sunColor = sun.ambient * sun.color;

    if(dayTime == 0 || dayTime == 1) return color * vec4(sunColor, 1.0);

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
    diffuse = max(diffuse, ambientStrength);

    return vec4(sunColor + diffuse + specular, 1.0) * color;
}