// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Circle-Colored" {
Properties {
 _Color ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
 _BackgoundColor ("BackgoundColor", Color) = (0.000000,0.000000,0.000000,1.000000)
 _Radius ("Radius", Float) = 1.000000
 _Border ("Border", Float) = 0.000000
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  Cull Off
  Blend One OneMinusSrcAlpha
  GpuProgramID 32374
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
uniform float4 _BackgoundColor;
uniform float _Radius;
uniform float _Border;
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
float4 xlat_mutable_BackgoundColor;
float4 xlat_mutable_Color;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    xlat_mutable_BackgoundColor = _BackgoundColor;
    xlat_mutable_Color = _Color;
    int tmpvar_1;
    tmpvar_1 = int(1);
    float4 tmpvar_2;
    float Distance_3;
    float tmpvar_4;
    tmpvar_4 = (_Border / _Radius);
    float tmpvar_5;
    float2 tmpvar_6;
    tmpvar_6 = (float2(0.5, 0.5) - in_f.xlv_TEXCOORD0);
    tmpvar_5 = sqrt(dot(tmpvar_6, tmpvar_6));
    Distance_3 = tmpvar_5;
    if((Distance_3>0.5))
    {
        discard;
    }
    else
    {
        if((Distance_3>(0.5 - tmpvar_4)))
        {
            xlat_mutable_Color.xyz = (_Color.xyz * _Color.w);
            tmpvar_2 = xlat_mutable_Color;
            tmpvar_1 = int(0);
        }
    }
    if(tmpvar_1)
    {
        xlat_mutable_BackgoundColor.xyz = (_BackgoundColor.xyz * _BackgoundColor.w);
        tmpvar_2 = xlat_mutable_BackgoundColor;
        tmpvar_1 = int(0);
    }
    out_f.color = tmpvar_2;
    return out_f;
}


ENDCG

}
}
}