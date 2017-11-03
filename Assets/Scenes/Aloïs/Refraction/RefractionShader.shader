Shader "Alo/Experiments/RefractionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "grey" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }

		GrabPass{
			"_BackgroundTex"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
				float4 grabPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BackgroundTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 bgColor = tex2Dproj(_BackgroundTex, i.grabPos);
				return col * bgColor;
			}
			ENDCG
		}
	}
}
