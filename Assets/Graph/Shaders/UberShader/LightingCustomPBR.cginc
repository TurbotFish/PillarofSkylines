
	#if !defined (LIGHTING_CUSTOM_PBR_INCLUDED)

	#define LIGHTING_CUSTOM_PBR_INCLUDED

	float4 _Color;
	sampler2D _MainTex, _DetailTex, _DetailMask;
	float4 _MainTex_ST, _DetailTex_ST;

	sampler2D _MetallicMap;
	float _Smoothness;
	float _Metallic;

	sampler2D _NormalMap, _DetailNormalMap;
	float _BumpScale, _DetailBumpScale;

	sampler2D _EmissionMap;
	float3 _Emission;

	sampler2D _OcclusionMap;
	float _OcclusionStrength;

	float _AlphaCutoff;

	#if defined(_SSS)
		float _DistortionSSS;
		float _ScaleSSS;
		float _PowerSSS;
		sampler2D _ThicknessMap;
		float _AttenuationSSS;
		float _AmbientSSS;
		float3 _DiffuseSSS;
	#endif

	#if defined(NORMAL_DISTANCE_FADE)
		float _NormalDistFull;
		float _NormalDistCulled;
	#endif

	#if defined(_DISTANCE_DITHER)
		float _DitherDistMin;
		float _DitherDistMax;
	#endif

	#if defined(_DITHER_OBSTRUCTION)
		float _DitherObstrMin;
		float _DitherObstrMax;
		float _DistFromCam;
	#endif

	#if defined(_REFRACTION)
		sampler2D _BackgroundTex;
		float _RefractionAmount;
	#endif

	float3 _PlayerPos;

	#if defined(_VERTEX_BEND) || defined(_VERTEX_WIND)
		float _MaxBendAngle;
		float _WindIntensity;
		sampler2D _WindTex;
		float _VertMaskMultiplier;
		float _VertMaskFlat;
	#endif

	#if defined(_VERTEX_BEND)
		float _BendingDistMin;
		float _BendingDistMax;
	#endif

	#include "AloPBSLighting.cginc"
	#include "AutoLight.cginc"
	#include "WindSystem.cginc"

	#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
		#if !defined(FOG_DISTANCE)
			#define FOG_DEPTH 1
		#endif
		#define FOG_ON 1
	#endif


	struct VertexData {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float2 uv : TEXCOORD0;

		#if defined(_VERTEX_MASK_COLOUR)
			float4 vertColour : COLOR;
		#endif
	};

	struct Interpolators {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		float3 normal : TEXCOORD1;

		#if defined(BINORMAL_PER_FRAGMENT)
			float4 tangent : TEXCOORD2;
		#else
			float3 tangent : TEXCOORD2;
			float3 binormal : TEXCOORD3;
		#endif

		#if FOG_DEPTH
			float4 worldPos : TEXCOORD4;
		#else
			float3 worldPos : TEXCOORD4;
		#endif

		SHADOW_COORDS(5)

		#if defined(VERTEXLIGHT_ON)
			float3 vertexLightColor : TEXCOORD6;
		#endif

		#if defined(_DISTANCE_DITHER) || defined(_DITHER_OBSTRUCTION)
			float4 screenPos : TEXCOORD7;
		#endif

		#if defined(_REFRACTION)
			float4 grabPos : TEXCOORD8;
			float4 refraction : TEXCOORD9;
		#endif
	};

	void ComputeVertexLightColor(inout Interpolators i){
		#if defined(VERTEXLIGHT_ON)
			i.vertexLightColor = Shade4PointLights(
				unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				unity_LightColor[0].rgb, unity_LightColor[1].rgb,
				unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				unity_4LightAtten0, i.worldPos.xyz, i.normal
			);
		#endif
	}

	float3 CreateBinormal(float3 normal, float3 tangent, float binormalSign){
		return cross(normal, tangent.xyz) * (binormalSign * unity_WorldTransformParams.w);
	}

	float GetDetailMask(Interpolators i){
		#if defined(_DETAIL_MASK)
			return tex2D(_DetailMask, i.uv.xy).a;
		#else
			return 1;
		#endif
	}

	half GetCelShadingMask(){
		half mask = 0;
		#if defined(_CELSHADED)
			mask = 1;
		#endif
		return mask;
	}

	float3 GetAlbedo(Interpolators i){
		float3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;
		#if defined(_DETAIL_ALBEDO_MAP)
			float3 details = tex2D(_DetailTex, i.uv.zw) * unity_ColorSpaceDouble;
			albedo = lerp(albedo, albedo * details, GetDetailMask(i));
		#endif
		return albedo; 
	}

	//checker test
	float3 GetAlbedoDebug(Interpolators i){
		float xValue = floor(i.worldPos.x * 0.5) - floor(floor(i.worldPos.x * 0.5) * 0.5) * 2.0;
		float yValue = floor(i.worldPos.y * 0.5) - floor(floor(i.worldPos.y * 0.5) * 0.5) * 2.0;
		float zValue = floor(i.worldPos.z * 0.5) - floor(floor(i.worldPos.z * 0.5) * 0.5) * 2.0;
		return clamp(0.45, 0.6,abs(yValue - abs(xValue - zValue)));
	}

	float4 GetLocalNormalDebug(Interpolators i){
		return float4(i.normal.xyz, 1);
	}

	float GetThickness(Interpolators i){
		#if defined(_SSS) && !defined(_LOCAL_NORMAL_DEBUG)
			float thickness = 1.0 - tex2D(_ThicknessMap, i.uv).a;
			return thickness;
		#else
			return 1.0;
		#endif
	}

	float GetSmoothness(Interpolators i){
		float smoothness = 1;

		#if defined(_SMOOTHNESS_ALBEDO)
			smoothness = tex2D(_MainTex, i.uv.xy).a;
		#elif defined(_SMOOTHNESS_METALLIC) && defined (_METALLIC_MAP)
			smoothness = tex2D(_MetallicMap, i.uv.xy).a;
		#endif

		return smoothness * _Smoothness;
	}

	float3 GetEmission (Interpolators i){
		#if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
			#if defined(_EMISSION_MAP)
				return tex2D(_EmissionMap, i.uv.xy) * _Emission;
			#else
				return _Emission;
			#endif
		#else
			return 0;
		#endif
	}

	float GetOcclusion(Interpolators i){
		#if defined(_OCCLUSION_MAP)
			return lerp(1, tex2D(_OcclusionMap, i.uv.xy).g, _OcclusionStrength);
		#else
		 return 1;
		#endif
	}

	float GetAlpha(Interpolators i){
		float alpha = _Color.a;
		#if !defined(_SMOOTHNESS_ALBEDO)
			alpha *= tex2D(_MainTex, i.uv.xy).a;
		#endif
		return alpha;
	}

	float4 ApplyFog(float4 color, Interpolators i){
		#if FOG_ON
			float viewDistance = length(_WorldSpaceCameraPos - i.worldPos.xyz);
			#if FOG_DEPTH
				viewDistance = UNITY_Z_0_FAR_FROM_CLIPSPACE(i.worldPos.w);
			#endif

			UNITY_CALC_FOG_FACTOR_RAW(viewDistance);

			float3 fogColor = 0;
			#if defined(FORWARD_BASE_PASS)
				fogColor = unity_FogColor.rgb;
			#endif
			color.rgb = lerp(fogColor, color.rgb, saturate(unityFogFactor));
			
		#endif
		return color;
	}

	#if defined(_DISTANCE_DITHER) || defined(_DITHER_OBSTRUCTION)
		float Dither8x8Bayer( int x, int y )
		{
			const float dither[ 64 ] = {
				 1, 49, 13, 61,  4, 52, 16, 64,
				33, 17, 45, 29, 36, 20, 48, 32,
				 9, 57,  5, 53, 12, 60,  8, 56,
				41, 25, 37, 21, 44, 28, 40, 24,
				 3, 51, 15, 63,  2, 50, 14, 62,
				35, 19, 47, 31, 34, 18, 46, 30,
				11, 59,  7, 55, 10, 58,  6, 54,
				43, 27, 39, 23, 42, 26, 38, 22};
			int r = y * 8 + x;
			return (dither[r]) / 64;
		}
	#endif


	Interpolators MyVertexProgram(VertexData v) {
		Interpolators i;



		#if defined(_VERTEX_BEND) || defined(_VERTEX_WIND)

			#if defined(_VERTEX_MASK_COLOUR)
				float rotationMask = v.vertColour.r;
			#elif defined(_VERTEX_MASK_CUSTOM)
				float rotationMask = saturate(v.vertex.y * _VertMaskMultiplier + _VertMaskFlat);
			#endif
			float4 pivotWS =  mul(unity_ObjectToWorld,float4(0,0,0,1));


			#if defined(_VERTEX_BEND)
				float2 player2Vert2D = pivotWS.xz - _PlayerPos.xz;
				float3 player2Vert3D = pivotWS.xyz - _PlayerPos.xyz;
				float sqrDist = dot(player2Vert3D, player2Vert3D); 
				float bendPercent = 1 - saturate((sqrDist - _BendingDistMin)/(_BendingDistMax - _BendingDistMin));
				float bendAmount = bendPercent * _MaxBendAngle * rotationMask;
				float3 _bendRotation = normalize(float3(player2Vert2D.y,0, -player2Vert2D.x)) * bendAmount;
				v.vertex.xyz = ApplyWind(v.vertex.xyz, _bendRotation);
				v.normal = ApplyWind(v.normal.xyz, _bendRotation);

			#endif

			#if defined(_VERTEX_WIND)
				float2 windDir = normalize(float2(3,0));
				//float3 windDirTemp = mul(unity_WorldToObject, float4(normalize(float3(3,0,0)),0)).xyz;
				//float2 windDir = normalize(windDirTemp.xz);
				float windIntensity = tex2Dlod(_WindTex, float4(_Time.x, _Time.x, 0,0)).r;
				windIntensity = saturate(windIntensity + 0.2);//not necessary with a good wind map
				//float windIntensity = 1;
				float windSpeed = 2;
				float _offset = (pivotWS.x * (0.9) * -sign(windDir.x) + pivotWS.z * (0.6) * -sign(windDir.y) + pivotWS.y * 0.8) * 0.5;
				float angle = windIntensity * (sin(_Time.y * windSpeed + _offset) * 0.65+0.35) * _MaxBendAngle * rotationMask;
				//float angle = 60;
				float3 _windRotation = float3( windDir.y, 0, -windDir.x) * angle;
				v.vertex.xyz = ApplyWind(v.vertex.xyz, _windRotation);
				v.normal = ApplyWind(v.normal.xyz, _windRotation);
			#endif
		#endif

		i.pos = UnityObjectToClipPos(v.vertex);
		i.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex);

		#if defined(_DISTANCE_DITHER) || defined(_DITHER_OBSTRUCTION)
			i.screenPos = ComputeScreenPos(i.pos);
		#endif

		#if FOG_DEPTH
			i.worldPos.w = i.pos.z;
		#endif

		#if defined(_REFRACTION)
			i.grabPos = ComputeGrabScreenPos(i.pos);
			i.refraction = mul(unity_ObjectToWorld, float4(-v.normal.xy, v.normal.z, 1)) * _RefractionAmount;
		#endif


		i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
		i.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex);

		#if defined(_LOCAL_NORMAL_DEBUG)
			i.normal = v.normal;
		#else
			i.normal = UnityObjectToWorldNormal(v.normal);
		#endif


		#if defined(BINORMAL_PER_FRAGMENT)
			i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
		#else
			i.tangent = UnityObjectToWorldDir(v.tangent.xyz);
			i.binormal = CreateBinormal(i.normal, i.tangent, v.tangent.w);
		#endif

		TRANSFER_SHADOW(i);

		ComputeVertexLightColor(i);
		return i;
	}

	UnityLight CreateLight(Interpolators i){
		UnityLight light;

		#if defined(DEFERRED_PASS)
			light.dir = float3(0,1,0);
			light.color = 0;
		#else

			#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
				light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
			#else
				light.dir = _WorldSpaceLightPos0.xyz;
			#endif


			UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);


			light.color = _LightColor0.rgb * attenuation;

			//tweak the colour of received shadows here
			//light.color = lerp(float3(1,0,0),_LightColor0.rgb,  attenuation);

		#endif
		return light;
	}

	float3 BoxProjection(float3 direction, float3 position, float4 cubemapPosition, float3 boxMin, float3 boxMax){
	#if UNITY_SPECCUBE_BOX_PROJECTION
		UNITY_BRANCH
		if(cubemapPosition.w > 0){
			float3 factors = ((direction > 0 ? boxMax : boxMin) - position) / direction;
			float scalar = min(min(factors.x,factors.y),factors.z);
			direction = direction * scalar + (position - cubemapPosition);
		}
	#endif
		return direction;
	}

	UnityIndirect CreateIndirectLight(Interpolators i, float3 viewDir){
		UnityIndirect indirectLight;
		indirectLight.diffuse = 0;
		indirectLight.specular = 0;

		#if defined(VERTEXLIGHT_ON)
			indirectLight.diffuse = i.vertexLightColor;
		#endif

		#if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
			indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));
			float3 reflectionDir = reflect(-viewDir, i.normal);

			Unity_GlossyEnvironmentData envData;
			envData.roughness = 1 - GetSmoothness(i);
			envData.reflUVW = BoxProjection(
				reflectionDir, i.worldPos.xyz,
				unity_SpecCube0_ProbePosition,
				unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax
			);

			float3 probe0 = Unity_GlossyEnvironment(
				UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData
			);

			envData.reflUVW = BoxProjection(
				reflectionDir, i.worldPos.xyz,
				unity_SpecCube1_ProbePosition,
				unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax
			);

		#if UNITY_SPECCUBE_BLENDING
			float interpolator = unity_SpecCube0_BoxMin.w;
			UNITY_BRANCH
			if(interpolator < 0.99999){
				float3 probe1 = Unity_GlossyEnvironment(
					UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0), unity_SpecCube0_HDR, envData
				);

				indirectLight.specular = lerp(probe0, probe1, interpolator);
			} else {
				indirectLight.specular = probe0;
			}
		#else
			indirectLight.specular = probe0;
		#endif


		float occlusion = GetOcclusion(i);
		indirectLight.specular *= occlusion;
		indirectLight.diffuse *= occlusion;

			//cool specular colouring
			//indirectLight.specular = float3(1,0,0);

			#if defined(DEFERRED_PASS) && UNITY_ENABLE_REFLECTION_BUFFERS
				indirectLight.specular = 0;
			#endif

		#endif

		return indirectLight;
	}

	float3 GetTangentSpaceNormal(Interpolators i){
		float3 normal = float3(0,0,1);
		#if defined(_NORMAL_MAP)
			#if defined(NORMAL_DISTANCE_FADE)
				float3 viewDir = _WorldSpaceCameraPos - i.worldPos.xyz;
				float _SqrDistFromCamera = dot(viewDir, viewDir);
				float _LerpAmount = saturate((_SqrDistFromCamera - _NormalDistFull) / (_NormalDistCulled - _NormalDistFull));
				float _DistBump = lerp(_BumpScale,0, _LerpAmount);
				normal = UnpackScaleNormal(tex2D(_NormalMap, i.uv.xy), _DistBump);
			#else
				normal = UnpackScaleNormal(tex2D(_NormalMap, i.uv.xy), _BumpScale);
			#endif


		#endif
		#if defined(_DETAIL_NORMAL_MAP)
			#if defined(NORMAL_DISTANCE_FADE)
				float detailDistBump = lerp(_DetailBumpScale, 0, _LerpAmount);
				float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, i.uv.zw), detailDistBump);
			#else
				float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, i.uv.zw), _DetailBumpScale);
			#endif

			detailNormal = lerp(float3(0,0,1), detailNormal, GetDetailMask(i));
			normal = BlendNormals(normal, detailNormal);
		#endif

		return normal;
	}


	void InitializeFragmentNormal(inout Interpolators i){
		
		float3 tangentSpaceNormal = GetTangentSpaceNormal(i);

		#if defined(BINORMAL_PER_FRAGMENT)
			float3 binormal = CreateBinormal(i.normal, i.tangent.xyz, i.tangent.w);
		#else
			float3 binormal = i.binormal;
		#endif

		i.normal = normalize(
			tangentSpaceNormal.x * i.tangent +
			tangentSpaceNormal.y *  binormal +
			tangentSpaceNormal.z * i.normal
		);
	}

	float GetMetallic(Interpolators i){
		#if defined(_METALLIC_MAP)
			return tex2D(_MetallicMap, i.uv.xy).r;
		#else
			return _Metallic;
		#endif
	}


	struct FragmentOutput{
		#if defined(DEFERRED_PASS)
			float4 gBuffer0 : SV_Target0;
			float4 gBuffer1 : SV_Target1;
			float4 gBuffer2 : SV_Target2;
			float4 gBuffer3 : SV_Target3;
		#else
			float4 color : SV_Target;
		#endif
	};

	FragmentOutput MyFragmentProgram(Interpolators i) {
		float alpha = GetAlpha(i);
		#if defined(_RENDERING_CUTOUT)
			clip(alpha - _AlphaCutoff);
		#endif

		float3 viewVec = _WorldSpaceCameraPos - i.worldPos.xyz;
		float3 viewDir = normalize(viewVec);

		#if defined(_DISTANCE_DITHER) || defined(_DITHER_OBSTRUCTION)
			float2 clipScreen = (i.screenPos.xy / i.screenPos.w) * _ScreenParams.xy;
			float sqrViewDist = dot(viewVec, viewVec);
			float bayer = Dither8x8Bayer(fmod(clipScreen.x,8), fmod(clipScreen.y,8));

			#if defined(_DITHER_OBSTRUCTION)
				float obstrCamDist = saturate((_DistFromCam - _DitherObstrMin)/(_DitherObstrMax - _DitherObstrMin));
				clip(obstrCamDist - bayer);
			#endif


			#if defined(_DISTANCE_DITHER)
				float dist2Cam = 1 - saturate((sqrViewDist - _DitherDistMin) / (_DitherDistMax - _DitherDistMin));
				clip(dist2Cam - bayer);
			#endif
		#endif

		InitializeFragmentNormal(i);
		//i.normal = normalize(i.normal);


		float3 specularTint;
		float oneMinusReflectivity;

		#if defined (CHECKER_DEBUG)
			float3 albedo = DiffuseAndSpecularFromMetallic( 
				GetAlbedoDebug(i), GetMetallic(i), specularTint, oneMinusReflectivity
			);
		#else
			float3 albedo = DiffuseAndSpecularFromMetallic( 
				GetAlbedo(i), GetMetallic(i), specularTint, oneMinusReflectivity
			);
		#endif

		#if defined(_RENDERING_TRANSPARENT)
			albedo *= alpha;
			alpha = 1 - oneMinusReflectivity + alpha * oneMinusReflectivity;
		#endif

		float thickness = GetThickness(i);

		#if defined(_REFRACTION)
			#if defined(_SSS)
					float4 refracColour = tex2Dproj(_BackgroundTex, UNITY_PROJ_COORD(i.grabPos + i.refraction * thickness));

				#else
					float4 refracColour = tex2Dproj(_BackgroundTex, UNITY_PROJ_COORD(i.grabPos + i.refraction));
				#endif
			albedo *= refracColour;
		#endif


		#if defined(_LOCAL_NORMAL_DEBUG)
			float4 color = GetLocalNormalDebug(i);
		#else
			float4 color = ALO_BRDF_PBS(
				albedo, specularTint,
				oneMinusReflectivity, GetSmoothness(i),
				i.normal, viewDir,
				CreateLight(i), CreateIndirectLight(i, viewDir),
				i.uv
			);
			color.rgb += GetEmission(i);

			#if defined(_RENDERING_FADE) || defined(_RENDERING_TRANSPARENT)
				color.a = alpha;
			#endif
		#endif


		FragmentOutput output;
		#if defined(DEFERRED_PASS)
			#if !defined(UNITY_HDR_ON)
				color.rgb = exp2(-color.rgb);
			#endif
			output.gBuffer0.rgb = albedo;
			output.gBuffer0.a = thickness;
			output.gBuffer1.rgb = specularTint;
			output.gBuffer1.a = GetSmoothness(i);
			output.gBuffer2.rgba = float4(i.normal.xyz * 0.5 + 0.5, GetCelShadingMask());
			output.gBuffer3 = color;

		#else
			#if !defined(_LOCAL_NORMAL_DEBUG)
				output.color = ApplyFog(color, i);
			#endif
		#endif

		return output;
	}

	#endif