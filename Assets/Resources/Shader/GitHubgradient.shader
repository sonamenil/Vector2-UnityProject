// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nekki/Vector/GradientGit" {
Properties {
 _MainTex ("-", 2D) = "black" { }
 _Color1 ("-", Color) = (0.000000,0.000000,1.000000,0.000000)
 _Color2 ("-", Color) = (1.000000,0.000000,0.000000,0.000000)
 _Direction ("-", Vector) = (0.000000,1.000000,0.000000,0.000000)
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 64109
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform sampler2D _MainTex;
uniform float4 _Color1;
uniform float4 _Color2;
uniform float2 _Direction;
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
    float4 tmpvar_1;
    tmpvar_1.w = 1;
    tmpvar_1.xyz = in_v.vertex.xyz;
    out_v.vertex = UnityObjectToClipPos(tmpvar_1);
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 c_b_1;
    float4 src_2;
    float4 tmpvar_3;
    tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    src_2 = tmpvar_3;
    float2 x_4;
    x_4 = (in_f.xlv_TEXCOORD0 - 0.5);
    float4 tmpvar_5;
    float _tmp_dvx_404 = (dot(x_4, _Direction) + 0.5);
    tmpvar_5 = lerp(_Color1, _Color2, float4(_tmp_dvx_404, _tmp_dvx_404, _tmp_dvx_404, _tmp_dvx_404));
    c_b_1 = tmpvar_5;
    out_f.color = (src_2 * c_b_1);
    return out_f;
}


ENDCG

}
}
}