// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/MKGlowFastBlur" {
Properties {
 _MainTex ("", 2D) = "white" { }
 _Color ("Color", Color) = (1.000000,1.000000,1.000000,0.000000)
}
SubShader { 
 Pass {
  ZWrite Off
  Cull Off
  GpuProgramID 7054
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float _Shift;
uniform float4 _MainTex_TexelSize;
uniform sampler2D _MainTex;
uniform float4 _Color;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD0_1 :TEXCOORD0_1;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD0_1 :TEXCOORD0_1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float offY_1;
    float offX_2;
    float4 tmpvar_3;
    float4 tmpvar_4;
    float tmpvar_5;
    tmpvar_5 = (_MainTex_TexelSize.x * _Shift);
    offX_2 = tmpvar_5;
    float tmpvar_6;
    tmpvar_6 = (_MainTex_TexelSize.y * _Shift);
    offY_1 = tmpvar_6;
    float2 tmpvar_7;
    tmpvar_7.x = offX_2;
    tmpvar_7.y = offY_1;
    tmpvar_3.xy = (in_v.texcoord.xy + tmpvar_7);
    float2 tmpvar_8;
    tmpvar_8.x = (-offX_2);
    tmpvar_8.y = offY_1;
    tmpvar_3.zw = (in_v.texcoord.xy + tmpvar_8);
    float2 tmpvar_9;
    tmpvar_9.x = offX_2;
    tmpvar_9.y = (-offY_1);
    tmpvar_4.xy = (in_v.texcoord.xy + tmpvar_9);
    float2 tmpvar_10;
    tmpvar_10.x = (-offX_2);
    tmpvar_10.y = (-offY_1);
    tmpvar_4.zw = (in_v.texcoord.xy + tmpvar_10);
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_3;
    out_v.xlv_TEXCOORD0_1 = tmpvar_4;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 c_1;
    c_1 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy) + tex2D(_MainTex, in_f.xlv_TEXCOORD0.zw));
    c_1 = (c_1 + tex2D(_MainTex, in_f.xlv_TEXCOORD0_1.xy));
    c_1 = (c_1 + tex2D(_MainTex, in_f.xlv_TEXCOORD0_1.zw));
    float4 tmpvar_2;
    tmpvar_2 = (c_1 * _Color.w);
    out_f.color = tmpvar_2;
    return out_f;
}


ENDCG

}
}
Fallback Off
}