Shader "Orchid Seal/ParaDraw/Point"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Cutoff("Cutoff", Float) = 0.5
        [Toggle(SILHOUETTE_FADING_ON)] _SilhouetteFadingEnabled("Silhouette Fading", Float) = 1.0
        _SilhouetteFadeParams("Silhouette Fading Params", Vector) = (-0.5,1,0.2,0)
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    struct appdata_particles
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float4 color : COLOR;
        UNITY_FOG_COORDS(0)
        float2 uv : TEXCOORD1;
        #if defined(SILHOUETTE_FADING_ON) || defined(SOFTPARTICLES_ON) || defined(_FADING_ON)
            float4 projectedPosition : TEXCOORD3;
        #endif
        UNITY_VERTEX_OUTPUT_STEREO
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;
    UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
    float _Cutoff;
    float4 _SilhouetteFadeParams;

    #if defined(SILHOUETTE_FADING_ON) || defined(SOFTPARTICLES_ON) || defined(_FADING_ON)
    #define vertFading(o) \
        o.projectedPosition = ComputeScreenPos(clipPosition); \
        COMPUTE_EYEDEPTH(o.projectedPosition.z);
    #else
    #define vertFading(o)
    #endif

    #if defined(_ALPHAPREMULTIPLY_ON)
    #define ALBEDO_MUL albedo
    #else
    #define ALBEDO_MUL albedo.a
    #endif

    #define SILHOUETTE_NEAR_FADE _SilhouetteFadeParams.x
    #define SILHOUETTE_FAR_FADE _SilhouetteFadeParams.y

    #if defined(SILHOUETTE_FADING_ON)
    #define fragSilhouetteCameraFading(i) \
        float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projectedPosition))); \
        float silhouetteFade = saturate((1.0 / (SILHOUETTE_FAR_FADE - SILHOUETTE_NEAR_FADE)) * ((sceneZ - SILHOUETTE_NEAR_FADE) - i.projectedPosition.z)); \
        ALBEDO_MUL *= silhouetteFade;
    #else
    #define fragSilhouetteCameraFading(i) \
        float silhouetteFade = 1.0f;
    #endif

    v2f vert(appdata_particles v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        float4 clipPosition = UnityObjectToClipPos(v.vertex);

        o.color = v.color;
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.vertex = clipPosition;

        vertFading(o);

        UNITY_TRANSFER_FOG(o, o.vertex);
        return o;
    }
    ENDCG

    SubShader
    {
        Tags
        {
            "IgnoreProjector" = "True"
            "Queue" = "AlphaTest"
            "RenderType" = "TransparentCutout"
        }

        Lighting Off

        Pass
        {
            Name "Silhouette"
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            Cull Off
            ZTest Always
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            #pragma shader_feature_local SILHOUETTE_FADING_ON

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 albedo = i.color * tex2D(_MainTex, i.uv);
                albedo.a *= _SilhouetteFadeParams.z;
                clip(albedo.a - _Cutoff);

                fragSilhouetteCameraFading(i);
                UNITY_APPLY_FOG(i.fogCoord, albedo);

                return albedo;
            }
            ENDCG
        }

        Pass
        {
            Name "Point"
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            Cull Off
            ZTest LEqual
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 albedo = i.color * tex2D(_MainTex, i.uv);
                clip(albedo.a - _Cutoff);

                UNITY_APPLY_FOG(i.fogCoord, albedo);

                return albedo;
            }
            ENDCG
        }
    }
}
