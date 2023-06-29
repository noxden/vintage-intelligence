// Created by Krista Plagemann using https://roystan.net/articles/toon-water/ //

Shader "Unlit/WaterShader"
{
    Properties
    {
        _FoamColor("Foam Color", Color) = (1,1,1,1)

        _SurfaceNoise ("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseCutoff("Surface noise cutoff", Range(0,1)) = 0.777
        _FoamMaxDistance("Foam maximum distance", Float) = 0.4
        _FoamMinDistance("Foam minimum distance", Float) = 0.04
        _DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance("Depth Maximum Distance", Float) = 1

        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}
        _SurfaceDistortionAmount("Surface Distortion Amount", Range(0,1)) = 0.27
        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03,0.03,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #define SMOOTHSTEP_AA 0.01
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 viewNormal : NORMAL;
                float2 noiseUV : TEXCOORD0;
                float2 distortUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 screenPosition : TEXCOORD2;
            };

            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;

            sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.viewNormal = COMPUTE_VIEW_NORMAL;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);

                // get the screenPosition
                o.screenPosition = ComputeScreenPos(o.vertex);

                return o;
            }

            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;          
            float _DepthMaxDistance;
            sampler2D _CameraDepthTexture;

            float _SurfaceNoiseCutoff;
            float _FoamMinDistance;
            float _FoamMaxDistance;

            float2 _SurfaceNoiseScroll;
            float _SurfaceDistortionAmount;

            sampler2D _CameraNormalsTexture;

            float4 _FoamColor;

            float4 alphaBlend(float4 top, float4 bottom)
            {
                float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
                float alpha = top.a + bottom.a * (1- top.a);

                return float4(color, alpha);
            }

            fixed4 frag (v2f i) : SV_Target
            {
            ///////// Water depth
                // samples the depth texture to a range of 0 to 1, then puts it into a linear space
                float cameraDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
                float cameraDepthLinear = LinearEyeDepth(cameraDepth);

                // Calculates the depth relative to the surface
                float depthDifference = cameraDepthLinear - i.screenPosition.w;

                // Change the range to fit to maximum depth by using saturate(kinda a clamp)
                float waterDepthDifference = saturate(depthDifference / _DepthMaxDistance);
                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference);

            ///////// Scrolling noise with distortion
                float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;
                float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);

            ///////// Edge foam
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;

                float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition));
                float3 normalDot = saturate(dot(existingNormal, i.viewNormal));

                float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
                float foamDepthDifference = saturate(depthDifference / foamDistance);
                float surfaceNoiseCutoff = foamDepthDifference * _SurfaceNoiseCutoff;

                float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);    // all values darker than threshhold are ignored, rest is white
                
                float4 surfaceNoiseColor = _FoamColor;
                surfaceNoiseColor.a *= surfaceNoise;
                return alphaBlend(surfaceNoiseColor, waterColor);
            }
            ENDCG
        }
    }
}
