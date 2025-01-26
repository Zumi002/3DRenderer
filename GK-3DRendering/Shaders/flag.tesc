#version 410 core

layout(vertices=16) out;

uniform mat4 view;
uniform mat4 model;
uniform ivec2 patchSize;

void main()
{
    gl_out[gl_InvocationID].gl_Position = gl_in[gl_InvocationID].gl_Position;
    if(gl_InvocationID == 0)
    {
        const int MIN_TESS_LEVEL = 5;
        const int MAX_TESS_LEVEL = 32;
        const float MIN_DISTANCE = 0.01;
        const float MAX_DISTANCE = 20;

        vec4 eyeSpacePos00 = gl_in[0].gl_Position * model * view;
        vec4 eyeSpacePos01 = gl_in[(patchSize.y-1)*(patchSize.x)].gl_Position * model * view;
        vec4 eyeSpacePos10 = gl_in[patchSize.x-1].gl_Position * model * view;
        vec4 eyeSpacePos11 = gl_in[patchSize.y*patchSize.x-1].gl_Position * model * view;

        float distance00 = clamp( (abs(eyeSpacePos00.z) - MIN_DISTANCE) / (MAX_DISTANCE-MIN_DISTANCE), 0.0, 1.0 );
        float distance01 = clamp( (abs(eyeSpacePos01.z) - MIN_DISTANCE) / (MAX_DISTANCE-MIN_DISTANCE), 0.0, 1.0 );
        float distance10 = clamp( (abs(eyeSpacePos10.z) - MIN_DISTANCE) / (MAX_DISTANCE-MIN_DISTANCE), 0.0, 1.0 );
        float distance11 = clamp( (abs(eyeSpacePos11.z) - MIN_DISTANCE) / (MAX_DISTANCE-MIN_DISTANCE), 0.0, 1.0 );

        float tessLevel0 = mix( MAX_TESS_LEVEL, MIN_TESS_LEVEL, min(distance10, distance00) );
        float tessLevel1 = mix( MAX_TESS_LEVEL, MIN_TESS_LEVEL, min(distance00, distance01) );
        float tessLevel2 = mix( MAX_TESS_LEVEL, MIN_TESS_LEVEL, min(distance01, distance11) );
        float tessLevel3 = mix( MAX_TESS_LEVEL, MIN_TESS_LEVEL, min(distance11, distance10) );

        gl_TessLevelOuter[0] = tessLevel0;
        gl_TessLevelOuter[1] = tessLevel1;
        gl_TessLevelOuter[2] = tessLevel2;
        gl_TessLevelOuter[3] = tessLevel3;

        gl_TessLevelInner[0] = max(tessLevel1, tessLevel3);
        gl_TessLevelInner[1] = max(tessLevel0, tessLevel2);
    }
}