Shader "UI/Particles/Hidden" {
SubShader { 
 LOD 100
 Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
  ZWrite Off
  Cull Off
  GpuProgramID 25687
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


struct appdata_t
{
};

struct OUT_Data_Vert
{
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 vertex :Position;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

#define CODE_BLOCK_VERTEX
OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    out_v.vertex = float4(0, 0, 0, 0);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    discard;
    out_f.color = float4(0, 0, 0, 0);
    return out_f;
}


ENDCG

}
}
}