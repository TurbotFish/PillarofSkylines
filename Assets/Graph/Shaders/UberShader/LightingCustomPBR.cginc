// Upgrade NOTE: upgraded instancing buffer 'InstanceProperties' to new syntax.


	#if !defined (LIGHTING_CUSTOM_PBR_INCLUDED)

	#define LIGHTING_CUSTOM_PBR_INCLUDED

	//float4 _Color;//changed for instancing
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

	float _Cutoff;

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

	#if defined(_WALL_TINT)
		float _WallTintPow;
		float4 _WallTintCol;
	#endif

	#if defined(_GROUND_TINT)
		float _GroundTintPow;
		float4 _GroundTintCol;
	#endif

	#if defined(_RIMLIT)
		float _RimScale;
		float _RimPow;
		float4 _RimColor;
	#endif 


	//GPUI colour variation with worldpos
	#if defined(_ALBEDO_WORLDPOS)
		sampler2D _GPUIColorMap;
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

	#if !defined(LIGHTMAP_ON) && defined(SHADOWS_SCREEN)
		#if defined (SHADOWS_SHADOWMASK) && !defined(UNITY_NO_SCREENSPACE_SHADOWS)
			#define ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS 1
		#endif
	#endif

	#if defined(LIGHTMAP_ON) && defined(SHADOWS_SCREEN)
		#if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK)
			#define SUBTRACTIVE_LIGHTING 1
		#endif
	#endif


	struct VertexData {
		UNITY_VERTEX_INPUT_INSTANCE_ID
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float2 uv : TEXCOORD0;

		#if defined(_VERTEX_MASK_COLOUR) || defined(_ALBEDO_VERTEX_MASK)
			float4 color : COLOR;
		#endif

		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
	};

	struct Interpolators {
		UNITY_VERTEX_INPUT_INSTANCE_ID
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

		UNITY_SHADOW_COORDS(5)

		#if defined(VERTEXLIGHT_ON)
			float3 vertexLightColor : TEXCOORD6;
		#endif

		#if defined(LIGHTMAP_ON) || ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS
			float2 lightmapUV : TEXCOORD6;
		#endif

		#if defined(_DISTANCE_DITHER) || defined(_DITHER_OBSTRUCTION)
			float4 screenPos : TEXCOORD7;
		#endif

		#if defined(_REFRACTION)
			float4 grabPos : TEXCOORD8;
			float4 refraction : TEXCOORD10;
		#endif

		#if defined(_ALBEDO_VERTEX_MASK)
			float4 color : COLOR;
		#endif

		#if defined(DYNAMICLIGHTMAP_ON)
			float2 dynamicLightmapUV : TEXCOORD9;
		#endif
	};

	UNITY_INSTANCING_BUFFER_START(InstanceProperties)
		UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
#define _Color_arr InstanceProperties
	UNITY_INSTANCING_BUFFER_END(InstanceProperties)

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
		half mask = 1;
		#if defined(_CELSHADED)
			mask = 0;
		#endif
		return mask;
	}

	float3 GetAlbedo(Interpolators i){
		float3 albedoTex = tex2D(_MainTex, i.uv.xy).rgb;
		float3 albedo = albedoTex * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color).rgb;
		#if defined(_ALBEDO_VERTEX_MASK)
			albedo = lerp(albedoTex, albedo, i.color.g);
		#endif

		#if defined(_DETAIL_ALBEDO_MAP)
			float3 details = tex2D(_DetailTex, i.uv.zw) * unity_ColorSpaceDouble;
			albedo = lerp(albedo, albedo * details, GetDetailMask(i));
		#endif


		#if defined(_WALL_TINT)
			float _coeff = pow(abs(i.normal.z), _WallTintPow);
			albedo = lerp(albedo, albedo * _WallTintCol, _coeff);
		#endif

		#if defined(_GROUND_TINT)
			float _groundCoeff = pow(saturate(i.normal.y), _GroundTintPow);
			albedo = lerp(albedo, albedo * _GroundTintCol, _groundCoeff);
		#endif

		#if defined(_ALBEDO_WORLDPOS)
			float2 surfaceUV = float2(((i.worldPos.z + 250)/500), (i.worldPos.y + 250)/500);
				

