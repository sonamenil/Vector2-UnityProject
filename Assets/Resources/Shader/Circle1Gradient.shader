// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GUI/Circle1Gradient" {
Properties {
 _Color ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
 _ColorA ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
 _ColorB ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
 _Param ("Param", Range(0.000000,1.000000)) = 0.500000
 _LeftBorder ("Left border", Range(0.000000,1.000000)) = 0.000000
 _RightBorder ("Right border", Range(0.000000,1.000000)) = 1.000000
 _CenterGradientMin ("CenterGradientMin", Range(0.000000,1.000000)) = 0.500000
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 1508
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _ColorA;
uniform float4 _ColorB;
uniform float _Param;
uniform float _CenterGradientMin;
uniform float _LeftBorder;
uniform float _RightBorder;
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
    float4 result_1;
    float tmpvar_2;
    float tmpvar_3;
    tmpvar_3 = (in_f.xlv_TEXCOORD0.y - 0.5);
    tmpvar_2 = abs(tmpvar_3);
    float tmpvar_4;
    tmpvar_4 = abs(tmpvar_3);
    float tmpvar_5;
    tmpvar_5 = abs(tmpvar_3);
    float tmpvar_6;
    tmpvar_6 = abs(tmpvar_3);
    float tmpvar_7;
    tmpvar_7 = (sign(max(((-tmpvar_5) + _Param), 0)) * (((max(((-tmpvar_6) + _Param), 0) / _Param) * (1 - _CenterGradientMin)) + _CenterGradientMin));
    float4 tmpvar_8;
    tmpvar_8 = (((_ColorA * (sign(max((tmpvar_2 - _Param), 0)) * (1 - (max((tmpvar_4 - _Param), 0) * (1 / (0.5 - _Param)))))) + (tmpvar_7 * _ColorB)) + ((sign(tmpvar_7) * (1 - tmpvar_7)) * _ColorA));
    result_1 = tmpvar_8;
    if(((in_f.xlv_TEXCOORD0.x<_LeftBorder) || (in_f.xlv_TEXCOORD0.x>_RightBorder)))
    {
        result_1.w = 0;
    }
    out_f.color = result_1;
    return out_f;
}


ENDCG

}
}
}