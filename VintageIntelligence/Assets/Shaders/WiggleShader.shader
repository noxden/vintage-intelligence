Shader "Unlit/WiggleShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color Tint", Color) = (1,1,1,1)        // Color Tint
        _Speed("Speed", Range(0,50)) = 2.0              // Speed of the wobble
        _Amplitude("Amplitude(Wobble size)", float) = 1.0   // how far it wobbles
        _Frequency("Frequency", float) = 1.0                // how often it wobbles in a flame
        _CenterShift("Center Shift", float) = 1.0           // where the center of wobble is
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque"}
            LOD 100

            // ZWrite Off  // Turns off ZWrite
            // Blend One One   // adds to behind objects (blend mode addition)

            // turn off ZBuffer writing
            // front to back Rendering(Painters Algorithm)
            // set our blend mode

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                // Variables for wobble
                float _Speed;
                float _Amplitude;
                float _Frequency;
                float _CenterShift;

                v2f vert(appdata v)
                {
                    v2f o;
                    float wobble = sin(v.vertex.y * _Frequency + _Time.y * _Speed) * _Amplitude;  // makes the wobble value

                    float strength = clamp(v.vertex.y + _CenterShift, 0, 1);    // makes the wiggle go from CenterShift or smth
                    wobble *= strength;     // changed the strength of wobble

                    v.vertex.x += wobble;   // adds the wobble to the vertex

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }


                float4 _Color;

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col *= _Color;
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
}
