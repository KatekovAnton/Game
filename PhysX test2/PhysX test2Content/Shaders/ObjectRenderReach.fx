uniform extern float4x4 World;
uniform extern float4x4 View;
uniform extern float4x4 Projection;

uniform extern texture DiffuseTexture;

uniform extern float3 SunDirection;
uniform extern float3x3 WorldRotation;

uniform extern float4x4 Frame[28];


const float alphaClamp = 0.6;
const float4 mask = float4(1.0/255.0,1.0/255.0,1.0/255.0,0.0);
const float4 decode_mask = float4(1.0f, 1.0/255.0, 1.0/65025.0, 1.0/16581375.0);

inline float4 FloatToFloat4( float v )
{
    float4 enc = float4(1.0, 255.0, 65025.0, 16581375.0) * v;
    enc = frac(enc);
    enc -= enc.yzww * mask;
    return enc;
}

inline float Float4ToFloat( float4 rgba )
{
    return dot( rgba, decode_mask );
}

//����
float4x4 LightViewProj;
float3 LightDirection = normalize(float3(-1, -1, -1));
float4 AmbientColor = float4(0.15, 0.15, 0.15, 0);
float DepthBias = 0.001f;


struct Vertex
{
  float4 Position : POSITION;
  float3 Normal : NORMAL;
  float2 TextureCoordinate : TEXCOORD;
};



struct SkinnedVertex
{
  float4 Position : POSITION;
  float3 Normal : NORMAL;
  float2 TextureCoordinate : TEXCOORD;
  float3 BoneIndices : BLENDINDICES;
  float3 BoneWeights : BLENDWEIGHT;
};



struct PS_INPUT
{
	float4 Position : POSITION;
	float3 Normal : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
	float4 WorldPos : TEXCOORD1;
};



sampler TextureSampler = sampler_state
{
	Texture = <DiffuseTexture>;
	mipfilter = LINEAR;
};

texture ShadowMap;
sampler ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;

};

struct CreateShadowMap_VSOut
{
    float4 Position : POSITION;
    float Depth     : TEXCOORD0;
};

CreateShadowMap_VSOut CreateShadowMap_StaticVertexShader(Vertex vertex)
{
    CreateShadowMap_VSOut Out;
    Out.Position = mul(vertex.Position, mul(World, LightViewProj)); 
    Out.Depth = Out.Position.z / Out.Position.w;    
    return Out;
}

CreateShadowMap_VSOut CreateShadowMap_SkinnedVertexShader(SkinnedVertex vertex)
{
    CreateShadowMap_VSOut Out;
    float d = 1.0 / (vertex.BoneWeights[1]+vertex.BoneWeights[0]);
    float4x4 animation = Frame[(int)vertex.BoneIndices[0]]*vertex.BoneWeights[0]*d;
    animation += Frame[(int)vertex.BoneIndices[1]]*vertex.BoneWeights[1]*d;

  
    Out.Position = mul(mul(mul(vertex.Position, animation), World), LightViewProj);
    Out.Depth = Out.Position.z / Out.Position.w;    
    return Out;
}

float4 CreateShadowMap_PixelShaderR(CreateShadowMap_VSOut input) : COLOR
{ 
    return FloatToFloat4(input.Depth);
}










//===============================VERTEX SHADER======================================

PS_INPUT StaticVertexShaderSM(Vertex vertex)
{
	PS_INPUT f = (PS_INPUT)0;
	f.Position = mul(mul(mul(vertex.Position, World), View), Projection);
	f.TextureCoordinate = vertex.TextureCoordinate;
	f.Normal = normalize(mul(vertex.Normal, (float3x3)World));  
	f.WorldPos = mul(vertex.Position, World);
	return f;
}

PS_INPUT StaticVertexShaderNoSM(Vertex vertex)
{
	PS_INPUT f = (PS_INPUT)0;
	f.Position = mul(mul(mul(vertex.Position, World), View), Projection);
	f.TextureCoordinate = vertex.TextureCoordinate;
  
	f.Normal = normalize(mul(vertex.Normal, (float3x3)World));  
	return f;
}

PS_INPUT SkinnedVertexShaderSM(SkinnedVertex vertex)
{
    PS_INPUT f = (PS_INPUT)0;

    float d = 1.0 / (vertex.BoneWeights[1]+vertex.BoneWeights[0]);
    float4x4 animation = Frame[(int)vertex.BoneIndices[0]]*vertex.BoneWeights[0];
    animation += Frame[(int)vertex.BoneIndices[1]]*vertex.BoneWeights[1];
    // animation += Frame[(int)vertex.BoneIndices[2]]*vertex.BoneWeights[2];

    f.Position = mul(mul(mul(mul(vertex.Position, animation), World), View), Projection);
    f.Normal = normalize(mul(mul(vertex.Normal,(float3x3)animation), (float3x3)World));
    f.TextureCoordinate = vertex.TextureCoordinate;

	f.WorldPos = mul(mul(vertex.Position, animation), World);
    return f;
}

PS_INPUT SkinnedVertexShaderNoSM(SkinnedVertex vertex)
{
    PS_INPUT f = (PS_INPUT)0;

    float d = 1.0 / (vertex.BoneWeights[1]+vertex.BoneWeights[0]);
    float4x4 animation = Frame[(int)vertex.BoneIndices[0]]*vertex.BoneWeights[0];
    animation += Frame[(int)vertex.BoneIndices[1]]*vertex.BoneWeights[1];
    // animation += Frame[(int)vertex.BoneIndices[2]]*vertex.BoneWeights[2];

    f.Position = mul(mul(mul(mul(vertex.Position, animation), World), View), Projection);
    f.Normal = normalize(mul(mul(vertex.Normal,(float3x3)animation), (float3x3)World));
    f.TextureCoordinate = vertex.TextureCoordinate;

    return f;
}

