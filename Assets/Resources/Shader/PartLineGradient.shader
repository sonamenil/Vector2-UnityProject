// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GUI/PartLineGradient" {
Properties {
 _Left ("Left", Range(0.000000,1.000000)) = 0.500000
 _Right ("Right", Range(0.000000,1.000000)) = 0.000000
 _Gradient ("Gradient", Range(0.000000,1.000000)) = 0.000000
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" { }
 _Color ("Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
 _StencilComp ("Stencil Comparison", Float) = 8.000000
 _Stencil ("Stencil ID", Float) = 0.000000
 _StencilOp ("Stencil Operation", Float) = 0.000000
 _StencilWriteMask ("Stencil Write Mask", Float) = 255.000000
 _StencilReadMask ("Stencil Read Mask", Float) = 255.000000
 _ColorMask ("Color Mask", Float) = 15.000000
[Toggle(UNITY_UI_ALPHACLIP)]  _UseUIAlphaClip ("Use Alpha Clip", Float) = 0.000000
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZTest [unity_GUIZTestMode]
  ZWrite Off
  Cull Off
  Stencil {
   Ref [_Stencil]
   ReadMask [_StencilReadMask]
   WriteMask [_StencilWriteMask]
   Comp [_StencilComp]
   Pass [_StencilOp]
  }
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask [_ColorMask]
  GpuProgramID 20000
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
uniform float4 _TextureSampleAdd;
uniform float4 _ClipRect;
uniform sampler2D _MainTex;
uniform float _Left;
uniform float _Right;
uniform float _Gradient;
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
    float2 tmpvar_1;
    tmpvar_1 = in_v.texcoord.xy;
    float4 tmpvar_2;
    float2 tmpvar_3;
    tmpvar_3 = tmpvar_1;
    tmpvar_2 = (in_v.color * _Color);
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = tmpvar_2;
    out_v.xlv_TEXCOORD0 = tmpvar_3;
    out_v.xlv_TEXCOORD1 = in_v.vertex;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    float4 color_2;
    float4 tmpvar_3;
    tmpvar_3 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0) + _TextureSampleAdd) * in_f.xlv_COLOR);
    color_2 = tmpvar_3;
    float tmpvar_4;
    float2 tmpvar_5;
    tmpvar_5.x = float((_ClipRect.z>=in_f.xlv_TEXCOORD1.x));
    tmpvar_5.y = float((_ClipRect.w>=in_f.xlv_TEXCOORD1.y));
    float2 tmpvar_6;
    tmpvar_6 = (float2(bool2(in_f.xlv_TEXCOORD1.xy >= _ClipRect.xy)) * tmpvar_5);
    tmpvar_4 = (tmpvar_6.x * tmpvar_6.y);
    color_2.w = (color_2.w * tmpvar_4);
    if(((in_f.xlv_TEXCOORD0.x>_Left) && (in_f.xlv_TEXCOORD0.x<_Right)))
    {
        if((in_f.xlv_TEXCOORD0.x<(_Left + _Gradient)))
        {
            color_2 = (color_2 * ((in_f.xlv_TEXCOORD0.x - _Left) / _Gradient));
        }
        if(((in_f.xlv_TEXCOORD0.x + _Gradient)>_Right))
        {
            color_2 = (color_2 * ((_Right - in_f.xlv_TEXCOORD0.x) / _Gradient));
        }
    }
    else
    {
        color_2.w = 0;
    }
    tmpvar_1 = color_2;
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
}