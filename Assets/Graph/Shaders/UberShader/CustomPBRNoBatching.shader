// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Alo/PBR/CustomPBR(Hubert) - No Batching" {
//no LLPV support
	Properties {
		_Color ("Tint", Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Albedo", 2D) = "white"{}

		[NoScaleOffset] _NormalMap ("Normals", 2D) = "bump"{}
		_BumpScale ("Bump Scale", Float) = 1

		[NoScaleOffset] _MetallicMap ("Metallic", 2D) = "white"{}
		[Gamma]_Metallic ("Metallic", Range(0,1)) = 0
		_Smoothness ("Smoothness", Range(0,1)) = 0.5


		_DetailTex ("Detail Albedo", 2D) = "gray"{}
		[NoScaleOffset] _DetailNormalMap ("Detail Normals", 2D) = "bump"{}
		_DetailBumpScale ("Detail Bump Scale", Float) = 1

		_ShadowTransition ("Transition", Range(0,1)) = 0.22 //Custom celshaded
		_ShadowStrength ("Strength", Range(0,1)) = 0.4 //Custom celshaded

		//bonjour alois
		[NoScaleOffset] _EmissionMap ("Emission", 2D) = "black"{}
		_Emission ("Emission", Color) = (0, 0, 0)

		[NoScaleOffset] _OcclusionMap ("Occlusion", 2D) = "white"{}
		_OcclusionStrength ("Occlusion Strength", Range(0,1)) = 1

		[NoScaleOffset] _DetailMask ("Detail Mask", 2D) = "white"{}

		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

		_ThicknessMap ("Thickness", 2D) = "black" {}
		_DistortionSSS ("Distortion", Range(0,1)) = 1
		_ScaleSSS ("Scale", Range(0,10)) = 1
		_PowerSSS ("Power", Range(0,10)) = 1
		_AttenuationSSS ("Attenuation", Range(0,1)) = 1
		_AmbientSSS ("Ambient", Range(0,1)) = 1
		_DiffuseSSS ("Diffuse", Color) = (1,1,1)

		[HideInspector] _SrcBlend ("_SrcBlend", Float) = 1
		[HideInspector] _DstBlend ("_DstBlend", Float) = 0
		[HideInspector] _ZWrite ("_ZWrite", Float) = 1
		[HideInspector] _Cull ("_Cull", Float) = 2

		_NormalDistFull ("Normal Distance Full", Float) = 1.2
		_NormalDistCulled ("Normal Distance Culled", Float) = 1.4

		_DitherDistMin ("Dither Distance Min", Float) = 1.5
		_DitherDistMax ("Dither Distance Max", Float) = 5

		_DitherObstrMin ("Dither Obstruction Min", Float) = 0.3
		_DitherObstrMax ("Dither Obstruction Max", Float) = 1
		_DistFromCam ("Distance From Camera", Float) = 0

		_RefractionAmount ("Refraction Amount", Range(-0.1,0.1)) = 0

		//_PlayerPos ("Player World Position", Vector) = (1,1,1,0)
		_MaxBendAngle ("Maximum Bending Angle", Float) = 40
		_BendingDistMin ("Full Bending Distance", Float) = 0.3
		_BendingDistMax ("No Bending Distance", Float) = 0.8
		_VertMaskMultiplier ("Vertex Mask Multiplier", Float) = 1
		_VertMaskFlat ("Vertex Mask Vertical Offset", Float) = 0.2

		_RenderQueue ("Render Queue", int) = 2000

		_WallTintPow ("Power", Float) = 1
		_WallTintCol ("Tint", Color) = (1,1,1,1)
		_GroundTintPow ("Power", Float) = 1
		_GroundTintCol ("Tint", Color) = (1,1,1,1)


		_RimPow ("Rim Power", Float) = 1
		_RimScale ("Rim Scale", Float) = 1
		_RimColor ("Rim Colour", Color) = (1,1,1,1)
	}

	CGINCLUDE


	//#define BINORMAL_PER_FRAGMENT
	#define FOG_DISTANCE

	ENDCG

	SubShader {

		Tags {
			"RenderType" = "Opaque"
			"DisableBatching" = "True"
		}

		GrabPass{
			Tags{ "LightMode" = "Always"}
			"_BackgroundTex"
		}

		Pass {
			Tags {
				"LightMode" = "ForwardBase"

			}
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			Cull [_Cull]

			CGPROGRAM

			#pragma target 3.0

			#pragma shader_feature _METALLIC_MAP
			#pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _EMISSION_MAP
			#pragma shader_feature _OCCLUSION_MAP
			#pragma shader_feature _DETAIL_MASK
			#pragma shader_feature _DETAIL_ALBEDO_MAP
			#pragma shader_feature _DETAIL_NORMAL_MAP
			#pragma shader_feature _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT

			#pragma shader_feature _ _CELSHADED
			#pragma shader_feature _ _SSS
			#pragma shader_feature _ _LOCAL_NORMAL_DEBUG
			#pragma shader_feature _ NORMAL_DISTANCE_FADE
			#pragma shader_feature _ _DISTANCE_DITHER
			#pragma shader_feature _CULL_BACK _CULL_FRONT _CULL_OFF
			#pragma shader_feature _ _REFRACTION
			#pragma shader_feature _VERTEX_MASK_CUSTOM _VERTEX_MASK_COLOUR
			#pragma shader_feature _VERTEX_OFFSET_XZ _VERTEX_OFFSET_YZ _VERTEX_OFFSET_XY
			#pragma shader_feature _ _PLAYER_SHADER
			#pragma shader_feature _ _SSS_DIFFUSE_MAP
			#pragma shader_feature _ALBEDO_VERTEX_MASK
			#pragma shader_feature _WALL_TINT
			#pragma shader_feature _GROUND_TINT
			#pragma shader_feature _RIMLIT

			#pragma shader_feature _ _VERTEX_WIND
			#pragma shader_feature _ _VERTEX_BEND

			#pragma multi_compile_fog
			#pragma multi_compile _ _DITHER_OBSTRUCTION
			#pragma multi_compile_instancing
			#pragma instancing_options lodfade
			#pragma multi_compile_fwdbase
			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#define FORWARD_BASE_PASS

			#if defined(_CELSHADED)
			float _ShadowTransition;
			float _ShadowStrength;
			#endif

			#include "LightingCustomPBR.cginc"


			ENDCG
		}

		Pass {
			Tags {
				"LightMode" = "ForwardAdd"
			}

			Blend [_SrcBlend] One
			ZWrite Off

			Cull [_Cull]

			CGPROGRAM

			#pragma target 3.0

			#pragma shader_feature _METALLIC_MAP
			#pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _DETAIL_MASK
			#pragma shader_feature _DETAIL_ALBEDO_MAP
			#pragma shader_feature _DETAIL_NORMAL_MAP
			#pragma shader_feature _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
			#pragma shader_feature _ _SSS
			#pragma shader_feature _ NORMAL_DISTANCE_FADE
			#pragma shader_feature _ _DISTANCE_DITHER
			#pragma shader_feature _CULL_BACK _CULL_FRONT _CULL_OFF
			#pragma shader_feature _ _REFRACTION
			#pragma shader_feature _VERTEX_MASK_CUSTOM _VERTEX_MASK_COLOUR
			#pragma shader_feature _VERTEX_OFFSET_XZ _VERTEX_OFFSET_YZ _VERTEX_OFFSET_XY
			#pragma shader_feature _ _PLAYER_SHADER
			#pragma shader_feature _ _SSS_DIFFUSE_MAP
			#pragma shader_feature _ALBEDO_VERTEX_MASK
			#pragma shader_feature _WALL_TINT
			#pragma shader_feature _GROUND_TINT

			#pragma shader_feature _ _VERTEX_WIND
			#pragma shader_feature _ _VERTEX_BEND

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			#pragma multi_compile _ _DITHER_OBSTRUCTION
			#pragma multi_compile _ LOD_FADE_CROSSFADE


			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "LightingCustomPBR.cginc"

			ENDCG
		}

		Pass{
			Tags{
				"LightMode" = "Deferred"
			}
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			Cull [_Cull]

			CGPROGRAM

			#pragma target 3.0
			#pragma exclude_renderers nomrt

			#pragma shader_feature _METALLIC_MAP
			#pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _EMISSION_MAP
			#pragma shader_feature _OCCLUSION_MAP
			#pragma shader_feature _DETAIL_MASK
			#pragma shader_feature _DETAIL_ALBEDO_MAP
			#pragma shader_feature _DETAIL_NORMAL_MAP
			#pragma shader_feature _ _RENDERING_CUTOUT
			#pragma shader_feature _ _SSS
			#pragma shader_feature _CULL_BACK _CULL_FRONT _CULL_OFF
			#pragma shader_feature _LOCAL_NORMAL_DEBUG
			#pragma shader_feature _ CHECKER_DEBUG
			#pragma shader_feature _ NORMAL_DISTANCE_FADE
			#pragma shader_feature _ _DISTANCE_DITHER
			#pragma shader_feature _VERTEX_MASK_CUSTOM _VERTEX_MASK_COLOUR
			#pragma shader_feature _VERTEX_OFFSET_XZ _VERTEX_OFFSET_YZ _VERTEX_OFFSET_XY
			#pragma shader_feature _ _PLAYER_SHADER
			#pragma shader_feature _ _SSS_DIFFUSE_MAP

			#pragma shader_feature _ _CELSHADED
			#pragma shader_feature _ _REFRACTION
			#pragma shader_feature _ALBEDO_VERTEX_MASK
			#pragma shader_feature _WALL_TINT
			#pragma shader_feature _GROUND_TINT
			#pragma shader_feature _RIMLIT
			#pragma shader_feature _ALBEDO_WORLDPOS

			#pragma shader_feature _ _VERTEX_WIND
			#pragma shader_feature _ _VERTEX_BEND
			#pragma shader_feature _RECALCULATE_NORMALS

			#pragma multi_compile _ _DITHER_OBSTRUCTION
			#pragma multi_compile_instancing
			#pragma instancing_options lodfade
			#pragma multi_compile_prepassfinal
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile _ _GPUI_EAST

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#define DEFERRED_PASS

			#include "LightingCustomPBR.cginc"


			ENDCG
		}


		Pass {
			Tags {
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile_shadowcaster

			#pragma vertex ShadowVertexProgram
			#pragma fragment ShadowFragmentProgram
			#pragma shader_feature _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
			#pragma shader_feature _SMOOTHNESS_ALBEDO
			#pragma shader_feature _SEMITRANSPARENT_SHADOWS
			#pragma multi_compile_instancing
			#pragma instancing_options lodfade
			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#include "ShadowsCustomPBR.cginc"

			ENDCG
		}

		Pass {
			Tags {
				"LightMode" = "Meta"
			}

			Cull Off

			CGPROGRAM

			#pragma vertex LightmappingVertexProgram
			#pragma fragment LightmappingFragmentProgram

			#pragma shader_feature _METALLIC_MAP
			#pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
			#pragma shader_feature _EMISSION_MAP
			#pragma shader_feature _DETAIL_MASK
			#pragma shader_feature _DETAIL_ALBEDO_MAP

			#pragma shader_feature _ALBEDO_VERTEX_MASK
			#pragma shader_feature _WALL_TINT
			#pragma shader_feature _GROUND_TINT

			#include"AloLightmapping.cginc"

			ENDCG

		}
	}
	CustomEditor "CustomPBR_GUI"
}
