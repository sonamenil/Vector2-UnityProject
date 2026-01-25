Shader "DynamicShadowProjector/Blit/Blit" {
Properties {
    _MainTex ("Albedo (RGB)", 2D) = "white" { }
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

uniform float _MipLevel;
uniform sampler2D _MainTex;

struct appdata_t {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
};

struct OUT_Data_Vert {
    float2 uv : TEXCOORD0;
    float vertexLod : TEXCOORD1;
    float4 vertex : SV_POSITION;
};

struct v2f {
    float2 uv : TEXCOORD0;
    float vertexLod : TEXCOORD1;
};

struct OUT_Data_Frag {
    float4 color : SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v) {
    OUT_Data_Vert out_v;
    out_v.uv = in_v.texcoord.xy;
    out_v.vertexLod = _MipLevel;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    return out_v;
}

OUT_Data_Frag frag(v2f in_f) {
    OUT_Data_Frag out_f;
    #if defined(GL_EXT_shader_texture_lod)
        out_f.color = tex2Dlod(_MainTex, float4(in_f.uv, 0, in_f.vertexLod));
    #else
        out_f.color = tex2D(_MainTex, in_f.uv);
    #endif
    return out_f;
}

ENDCG
 }
}
}
