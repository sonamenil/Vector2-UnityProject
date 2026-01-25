// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nekki/Vector/Gradient" {
Properties {
 _MainTex ("Texture", 2D) = "white" { }
 _Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _Color2 ("Main Color 2", Color) = (1.000000,1.000000,1.000000,1.000000)
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 22184
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
uniform float4 _Color2;
struct appdata_t
{
    float4 vertex :POSITION;
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
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_COLOR :COLOR;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    float4 tmpvar_2;
    tmpvar_2 = lerp(_Color, _Color2, in_v.texcoord.xxxx);
    tmpvar_1.xyz = tmpvar_2.xyz;
    float tmpvar_3;
    tmpvar_3 = lerp(_Color.w, _Color2.w, in_v.texcoord.x);
    tmpvar_1.w = tmpvar_3;
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = tmpvar_1;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float alpha_1;
    float tmpvar_2;
    tmpvar_2 = lerp(_Color.w, _Color2.w, in_f.xlv_TEXCOORD0.x);
    alpha_1 = tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.xyz = in_f.xlv_COLOR.xyz;
    tmpvar_3.w = alpha_1;
    out_f.color = tmpvar_3;
    return out_f;
}


ENDCG

}
}
}