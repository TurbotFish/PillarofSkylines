Shader "Alo/Decal/DecalDiffuse"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_AtlasSizeX ("Atlas Size X",Int) = 4
		_AtlasSizeY ("Atlas Size Y",Int) = 4

		_AtlasCoordX ("Texture Coord X", Int) = 1
		_AtlasCoordY ("Texture Coord Y", Int) = 1
	}
	SubShader
	{

		Pass
		{

			Fog {Mode Off}
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers nomrt
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenUV : TEXCOORD1;
				float3 ray : TEXCOORD2;
				half3 orientation : TEXCOORD3;
			};

			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			sampler2D _NormalsCopy;
			float _AtlasSizeX;
			float _AtlasSizeY;

			float _AtlasCoordX;
			float _AtlasCoordY;
			
			v2f vert (float3 v : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(float4(v,1));
				o.uv = v.xz + 0.5;
				o.screenUV = ComputeScreenPos (o.pos);
				o.ray = UnityObjectToViewPos(float4(v,1)).xyz * float3(-1,-1,1);
				o.orientation = mul((float3x3)unity_ObjectToWorld, float3(0,1,0));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
				float2 uv = i.screenUV.xy / i.screenUV.w;
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth(depth);
				float4 vpos = float4(i.ray * depth, 1);
				float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
				float3 opos = mul(unity_WorldToObject, float4(wpos,1)).xyz;

				clip(float3(0.5,0.5,0.5) - abs(opos.xyz));

				i.uv = opos.xz + 0.5;

				half3 normal = tex2D(_NormalsCopy, uv).rgb;
				fixed3 wnormal = normal.rgb * 2.0 - 1.0;
				clip(dot(wnormal, i.orientation) - 0.3);

				fixed2 _atlasWalk;
				_atlasWalk.x = 1/_AtlasSizeX;
				_atlasWalk.y = 1/_AtlasSizeY;

				float2 _atlasUV;
				_atlasUV.x = i.uv.x * _atlasWalk.x + _atlasWalk.x * (_AtlasCoordX - 1);
				_atlasUV.y = i.uv.y * _atlasWalk.y + (_AtlasSizeY - _AtlasCoordY)/_AtlasSizeY;

				fixed4 col = tex2D(_MainTex, _atlasUV);
				return col;
			}
			ENDCG
		}
	}
}
