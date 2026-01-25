// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Glow Effect/Glow Effect" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
}
SubShader { 
 Pass {
  Name "GLOW"
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 55448
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform sampler2D _MainTex;
uniform sampler2D _Glow;
uniform float _GlowStrength;
uniform float4 _GlowColorMultiplier;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
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
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_1;
    out_v.xlv_TEXCOORD1 = tmpvar_1;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    float4 glow_2;
    float4 tmpvar_3;
    tmpvar_3 = tex2D(_Glow, in_f.xlv_TEXCOORD1);
    float4 tmpvar_4;
    tmpvar_4 = ((tmpvar_3 * _GlowStrength) * _GlowColorMultiplier);
    glow_2 = tmpvar_4;
    tmpvar_1 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) + glow_2);
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "SIMPLEBLUR"
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 90684
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_TexelSize;
uniform float _BlurSpread;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float2 xlv_TEXCOORD1_2 :TEXCOORD1_2;
    float2 xlv_TEXCOORD1_3 :TEXCOORD1_3;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float2 xlv_TEXCOORD1_2 :TEXCOORD1_2;
    float2 xlv_TEXCOORD1_3 :TEXCOORD1_3;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float tmpvar_1;
    tmpvar_1 = (-_BlurSpread);
    float2 tmpvar_2;
    tmpvar_2.x = _BlurSpread;
    tmpvar_2.y = tmpvar_1;
    float2 tmpvar_3;
    tmpvar_3.x = tmpvar_1;
    tmpvar_3.y = _BlurSpread;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    out_v.xlv_TEXCOORD1 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(_BlurSpread, _BlurSpread)));
    out_v.xlv_TEXCOORD1_1 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(tmpvar_1, tmpvar_1)));
    out_v.xlv_TEXCOORD1_2 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * tmpvar_2));
    out_v.xlv_TEXCOORD1_3 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * tmpvar_3));
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 blur_1;
    blur_1 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0) * 0.4) + (tex2D(_MainTex, in_f.xlv_TEXCOORD1) * 0.15));
    blur_1 = (blur_1 + (tex2D(_MainTex, in_f.xlv_TEXCOORD1_1) * 0.15));
    blur_1 = (blur_1 + (tex2D(_MainTex, in_f.xlv_TEXCOORD1_2) * 0.15));
    blur_1 = (blur_1 + (tex2D(_MainTex, in_f.xlv_TEXCOORD1_3) * 0.15));
    out_f.color = blur_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "BLURANDALPHAMULTRGB"
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 144285
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_TexelSize;
uniform float _BlurSpread;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float2 xlv_TEXCOORD1_2 :TEXCOORD1_2;
    float2 xlv_TEXCOORD1_3 :TEXCOORD1_3;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float2 xlv_TEXCOORD1_2 :TEXCOORD1_2;
    float2 xlv_TEXCOORD1_3 :TEXCOORD1_3;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float tmpvar_1;
    tmpvar_1 = (-_BlurSpread);
    float2 tmpvar_2;
    tmpvar_2.x = _BlurSpread;
    tmpvar_2.y = tmpvar_1;
    float2 tmpvar_3;
    tmpvar_3.x = tmpvar_1;
    tmpvar_3.y = _BlurSpread;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    out_v.xlv_TEXCOORD1 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(_BlurSpread, _BlurSpread)));
    out_v.xlv_TEXCOORD1_1 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(tmpvar_1, tmpvar_1)));
    out_v.xlv_TEXCOORD1_2 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * tmpvar_2));
    out_v.xlv_TEXCOORD1_3 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * tmpvar_3));
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 blur_1;
    float4 mainTex_2;
    float4 tmpvar_3;
    tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    mainTex_2 = (tmpvar_3 * (tmpvar_3.w * 0.4));
    blur_1 = mainTex_2;
    float4 tmpvar_4;
    tmpvar_4 = tex2D(_MainTex, in_f.xlv_TEXCOORD1);
    mainTex_2 = (tmpvar_4 * (tmpvar_4.w * 0.15));
    blur_1 = (blur_1 + mainTex_2);
    float4 tmpvar_5;
    tmpvar_5 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_1);
    mainTex_2 = (tmpvar_5 * (tmpvar_5.w * 0.15));
    blur_1 = (blur_1 + mainTex_2);
    float4 tmpvar_6;
    tmpvar_6 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_2);
    mainTex_2 = (tmpvar_6 * (tmpvar_6.w * 0.15));
    blur_1 = (blur_1 + mainTex_2);
    float4 tmpvar_7;
    tmpvar_7 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_3);
    mainTex_2 = (tmpvar_7 * (tmpvar_7.w * 0.15));
    blur_1 = (blur_1 + mainTex_2);
    out_f.color = blur_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "SIMPLEBLURANDGLOW"
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 237842
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Glow_TexelSize;
uniform sampler2D _MainTex;
uniform sampler2D _Glow;
uniform float _GlowStrength;
uniform float4 _GlowColorMultiplier;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD2 :TEXCOORD2;
    float2 xlv_TEXCOORD2_1 :TEXCOORD2_1;
    float2 xlv_TEXCOORD2_2 :TEXCOORD2_2;
    float2 xlv_TEXCOORD2_3 :TEXCOORD2_3;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD2 :TEXCOORD2;
    float2 xlv_TEXCOORD2_1 :TEXCOORD2_1;
    float2 xlv_TEXCOORD2_2 :TEXCOORD2_2;
    float2 xlv_TEXCOORD2_3 :TEXCOORD2_3;
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
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_1;
    out_v.xlv_TEXCOORD1 = tmpvar_1;
    out_v.xlv_TEXCOORD2 = (in_v.texcoord.xy + (_Glow_TexelSize.xy * float2(2.5, 2.5)));
    out_v.xlv_TEXCOORD2_1 = (in_v.texcoord.xy + (_Glow_TexelSize.xy * float2(-2.5, (-2.5))));
    out_v.xlv_TEXCOORD2_2 = (in_v.texcoord.xy + (_Glow_TexelSize.xy * float2(2.5, (-2.5))));
    out_v.xlv_TEXCOORD2_3 = (in_v.texcoord.xy + (_Glow_TexelSize.xy * float2(-2.5, 2.5)));
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 blur_1;
    blur_1 = ((tex2D(_Glow, in_f.xlv_TEXCOORD1) * 0.3) + (tex2D(_Glow, in_f.xlv_TEXCOORD2) * 0.175));
    blur_1 = (blur_1 + (tex2D(_Glow, in_f.xlv_TEXCOORD2_1) * 0.175));
    blur_1 = (blur_1 + (tex2D(_Glow, in_f.xlv_TEXCOORD2_2) * 0.175));
    blur_1 = (blur_1 + (tex2D(_Glow, in_f.xlv_TEXCOORD2_3) * 0.175));
    blur_1 = (blur_1 * (_GlowStrength * _GlowColorMultiplier));
    float4 tmpvar_2;
    tmpvar_2 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) + blur_1);
    out_f.color = tmpvar_2;
    return out_f;
}