//			#if defined(_GPUI_EAST)
//				surfaceUV.x = 1 - surfaceUV.x;
//			#endif
			albedo = tex2D(_GPUIColorMap, surfaceUV).rgb;
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
			float4 sssMapSample = tex2D(_ThicknessMap, i.uv);
			float thickness = 1.0 - sssMapSample.a;

			#if defined(_SSS_DIFFUSE_MAP)
				_DiffuseSSS = sssMapSample.rgb;
			#endif

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
		float alpha = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color).a;
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
		UNITY_INITIALIZE_OUTPUT(Interpolators, i);
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v,i);

		#if defined(_LOCAL_NORMAL_DEBUG)
			i.normal = v.normal;
		#else
			i.normal = UnityObjectToWorldNormal(v.normal);
		#endif

		#if defined(_VERTEX_BEND) || defined(_VERTEX_WIND)

			#if defined(_VERTEX_MASK_COLOUR)
				float rotationMask = v.color.r;
			#elif defined(_VERTEX_MASK_CUSTOM)
				float rotationMask = saturate(v.vertex.y * _VertMaskMultiplier + _VertMaskFlat);
			#endif
			float4 pivotWS =  mul(unity_ObjectToWorld,float4(0,0,0,1));


			#if defined(_VERTEX_BEND)

				float _instanceID = 0;
				#if defined (UNITY_INSTANCING_ENABLED)
				_instanceID = unity_InstanceID;
				#endif


				_PlayerPos = float3(_PlayerPos.x, _PlayerPos.y - fmod(_instanceID * 0.1, 1.5), _PlayerPos.z);
				//object space playerpos
				float3 _OS_PlayerPos = mul(unity_WorldToObject, float4(_PlayerPos, 1));

				float2 player2Vert2D = normalize(- _OS_PlayerPos.xz);


				float3 player2Vert3D = pivotWS.xyz - _PlayerPos.xyz;
				float sqrDist = dot(player2Vert3D, player2Vert3D); 
				float bendPercent = 1 - saturate((sqrDist - _BendingDistMin)/(_BendingDistMax - _BendingDistMin));
				float bendAmount = bendPercent * _MaxBendAngle * rotationMask;


				float3 _bendRotation = float3(player2Vert2D.y,0, -player2Vert2D.x);


				_bendRotation *=  bendAmount;
				v.vertex.xyz = ApplyWind(v.vertex.xyz, _bendRotation);


				#if defined(_RECALCULATE_NORMALS)
				i.normal = (ApplyWind(i.normal.xyz, _bendRotation));
				i.normal.y = abs(i.normal.y);
				//v.normal *= sign(v.normal);
				i.normal.z = abs(i.normal.z);
				#endif

			#endif

