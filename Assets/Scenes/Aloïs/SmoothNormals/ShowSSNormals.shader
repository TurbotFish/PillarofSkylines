Shader "Unlit/ShowSSNormals"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
				float4 screenPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _CameraDepthNormalsTexture;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				//o.screenPos.y = 1 - o.screenPos.y;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 screenNormals;
				float screenDepth;
				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.screenPos.xy), screenDepth, screenNormals);
				float3 col = screenNormals;


				//float4 refracColour = tex2Dproj(_BackgroundTex, UNITY_PROJ_COORD(i.grabPos + i.refraction * GetThickness(i)));


				return fixed4(col, 1);
			}
			ENDCG
		}
	}
}