ENDCG

}
 Pass {
  Name "SIMPLEBLURGLOWFROMALPHA"
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 309017
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_TexelSize;
uniform sampler2D _MainTex;
uniform float _GlowStrength;
uniform float4 _GlowColorMultiplier;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float2 xlv_TEXCOORD1_2 :TEXCOORD1_2;
    float2 xlv_TEXCOORD1_3 :TEXCOORD1_3;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float2 xlv_TEXCOORD1_2 :TEXCOORD1_2;
    float2 xlv_TEXCOORD1_3 :TEXCOORD1_3;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    out_v.xlv_TEXCOORD1 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(2.5, 2.5)));
    out_v.xlv_TEXCOORD1_1 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(-2.5, (-2.5))));
    out_v.xlv_TEXCOORD1_2 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(2.5, (-2.5))));
    out_v.xlv_TEXCOORD1_3 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(-2.5, 2.5)));
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 blur_1;
    float4 tmpvar_2;
    tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    blur_1 = ((tmpvar_2 * 0.3) + (tex2D(_MainTex, in_f.xlv_TEXCOORD1) * 0.175));
    blur_1 = (blur_1 + (tex2D(_MainTex, in_f.xlv_TEXCOORD1_1) * 0.175));
    blur_1 = (blur_1 + (tex2D(_MainTex, in_f.xlv_TEXCOORD1_2) * 0.175));
    blur_1 = (blur_1 + (tex2D(_MainTex, in_f.xlv_TEXCOORD1_3) * 0.175));
    blur_1 = (blur_1 * (_GlowStrength * _GlowColorMultiplier));
    float4 tmpvar_3;
    tmpvar_3 = (tmpvar_2 + (blur_1 * blur_1.w));
    out_f.color = tmpvar_3;
    return out_f;
}


ENDCG

}
}
Fallback Off
}