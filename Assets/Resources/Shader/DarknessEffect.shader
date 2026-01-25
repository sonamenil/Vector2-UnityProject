// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nekki/Vector/Darkness" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Range ("Range", Float) = 0.500000
 _CenterX ("CenterX", Float) = 0.500000
 _CenterY ("CenterY", Float) = 0.500000
 _Alpha ("Alpha", Float) = 0.500000
 _ScaleX ("ScaleX", Float) = 1.000000
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 26562
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform sampler2D _MainTex;
uniform float _Range;
uniform float _CenterX;
uniform float _CenterY;
uniform float _Alpha;
uniform float _ScaleX;
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
    float4 darknessCol_1;
    float2 dp_2;
    float2 tmpvar_3;
    tmpvar_3.x = (_CenterX * _ScaleX);
    tmpvar_3.y = _CenterY;
    dp_2 = in_f.xlv_TEXCOORD0;
    dp_2.x = (dp_2.x * _ScaleX);
    float2 tmpvar_4;
    tmpvar_4 = (tmpvar_3 - dp_2);
    float tmpvar_5;
    tmpvar_5 = clamp(((sqrt(dot(tmpvar_4, tmpvar_4)) - _Range) / (-_Range)), 0, 1);
    float tmpvar_6;
    tmpvar_6 = max((tmpvar_5 * (tmpvar_5 * (3 - (2 * tmpvar_5)))), _Alpha);
    float4 tmpvar_7;
    tmpvar_7.x = tmpvar_6;
    tmpvar_7.y = tmpvar_6;
    tmpvar_7.z = tmpvar_6;
    tmpvar_7.w = _Alpha;
    darknessCol_1 = tmpvar_7;
    float4 tmpvar_8;
    tmpvar_8 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * darknessCol_1);
    out_f.color = tmpvar_8;
    return out_f;
}


ENDCG

}
}
Fallback Off
}