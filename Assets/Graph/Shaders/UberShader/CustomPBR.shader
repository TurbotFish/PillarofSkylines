// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Alo/PBR/CustomPBR" {

	Properties {
		_Tint ("Tint", Color) = (1.0,1.0,1.0,1.0)
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

		[NoScaleOffset] _EmissionMap ("Emission", 2D) = "black"{}
		_Emission ("Emission", Color) = (0, 0, 0)

		_TempTex ("Shadow Texture Test", 2D) = "black"{} //Initen

		[NoScaleOffset] _OcclusionMap ("Occlusion", 2D) = "white"{}
		_OcclusionStrength ("Occlusion Strength", Range(0,1)) = 1

		[NoScaleOffset] _DetailMask ("Detail Mask", 2D) = "white"{}

		_AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5

		_ThicknessMap ("Thickness Map", 2D) = "white" {}
		_DistortionSSS ("Distortion", Range(0,1)) = 1
		_ScaleSSS ("Scale", Range(0,10)) = 1
		_PowerSSS ("Power", Range(0,10)) = 1
		_AttenuationSSS ("Attenuation", Range(0,1)) = 1
		_AmbientSSS ("Ambient", Range(0,1)) = 1
		_DiffuseSSS ("Diffuse", Color) = (1,1,1)

		[HideInspector] _SrcBlend ("_SrcBlend", Float) = 1
		[HideInspector] _DstBlend ("_DstBlend", Float) = 0
		[HideInspector] _ZWrite ("_ZWrite", Float) = 1
	}

	CGINCLUDE

	#define BINORMAL_PER_FRAGMENT
	#define FOG_DISTANCE

	ENDCG

	SubShader {


		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

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

			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile _ VERTEXLIGHT_ON
			#pragma multi_compile_fog

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#define FORWARD_BASE_PASS
			//#define INITEN

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

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog

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

			#pragma shader_feature _ _CELSHADED

			#pragma multi_compile _ UNITY_HDR_ON

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

			#include "ShadowsCustomPBR.cginc"

			ENDCG
		}
	}
	CustomEditor "CustomPBR_GUI"
}
