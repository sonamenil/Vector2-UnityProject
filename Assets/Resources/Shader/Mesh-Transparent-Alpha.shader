// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent Alpha" {
Properties {
 _Alpha ("Alpha", Range(0.000000,1.000000)) = 1.000000
 _Color ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1.000000, -1.000000
  GpuProgramID 44451
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float _Alpha;
uniform float4 _Color;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
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
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_1;
    out_v.xlv_COLOR = in_v.color;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
float4 xlat_mutable_Color;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    xlat_mutable_Color.xyz = _Color.xyz;
    xlat_mutable_Color.w = ((_Color.w * in_f.xlv_COLOR.w) * _Alpha);
    out_f.color = xlat_mutable_Color;
    return out_f;
}


ENDCG

}
}
}