//			#if defined(_VERTEX_WIND)
//				float3 windDir = float3(0.0,0.0,1.0);//global in the future
//
//				float windSpeed = 3;
//				float _offset = (pivotWS.x * (2.9) * -sign(windDir.x) + pivotWS.z * (2.9) * -sign(windDir.z) + pivotWS.y * 0.8) * 0.5;
//
//				//windDir = mul(unity_WorldToObject, windDir);
//				windDir = normalize(windDir);
//
//				float windIntensity = tex2Dlod(_WindTex, float4(_Time.x,_Time.x, 0,0)).r;
//				windIntensity = saturate(windIntensity + 0.0);//not necessary with a good wind map
//
//				float angle = windIntensity * (sin(_Time.y * windSpeed + _offset) * 0.65+0.35) * (_MaxBendAngle) * rotationMask;
//
//
//
//				float3 _windRotation = float3( windDir.z, 0, -windDir.x) * angle;
//				
//
//				v.vertex.xyz = ApplyWind(v.vertex.xyz, _windRotation);
//				i.normal = ApplyWind(i.normal.xyz, _windRotation);
//			#endif

			#if defined(_VERTEX_WIND)
				float3 _windDir = normalize(float3(0,1,1));
				float3 _OSWindDir = mul(unity_WorldToObject,_windDir);
				float _windSpeed = 0.004;

				float2 surfaceUV = float2(((pivotWS.z + 250)/500) + _windDir.z * _Time.y * _windSpeed, (pivotWS.y + 250)/500) + _windDir.y * _Time.y * _windSpeed;
				//float2 surfaceUV = float2(((pivotWS.z + 250)/500), (pivotWS.y + 250)/500);
				float _windIntensity = tex2Dlod(_WindTex, float4(surfaceUV, 0,0)).r;

				float3 _windRotation = float3(_OSWindDir.z, 0, -_OSWindDir.x);
				_windRotation *= _MaxBendAngle * rotationMask * _windIntensity;

				v.vertex.xyz = ApplyWind(v.vertex.xyz, _windRotation);

				#if defined(_RECALCULATE_NORMALS)
				i.normal = (ApplyWind(i.normal.xyz, _windRotation));
				i.normal.y = abs(i.normal.y);
				i.normal.z = abs(i.normal.z);
				#endif

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

		#if defined(LIGHTMAP_ON) || ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS
			i.lightmapUV = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
		#endif

		#if defined(DYNAMICLIGHTMAP_ON)
			i.dynamicLightmapUV = v.uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
		#endif



		#if defined(_ALBEDO_VERTEX_MASK)
			i.color = v.color;
		#endif


		#if defined(BINORMAL_PER_FRAGMENT)
			i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
		#else
			i.tangent = UnityObjectToWorldDir(v.tangent.xyz);
			i.binormal = CreateBinormal(i.normal, i.tangent, v.tangent.w);
		#endif

		UNITY_TRANSFER_SHADOW(i, v.uv1);

		ComputeVertexLightColor(i);
		return i;
	}


	float FadeShadows(Interpolators i, float attenuation){
	#if HANDLE_SHADOWS_BLENDING_IN_GI || ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS

		#if ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS
			attenuation = SHADOW_ATTENUATION(i);
		#endif

		float viewZ = dot(_WorldSpaceCameraPos - i.worldPos, UNITY_MATRIX_V[2].xyz);
		float shadowFadeDistance = UnityComputeShadowFadeDistance(i.worldPos, viewZ);
		float shadowFade = UnityComputeShadowFade(shadowFadeDistance);

		float bakedAttenuation = UnitySampleBakedOcclusion(i.lightmapUV, i.worldPos);

		attenuation = UnityMixRealtimeAndBakedShadows(attenuation, bakedAttenuation, shadowFade);
	#endif
		return attenuation;
	}

	void ApplySubtractiveLighting ( Interpolators i, inout UnityIndirect indirectLight){
	#if SUBTRACTIVE_LIGHTING
		UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
		attenuation = FadeShadows(i, attenuation);

		float ndotl = saturate(dot(i.normal, _WorldSpaceLightPos0.xyz));
		float3 shadowedLightEstimate = ndotl * (1 - attenuation) * _LightColor0.rgb;
		float3 subtractedLight = indirectLight.diffuse - shadowedLightEstimate;
		subtractedLight = max(subtractedLight, unity_ShadowColor.rgb);
		subtractedLight = lerp(subtractedLight, inditectLight.diffuse, _LightShadowData.x);
		indirectLight.diffuse = min(subtractedLight, indirectLight.diffuse);
	#endif

	}

	UnityLight CreateLight(Interpolators i){
		UnityLight light;

		#if defined(DEFERRED_PASS) || SUBTRACTIVE_LIGHTING
			light.dir = float3(0,1,0);
			light.color = 0;
		#else

			#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
				light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
			#else
				light.dir = _WorldSpaceLightPos0.xyz;
			#endif


			UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
			attenuation = FadeShadows(i, attenuation);

			light.color = _LightColor0.rgb * attenuation;

			//tweak the colour of received shadows here
			//light.color = lerp(float3(1,0,0),_LightColor0.rgb,  0);

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
			#if defined(LIGHTMAP_ON)
				indirectLight.diffuse = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV));

				#if defined(DIRLIGHTMAP_COMBINED)
					float4 lightmapDirection = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, i.lightmapUV);
					indirectLight.diffuse = DecodeDirectionalLightmap(indirectLight.diffuse, lightmapDirection, i.normal);
				#endif

				ApplySubtractiveLighting(i, indirectLight);
			#endif


			#if defined(DYNAMICLIGHTMAP_ON)
				float3 dynamicLightDiffuse = DecodeRealtimeLightmap(UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, i.dynamicLightmapUV));

				#if defined(DIRLIGHTMAP_COMBINED)
					float4 dynamicLightmapDirection = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, i.dynamicLightmapUV);
					indirectLight.diffuse += DecodeDirectionalLightmap(dynamicLightDiffuse, dynamicLightmapDirection, i.normal);
				
				#else
					indirectLight.diffuse += dynamicLightDiffuse;
				#endif
			#endif


			#if !defined(LIGHTMAP_ON) && !defined(DYNAMICLIGHTMAP_ON)
			#if UNITY_LIGHT_PROBE_PROXY_VOLUME
				if (unity_ProbeVolumeParams.x == 1) {
					indirectLight.diffuse = SHEvalLinearL0L1_SampleProbeVolume(
						float4(i.normal, 1), i.worldPos
					);
					indirectLight.diffuse = max(0, indirectLight.diffuse);
					#if defined(UNITY_COLORSPACE_GAMMA)
			            indirectLight.diffuse =
			            	LinearToGammaSpace(indirectLight.diffuse);
			        #endif
				}
				else {
					indirectLight.diffuse +=
						max(0, ShadeSH9(float4(i.normal, 1)));
				}
			#else
				indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));
			#endif
		#endif

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

			#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
				float4 gBuffer4 : SV_Target4;
			#endif


		#else
			float4 color : SV_Target;
		#endif
	};

	FragmentOutput MyFragmentProgram(Interpolators i) {
		UNITY_SETUP_INSTANCE_ID(i);
		float alpha = GetAlpha(i);
		#if defined(_RENDERING_CUTOUT)
			clip(alpha - _Cutoff);
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


			#if defined(_RIMLIT)
				float fresnel = 1 - saturate(dot(viewDir, i.normal));
				fresnel = pow(fresnel, _RimPow) * _RimScale;
				albedo = lerp(albedo, _RimColor, fresnel);
			#endif

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
			//output.gBuffer0.a = thickness;
			output.gBuffer1.rgb = specularTint;
			output.gBuffer1.a = GetSmoothness(i);
			output.gBuffer2.rgba = float4(i.normal.xyz * 0.5 + 0.5, GetCelShadingMask());
			output.gBuffer3 = color;

			#if defined (SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
				float2 shadowUV = 0;
				#if defined(LIGHTMAP_ON)
					shadowUV = i.lightmapUV;
				#endif

				output.gBuffer4 = UnityGetRawBakedOcclusions(shadowUV, i.worldPos.xyz);
			#endif





		#else
			#if !defined(_LOCAL_NORMAL_DEBUG)
				output.color = ApplyFog(color, i);
			#endif
		#endif

		return output;
	}

	#endif