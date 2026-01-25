// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/LightningBoltShaderMesh" {
Properties {
 _MainTex ("Color (RGB) Alpha (A)", 2D) = "white" { }
 _GlowTex ("Glow Color (RGB) Alpha (A)", 2D) = "white" { }
 _TintColor ("Tint Color (RGB)", Color) = (1.000000,1.000000,1.000000,1.000000)
 _GlowTintColor ("Glow Tint Color (RGB)", Color) = (1.000000,1.000000,1.000000,1.000000)
 _InvFade ("Soft Particles Factor", Range(0.010000,3.000000)) = 1.000000
 _JitterMultiplier ("Jitter Multiplier (Float)", Float) = 0.000000
 _Turbulence ("Turbulence (Float)", Float) = 0.000000
 _TurbulenceVelocity ("Turbulence Velocity (Vector)", Vector) = (0.000000,0.000000,0.000000,0.000000)
}
SubShader { 
 Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent+10" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" }
 Pass {
  Name "GLOWPASS"
  Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent+10" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha One
  GpuProgramID 45333
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


#define CODE_BLOCK_VERTEX
//uniform float4 _Time;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 UNITY_MATRIX_IT_MV;
uniform float4 _GlowTintColor;
uniform sampler2D _GlowTex;
struct appdata_t
{
    float4 tangent :TANGENT;
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
    float4 texcoord1 :TEXCOORD1;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_COLOR0 :COLOR0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_COLOR0 :COLOR0;
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
    float4 tmpvar_3;
    float tmpvar_4;
    tmpvar_4 = (in_v.texcoord.x - 0.5);
    float4 tmpvar_5;
    tmpvar_5.w = 0;
    tmpvar_5.xyz = float3(((in_v.normal * (-tmpvar_4)) * 2));
    float4 v_6;
    v_6.x = conv_mxt4x4_0(UNITY_MATRIX_IT_MV).x;
    v_6.y = conv_mxt4x4_1(UNITY_MATRIX_IT_MV).x;
    v_6.z = conv_mxt4x4_2(UNITY_MATRIX_IT_MV).x;
    v_6.w = conv_mxt4x4_3(UNITY_MATRIX_IT_MV).x;
    float4 v_7;
    v_7.x = conv_mxt4x4_0(UNITY_MATRIX_IT_MV).y;
    v_7.y = conv_mxt4x4_1(UNITY_MATRIX_IT_MV).y;
    v_7.z = conv_mxt4x4_2(UNITY_MATRIX_IT_MV).y;
    v_7.w = conv_mxt4x4_3(UNITY_MATRIX_IT_MV).y;
    tmpvar_3 = UnityObjectToClipPos((((in_v.vertex + (in_v.vertex + tmpvar_5)) * 0.5) + (((sqrt(dot(in_v.tangent.xyz, in_v.tangent.xyz)) * in_v.texcoord1.x) * 2) * ((tmpvar_4 * v_6) + ((in_v.texcoord.y - 0.5) * (-v_7))))));
    int tmpvar_8;
    tmpvar_8 = int(1);
    float4 tmpvar_9;
    if((_Time.y<in_v.color.y))
    {
        float4 tmpvar_10;
        float _tmp_dvx_316 = ((_Time.y - in_v.color.x) / (in_v.color.y - in_v.color.x));
        tmpvar_10 = float4(_tmp_dvx_316, _tmp_dvx_316, _tmp_dvx_316, _tmp_dvx_316);
        tmpvar_9 = tmpvar_10;
        tmpvar_8 = int(0);
    }
    else
    {
        if((_Time.y<in_v.color.z))
        {
            tmpvar_9 = float4(1, 1, 1, 1);
            tmpvar_8 = int(0);
        }
    }
    if(tmpvar_8)
    {
        float4 tmpvar_11;
        float _tmp_dvx_317 = ((_Time.y - in_v.color.z) / (in_v.color.w - in_v.color.z));
        tmpvar_11 = (float4(1, 1, 1, 1) - float4(_tmp_dvx_317, _tmp_dvx_317, _tmp_dvx_317, _tmp_dvx_317));
        tmpvar_9 = tmpvar_11;
        tmpvar_8 = int(0);
    }
    tmpvar_2 = (tmpvar_9 * _GlowTintColor);
    tmpvar_2.w = (tmpvar_2.w * in_v.texcoord1.y);
    out_v.xlv_TEXCOORD0 = tmpvar_1;
    out_v.xlv_COLOR0 = tmpvar_2;
    out_v.vertex = tmpvar_3;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    tmpvar_1 = (tex2D(_GlowTex, in_f.xlv_TEXCOORD0) * in_f.xlv_COLOR0);
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "LINEPASS"
  Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent+10" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha One
  GpuProgramID 99587
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4 _Time;
//uniform float3 _WorldSpaceCameraPos;
//uniform float4 unity_OrthoParams;
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float _JitterMultiplier;
uniform float _Turbulence;
uniform float4 _TurbulenceVelocity;
uniform float4 _TintColor;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 tangent :TANGENT;
    float4 vertex :POSITION;
    float4 color :COLOR;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_COLOR0 :COLOR0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_COLOR0 :COLOR0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    tmpvar_1 = in_v.tangent;
    float2 tmpvar_2;
    tmpvar_2 = in_v.texcoord.xy;
    float4 tmpvar_3;
    float tmpvar_4;
    tmpvar_4 = (1 + (frac((sin(dot((_Time.yzw * in_v.vertex.xyz), float3(12.9898, 78.233, 45.5432))) * 43758.55)) * _JitterMultiplier));
    float tmpvar_5;
    tmpvar_5 = ((_Time.y - in_v.color.x) / (in_v.color.w - in_v.color.x));
    float tmpvar_6;
    tmpvar_6 = ((_Turbulence / max(0.5, abs(in_v.tangent.w))) * tmpvar_5);
    float4 tmpvar_7;
    tmpvar_7 = (_TurbulenceVelocity * float4(tmpvar_5, tmpvar_5, tmpvar_5, tmpvar_5));
    if((unity_OrthoParams.w==0))
    {
        float4 tmpvar_8;
        tmpvar_8.w = 0;
        tmpvar_8.xyz = normalize(in_v.tangent.xyz);
        float3 tmpvar_9;
        tmpvar_9 = (_WorldSpaceCameraPos - in_v.vertex.xyz);
        float4 tmpvar_10;
        tmpvar_10.w = 0;
        tmpvar_10.xyz = (normalize(((in_v.tangent.yzx * tmpvar_9.zxy) - (in_v.tangent.zxy * tmpvar_9.yzx))) * in_v.tangent.w);
        tmpvar_3 = UnityObjectToClipPos(((in_v.vertex + (tmpvar_10 * tmpvar_4)) + (tmpvar_7 + (tmpvar_8 * tmpvar_6))));
    }
    else
    {
        float4 tmpvar_11;
        tmpvar_11.zw = float2(0, 0);
        tmpvar_11.xy = tmpvar_7.xy;
        float4 tmpvar_12;
        tmpvar_12.zw = float2(0, 0);
        tmpvar_12.xy = normalize(in_v.tangent).xy;
        float2 tmpvar_13;
        tmpvar_13.x = (-in_v.tangent.y);
        tmpvar_13.y = tmpvar_1.x;
        float4 tmpvar_14;
        tmpvar_14.zw = float2(0, 0);
        tmpvar_14.xy = (normalize(tmpvar_13) * in_v.tangent.w);
        tmpvar_3 = UnityObjectToClipPos(((in_v.vertex + (tmpvar_14 * tmpvar_4)) + (tmpvar_11 + (tmpvar_12 * tmpvar_6))));
    }
    int tmpvar_15;
    tmpvar_15 = int(1);
    float4 tmpvar_16;
    if((_Time.y<in_v.color.y))
    {
        float4 tmpvar_17;
        float _tmp_dvx_308 = ((_Time.y - in_v.color.x) / (in_v.color.y - in_v.color.x));
        tmpvar_17 = float4(_tmp_dvx_308, _tmp_dvx_308, _tmp_dvx_308, _tmp_dvx_308);
        tmpvar_16 = tmpvar_17;
        tmpvar_15 = int(0);
    }
    else
    {
        if((_Time.y<in_v.color.z))
        {
            tmpvar_16 = float4(1, 1, 1, 1);
            tmpvar_15 = int(0);
        }
    }
    if(tmpvar_15)
    {
        float4 tmpvar_18;
        float _tmp_dvx_309 = ((_Time.y - in_v.color.z) / (in_v.color.w - in_v.color.z));
        tmpvar_18 = (float4(1, 1, 1, 1) - float4(_tmp_dvx_309, _tmp_dvx_309, _tmp_dvx_309, _tmp_dvx_309));
        tmpvar_16 = tmpvar_18;
        tmpvar_15 = int(0);
    }
    out_v.xlv_TEXCOORD0 = tmpvar_2;
    out_v.xlv_COLOR0 = (tmpvar_16 * _TintColor);
    out_v.vertex = tmpvar_3;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    tmpvar_1 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * in_f.xlv_COLOR0);
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
}
Fallback "Particles/Additive (Soft)"
}