PS_INPUT HardwareInstancingVertexShaderNoSM(Vertex vertex,  float4x4 instanceTransform : BLENDWEIGHT)
{
    PS_INPUT f = (PS_INPUT)0;
	f.Position = mul(mul(mul(vertex.Position, instanceTransform), View), Projection);
	f.TextureCoordinate = vertex.TextureCoordinate;
  
	f.Normal = normalize(mul(vertex.Normal, (float3x3)instanceTransform));  
	return f;
}

PS_INPUT HardwareInstancingVertexShaderSM(Vertex vertex,  float4x4 instanceTransform : BLENDWEIGHT)
{
    PS_INPUT f = (PS_INPUT)0;
	f.Position = mul(mul(mul(vertex.Position, instanceTransform), View), Projection);
	f.TextureCoordinate = vertex.TextureCoordinate;
  
	f.Normal = normalize(mul(vertex.Normal, (float3x3)instanceTransform));  
	f.WorldPos = mul(vertex.Position, instanceTransform);
	return f;
}


//===============================PIXEL SHADER======================================

float4 SolidTextureNoSM(PS_INPUT f) : COLOR0
{
	float4 color = tex2D(TextureSampler, f.TextureCoordinate);
	if(color.a < alphaClamp)
		discard;

	float shadowintens = 0.4f;
	float minclamp = 0.1;
	


	float shadowcoeff =0;
	float lambertfactor = clamp( dot( -f.Normal,normalize(LightDirection)),minclamp,1.0f)+shadowintens - minclamp;

	lambertfactor*=1.1;
    
    float3 color1 = color.rgb* lambertfactor;
    return float4(color1, color.a);

}

float4 SolidTextureSelfIll(PS_INPUT f) : COLOR0
{
	float4 color = tex2D(TextureSampler, f.TextureCoordinate);
	if(color.a < alphaClamp)
		discard;
	//color *= color.a;
	return color;
}

float4 SolidTextureSMR(PS_INPUT f) : COLOR0
{
	float4 color = tex2D(TextureSampler, f.TextureCoordinate);
	if(color.a < alphaClamp)
		discard;

	float4 lightingPosition = mul(f.WorldPos, LightViewProj);
	float2 ShadowTexCoord = 0.5 * lightingPosition.xy / lightingPosition.w + float2( 0.5, 0.5 );
	ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;
	float shadowdepth =Float4ToFloat( tex2D(ShadowMapSampler, ShadowTexCoord));    
	float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;
	


	float shadowintens = 0.4f;
	float minclamp = 0.1;
	


	float shadowcoeff =0;
	float lambertfactor = clamp( dot( -f.Normal,normalize(LightDirection)),minclamp,1.0f)+shadowintens - minclamp;

	if (shadowdepth < ourdepth)
	{
		shadowcoeff = 0.15;
		if(lambertfactor !=shadowintens)
			lambertfactor = shadowintens;
	}
	lambertfactor *= 1.1;
	float3 color1 = color.rgb * lambertfactor;
	return float4(color1, color.a);
}
//=================================================================================





//////////////////////////////////////////////////////////////////////////////////////
////�������� ���������
technique CreateStaticShadowMapR
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 CreateShadowMap_StaticVertexShader();
        PixelShader = compile ps_2_0 CreateShadowMap_PixelShaderR();
    }
}

technique CreateAnimShadowMapR
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 CreateShadowMap_SkinnedVertexShader();
        PixelShader = compile ps_2_0 CreateShadowMap_PixelShaderR();
    }
}

//////////////////////////////////////////////////////////////////////////////////////
//������ ��� ���������
technique AnimRenderNoSM
{
  pass P0
  {
    VertexShader = compile vs_2_0 SkinnedVertexShaderNoSM();
    PixelShader = compile ps_2_0 SolidTextureNoSM();
  }
}

technique NotAnimRenderNoSM
{
  pass P0
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureNoSM();
  }
}

technique TransparentNoSM
{
  pass Pass1
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureNoSM();
  }
}

technique InstancedNoSM
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 HardwareInstancingVertexShaderNoSM();
        PixelShader = compile ps_2_0 SolidTextureNoSM();
    }
}

technique InstancedTransparentNoSM
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 HardwareInstancingVertexShaderNoSM();
        PixelShader = compile ps_2_0 SolidTextureNoSM();
    }
}

//////////////////////////////////////////////////////////////////////////////////////
//������ � ������������ ����������
technique AnimRenderSMR
{
  pass P0
  {
    VertexShader = compile vs_2_0 SkinnedVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSMR();
  }
}

technique NotAnimRenderSMR
{
  pass P0
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSMR();
  }
}

technique TransparentSMR
{
  pass P0
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSMR();
  }
}

technique InstancedSMR
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 HardwareInstancingVertexShaderSM();
        PixelShader = compile ps_2_0 SolidTextureSMR();
    }
}

technique InstancedTransparentSMR
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 HardwareInstancingVertexShaderSM();
        PixelShader = compile ps_2_0 SolidTextureSMR();
    }
}

//////////////////////////////////////////////////////////////////////////////////////
//������ �������� ���������� ��������
technique TransparentSelfIlmnNoSM
{
  pass Pass1
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSelfIll();
  }
}

technique InstancedTransparentSelfIlmnNoSM
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 HardwareInstancingVertexShaderNoSM();
        PixelShader = compile ps_2_0 SolidTextureSelfIll();
    }
}