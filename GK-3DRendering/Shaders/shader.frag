#version 410 core

#define MAX_DIR_LIGHTS 2
#define MAX_SPOT_LIGHTS 5
#define MAX_POINT_LIGHTS 5

#define FOG_DENSITY 0.01
#define FOG_COLOR vec3(0.7)

struct Material 
{    
    vec3 color;

    
    float ambient;
    float diffuse;
    float specular;

    float shininess;
};

struct DirectionalLight
{
    vec3 color;
    vec3 direction;

    float ambient;
    float diffuse;
    float specular;
};

struct PointLight
{
    vec3 color;
    vec3 position;

    float ambient;
    float diffuse;
    float specular;

    float constant;
    float linear;
    float quadratic;
};

struct SpotLight
{
    vec3 color;
    vec3 direction;
    vec3 position;

    float cutOff;
    float outerCutOff;

    float ambient;
    float diffuse;
    float specular;

    float constant;
    float linear;
    float quadratic;
};

uniform Material material;

uniform DirectionalLight directionalLights[MAX_DIR_LIGHTS];
uniform SpotLight spotLights[MAX_SPOT_LIGHTS];
uniform PointLight pointLights[MAX_POINT_LIGHTS];

uniform int directionalLightsCount;
uniform int spotLightsCount;
uniform int pointLightsCount;
uniform bool oneSided;

in vec3 FragPos;
in vec3 Normal;


out vec4 FragColor;

vec3 CalcDirLight(DirectionalLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
float fogFactor(float distance);

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(-FragPos);

    if(oneSided&&!gl_FrontFacing)
    {
         norm = -norm;
    }

    vec3 resultColor = vec3(0.0);
    
    float dlc = min(directionalLightsCount, MAX_DIR_LIGHTS);
    float slc = min(spotLightsCount, MAX_SPOT_LIGHTS);
    float plc = min(pointLightsCount, MAX_POINT_LIGHTS);

    for(int i = 0; i < dlc; i++)
        resultColor += CalcDirLight(directionalLights[i], norm, viewDir);
    for(int i = 0; i < slc; i++)
        resultColor += CalcSpotLight(spotLights[i], norm, FragPos, viewDir);
    for(int i = 0; i < plc; i++)
        resultColor += CalcPointLight(pointLights[i], norm, FragPos, viewDir);

    resultColor = mix(FOG_COLOR, resultColor, fogFactor(length(FragPos)));

    FragColor = vec4(resultColor, 1.0f);
}

float fogFactor(float distance)
{
    return clamp(exp(-FOG_DENSITY*distance), 0.0, 1.0);
}

vec3 CalcDirLight(DirectionalLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    //combine results
    vec3 ambient  = material.ambient  * material.color * light.ambient * light.color;
    vec3 diffuse  = material.diffuse  * diff * material.color * light.diffuse * light.color;
    vec3 specular = material.specular * spec * material.color * light.specular * light.color;
    return (ambient + diffuse + specular);
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{

    //diffuse shading
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    //attenuation
    float distance    = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    //spotlight intensity
    float theta     = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    //combine results
    vec3 ambient  = material.ambient  * material.color * light.ambient * light.color;
    vec3 diffuse  = material.diffuse  * diff * material.color * light.diffuse * light.color;
    vec3 specular = material.specular * spec * material.color * light.specular * light.color;
    ambient  *= attenuation;
    diffuse  *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}


vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    //attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));
    //combine results
    vec3 ambient  = material.ambient  * material.color * light.ambient * light.color;
    vec3 diffuse  = material.diffuse  * diff * material.color * light.diffuse * light.color;
    vec3 specular = material.specular * spec * material.color * light.specular * light.color;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
} 