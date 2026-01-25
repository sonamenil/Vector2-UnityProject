Shader "Custom/Unlit/Transparent Colored With Mask 1" {
Properties {
    _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
    _AlphaTex ("MaskTexture", 2D) = "white" { }
    _RotationSpeed ("Rotation Speed", Float) = 2.0
}
SubShader {
    LOD 200
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }

    Pass {
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        uniform float _RotationSpeed;
        uniform float4 _MainTex_ST;
        uniform sampler2D _MainTex;
        uniform sampler2D _AlphaTex;
        uniform float4 _ClipRange0;
        uniform float2 _ClipArgs0;

        struct appdata_t {
            float4 vertex : POSITION;
            float4 color : COLOR;
            float4 texcoord : TEXCOORD0;
        };

        struct v2f {
            float2 uv : TEXCOORD0;
            float2 uvMask : TEXCOORD1;
            float4 color : COLOR;
            float2 clip : TEXCOORD2;
            float4 vertex : SV_POSITION;
        };

        v2f vert(appdata_t v) {
            v2f o;
            // Transform UVs
            float2 uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            float angle = _RotationSpeed * _Time.y;
            float s = sin(angle);
            float c = cos(angle);
            float2 centered = uv - 0.5;
            float2 rotated = float2(centered.x * c - centered.y * s, centered.x * s + centered.y * c) + 0.5;
            o.uv = rotated;
            o.uvMask = uv;
            o.color = v.color;
            o.clip = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
            o.vertex = UnityObjectToClipPos(v.vertex);
            return o;
        }

        struct OUT_Data_Frag {
            float4 color : SV_Target0;
        };

        OUT_Data_Frag frag(v2f i) {
            OUT_Data_Frag o;
            float4 col = tex2D(_MainTex, i.uv) * i.color;
            float alphaMask = tex2D(_AlphaTex, i.uvMask).a;
            col.a = min(col.a, alphaMask);
            float clipFactor = clamp(min(1.0 - abs(i.clip.x), 1.0 - abs(i.clip.y)) * _ClipArgs0.x, 0, 1);
            col.a *= clipFactor;
            o.color = col;
            return o;
        }
        ENDCG
    }

Pass {
    LOD 100
    ZWrite Off
    Cull Off
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB

    CGPROGRAM
    #pragma vertex vert2
    #pragma fragment frag2
    #include "UnityCG.cginc"

    uniform float4 _MainTex_ST;
    uniform sampler2D _MainTex;

    struct appdata_t {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float4 texcoord : TEXCOORD0;
    };

    struct v2f {
        float4 color : COLOR;
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

    v2f vert2(appdata_t v) {
        v2f o;
        o.color = saturate(v.color);
        o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
        o.vertex = UnityObjectToClipPos(v.vertex);
        return o;
    }

    // Corrected fragment shader
    fixed4 frag2(v2f i) : SV_Target {
        fixed4 col = tex2D(_MainTex, i.uv) * i.color;
        if(col.a <= 0.01) discard;
        return col;
    }
    ENDCG
}

}
}
