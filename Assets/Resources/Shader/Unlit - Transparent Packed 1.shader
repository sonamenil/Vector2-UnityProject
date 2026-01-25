// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Unlit/Transparent Packed 1" {
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
  GpuProgramID 17202
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _ClipRange0;
uniform sampler2D _MainTex;
uniform float2 _ClipArgs0;
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
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR :COLOR;
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = in_v.color;
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    out_v.xlv_TEXCOORD1 = ((in_v.vertex.xy * _ClipRange0.zw) + _ClipRange0.xy);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 col_1;
    float4 mask_2;
    float4 tmpvar_3;
    tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    mask_2 = tmpvar_3;
    float4 tmpvar_4;
    tmpvar_4 = clamp(ceil((in_f.xlv_COLOR - 0.5)), 0, 1);
    float4 tmpvar_5;
    tmpvar_5 = clamp((((tmpvar_4 * 0.51) - in_f.xlv_COLOR) / (-0.49)), 0, 1);
    col_1.xyz = tmpvar_5.xyz;
    float2 tmpvar_6;
    tmpvar_6 = ((float2(1, 1) - abs(in_f.xlv_TEXCOORD1)) * _ClipArgs0);
    mask_2 = (mask_2 * tmpvar_4);
    float tmpvar_7;
    tmpvar_7 = clamp(min(tmpvar_6.x, tmpvar_6.y), 0, 1);
    col_1.w = (tmpvar_5.w * tmpvar_7);
    col_1.w = (col_1.w * ((mask_2.x + mask_2.y) + (mask_2.z + mask_2.w)));
    out_f.color = col_1;
    return out_f;
}


ENDCG

}
}
Fallback Off
}