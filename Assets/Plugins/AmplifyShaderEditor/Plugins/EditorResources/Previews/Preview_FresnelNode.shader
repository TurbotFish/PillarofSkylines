Shader "Hidden/FresnelNode"
{
	Properties
	{
		_A ("_Normal", 2D) = "white" {}
		_B ("_Bias", 2D) = "white" {}
		_C ("_Scale", 2D) = "white" {}
		_D ("_Power", 2D) = "white" {}
	}
	SubShader
	{
		Pass //not connected world
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			//sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );
				r = saturate( r );

				float2 uvs;
				float f = ( 1 - sqrt( 1 - r ) ) / r;
				uvs.x = p.x;
				uvs.y = p.y;
				float3 vertexPos = float3( uvs, ( f - 1 ) * 2 );
				float3 normal = normalize(vertexPos);
				float3 worldNormal = UnityObjectToWorldNormal(normal);
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				return fresnel;
			}
			ENDCG
		}

		Pass //connected world
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );
				r = saturate( r );

				float2 uvs;
				float f = ( 1 - sqrt( 1 - r ) ) / r;
				uvs.x = p.x;
				uvs.y = p.y;
				float3 vertexPos = float3( uvs, ( f - 1 ) * 2 );
				float3 worldNormal = tex2D( _A, i.uv );
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				return fresnel;
			}
			ENDCG
		}

		Pass //connected tangent
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;

			float4 frag(v2f_img i) : SV_Target
			{
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );
				r = saturate( r );

				float2 uvs;
				float f = ( 1 - sqrt( 1 - r ) ) / r;
				uvs.x = p.x;
				uvs.y = p.y;
				float3 vertexPos = float3( uvs, ( f - 1 ) * 2 );
				float3 normal = normalize(vertexPos);
				float3 worldNormal = UnityObjectToWorldNormal(normal);

				float3 tangent = normalize(float3( (1-f)*2, p.y*0.01, p.x ));
				float3 worldPos = mul(unity_ObjectToWorld, float4(vertexPos,1)).xyz;
				float3 worldTangent = UnityObjectToWorldDir(tangent);
				float tangentSign = -1;
				float3 worldBinormal = normalize( cross(worldNormal, worldTangent) * tangentSign);
				float4 tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				float4 tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				float4 tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				float2 sphereUVs = i.uv;

				sphereUVs.x = (atan2(vertexPos.x, -vertexPos.z) / (UNITY_PI) + 0.5);
				float3 tangentNormal = tex2D(_A, sphereUVs).xyz;

				worldNormal = fixed3( dot( tSpace0.xyz, tangentNormal ), dot( tSpace1.xyz, tangentNormal ), dot( tSpace2.xyz, tangentNormal ) );

				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float fresnel = (b + s*pow(s - dot( worldNormal, worldViewDir ) , pw));
				return fresnel;
			}
			ENDCG
		}
	}
}
