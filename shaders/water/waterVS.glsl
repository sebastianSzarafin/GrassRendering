﻿#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;

out VS_OUT {
    vec3 normal;
    vec3 worldPos;
    vec2 texCoord;
    vec4 clipSpace;
    vec3 toCameraVector;
    float distanceFromCamera;
} vs_out;

uniform mat4 view;
uniform mat4 projection;
uniform vec3 cameraPos;
uniform vec4 plane;

void main() {
    gl_ClipDistance[0] = dot(vec4(aPosition, 1.0), plane);

    vs_out.normal = aNormal;
    vs_out.worldPos = aPosition;
    vs_out.toCameraVector = cameraPos - aPosition;
    vs_out.distanceFromCamera = length(aPosition - cameraPos);

    vs_out.clipSpace = vec4(aPosition, 1.0) * view * projection;
    vs_out.texCoord = vec2(aPosition.x / 2.0 + 0.5, aPosition.z / 2.0 + 0.5);

    gl_Position = vs_out.clipSpace;
}