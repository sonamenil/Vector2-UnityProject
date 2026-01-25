Shader "Hidden/MKGlowFullScreenDownSample" {
Properties {
    _Color ("Color", Color) = (1.000000,1.000000,1.000000,0.000000)
    _MainTex ("", 2D) = "white" { }
}
SubShader { 
 Pass {
  ZWrite Off
  Cull Off

CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

uniform float4 _MainTex_TexelSize;
uniform sampler2D _MainTex;
uniform float4 _Color;

struct appdata_t {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
};

struct OUT_Data_Vert {
    float2 xlv_TEXCOORD0   : TEXCOORD0;
    float2 xlv_TEXCOORD0_1 : TEXCOORD1;
    float2 xlv_TEXCOORD0_2 : TEXCOORD2;
    float2 xlv_TEXCOORD0_3 : TEXCOORD3;
    float4 vertex : SV_POSITION;
};

struct v2f {
    float2 xlv_TEXCOORD0   : TEXCOORD0;
    float2 xlv_TEXCOORD0_1 : TEXCOORD1;
    float2 xlv_TEXCOORD0_2 : TEXCOORD2;
    float2 xlv_TEXCOORD0_3 : TEXCOORD3;
};

struct OUT_Data_Frag {
    float4 color : SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v) {
    OUT_Data_Vert out_v;
    float2 inUV = in_v.texcoord.xy;

    float offX = _MainTex_TexelSize.x;
    float offY = _MainTex_TexelSize.y;

    float4 uv_4;
    uv_4.xy = inUV;
    uv_4.zw = 0;

    float4 tmpvar_9;  tmpvar_9.xy = float2(-offX, -offY);
    float4 tmpvar_10; tmpvar_10.xy = float2( offX, -offY);
    float4 tmpvar_11; tmpvar_11.xy = float2( offX,  offY);
    float4 tmpvar_12; tmpvar_12.xy = float2(-offX,  offY);

    out_v.vertex        = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0   = (uv_4 + tmpvar_9).xy;
    out_v.xlv_TEXCOORD0_1 = (uv_4 + tmpvar_10).xy;
    out_v.xlv_TEXCOORD0_2 = (uv_4 + tmpvar_11).xy;
    out_v.xlv_TEXCOORD0_3 = (uv_4 + tmpvar_12).xy;

    return out_v;
}

OUT_Data_Frag frag(v2f in_f) {
    OUT_Data_Frag out_f;
    float4 c = tex2D(_MainTex, in_f.xlv_TEXCOORD0) +
               tex2D(_MainTex, in_f.xlv_TEXCOORD0_1) +
               tex2D(_MainTex, in_f.xlv_TEXCOORD0_2) +
               tex2D(_MainTex, in_f.xlv_TEXCOORD0_3);

    c /= 4;
    c.rgb *= _Color.rgb;
    c.rgb *= (c.a + _Color.a);
    c.a = 0;

    out_f.color = c;
    return out_f;
}

ENDCG

 }
}
Fallback Off
}
