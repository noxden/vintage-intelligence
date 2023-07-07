Shader "Unlit/Hologram"
{
    Properties  // what shows up in material details. Shader UI
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Power("Contrast", Float) = 1
    }
        SubShader   // can have multiple subshaders, but usually one
        {
            Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}  // tell unity what the intention of the shader is
            LOD 100     // sort out sub sahders by LOD
            Blend One One
            Zwrite Off

            Pass    // shader code, subshader can have multiple passes, renderes the object multiple times. 
            {
                CGPROGRAM   // actual shader code
                #pragma vertex vert     // #pragma tells the compiler directly. This: vertex shader is called vert
                #pragma fragment frag   // fragment shader is called frag
                // make fog work
                //#pragma multi_compile_fog

                #include "UnityCG.cginc"    // funtions to interface with Unity, e.g. light system or fog system

                struct appdata  // struct is a class without functions (container of variables) -> this: what I request from my object
                {
                    float4 vertex : POSITION;   //shader semantics
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL; // we need normal for hologram shader
                };
                struct v2f      // vertex to fragment
                {
                    float2 uv : TEXCOORD0;
                    //UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                    float3 normal : NORMAL; // we need normal for hologram shader
                    float3 worldPos : TEXCOORD1; // vertex position for calculations (store it in texCoord1)
                };

                sampler2D _MainTex; //
                float4 _MainTex_ST; // 

                v2f vert(appdata v)    // vertex shader
                {
                    v2f o;  // new v2f object
                    o.vertex = UnityObjectToClipPos(v.vertex);  // fills member variable "vertex" which holds vertex position. This: multiply with MVP Matrix
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);   // Texture Gizmos in Detail
                    o.normal = mul(UNITY_MATRIX_M, v.normal);
                    float4 objectSpacePos = v.vertex;
                    o.worldPos = mul(UNITY_MATRIX_M, objectSpacePos); // get world space position
                    return o;   // return the object
                }

                float4 _Color;
                float _Power;


                float4 frag(v2f i) : SV_Target // default fixed4, we use float4 for higher bit precision
                {
                    i.normal = normalize(i.normal);
                    // sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv); // 4 values for color RGBA, what color is at texture uv position, return to screen
                    col.rgb = _Color; // add color
                    float3 viewDir = i.worldPos - _WorldSpaceCameraPos;// vertPos - camPos
                    viewDir = normalize(viewDir);
                    float facing = dot(i.normal.xyz, viewDir) * - 1; // everything pointing away from each other is below 0 in dot product, so multiply by -1
                    float inverseFacing = 1 - facing;
                    //col.xyz = i.normal.xyz;    // color will be multiplied with normal // color.rgb = color.xyz
                    return col * pow(inverseFacing, _Power);  // to the power of makes sharper
                }
                ENDCG
            }
        }

}