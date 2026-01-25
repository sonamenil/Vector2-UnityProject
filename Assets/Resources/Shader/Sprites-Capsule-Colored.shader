// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Capsule-Colored" {
Properties {
 _Color ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
 _Radius ("Radius", Float) = 1.000000
 _Height ("Height", Float) = 1.000000
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  Cull Off
  Blend One OneMinusSrcAlpha
  GpuProgramID 59176
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
uniform float _Radius;
uniform float _Height;
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
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    int tmpvar_1;
    tmpvar_1 = int(1);
    float4 tmpvar_2;
    float tmpvar_3;
    tmpvar_3 = (_Radius * (1 / ((2 * _Radius) + _Height)));
    if((in_f.xlv_TEXCOORD0.y<=tmpvar_3))
    {
        float2 tmpvar_4;
        tmpvar_4.x = in_f.xlv_TEXCOORD0.x;
        tmpvar_4.y = ((in_f.xlv_TEXCOORD0.y * 0.5) / tmpvar_3);
        float tmpvar_5;
        float2 tmpvar_6;
        tmpvar_6 = (float2(0.5, 0.5) - tmpvar_4);
        tmpvar_5 = sqrt(dot(tmpvar_6, tmpvar_6));
        if((tmpvar_5>0.5))
        {
            discard;
        }
        else
        {
            tmpvar_2 = _Color;
            tmpvar_1 = int(0);
        }
    }
    else
    {
        if((in_f.xlv_TEXCOORD0.y>=(1 - tmpvar_3)))
        {
            float2 tmpvar_7;
            tmpvar_7.x = in_f.xlv_TEXCOORD0.x;
            tmpvar_7.y = (((1 - in_f.xlv_TEXCOORD0.y) * 0.5) / tmpvar_3);
            float tmpvar_8;
            float2 tmpvar_9;
            tmpvar_9 = (float2(0.5, 0.5) - tmpvar_7);
            tmpvar_8 = sqrt(dot(tmpvar_9, tmpvar_9));
            if((tmpvar_8>0.5))
            {
                discard;
            }
            else
            {
                tmpvar_2 = _Color;
                tmpvar_1 = int(0);
            }
        }
        else
        {
            tmpvar_2 = _Color;
            tmpvar_1 = int(0);
        }
    }
    if(tmpvar_1)
    {
        tmpvar_2 = float4(1, 1, 1, 1);
        tmpvar_1 = int(0);
    }
    out_f.color = tmpvar_2;
    return out_f;
}


ENDCG

}
}
}