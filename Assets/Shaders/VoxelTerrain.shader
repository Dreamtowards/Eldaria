Shader "Eldaria/VoxelTerrain"
{
    Properties
    {
        _TexDiff ("Diffuse Atlas Map", 2D) = "white" {}
        _TexNorm ("Normal Atlas Map", 2D) = "white" {}
        _TexDRAM ("DRAM Atlas Map (Disp, Rough, AO, Metal)", 2D) = "white" {}
        _MtlTexScale ("MtlTexScale", Range(0,10)) = 2.4
        _MtlTexCap ("MtlTexCap", Range(0, 100)) = 2
        _MtlTriplanarBlendPow ("MtlTriplanarBlendPow", Range(0, 10)) = 6.0
        _MtlHeightmapBlendPow ("MtlHeightmapBlendPow", Range(0, 10)) = 0.6
    }
    SubShader
    {
        //Pass
        //{
            //Tags { "LightMode" = "ForwardBase" }
            //Tags { "RenderType" = "Opaque" } 
            Tags { "LightMode" = "UniversalForward" }
            Tags { "Queue" = "Transparent" }

            
            //#include "UnityCG.cginc" 

            //sampler2D _TexDiff;
            //float _MtlTexScale;
            //float _MtlTriplanarBlendPow; 
            
        
        //}
        //v2f vert(float4 in_pos : POSITION, float2 in_uv : TEXCOORD0, float3 in_norm : NORMAL)
        //{
        //    v2f o;
        //    o.ClipPos = UnityObjectToClipPos(in_pos);
        //    o.WorldPos = mul(unity_ObjectToWorld, in_pos).xyz;

        //    o.WorldNorm = UnityObjectToWorldNormal(in_norm);
                
        //    o.MtlId = in_uv.x;
        //    return o;
        //}

        //float4 TexTrip(sampler2D tex, float3 p, float3 weights) 
        //{
        //    return tex2D(tex, p.zy) * weights.x +
        //           tex2D(tex, p.xz) * weights.y +
        //           tex2D(tex, p.xy) * weights.z;
        //}

        //float4 surf(v2f i) : SV_Target 
        //{
        //    float4 Albedo = 0;

        //    //c.xyz = i.WorldNorm;// * 0.5 + 0.5;

        //    // use Norm AbsVal as weights
        //    float3 tripBlend = pow(abs(i.WorldNorm), _MtlTriplanarBlendPow);
        //    tripBlend /= dot(tripBlend, 1.0);  // make sure the weights sum up to 1 (divide by sum of x+y+z)

        //    Albedo.xyz = TexTrip(_TexDiff, i.WorldPos / _MtlTexScale, tripBlend);
        //    Albedo.w = 1.0;

        //    return Albedo;//float4(i.pos.x, i.pos.z, 1, 1);
        //}
    }
}




// SurfaceShader
            //CGPROGRAM
            //#pragma surface surf Standard 

            //struct Input 
            //{
            //    float4 color : COLOR; 
            //    //float3 worldPos; 
            //    //float3 worldNorm;
            //};
            
            //void surf(Input IN, inout SurfaceOutputStandard o)
            //{
            //    o.Albedo = 0.6; 
            //    // o.Normal;
            //    // o.Emission;
            //    // o.Specular;
            //    // o.Gloss;
            //    // o.Alpha;
            //}

            //ENDCG