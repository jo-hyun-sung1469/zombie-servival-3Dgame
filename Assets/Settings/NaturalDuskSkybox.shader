Shader "Game/Skybox/Natural Dusk"
{
    Properties
    {
        _ZenithColor ("Upper Sky", Color) = (0.12, 0.17, 0.25, 1)
        _MidColor ("Twilight", Color) = (0.34, 0.29, 0.38, 1)
        _HorizonColor ("Sunset Horizon", Color) = (0.68, 0.34, 0.24, 1)
        _AwayColor ("Opposite Horizon", Color) = (0.36, 0.30, 0.38, 1)
        _GroundColor ("Below Horizon", Color) = (0.19, 0.18, 0.23, 1)
        _SunColor ("Faint Sun", Color) = (0.78, 0.43, 0.23, 1)
        _SunSize ("Sun Radius (Degrees)", Range(0, 1)) = 0.2
        _Exposure ("Exposure", Range(0, 3)) = 1
        // DayNightSystem writes these properties while driving the sun rotation.
        [HideInInspector] _Transition ("Transition", Float) = 1
        [HideInInspector] _FogDensity ("Sky Fog Density", Float) = 0
        [HideInInspector] _FogColor ("Sky Fog Color", Color) = (0, 0, 0, 1)
        [HideInInspector] _StarIntensity ("Star Intensity", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            half4 _ZenithColor, _MidColor, _HorizonColor, _AwayColor, _GroundColor, _SunColor;
            float _SunSize, _Exposure;

            struct Attributes
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 direction : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.direction = mul((float3x3)unity_ObjectToWorld, input.vertex.xyz);
                return output;
            }
            half4 frag(Varyings input) : SV_Target
            {
                float3 direction = normalize(input.direction);
                float3 sunDirection = normalize(_WorldSpaceLightPos0.xyz);
                float height = max(direction.y, 0.0);
                float2 horizontal = direction.xz / max(length(direction.xz), 0.0001);
                float2 sunHorizontal = sunDirection.xz / max(length(sunDirection.xz), 0.0001);
                float sunsetFacing = pow(saturate(dot(horizontal, sunHorizontal) * 0.5 + 0.5), 3.0);
                half3 horizon = lerp(_AwayColor.rgb, _HorizonColor.rgb, sunsetFacing);
                half3 sky = lerp(horizon, _MidColor.rgb, smoothstep(0.0, 0.23, height));
                sky = lerp(sky, _ZenithColor.rgb, smoothstep(0.08, 0.7, height));
                sky = lerp(_GroundColor.rgb, sky, smoothstep(-0.12, 0.015, direction.y));

                // The small, soft disc follows DayNightSystem's directional light.
                float angularDistance = length(direction - sunDirection);
                float radius = radians(_SunSize);
                float disc = 1.0 - smoothstep(radius * 0.6, max(radius * 1.8, 0.00001), angularDistance);
                disc *= smoothstep(-0.005, 0.008, direction.y);
                sky = lerp(sky, _SunColor.rgb, disc * 0.6);
                return half4(sky * _Exposure, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
