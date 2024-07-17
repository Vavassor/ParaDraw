Shader "Orchid Seal/ParaDraw/Fixed Lit Transparent"
{
    Properties
    {
        [PerRendererData] _SurfaceColor("Surface Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex ("Texture", 2D) = "white" {}
        _FixedAmbientColor("Ambient Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _FixedLightColor("Light Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _FixedLightDirection("Light Direction", Vector) = (-0.6929, -0.6929, -0.6929, 0.0)
        _DepthOffset("Depth offset", Range(0, 0.001)) = 0.00001
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Fixed Lit.cginc"
            ENDCG
        }
    }
}
