// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FastBlur" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Bloom ("Bloom (RGB)", 2D) = "black" { }
}
SubShader { 
 Pass {
  ZTest False
  ZWrite Off
  Cull Off
  GpuProgramID 24581
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_TexelSize;
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
    float2 xlv_TEXCOORD2 :TEXCOORD2;
    float2 xlv_TEXCOORD3 :TEXCOORD3;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
    float2 xlv_TEXCOORD2 :TEXCOORD2;
    float2 xlv_TEXCOORD3 :TEXCOORD3;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = (in_v.texcoord.xy + _MainTex_TexelSize.xy);
    out_v.xlv_TEXCOORD1 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(-0.5, (-0.5))));
    out_v.xlv_TEXCOORD2 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(0.5, (-0.5))));
    out_v.xlv_TEXCOORD3 = (in_v.texcoord.xy + (_MainTex_TexelSize.xy * float2(-0.5, 0.5)));
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 color_1;
    color_1 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) + tex2D(_MainTex, in_f.xlv_TEXCOORD1));
    color_1 = (color_1 + tex2D(_MainTex, in_f.xlv_TEXCOORD2));
    color_1 = (color_1 + tex2D(_MainTex, in_f.xlv_TEXCOORD3));
    float4 tmpvar_2;
    tmpvar_2 = (color_1 / 4);
    out_f.color = tmpvar_2;
    return out_f;
}


