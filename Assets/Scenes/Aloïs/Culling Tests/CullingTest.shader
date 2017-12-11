Shader "Alo/Culling/CullingTest"
{
	Properties
	{
		_MainColor ("Colour", Color) = (0.5,0.1,0.7,1)
		_Cull ("Culling", Float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{

			Cull [_Cull]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _MainColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _MainColor;
				return col;
			}
			ENDCG
		}
	}
	CustomEditor "CullingTestEditor"
}
