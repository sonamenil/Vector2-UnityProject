// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GUI/RoundHoleGradient" {
Properties {
 _Color ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
 _HoleRadius ("HoleRadius", Range(0.000000,0.500000)) = 0.500000
 _Alpha ("Alpha", Range(0.000000,1.000000)) = 1.000000
 _Intensive ("Intensive", Range(0.000000,1.000000)) = 0.500000
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 8426
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
uniform float _Alpha;
uniform float _HoleRadius;
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
    float2 tmpvar_1;
    tmpvar_1 = in_v.texcoord.xy;
    float2 tmpvar_2;
    tmpvar_2 = tmpvar_1;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_2;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
float4 xlat_mutable_Color;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    xlat_mutable_Color.xyz = _Color.xyz;
    float4 tmpvar_1;
    float Distance_2;
    float tmpvar_3;
    float2 tmpvar_4;
    tmpvar_4 = (float2(0.5, 0.5) - in_f.xlv_TEXCOORD0);
    tmpvar_3 = sqrt(dot(tmpvar_4, tmpvar_4));
    Distance_2 = tmpvar_3;
    if((Distance_2<_HoleRadius))
    {
        discard;
    }
    if((Distance_2>0.5))
    {
        discard;
    }
    xlat_mutable_Color.w = (((0.5 - Distance_2) / (0.5 - _HoleRadius)) * _Alpha);
    xlat_mutable_Color.xyz = (_Color.xyz * xlat_mutable_Color.w);
    tmpvar_1 = xlat_mutable_Color;
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
}