#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D Texture;

float2 mousePos;      // Mouse position in screen space (normalized: [0,1])
float  circleRadius;  // Radius of the darkened circle (normalized: [0,1])
float4x4 view_projection; // Matrix to transform vertex positions
float aspectRatio; // Aspect ratio of the screen (width / height)

struct VS_INPUT
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct PS_INPUT
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

PS_INPUT VS_Main(VS_INPUT input)
{
    PS_INPUT output;

    output.Position = mul(input.Position, view_projection);
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;
    return output;
}

sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
};

float4 PS_Main(PS_INPUT input) : SV_TARGET
{
    // uv coordinates of texture
    float2 uv = input.TexCoord;
    uv.x *= aspectRatio; // Adjust for aspect ratio if needed

    float2 adjustedMousePos = mousePos;
    adjustedMousePos.x *= aspectRatio; // Adjust for aspect ratio if needed

    // Calculate distance from current pixel to mouse position
    float dist = distance(uv, adjustedMousePos);

    float4 color = tex2D(TextureSampler, uv) * input.Color;

    if (dist < circleRadius)
    {
        color = float4(color.rgb, 0);
    }

    return color;
}

technique CircleIndicator
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_Main();
        PixelShader = compile PS_SHADERMODEL PS_Main();
    }
};