ENDCG

}
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 73835
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_TexelSize;
uniform float4 _Parameter;
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
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    tmpvar_1.zw = float2(1, 1);
    tmpvar_1.xy = in_v.texcoord.xy;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_1;
    out_v.xlv_TEXCOORD1 = ((_MainTex_TexelSize.xy * float2(0, 1)) * _Parameter.x);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 color_1;
    float2 coords_2;
    coords_2 = (in_f.xlv_TEXCOORD0.xy - (in_f.xlv_TEXCOORD1 * 3));
    float4 tap_3;
    float4 tmpvar_4;
    tmpvar_4 = tex2D(_MainTex, coords_2);
    tap_3 = tmpvar_4;
    color_1 = (tap_3 * float4(0.0205, 0.0205, 0.0205, 0));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_5;
    float4 tmpvar_6;
    tmpvar_6 = tex2D(_MainTex, coords_2);
    tap_5 = tmpvar_6;
    color_1 = (color_1 + (tap_5 * float4(0.0855, 0.0855, 0.0855, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_7;
    float4 tmpvar_8;
    tmpvar_8 = tex2D(_MainTex, coords_2);
    tap_7 = tmpvar_8;
    color_1 = (color_1 + (tap_7 * float4(0.232, 0.232, 0.232, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_9;
    float4 tmpvar_10;
    tmpvar_10 = tex2D(_MainTex, coords_2);
    tap_9 = tmpvar_10;
    color_1 = (color_1 + (tap_9 * float4(0.324, 0.324, 0.324, 1)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_11;
    float4 tmpvar_12;
    tmpvar_12 = tex2D(_MainTex, coords_2);
    tap_11 = tmpvar_12;
    color_1 = (color_1 + (tap_11 * float4(0.232, 0.232, 0.232, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_13;
    float4 tmpvar_14;
    tmpvar_14 = tex2D(_MainTex, coords_2);
    tap_13 = tmpvar_14;
    color_1 = (color_1 + (tap_13 * float4(0.0855, 0.0855, 0.0855, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_15;
    float4 tmpvar_16;
    tmpvar_16 = tex2D(_MainTex, coords_2);
    tap_15 = tmpvar_16;
    color_1 = (color_1 + (tap_15 * float4(0.0205, 0.0205, 0.0205, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    out_f.color = color_1;
    return out_f;
}


ENDCG

}
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 180590
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_TexelSize;
uniform float4 _Parameter;
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
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_TEXCOORD0 :TEXCOORD0;
    float2 xlv_TEXCOORD1 :TEXCOORD1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    tmpvar_1.zw = float2(1, 1);
    tmpvar_1.xy = in_v.texcoord.xy;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_1;
    out_v.xlv_TEXCOORD1 = ((_MainTex_TexelSize.xy * float2(1, 0)) * _Parameter.x);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 color_1;
    float2 coords_2;
    coords_2 = (in_f.xlv_TEXCOORD0.xy - (in_f.xlv_TEXCOORD1 * 3));
    float4 tap_3;
    float4 tmpvar_4;
    tmpvar_4 = tex2D(_MainTex, coords_2);
    tap_3 = tmpvar_4;
    color_1 = (tap_3 * float4(0.0205, 0.0205, 0.0205, 0));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_5;
    float4 tmpvar_6;
    tmpvar_6 = tex2D(_MainTex, coords_2);
    tap_5 = tmpvar_6;
    color_1 = (color_1 + (tap_5 * float4(0.0855, 0.0855, 0.0855, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_7;
    float4 tmpvar_8;
    tmpvar_8 = tex2D(_MainTex, coords_2);
    tap_7 = tmpvar_8;
    color_1 = (color_1 + (tap_7 * float4(0.232, 0.232, 0.232, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_9;
    float4 tmpvar_10;
    tmpvar_10 = tex2D(_MainTex, coords_2);
    tap_9 = tmpvar_10;
    color_1 = (color_1 + (tap_9 * float4(0.324, 0.324, 0.324, 1)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_11;
    float4 tmpvar_12;
    tmpvar_12 = tex2D(_MainTex, coords_2);
    tap_11 = tmpvar_12;
    color_1 = (color_1 + (tap_11 * float4(0.232, 0.232, 0.232, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_13;
    float4 tmpvar_14;
    tmpvar_14 = tex2D(_MainTex, coords_2);
    tap_13 = tmpvar_14;
    color_1 = (color_1 + (tap_13 * float4(0.0855, 0.0855, 0.0855, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    float4 tap_15;
    float4 tmpvar_16;
    tmpvar_16 = tex2D(_MainTex, coords_2);
    tap_15 = tmpvar_16;
    color_1 = (color_1 + (tap_15 * float4(0.0205, 0.0205, 0.0205, 0)));
    coords_2 = (coords_2 + in_f.xlv_TEXCOORD1);
    out_f.color = color_1;
    return out_f;
}


ENDCG

}
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 254764
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_TexelSize;
uniform float4 _Parameter;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float4 xlv_TEXCOORD1_2 :TEXCOORD1_2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float4 xlv_TEXCOORD1_2 :TEXCOORD1_2;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    tmpvar_1.zw = float2(1, 1);
    tmpvar_1.xy = in_v.texcoord.xy;
    float tmpvar_2;
    tmpvar_2 = (_MainTex_TexelSize.y * _Parameter.x);
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_1.xy;
    out_v.xlv_TEXCOORD1 = (in_v.texcoord.xyxy + (tmpvar_2 * float4(0, (-3), 0, 3)));
    out_v.xlv_TEXCOORD1_1 = (in_v.texcoord.xyxy + (tmpvar_2 * float4(0, (-2), 0, 2)));
    out_v.xlv_TEXCOORD1_2 = (in_v.texcoord.xyxy + (tmpvar_2 * float4(0, (-1), 0, 1)));
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 color_1;
    float4 tmpvar_2;
    tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    color_1 = (tmpvar_2 * float4(0.324, 0.324, 0.324, 1));
    float4 tapB_3;
    float4 tapA_4;
    float4 tmpvar_5;
    tmpvar_5 = tex2D(_MainTex, in_f.xlv_TEXCOORD1.xy);
    tapA_4 = tmpvar_5;
    float4 tmpvar_6;
    tmpvar_6 = tex2D(_MainTex, in_f.xlv_TEXCOORD1.zw);
    tapB_3 = tmpvar_6;
    color_1 = (color_1 + ((tapA_4 + tapB_3) * float4(0.0205, 0.0205, 0.0205, 0)));
    float4 tapB_7;
    float4 tapA_8;
    float4 tmpvar_9;
    tmpvar_9 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_1.xy);
    tapA_8 = tmpvar_9;
    float4 tmpvar_10;
    tmpvar_10 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_1.zw);
    tapB_7 = tmpvar_10;
    color_1 = (color_1 + ((tapA_8 + tapB_7) * float4(0.0855, 0.0855, 0.0855, 0)));
    float4 tapB_11;
    float4 tapA_12;
    float4 tmpvar_13;
    tmpvar_13 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_2.xy);
    tapA_12 = tmpvar_13;
    float4 tmpvar_14;
    tmpvar_14 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_2.zw);
    tapB_11 = tmpvar_14;
    color_1 = (color_1 + ((tapA_12 + tapB_11) * float4(0.232, 0.232, 0.232, 0)));
    out_f.color = color_1;
    return out_f;
}


ENDCG

}
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 299746
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _MainTex_TexelSize;
uniform float4 _Parameter;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float4 xlv_TEXCOORD1_2 :TEXCOORD1_2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD1_1 :TEXCOORD1_1;
    float4 xlv_TEXCOORD1_2 :TEXCOORD1_2;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float tmpvar_1;
    tmpvar_1 = (_MainTex_TexelSize.x * _Parameter.x);
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
    out_v.xlv_TEXCOORD1 = (in_v.texcoord.xyxy + (tmpvar_1 * float4(-3, 0, 3, 0)));
    out_v.xlv_TEXCOORD1_1 = (in_v.texcoord.xyxy + (tmpvar_1 * float4(-2, 0, 2, 0)));
    out_v.xlv_TEXCOORD1_2 = (in_v.texcoord.xyxy + (tmpvar_1 * float4(-1, 0, 1, 0)));
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 color_1;
    float4 tmpvar_2;
    tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
    color_1 = (tmpvar_2 * float4(0.324, 0.324, 0.324, 1));
    float4 tapB_3;
    float4 tapA_4;
    float4 tmpvar_5;
    tmpvar_5 = tex2D(_MainTex, in_f.xlv_TEXCOORD1.xy);
    tapA_4 = tmpvar_5;
    float4 tmpvar_6;
    tmpvar_6 = tex2D(_MainTex, in_f.xlv_TEXCOORD1.zw);
    tapB_3 = tmpvar_6;
    color_1 = (color_1 + ((tapA_4 + tapB_3) * float4(0.0205, 0.0205, 0.0205, 0)));
    float4 tapB_7;
    float4 tapA_8;
    float4 tmpvar_9;
    tmpvar_9 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_1.xy);
    tapA_8 = tmpvar_9;
    float4 tmpvar_10;
    tmpvar_10 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_1.zw);
    tapB_7 = tmpvar_10;
    color_1 = (color_1 + ((tapA_8 + tapB_7) * float4(0.0855, 0.0855, 0.0855, 0)));
    float4 tapB_11;
    float4 tapA_12;
    float4 tmpvar_13;
    tmpvar_13 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_2.xy);
    tapA_12 = tmpvar_13;
    float4 tmpvar_14;
    tmpvar_14 = tex2D(_MainTex, in_f.xlv_TEXCOORD1_2.zw);
    tapB_11 = tmpvar_14;
    color_1 = (color_1 + ((tapA_12 + tapB_11) * float4(0.232, 0.232, 0.232, 0)));
    out_f.color = color_1;
    return out_f;
}


ENDCG

}
}
Fallback Off
}