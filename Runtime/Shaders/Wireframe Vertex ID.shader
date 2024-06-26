// Wireframe shader that requires a mesh where all triangles are split. And all triplets of vertices have sequential vertex IDs.
Shader "Orchid Seal/ParaDraw/Wireframe Vertex ID"
{
    Properties
    {
        [PerRendererData] _WireColor("Wireframe Color", Color) = (0, 0, 0)
        _WireSmoothness("Wireframe Smoothness", Range(0, 10)) = 1
        [PerRendererData] _WireThickness("Wire Thickness", RANGE(0, 800)) = 100
        _Cutoff("Alpha cutoff", Range(0.15, 0.85)) = 0.4
        _DepthOffset("Depth offset", Range(0, 0.001)) = 0.00001
    }
    SubShader
    {
        Tags
        {
            "IgnoreProjector" = "True"
            "Queue" = "AlphaTest"
            "RenderType" = "TransparentCutout"
        }

        LOD 100
        AlphaToMask On
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                uint vid : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 barycentric : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float3 _WireColor;
            float _WireSmoothness;
            float _WireThickness;
            fixed _Cutoff;
            float _DepthOffset;

            static const float3 barycentricCorners[3] =
            {
                float3(1.0, 0.0, 0.0),
                float3(0.0, 1.0, 0.0),
                float3(0.0, 0.0, 1.0)
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 positionCs = UnityObjectToClipPos(v.vertex);

#if defined(UNITY_REVERSED_Z)
                positionCs.z += _DepthOffset * positionCs.w;
#else
                positionCs.z -= _DepthOffset * positionCs.w;
#endif

                o.vertex = positionCs;
                o.barycentric.xyz = barycentricCorners[v.vid % 3];
                o.barycentric.w = 1.0 / positionCs.w;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 barys;
                barys.xy = i.barycentric.xy;
                barys.z = 1 - barys.x - barys.y;

                float3 deltas = fwidth(barys);
                float3 smoothing = deltas * _WireSmoothness;
                float3 thickness = deltas * 0.005 * _WireThickness;
                barys = smoothstep(thickness, thickness + smoothing, barys);

                float minBary = min(min(barys.x, barys.y), barys.z);

                float t = 1.0 - minBary;
                fixed4 col = fixed4(_WireColor, (t - _Cutoff) / max(fwidth(t), 0.0001) + 0.5);

                return col;
            }
            ENDCG
        }
    }
}
