Shader "BlendModes/MeshBumpGlass/Framebuffer" {
Properties {
    _BumpAmt ("Distortion", Range(0,128)) = 10
    _MainTex ("Tint Color (RGB)", 2D) = "white" { }
    _BumpMap ("Normalmap", 2D) = "bump" { }
}
SubShader { 
    Tags { "Queue"="Transparent" "RenderType"="Opaque" }

    Pass {
        Name "BASE"
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        uniform float4 _BumpMap_ST;
        uniform float4 _MainTex_ST;
        uniform sampler2D _MainTex;
        uniform sampler2D _BumpMap;

        struct appdata_t {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
        };

        struct v2f {
            float2 uvBump : TEXCOORD0;
            float2 uvMain : TEXCOORD1;
            float4 vertex : SV_POSITION;
        };

        v2f vert(appdata_t v) {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uvBump = TRANSFORM_TEX(v.texcoord.xy, _BumpMap);
            o.uvMain = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
            return o;
        }

        fixed4 frag(v2f i) : SV_Target {
            // Simple min blend between MainTex and BumpMap for distortion effect
            fixed4 mainCol = tex2D(_MainTex, i.uvMain);
            fixed4 bumpCol = tex2D(_BumpMap, i.uvBump);
            fixed3 blended = min(mainCol.rgb, bumpCol.rgb);
            return fixed4(blended, mainCol.a);
        }
        ENDCG
    }
}
CustomEditor "BMMaterialEditor"
}
