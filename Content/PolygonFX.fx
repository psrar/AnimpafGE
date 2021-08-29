#if OPENGL
#define SV_POSITION POSITION
#define PS_SHADERMODEL ps_3_0
#else
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;
const uniform float2 dimensions;
const uniform float2 vertices[12];
const uniform int verticesCount;

int isLeftOfLine(float2 pos, float2 linePoint1, float2 linePoint2)
{
    float2 lineDirection = linePoint2 - linePoint1;
    float2 lineNormal = float2(-lineDirection.y, lineDirection.x);
    float2 toPos = pos - linePoint1;
    float side = dot(toPos, lineNormal);
    if (side <= 0)
        return 1;
    else
        return 0;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    if (verticesCount == 0)
        return 0;
    else
    {
        float2 texCoords = coords * dimensions;
        int result = 0;
    
        for (int i = 0; i < verticesCount - 1; i++)
            result += isLeftOfLine(texCoords, vertices[i], vertices[i + 1]);

        result += isLeftOfLine(texCoords, vertices[verticesCount - 1], vertices[0]);
    
        if (result == verticesCount)
            return 1;
        else
            return 0;
    }
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}