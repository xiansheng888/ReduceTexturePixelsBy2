Shader "HZB/HZBBuild" {

    Properties {
        [HideInInspector] _DepthTexture("Depth Texture", 2D) = "black" {}
        [HideInInspector] _InvSize("Inverse Mipmap Size", Vector) = (0, 0, 0, 0) //x,y = (1/MipMapSize.x, 1/MipMapSize.y), zw = (0, 0)
    }

    SubShader {
		Pass {
            Cull Off ZWrite Off ZTest Always
            
			Name "HZBBuild"

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex HZBVert
            #pragma fragment HZBBuildFrag
            //#pragma enable_d3d11_debug_symbols

			sampler2D _DepthTexture;
 			float4 _InvSize;

            struct HZBAttributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct HZBVaryings
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;

            };

            inline float4 HZBReduce(sampler2D  mainTex, float2 inUV, float2 invSize)
            {
                float2 uv0 = inUV + float2(-0.25f, -0.25f) * invSize;
                float2 uv1 = inUV + float2(0.25f, -0.25f) * invSize;
                float2 uv2 = inUV + float2(-0.25f, 0.25f) * invSize;
                float2 uv3 = inUV + float2(0.25f, 0.25f) * invSize;

                float4 sample0  = tex2D(mainTex, uv0);
                float4 sample1  = tex2D(mainTex, uv1);
                float4 sample2  = tex2D(mainTex, uv2);
                float4 sample3  = tex2D(mainTex, uv3);

                float4 average = (sample0 + sample1 + sample2 + sample3) * 0.25;
                return average;
 
            }

            HZBVaryings HZBVert(HZBAttributes v)
            {
                HZBVaryings o;
                o.vertex = UnityObjectToClipPos(v.vertex.xyz);
                o.uv = v.uv;

                return o;
            }

			float4 HZBBuildFrag(HZBVaryings input) : Color
			{	   
				float2 invSize = _InvSize.xy;
				float2 inUV = input.uv;

				float4 depth = HZBReduce(_DepthTexture, inUV, invSize);

				//return float4(depth, 0, 0, 1.0f);
                return depth;
			}

            
			ENDCG
		}
    }
}