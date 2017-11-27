Shader "Unlit/GrabPassTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"
			"Queue" = "Transparent"
		}
		LOD 100

		GrabPass{
			"_BackgroundTex"
		}

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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BackgroundTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 colcol = tex2D(_BackgroundTex, float2(0.5,0.5))*fixed4(0.1,0.5,0.9,1.0);
				//col *= colcol;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return fixed4(0.2,0.6,0.8,1.0);
			}
			ENDCG
		}
	}
}
