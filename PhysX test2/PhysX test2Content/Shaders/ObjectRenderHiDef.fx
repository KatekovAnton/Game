uniform extern float4x4 World;
uniform extern float4x4 View;
uniform extern float4x4 Projection;

uniform extern texture DiffuseTexture;

uniform extern float3 SunDirection;
uniform extern float3x3 WorldRotation;

uniform extern float4x4 Frame[28];

const float alphaClamp = 0.6;

//ремх
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

float4 CreateShadowMap_PixelShader(CreateShadowMap_VSOut input) : COLOR
{ 
    return float4(input.Depth, 0, 0, 1);
}
//////////////////////////////////////////////////////////////////////////////////////////
PS_INPUT MainVertexShaderNoSM(Vertex vertex, float4x4 _matrix)
{
	PS_INPUT f = (PS_INPUT)0;
	f.Position = mul(mul(mul(vertex.Position, _matrix), View), Projection);
	f.TextureCoordinate = vertex.TextureCoordinate;
  
	f.Normal = normalize(mul(vertex.Normal, (float3x3)_matrix));
	return f;
}
PS_INPUT MainVertexShaderSM(Vertex vertex, float4x4 _matrix)
{
	PS_INPUT f = (PS_INPUT)0;
	f.Position = mul(mul(mul(vertex.Position, _matrix), View), Projection);
	f.TextureCoordinate = vertex.TextureCoordinate;
  
	f.Normal = normalize(mul(vertex.Normal, (float3x3)_matrix));
	f.WorldPos = mul(vertex.Position, _matrix);
	return f;
}
///////////////////////////////////////////////////////////////////////
PS_INPUT MainAnimVertexShaderNoSM(SkinnedVertex vertex, float4x4 _matrix)
{
	PS_INPUT f = (PS_INPUT)0;

    float d = 1.0 / (vertex.BoneWeights[1]+vertex.BoneWeights[0]);
    float4x4 animation = Frame[(int)vertex.BoneIndices[0]]*vertex.BoneWeights[0];
    animation += Frame[(int)vertex.BoneIndices[1]]*vertex.BoneWeights[1];
    // animation += Frame[(int)vertex.BoneIndices[2]]*vertex.BoneWeights[2];

    f.Position = mul(mul(mul(mul(vertex.Position, animation), _matrix), View), Projection);
    f.Normal = normalize(mul(mul(vertex.Normal,(float3x3)animation), (float3x3)World));
    f.TextureCoordinate = vertex.TextureCoordinate;


	return f;
}
PS_INPUT MainAnimVertexShaderSM(SkinnedVertex vertex, float4x4 _matrix)
{
	PS_INPUT f = (PS_INPUT)0;

    float d = 1.0 / (vertex.BoneWeights[1]+vertex.BoneWeights[0]);
    float4x4 animation = Frame[(int)vertex.BoneIndices[0]]*vertex.BoneWeights[0];
    animation += Frame[(int)vertex.BoneIndices[1]]*vertex.BoneWeights[1];
    // animation += Frame[(int)vertex.BoneIndices[2]]*vertex.BoneWeights[2];

    f.Position = mul(mul(mul(mul(vertex.Position, animation), _matrix), View), Projection);
    f.Normal = normalize(mul(mul(vertex.Normal,(float3x3)animation), (float3x3)World));
    f.TextureCoordinate = vertex.TextureCoordinate;

	f.WorldPos = mul(mul(vertex.Position, animation), _matrix);
	return f;
}
//////////////////////////////////////////////////////////////////////
PS_INPUT StaticVertexShaderSM(Vertex vertex)
{
	return MainVertexShaderSM(vertex,World);
}

PS_INPUT StaticVertexShaderNoSM(Vertex vertex)
{ 
	return MainVertexShaderNoSM(vertex,World);
}

PS_INPUT SkinnedVertexShaderSM(SkinnedVertex vertex)
{
    return MainAnimVertexShaderSM(vertex,World);
}

PS_INPUT SkinnedVertexShaderNoSM(SkinnedVertex vertex)
{
    return MainAnimVertexShaderNoSM(vertex,World);
}

PS_INPUT HardwareInstancingVertexShaderSM(Vertex vertex, float4x4 instanceTransform : BLENDWEIGHT)
{
	return MainVertexShaderSM(vertex,transpose(instanceTransform));
}

PS_INPUT HardwareInstancingVertexShaderNoSM(Vertex vertex, float4x4 instanceTransform : BLENDWEIGHT)
{
    return MainVertexShaderNoSM(vertex,transpose(instanceTransform));
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
	//color *=color.a;
	return color;
}

