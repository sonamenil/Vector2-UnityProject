Shader "Custom/DiagonalClipSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color   ("Tint", Color) = (1,1,1,1)

        // How steep the diagonal cut is.
        // 1.0  -> perfect corner-to-corner (bottom-left to top-right)
        // higher -> "flatter"
        // lower  -> "steeper"
        _Slope   ("Diagonal Slope", Range(0.1,4.0)) = 1.0

        // Push the cut up/down. 0 = cut goes exactly through top-right/bottom-left.
        _Offset  ("Cut Offset", Range(-1,1)) = 0.0

        _Type ("Type", Range(1,2)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Slope;
            float _Offset;
            float _Type;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;      // so it still works like a SpriteRenderer tint
            };

            struct v2f
            {
                float4 pos    : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.uv    = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // --- GEOMETRY MASK ---
                // For a perfect diagonal cut from bottom-left (0,0) to top-right (1,1),
                // the line is:  y = x
                //
                // We want to KEEP one side of that line and discard the other.
                //
                // I'm giving you control:
                //
                //   line(y) = _Slope * x + _Offset
                //
                // We'll only render pixels where i.uv.y >= (_Slope * i.uv.x + _Offset)
                // You can tweak _Slope and _Offset in the material to match your exact wedge.

                float lineY = _Slope * i.uv.x + _Offset;

                // If the pixel is "above" the allowed region, kill it (transparent)
                if (round(_Type) == 2)
                {
                    if (i.uv.y < lineY)
                    {
                        discard;
                    }
                }
                else
                {
                    if (i.uv.y > lineY)
                    {
                        discard;
                    }
                }


                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // You can also premultiply alpha or clamp alpha here if you like.
                return col;
            }
            ENDCG
        }
    }
}
