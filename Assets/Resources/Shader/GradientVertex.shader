// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nekki/Vector/GradientVertex" {
Properties {
 _MainTex ("Texture", 2D) = "white" { }
 _StencilComp ("Stencil Comparison", Float) = 8.000000
 _Stencil ("Stencil ID", Float) = 0.000000
 _StencilOp ("Stencil Operation", Float) = 0.000000
 _StencilWriteMask ("Stencil Write Mask", Float) = 255.000000
 _StencilReadMask ("Stencil Read Mask", Float) = 255.000000
 _ColorMask ("Color Mask", Float) = 15.000000
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
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
  GpuProgramID 9439
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_COLOR :COLOR;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR :COLOR;
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
    tmpvar_2.xyz = in_v.color.xyz;
    tmpvar_2.w = tmpvar_1.x;
    out_v.xlv_TEXCOORD0 = tmpvar_1;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = tmpvar_2;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(appdata_t in_f)
{
    OUT_Data_Frag out_f;
    out_f.color = in_f.color;
    return out_f;
}


ENDCG

}
}
}