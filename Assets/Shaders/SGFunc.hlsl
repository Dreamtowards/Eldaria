

int MaxIdx(float3 v) {
    float a=v.x;
    float b=v.y;
    float c=v.z;
    return a > b ? (a > c ? 0 : 2) : (b > c ? 1 : 2);
}

//#pragma require barycentrics
	//float3 BaryCoord : SV_Barycentrics,
float mod(float v, float n)
{
	float f = v % n;
	return f < 0 ? f + n : f;
}
	
float4 TexTrip(UnityTexture2D tex, float3 p, float MtlTexId, float3 weights, float MtlTexCap, UnitySamplerState TexSampleState) 
{
	float MtlTexSizeX = 1.0 / MtlTexCap;
	float MtlTexPosX  = MtlTexId / MtlTexCap;
	float2 uvX = float2(mod(p.z * MtlTexSizeX, MtlTexSizeX) + MtlTexPosX, p.y);
	float2 uvY = float2(mod(p.x * MtlTexSizeX, MtlTexSizeX) + MtlTexPosX, p.z);
	float2 uvZ = float2(mod(p.x * MtlTexSizeX, MtlTexSizeX) + MtlTexPosX, p.y);

    return SAMPLE_TEXTURE2D(tex, TexSampleState, uvX) * weights.x +
           SAMPLE_TEXTURE2D(tex, TexSampleState, uvY) * weights.y +
           SAMPLE_TEXTURE2D(tex, TexSampleState, uvZ) * weights.z;
}

void MtlBlend_float(
	float3 MtlIds,
	float3 WorldPos,
	float3 WorldNorm,
	float3 BaryCoord,
	UnityTexture2D TexDiff,
	UnityTexture2D TexNorm,
	UnityTexture2D TexDRAM,
	UnitySamplerState TexSampleState,
	float MtlTexScale,
	float MtlTexCap,
	float MtlBlendPowTriplanar,
	float MtlBlendPowHeightmap,

	out float3 oAlbedo,
	out float3 oNormal, 
	out float oMetallic,
	out float oSmoothness,
	out float3 oEmission,
	out float oAO)
{
	int i_MaxBary = MaxIdx(BaryCoord);
	
    // use Norm AbsVal as weights
    float3 BlendTrip = pow(abs(WorldNorm), 6.0);
    BlendTrip /= dot(BlendTrip, 1.0);  // make sure the weights sum up to 1 (divide by sum of x+y+z)

	MtlIds += MtlBlendPowTriplanar;

	float3 PosTrip = WorldPos / MtlTexScale;

	oAlbedo = 
	TexTrip(TexDiff, PosTrip, MtlIds[i_MaxBary], BlendTrip, MtlTexCap, TexSampleState);
					 
	oNormal = 
	TexTrip(TexNorm, PosTrip, MtlIds[i_MaxBary], BlendTrip, MtlTexCap, TexSampleState);


	oMetallic = 0;
	oSmoothness = 0.3;
	oEmission = 0;
	oAO = 0;
}