float4 SolidTextureSM(PS_INPUT f) : COLOR0
{
	float4 color = tex2D(TextureSampler, f.TextureCoordinate);
	if(color.a < alphaClamp)
		discard;

	float4 lightingPosition = mul(f.WorldPos, LightViewProj);
	float2 ShadowTexCoord = 0.5 * lightingPosition.xy / lightingPosition.w + float2(0.5, 0.5);
	ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;
	float shadowdepth = tex2D(ShadowMapSampler, ShadowTexCoord).r;    
	float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;
	


	float shadowintens = 0.4;
	float minclamp = 0.1;
	


	float shadowcoeff = 0;
	float lambertfactor = clamp(dot(-f.Normal,normalize(LightDirection)), minclamp, 1.0f) + shadowintens - minclamp;

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

float4 SolidTextureSMSmooth(PS_INPUT f) : COLOR0
{
	float4 color = tex2D(TextureSampler, f.TextureCoordinate);
	if(color.a < alphaClamp)
		discard;
	float4 lightingPosition = mul(f.WorldPos, LightViewProj);
	float2 ShadowTexCoord = 0.5 * lightingPosition.xy / lightingPosition.w + float2( 0.5, 0.5 );
	ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;
	float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;
	


	float shadowintens = 0.4f;
	float minclamp = 0.1;
	


	float shadowcoeff =0;
	float lambertfactor = clamp(dot(-f.Normal,normalize(LightDirection)), minclamp, 1.0f)+shadowintens - minclamp;


	if(tex2D(ShadowMapSampler, ShadowTexCoord).r < ourdepth)
		shadowcoeff = 0.35;
	if(tex2D(ShadowMapSampler, ShadowTexCoord + float2( -1.0/2048.0, -1.0/2048.0)).x < ourdepth)
		shadowcoeff += 0.0625;
	if(tex2D(ShadowMapSampler, ShadowTexCoord + float2( -1.0/2048.0, 0)).x < ourdepth)	
		shadowcoeff += 0.125;
	if(tex2D(ShadowMapSampler, ShadowTexCoord + float2( -1.0/2048.0, 1.0/2048.0)).x < ourdepth)
		shadowcoeff += 0.0625;
	if(tex2D(ShadowMapSampler, ShadowTexCoord + float2( 0, -1.0/2048.0)).x < ourdepth)
		shadowcoeff += 0.125;
	if(tex2D(ShadowMapSampler, ShadowTexCoord + float2( 0, 1.0/2048.0)).x < ourdepth)
		shadowcoeff += 0.125;
	if(tex2D(ShadowMapSampler, ShadowTexCoord + float2( 1.0/2048.0, -1.0/2048.0)).x < ourdepth)
		shadowcoeff += 0.0625;
	if(tex2D(ShadowMapSampler, ShadowTexCoord + float2( 1.0/2048.0, 0)).x < ourdepth)
		shadowcoeff += 0.125;
	if(tex2D(ShadowMapSampler, ShadowTexCoord + float2( 1, 1.0/2048.0)).x < ourdepth)
		shadowcoeff += 0.0625;
	

	shadowcoeff = 0.95 - shadowcoeff * shadowintens;


	if(shadowcoeff != 1.0 && lambertfactor > shadowcoeff)
		lambertfactor = shadowcoeff;


	lambertfactor *= 1.1;

	float3 color1 = color.rgb * lambertfactor;
	return float4(color1, color.a);
}
//=================================================================================


//////////////////////////////////////////////////////////////////////////////////////
//янгдюмхе ьл
technique CreateStaticShadowMap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 CreateShadowMap_StaticVertexShader();
        PixelShader = compile ps_2_0 CreateShadowMap_PixelShader();
    }
}

technique CreateAnimShadowMap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 CreateShadowMap_SkinnedVertexShader();
        PixelShader = compile ps_2_0 CreateShadowMap_PixelShader();
    }
}

//////////////////////////////////////////////////////////////////////////////////////
//пемдеп аег ьюднблюош
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
    VertexShader = compile vs_2_0 StaticVertexShaderNoSM();
    PixelShader = compile ps_2_0 SolidTextureNoSM();
  }
}

technique TransparentNoSM
{
  pass Pass1
  {
    VertexShader = compile vs_2_0 StaticVertexShaderNoSM();
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
//пемдеп ян яцкюфеммни ьюднблюони
technique AnimRenderSMSmooth
{
  pass P0
  {
    VertexShader = compile vs_2_0 SkinnedVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSMSmooth();
  }
}

technique NotAnimRenderSMSmooth
{
  pass P0
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSMSmooth();
  }
}

technique TransparentSMSmooth
{
  pass Pass1
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSMSmooth();
  }
}

technique InstancedSMSmooth
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 HardwareInstancingVertexShaderSM();
        PixelShader = compile ps_3_0 SolidTextureSMSmooth();
    }
}

technique InstancedTransparentSMSmooth
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 HardwareInstancingVertexShaderSM();
        PixelShader = compile ps_3_0 SolidTextureSMSmooth();
    }
}

//////////////////////////////////////////////////////////////////////////////////////
//пемдеп я меяцкюфеммни ьюднблюони
technique AnimRenderSM
{
  pass P0
  {
    VertexShader = compile vs_2_0 SkinnedVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSM();
  }
}

technique NotAnimRenderSM
{
  pass P0
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSM();
  }
}

technique TransparentSM
{
  pass Pass1
  {
    VertexShader = compile vs_2_0 StaticVertexShaderSM();
    PixelShader = compile ps_2_0 SolidTextureSM();
  }
}

technique InstancedSM
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 HardwareInstancingVertexShaderSM();
        PixelShader = compile ps_3_0 SolidTextureSM();
    }
}

technique InstancedTransparentSM
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 HardwareInstancingVertexShaderSM();
        PixelShader = compile ps_3_0 SolidTextureSM();
    }
}

//////////////////////////////////////////////////////////////////////////////////////
//пемдеп опнгпювмшу яюлнябер назейрнб
technique TransparentSelfIlmnNoSM
{
  pass Pass1
  {
    VertexShader = compile vs_2_0 StaticVertexShaderNoSM();
    PixelShader = compile ps_2_0 SolidTextureSelfIll();
  }
}

technique InstancedTransparentSelfIlmnNoSM
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 HardwareInstancingVertexShaderNoSM();
        PixelShader = compile ps_3_0 SolidTextureSelfIll();
    }
}

//////////////////////////////////////////////////////////////////////////////////////