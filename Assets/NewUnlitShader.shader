Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex1 ("Texture1", 2D) = "white" {}
         _MainTex2 ("Texture2", 2D) = "white" {}
          _MainTex3 ("Texture3", 2D) = "white" {}
           _MainTex4 ("Texture4", 2D) = "white" {}
            _MainTex5 ("Texture5", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
           
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex1;
             sampler2D _MainTex2;
              sampler2D _MainTe3;
               sampler2D _MainTe4;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
               
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex1, i.uv);
              
             
                return col;
            }
            ENDCG
        }
    }
}
