#ifndef FIXED_LIT_CGINC_
#define FIXED_LIT_CGINC_

#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float3 lightDirection : TEXCOORD1;
    float3 normal : TEXCOORD2;
    UNITY_FOG_COORDS(3)
    float4 vertex : SV_POSITION;
};

float4 _SurfaceColor;
sampler2D _MainTex;
float4 _MainTex_ST;

uniform float4 _FixedAmbientColor;
uniform float4 _FixedLightColor;
uniform float3 _FixedLightDirection;
float _DepthOffset;

v2f vert (appdata v)
{
    v2f o;

    float4 positionCs = UnityObjectToClipPos(v.vertex);

#if defined(UNITY_REVERSED_Z)
    positionCs.z += _DepthOffset * positionCs.w;
#else
    positionCs.z -= _DepthOffset * positionCs.w;
#endif

    o.vertex = positionCs;
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.normal = normalize(mul(v.normal, unity_WorldToObject));
    o.lightDirection = -normalize(mul((float3x3) UNITY_MATRIX_I_V, _FixedLightDirection));
    // o.lightDirection = -_FixedLightDirection;

    UNITY_TRANSFER_FOG(o,o.vertex);

    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = _SurfaceColor * tex2D(_MainTex, i.uv);

    float3 diffuseReflection = _FixedLightColor.rgb * max(0.0, dot(i.normal, i.lightDirection));
    col.rgb *= min(diffuseReflection + _FixedAmbientColor.rgb, 1.0);

    UNITY_APPLY_FOG(i.fogCoord, col);

    return col;
}

#endif // FIXED_LIT_CGINC_