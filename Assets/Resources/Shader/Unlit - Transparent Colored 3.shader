// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Unlit/Transparent Colored 3" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 20465
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
uniform float4 _ClipRange2;
uniform float4 _ClipArgs2;
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
    float2 xlv_TEXCOORD2 :TEXCOORD2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR :COLOR;
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD2 :TEXCOORD2;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

float4 tmpvar_1;
OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    tmpvar_1.xy = ((in_v.vertex.xy * _ClipRange0.zw) + _ClipRange0.xy);
    float2 ret_2;
    ret_2.x = ((in_v.vertex.x * _ClipArgs1.w) - (in_v.vertex.y * _ClipArgs1.z));
    ret_2.y = ((in_v.vertex.x * _ClipArgs1.z) + (in_v.vertex.y * _ClipArgs1.w));
    tmpvar_1.zw = ((ret_2 * _ClipRange1.zw) + _ClipRange1.xy);
    float2 ret_3;
    ret_3.x = ((in_v.vertex.x * _ClipArgs2.w) - (in_v.vertex.y * _ClipArgs2.z));
    ret_3.y = ((in_v.vertex.x * _ClipArgs2.z) + (in_v.vertex.y * _ClipArgs2.w));
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = in_v.color;
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    out_v.xlv_TEXCOORD1 = tmpvar_1;
    out_v.xlv_TEXCOORD2 = ((ret_3 * _ClipRange2.zw) + _ClipRange2.xy);
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
    float tmpvar_4;
    tmpvar_4 = min(min(tmpvar_3.x, tmpvar_3.y), min(factor_2.x, factor_2.y));
    factor_2 = ((float2(1, 1) - abs(in_f.xlv_TEXCOORD2)) * _ClipArgs2.xy);
    float4 tmpvar_5;
    tmpvar_5 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    float4 tmpvar_6;
    tmpvar_6 = (tmpvar_5 * in_f.xlv_COLOR);
    col_1.xyz = tmpvar_6.xyz;
    float tmpvar_7;
    tmpvar_7 = clamp(min(tmpvar_4, min(factor_2.x, factor_2.y)), 0, 1);
    col_1.w = (tmpvar_6.w * tmpvar_7);
    out_f.color = col_1;
    return out_f;
}


ENDCG

}
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  GpuProgramID 71517
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_COLOR0 :COLOR0;
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR0 :COLOR0;
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
    float4 tmpvar_2;
    tmpvar_2 = clamp(in_v.color, 0, 1);
    tmpvar_1 = tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.w = 1;
    tmpvar_3.xyz = in_v.vertex.xyz;
    out_v.xlv_COLOR0 = tmpvar_1;
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.vertex = UnityObjectToClipPos(tmpvar_3);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    tmpvar_1 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * in_f.xlv_COLOR0);
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
}