#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

uniform mat4 view;
uniform mat4 projection;

//out VS_OUT {
//    vec2 texCoord;
//} vs_out;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * view * projection;

    //vs_out.texCoord = aTexCoord;
}