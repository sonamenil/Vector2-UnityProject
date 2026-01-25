// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BlendModes/MeshBumpGlass/Grab" {
Properties {
 _BumpAmt ("Distortion", Range(0.000000,128.000000)) = 10.000000
 _MainTex ("Tint Color (RGB)", 2D) = "white" { }
 _BumpMap ("Normalmap", 2D) = "bump" { }
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Opaque" }
 GrabPass {
  Name "BASE"
  Tags { "LIGHTMODE"="Always" }
 }
 Pass {
  Name "BASE"
  Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent" "RenderType"="Opaque" }
  GpuProgramID 11361
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _BumpMap_ST;
uniform float4 _MainTex_ST;
uniform float _BumpAmt;
uniform sampler2D _GrabTexture;
uniform float4 _GrabTexture_TexelSize;
uniform sampler2D _BumpMap;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD2 :TEXCOORD2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD2 :TEXCOORD2;
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
    tmpvar_1 = UnityObjectToClipPos(in_v.vertex);
    tmpvar_2.xy = ((tmpvar_1.xy + tmpvar_1.w) * 0.5);
    tmpvar_2.zw = tmpvar_1.zw;
    out_v.vertex = tmpvar_1;
    out_v.xlv_TEXCOORD0 = tmpvar_2;
    out_v.xlv_TEXCOORD1 = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
    out_v.xlv_TEXCOORD2 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    float4 tmpvar_2;
    tmpvar_2.zw = in_f.xlv_TEXCOORD0.zw;
    float4 color_3;
    float4 grabColor_4;
    float4 tint_5;
    float4 col_6;
    float2 bump_7;
    float2 tmpvar_8;
    tmpvar_8 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD1).xyz * 2) - 1).xy;
    bump_7 = tmpvar_8;
    tmpvar_2.xy = (((bump_7 * _BumpAmt) * (_GrabTexture_TexelSize.xy * in_f.xlv_TEXCOORD0.z)) + in_f.xlv_TEXCOORD0.xy);
    float4 tmpvar_9;
    tmpvar_9 = tex2D(_GrabTexture, tmpvar_2);
    col_6 = tmpvar_9;
    float4 tmpvar_10;
    tmpvar_10 = tex2D(_MainTex, in_f.xlv_TEXCOORD2);
    tint_5 = tmpvar_10;
    grabColor_4 = col_6;
    color_3 = tint_5;
    float4 r_11;
    r_11.xyz = min(grabColor_4, color_3).xyz;
    r_11.w = color_3.w;
    tmpvar_1 = r_11;
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Opaque" }
 Pass {
  Name "BASE"
  Tags { "QUEUE"="Transparent" "RenderType"="Opaque" }
  Blend DstColor Zero
  GpuProgramID 75471
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_COLOR0 :COLOR0;
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
    float4 tmpvar_2;
    tmpvar_2 = clamp(float4(0, 0, 0, 1.1), 0, 1);
    tmpvar_1 = tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.w = 1;
    tmpvar_3.xyz = in_v.vertex.xyz;
    out_v.xlv_COLOR0 = tmpvar_1;
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.vertex = UnityObjectToClipPos(tmpvar_3);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    tmpvar_1 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
CustomEditor "BMMaterialEditor"
}