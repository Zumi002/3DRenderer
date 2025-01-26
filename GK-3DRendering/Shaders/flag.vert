#version 410 core


#define M_PI 3.1415926535897932384626433832795

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCord;
layout (location = 3) in vec2 aUV;

uniform float animTime;

void main()
{
    
    float phaseShift = (aUV.x*M_PI + animTime*M_PI)*2;
    
    if(aUV.x==0)
    {
        phaseShift = 0;
    }

    gl_Position = vec4(aPosition, 1.0)+vec4(0,0,sin(phaseShift)/2,0);
}