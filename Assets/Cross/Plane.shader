Shader "Unlit/Plane"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        // The shader is semi-transparent.
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" "IgnoreProjector"="True" }
        LOD 100

        // The clipping plane uses only an usual single render pass.
        // The only modification is the Stencil buffer test : only
        // pixels with a current value of 1 (back faces of the other object at the clip section)
        // will be rendered. A custom blending is also added for semi-transparency.
        // A little hack on the vertex position is made to scale up the plane (don't need
        // to do that in the Editor).

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil {
                Ref 1
                Comp Equal
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
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
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xyz *= 10000.;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
