// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GUI/RoundGradient" {
Properties {
 _MainTex ("Texture", 2D) = "white" { }
 _Color ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
 _CenterX ("CenterX", Range(0.000000,1.000000)) = 0.500000
 _CenterY ("CenterY", Range(0.000000,1.000000)) = 0.500000
 _Intensive ("Intensive", Range(0.000000,1.000000)) = 0.500000
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  Cull Off
  Blend One OneMinusSrcAlpha
  GpuProgramID 36939
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
uniform float _CenterX;
uniform float _CenterY;
uniform float _Intensive;
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
    float4 tmpvar_1;
    float4 c_2;
    float2 tmpvar_3;
    tmpvar_3.x = _CenterX;
    tmpvar_3.y = _CenterY;
    float tmpvar_4;
    float2 tmpvar_5;
    tmpvar_5 = (tmpvar_3 - in_f.xlv_TEXCOORD0);
    tmpvar_4 = sqrt(dot(tmpvar_5, tmpvar_5));
    if((tmpvar_4>_Intensive))
    {
        discard;
    }
    float4 tmpvar_6;
    tmpvar_6.w = 1;
    tmpvar_6.xyz = _Color.xyz;
    c_2.xyz = tmpvar_6.xyz;
    c_2.w = (((_Intensive - tmpvar_4) * 2) * _Color.w);
    c_2.xyz = (_Color.xyz * c_2.w);
    tmpvar_1 = c_2;
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
}