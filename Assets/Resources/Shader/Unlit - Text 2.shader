// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Unlit/Text 2" {
Properties {
 _MainTex ("Alpha (A)", 2D) = "white" { }
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1, -1
  GpuProgramID 16136
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _ClipRange0;
uniform float4 _ClipRange1;
uniform float4 _ClipArgs1;
uniform sampler2D _MainTex;
uniform float4 _ClipArgs0;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_COLOR :COLOR;
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR :COLOR;
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    tmpvar_1.xy = ((in_v.vertex.xy * _ClipRange0.zw) + _ClipRange0.xy);
    float2 ret_2;
    ret_2.x = ((in_v.vertex.x * _ClipArgs1.w) - (in_v.vertex.y * _ClipArgs1.z));
    ret_2.y = ((in_v.vertex.x * _ClipArgs1.z) + (in_v.vertex.y * _ClipArgs1.w));
    tmpvar_1.zw = ((ret_2 * _ClipRange1.zw) + _ClipRange1.xy);
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = in_v.color;
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    out_v.xlv_TEXCOORD1 = tmpvar_1;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 col_1;
    float2 factor_2;
    float2 tmpvar_3;
    tmpvar_3 = ((float2(1, 1) - abs(in_f.xlv_TEXCOORD1.xy)) * _ClipArgs0.xy);
    factor_2 = ((float2(1, 1) - abs(in_f.xlv_TEXCOORD1.zw)) * _ClipArgs1.xy);
    col_1.xyz = in_f.xlv_COLOR.xyz;
    float4 tmpvar_4;
    tmpvar_4 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    col_1.w = (in_f.xlv_COLOR.w * tmpvar_4.w);
    float tmpvar_5;
    tmpvar_5 = clamp(min(min(tmpvar_3.x, tmpvar_3.y), min(factor_2.x, factor_2.y)), 0, 1);
    col_1.w = (col_1.w * tmpvar_5);
    out_f.color = col_1;
    return out_f;
}


ENDCG

}
}
Fallback "Unlit/Text"
}