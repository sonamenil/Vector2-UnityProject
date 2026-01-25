// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nekki/Vector/Grayscale Effect" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _RampTex ("Base (RGB)", 2D) = "grayscaleRamp" { }
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 48511
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform sampler2D _MainTex;
uniform sampler2D _RampTex;
uniform float _RampOffset;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    tmpvar_1.w = 1;
    tmpvar_1.xyz = in_v.vertex.xyz;
    out_v.vertex = UnityObjectToClipPos(tmpvar_1);
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 xlat_varoutput_1;
    float grayscale_2;
    float4 tmpvar_3;
    tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    float3 c_4;
    c_4 = tmpvar_3.xyz;
    float tmpvar_5;
    tmpvar_5 = dot(c_4, unity_ColorSpaceLuminance.xyz);
    grayscale_2 = tmpvar_5;
    float2 tmpvar_6;
    tmpvar_6.y = 0.5;
    tmpvar_6.x = (grayscale_2 + _RampOffset);
    xlat_varoutput_1.xyz = tex2D(_RampTex, tmpvar_6).xyz;
    xlat_varoutput_1.w = tmpvar_3.w;
    out_f.color = xlat_varoutput_1;
    return out_f;
}


ENDCG

}
}
Fallback Off
}