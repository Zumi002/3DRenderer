#version 410 core


layout(quads, equal_spacing, ccw) in;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform ivec2 patchSize;


out vec3 Normal;
out vec3 FragPos;

float coefN[25];
float coefM[25];
float coefN1[24];
float coefM1[24];

float epsilon = 1e-8; 

void calcBinom(int n, int m);
void calcBern(float u, float v, int n, int m);



void main()
{
    float u = gl_TessCoord.x;
    float v = gl_TessCoord.y;

   vec3 pos = vec3(0);
   vec3 du = vec3(0);
   vec3 dv = vec3(0);

   calcBinom(patchSize.x-1, patchSize.y-1);
   calcBern(u,v,patchSize.x-1, patchSize.y-1);

   for(int i = 0; i<patchSize.y; i++)
   {
        vec3 line = vec3(0);
        for(int j = 0; j<patchSize.x; j++)
        {
            line+=vec3(gl_in[i*patchSize.x+j].gl_Position)*coefN[j];
        }
        line*=coefM[i];
        pos+=line;
   }

   for(int i = 0; i<patchSize.y; i++)
   {
        vec3 line = vec3(0);
        for(int j = 0; j<patchSize.x-1; j++)
        {
            line+=vec3(gl_in[i*patchSize.x+j+1].gl_Position-gl_in[i*patchSize.x+j].gl_Position)*coefN1[j];
        }
        line*=coefM[i];
        du+=line;
   }

   for(int i = 0; i<patchSize.y-1; i++)
   {
        vec3 line = vec3(0);
        for(int j = 0; j<patchSize.x; j++)
        {
            line+=vec3(gl_in[(i+1)*patchSize.x+j].gl_Position-gl_in[(i)*patchSize.x+j].gl_Position)*coefN[j];
        }
        line*=coefM1[i];
        dv+=line;
   }
   
   if(u < epsilon)
   {
        pos.z = 0;
   }
   gl_Position = vec4(pos, 1.0) * model * view * projection;
   FragPos = vec3(vec4(pos, 1.0) * model * view);
   Normal = cross(du,dv) * mat3(transpose(inverse(model * view)));
}

void calcBinom(int n, int m)
{
    coefN[0] = 1;
    for(int i = 1; i<=n; i++)
    {
        coefN[i] = coefN[i - 1] * (n-i+1.0) / (i);
    }

    coefM[0] = 1;
    for(int i = 1; i<=m; i++)
    {
        coefM[i] = coefM[i - 1] * (m-i+1.0) / (i);
    }

    coefN1[0] = 1;
    for(int i = 1; i<=n-1; i++)
    {
        coefN1[i] = coefN1[i - 1] * (n-i) / (i);
    }

    coefM1[0] = 1;
    for(int i = 1; i<=m-1; i++)
    {
        coefM1[i] = coefM1[i - 1] * (m-i) / (i);
    }
}

void calcBern(float u, float v, int n, int m)
{
    

   
    for(int i = 0; i<=n; i++)
    {
        float powui = (u>epsilon)?pow(u,i):((i==0)?1:0);
        float pow1ui = (1.0-u>epsilon)?pow(1.0-u,n-i):((n-i==0)?1:0);
        coefN[i] *= powui*pow1ui;
    }
   

    for(int i = 0; i<=m; i++)
    {
        float powvi = (v>epsilon)?pow(v,i):((i==0)?1:0);
        float pow1vi = (1.0-v>epsilon)?pow(1.0-v,m-i):((m-i==0)?1:0);
        coefM[i] *=  powvi*pow1vi;
    }

    for(int i = 0; i<=n-1; i++)
    {
        float powui = (u>epsilon)?pow(u,i):((i==0)?1:0);
        float pow1ui = (1.0-u>epsilon)?pow(1.0-u,n-i-1):((n-i-1==0)?1:0);
        coefN1[i] *= powui*pow1ui;
    }

    for(int i = 0; i<=m-1; i++)
    {
        float powvi = (v>epsilon)?pow(v,i):((i==0)?1:0);
        float pow1vi = (1.0-v>epsilon)?pow(1.0-v,m-i-1):((m-i-1==0)?1:0);
        coefM1[i] *= powvi*pow1vi;
    }
}