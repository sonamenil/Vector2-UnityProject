// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mesh/Colored" {
Properties {
 _Color ("Color", Color) = (0.000000,0.000000,0.000000,1.000000)
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  Cull Off
  Blend One OneMinusSrcAlpha
  GpuProgramID 24223
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
};

struct OUT_Data_Vert
{
    float4 xlv_COLOR :COLOR;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR :COLOR;
    float4 vertex :Position;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    tmpvar_1 = in_v.color;
    float4 tmpvar_2;
    tmpvar_2 = tmpvar_1;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = tmpvar_2;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
float4 xlat_mutable_Color;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    xlat_mutable_Color.w = _Color.w;
    xlat_mutable_Color.xyz = (_Color.xyz * _Color.w);
    out_f.color = xlat_mutable_Color;
    return out_f;
}


ENDCG

}
}
}