Shader "Custom/LightningBoltShaderMeshNoGlow" {
Properties {
 _MainTex ("Color (RGB) Alpha (A)", 2D) = "white" { }
 _TintColor ("Tint Color (RGB)", Color) = (1.000000,1.000000,1.000000,1.000000)
 _InvFade ("Soft Particles Factor", Range(0.010000,3.000000)) = 1.000000
 _JitterMultiplier ("Jitter Multiplier (Float)", Float) = 0.000000
 _Turbulence ("Turbulence (Float)", Float) = 0.000000
 _TurbulenceVelocity ("Turbulence Velocity (Vector)", Vector) = (0.000000,0.000000,0.000000,0.000000)
}
SubShader { 
 Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent+10" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" }
 UsePass "Custom/LightningBoltShaderMesh/LINEPASS"
}
Fallback "Particles/Additive (Soft)"
}