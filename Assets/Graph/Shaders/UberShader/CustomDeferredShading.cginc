	#if !defined(CUSTOM_DEFERRED_SHADING)
	#define CUSTOM_DEFERRED_SHADING
	//#include "AloPBSLighting.cginc"

	#include "UnityShaderVariables.cginc"
	#include "UnityStandardConfig.cginc"
	#include "UnityLightingCommon.cginc"
	#include "UnityGBuffer.cginc"
	#include "AloGlobalIllumination.cginc"
	#include "AloStandardBRDF.cginc"

	struct VertexData
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct Interpolators
	{
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		float3 ray : TEXCOORD1;
	};



	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	sampler2D _CameraGBufferTexture0;
	sampler2D _CameraGBufferTexture1;
	sampler2D _CameraGBufferTexture2;

	#if defined(SHADOWS_SCREEN)
		sampler2D _ShadowMapTexture;
	#endif

	float4 _LightColor, _LightDir, _LightPos;

	#if defined(POINT_COOKIE)
		samplerCUBE _LightTexture0;
	#else
		sampler2D _LightTexture0;
	#endif

	sampler2D _LightTextureB0;
	float4x4 unity_WorldToLight;
	float _LightAsQuad;

	UnityLight CreateLight(float2 uv, float3 worldPos, float viewZ) {

		UnityLight light;
		float attenuation = 1;
		float shadowAttenuation = 1;
		bool shadowed = false;

		#if defined(DIRECTIONAL) || (DIRECTIONAL_COOKIE)
				light.dir = -_LightDir;

			#if defined(DIRECTIONAL_COOKIE)
				float2 uvCookie = mul(unity_WorldToLight, float4(worldPos,1)).xy;
				attenuation *= tex2Dbias(_LightTexture0, float4(uvCookie,0,-8)).w;

			#endif

			#if defined(SHADOWS_SCREEN)
				shadowed = true;
				shadowAttenuation = tex2D(_ShadowMapTexture, uv).r;
			#endif
		#else
			float3 lightVec = _LightPos.xyz - worldPos;
			light.dir = normalize(lightVec);

			attenuation *= tex2D(_LightTextureB0, (dot(lightVec,lightVec) * _LightPos.w).rr).UNITY_ATTEN_CHANNEL;

			#if defined(SPOT)
				float4 uvCookie = mul(unity_WorldToLight, float4(worldPos,1));
				uvCookie.xy /= uvCookie.w;
				attenuation *= tex2Dbias(_LightTexture0, float4(uvCookie.xy,0,-8)).w;
				attenuation *= uvCookie.w < 0;

				#if defined(SHADOWS_DEPTH)
					shadowed = true;
					shadowAttenuation = UnitySampleShadowmap(mul(unity_WorldToShadow[0], float4(worldPos, 1)));
				#endif
			#else

				#if defined(POINT_COOKIE)
					float3 uvCookie = mul(unity_WorldToLight, float4(worldPos, 1)).xyz;
					attenuation *= texCUBEbias(_LightTexture0, float4(uvCookie, -8)).w;
				#endif

				#if defined(SHADOWS_CUBE)
					shadowed = true;
					shadowAttenuation = UnitySampleShadowmap(-lightVec);
				#endif

			#endif
		#endif

		if(shadowed){
			float shadowFadeDistance = UnityComputeShadowFadeDistance(worldPos, viewZ);
			float shadowFade = UnityComputeShadowFade(shadowFadeDistance);
			shadowAttenuation = saturate(shadowAttenuation + shadowFade);

			#if defined(UNITY_FAST_COHERENT_DYNAMIC_BRANCHING) && defined(SHADOWS_SOFT)
				UNITY_BRANCH
				if(shadowFade > 0.99){
					shadowAttenuation = 1;
				}
			#endif
		}

		light.color = _LightColor.rgb * (attenuation * shadowAttenuation);
		return light;
	}
	
	Interpolators VertexProgram (VertexData v)
	{
		Interpolators i;
		i.pos = UnityObjectToClipPos(v.vertex);
		i.uv = ComputeScreenPos(i.pos);
		i.ray = lerp(
			UnityObjectToViewPos(v.vertex) * float3(-1,-1,1),
			v.normal,
			_LightAsQuad
		);
		return i;
	}
	
	float4 FragmentProgram (Interpolators i) : SV_Target
	{
		float2 uv = i.uv.xy / i.uv.w;

		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
		depth = Linear01Depth(depth);

		float3 rayToFarPlane = i.ray * _ProjectionParams.z / i.ray.z;
		float3 viewPos = rayToFarPlane * depth;
		float3 worldPos = mul(unity_CameraToWorld, float4(viewPos,1)).xyz;
		float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);

		float3 albedo = tex2D(_CameraGBufferTexture0, uv).rgb;
		float3 specularTint = tex2D(_CameraGBufferTexture1, uv).rgb;
		float3 smoothness = tex2D(_CameraGBufferTexture1, uv).a;

		//float3 normal = tex2D(_CameraGBufferTexture2, uv).rgb * 2 - 1;

		float3 gBuffer2 = tex2D(_CameraGBufferTexture2, uv).xyz;

		//unpack z normal
		float2 fenc = gBuffer2.xy * 4 - 2;
		float f = dot(fenc, fenc);
		float g = sqrt(1 - f/4);
		float3 normal;
		normal.xy = fenc * g;
		normal.z = 1 - f/2;
		//normal.z = clamp(abs(normal.z), 0.0, 1.0) * sign(normal.z);

		//unpack sss colour
		float _PackedNormalZ = gBuffer2.z;
		float _DiffuseSSS10 = _PackedNormalZ * 10;
		float _DiffuseSSS100 = _PackedNormalZ * 100;
		float _GreenSSS = floor(_DiffuseSSS10) * 0.1;
		float _RedSSS = frac(_DiffuseSSS10) - frac(_DiffuseSSS100) * 0.1;
		float _BlueSSS = frac(_DiffuseSSS100);

		float3 _DiffuseSSS = float3(_RedSSS, _GreenSSS, _BlueSSS);

		float oneMinusReflectivity = 1 - SpecularStrength(specularTint);
		float thickness = 1.0 - tex2D(_CameraGBufferTexture0, uv).a;

		UnityLight light = CreateLight(uv, worldPos, viewPos.z);
		UnityIndirect indirectLight;
		indirectLight.diffuse = 0;
		indirectLight.specular = 0;

		float4 color = ALO_BRDF_PBS(
			albedo, specularTint, oneMinusReflectivity, smoothness,
			normal, viewDir, light, indirectLight, thickness, _DiffuseSSS
		);

		#if !defined(UNITY_HDR_ON)
			color = exp2(-color);
		#endif

		return color;
	}

	#endif