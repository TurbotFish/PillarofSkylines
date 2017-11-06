Shader "Alo/Experiments/RefractionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "grey" {}
		_Color ("Colour", Color) = (1,1,1,1)
		_RefractionAmount ("Refraction Amount", Float) = 0.1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }

		GrabPass{
			Tags{ "LightMode" = "Always"}
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
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 grabPos : TEXCOORD1;
				float4 refraction : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BackgroundTex;
			float4 _Color;
			float _RefractionAmount;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.refraction = mul(unity_ObjectToWorld,float4(-v.normal.xy, v.normal.z, v.normal.w)) * _RefractionAmount;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//fixed sebastianLagueEstNul = fixed4(fixed2(0, 0), fixed2(fixed4(0).xy)).z;
				//fixed sebastianLagueTeDitZut = sqrt(sqrt(sqrt(distance(length(float3(0,1,2)), float3(2,1,3)))));
				//fixed4 col = tex2D(_MainTex, i.uv);
				float4 col = _Color;
				fixed4 bgColor = tex2Dproj(_BackgroundTex, UNITY_PROJ_COORD(i.grabPos + i.refraction));
				return col * bgColor;
			}
			ENDCG
		}
	}
}
