Shader "Unlit/ClippedObject"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        // The shader is semi-transparent
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        LOD 100


        CGINCLUDE
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
                float4 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
                
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _Plane;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(UNITY_MATRIX_M, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distance = dot(i.worldPos, _Plane.xyz);
		distance = distance + _Plane.w;
		clip(-distance);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG

        // The first pass will write only in the
        // stencil buffer the value 2 for the clipped
        // object. Only front faces are rendered to get the difference
        // for back faces. All depth and color buffer related stuff are discarded.
        Pass
        {
            Cull Back
            ZTest Always
            ZWrite Off
            Blend Zero One

            Stencil {
                Ref 2
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            ENDCG
        }

        // The second pass will write only in the stencil buffer
        // the value 1 for pixels with a current value of 0. Since
        // we have previously written value 2 for front faces, the only
        // remaining pixels available will be the clipping plane part, where
        // the back faces are visible (if any).
        // All depth and color buffer related stuff are discarded.
        Pass {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend Zero One

            Stencil {
                Ref 0
                Comp Equal
                Pass IncrSat
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            ENDCG
        }

        // The last pass for the clipped object is actually the real render pass.
        // Only visible front faces with a pixel value in stencil buffer equals to 2
        // will be rendered. A traditional blending operation for semi-transparency
        // is used.
        // NOTE : Actually this pass also uses the fragment shader with the clipping
        // function, but it is useless as the clipped part of the object has a value
        // of 0 in the stencil buffer and won't be rendered anyway. I was too lazy to
        // copy/paste the fragment shader...
        Pass
        {
            Stencil {
                Ref 2
                Comp Equal
            }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            ENDCG
        }
    }
}
