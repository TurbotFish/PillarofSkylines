// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom_shaders/POM_adaptive"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Tiling("Tiling", Range( 0 , 10)) = 0
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 0.5
		_Metallic("Metallic", 2D) = "white" {}
		_Roughness("Roughness", 2D) = "white" {}
		_RoughScale("Rough Scale", Float) = 0.5
		_Occlusion("Occlusion", 2D) = "white" {}
		_HeightMap("HeightMap", 2D) = "white" {}
		_Scale("Scale", Range( 0 , 1)) = 0.4247461
		_CurvatureU("Curvature U", Range( 0 , 100)) = 0
		_CurvatureV("Curvature V", Range( 0 , 30)) = 0
		_Start_transition("Start_transition", Float) = 0
		_End_transition("End_transition", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[Header(Parallax Occlusion Mapping)]
		_CurvFix("Curvature Bias", Range( 0 , 1)) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float eyeDepth;
			float2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float _NormalScale;
		uniform sampler2D _Normal;
		uniform float _Start_transition;
		uniform float _End_transition;
		uniform float4 _Albedo_ST;
		uniform float _Tiling;
		uniform sampler2D _HeightMap;
		uniform float _Scale;
		uniform float _CurvFix;
		uniform float _CurvatureU;
		uniform float _CurvatureV;
		uniform float4 _HeightMap_ST;
		uniform sampler2D _Albedo;
		uniform sampler2D _Metallic;
		uniform float _RoughScale;
		uniform sampler2D _Roughness;
		uniform sampler2D _Occlusion;


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv )
		{
			float3 result = 0;
			int stepIndex = 0;
			int numSteps = ( int )lerp( maxSamples, minSamples, dot( normalWorld, viewWorld ) );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
				result.z = dot( curv, currTexOffset * currTexOffset );
				currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r * ( 1 - result.z );
				if ( currHeight > currRayZ )
				{
					stepIndex = numSteps + 1;
				}
				else
				{
					stepIndex++;
					prevTexOffset = currTexOffset;
					prevRayZ = currRayZ;
					prevHeight = currHeight;
					currTexOffset += deltaTex;
					currRayZ -= layerHeight * ( 1 - result.z ) * (1+_CurvFix);
				}
			}
			int sectionSteps = 10;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
				intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
				finalTexOffset = prevTexOffset + intersection * deltaTex;
				newZ = prevRayZ - intersection * layerHeight;
				newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
				if ( newHeight > newZ )
				{
					currTexOffset = finalTexOffset;
					currHeight = newHeight;
					currRayZ = newZ;
					deltaTex = intersection * deltaTex;
					layerHeight = intersection * layerHeight;
				}
				else
				{
					prevTexOffset = finalTexOffset;
					prevHeight = newHeight;
					prevRayZ = newZ;
					deltaTex = ( 1 - intersection ) * deltaTex;
					layerHeight = ( 1 - intersection ) * layerHeight;
				}
				sectionIndex++;
			}
			#ifdef UNITY_PASS_SHADOWCASTER
			if ( unity_LightShadowBias.z == 0.0 )
			{
			#endif
				if ( result.z > 1 )
					clip( -1 );
			#ifdef UNITY_PASS_SHADOWCASTER
			}
			#endif
			return uvs + finalTexOffset;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_53_0 = ( _Start_transition + _ProjectionParams.y );
			float temp_output_57_0 = ( ( i.eyeDepth + -temp_output_53_0 ) / ( _End_transition - temp_output_53_0 ) );
			float Scale = temp_output_57_0;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float2 temp_output_47_0 = ( uv_Albedo * _Tiling );
			float Scale_transitioned = clamp( lerp( _Scale , 0.0 , temp_output_57_0 ) , 0.0 , 1.0 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float2 appendResult46 = float2( _CurvatureU , _CurvatureV );
			float2 OffsetPOM8 = POM( _HeightMap, temp_output_47_0, ddx(temp_output_47_0), ddx(temp_output_47_0), ase_worldNormal, worldViewDir, i.viewDir, 1, 50, Scale_transitioned, 0, _HeightMap_ST.xy, appendResult46 );
			float2 ifLocalVar62 = 0;
			if( Scale > 1.0 )
				ifLocalVar62 = temp_output_47_0;
			else if( Scale == 1.0 )
				ifLocalVar62 = temp_output_47_0;
			else if( Scale < 1.0 )
				ifLocalVar62 = OffsetPOM8;
			float2 customUVs = ifLocalVar62;
			float2 temp_output_40_0 = ddx( temp_output_47_0 );
			float2 temp_output_41_0 = ddy( temp_output_47_0 );
			o.Normal = UnpackScaleNormal( tex2D( _Normal,customUVs, temp_output_40_0, temp_output_41_0) ,_NormalScale );
			o.Albedo = tex2D( _Albedo,customUVs, temp_output_40_0, temp_output_41_0).xyz;
			o.Metallic = tex2D( _Metallic,customUVs, temp_output_40_0, temp_output_41_0).r;
			o.Smoothness = ( 1.0 - ( _RoughScale * tex2D( _Roughness,customUVs, temp_output_40_0, temp_output_41_0).r ) );
			o.Occlusion = tex2D( _Occlusion,customUVs, temp_output_40_0, temp_output_41_0).r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=7003
299;121;1462;715;2648.446;-1133.665;1.929953;True;True
Node;AmplifyShaderEditor.CommentaryNode;64;-2363.007,1495.669;Float;False;1639.68;811.3904;Transition;15;13;60;70;61;59;72;57;55;54;58;52;56;53;50;51;
Node;AmplifyShaderEditor.RangedFloatNode;51;-2295.657,1940.591;Float;False;Property;_Start_transition;Start_transition;12;0;0;0;0;FLOAT
Node;AmplifyShaderEditor.ProjectionParams;50;-2294.357,2037.891;Float;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-2085.158,1978.892;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.NegateNode;56;-1938.758,1834.091;Float;False;0;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;52;-1946.358,1919.292;Float;False;Property;_End_transition;End_transition;13;0;0;0;0;FLOAT
Node;AmplifyShaderEditor.SurfaceDepthNode;58;-2265.157,1797.892;Float;False;0;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-1777.358,1801.691;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;55;-1751.658,1971.191;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;-1585.557,1871.191;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-1734.666,1673.407;Float;False;Property;_Scale;Scale;9;0;0.4247461;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;60;-1600.163,1760.49;Float;False;Constant;_Float0;Float 0;14;0;0;0;0;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;49;-2403.093,-111.8605;Float;False;1662.87;1128.24;POM;16;40;39;41;10;48;47;63;71;62;9;69;15;8;46;43;45;
Node;AmplifyShaderEditor.LerpOp;59;-1409.361,1756.89;Float;False;0;FLOAT;0.0;False;1;FLOAT;0,0;False;2;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-2288.164,62.90876;Float;False;0;11;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;45;-2136.162,759.3074;Float;False;Property;_CurvatureV;Curvature V;11;0;0;0;30;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;-2133.862,665.9072;Float;False;Property;_CurvatureU;Curvature U;10;0;0;0;100;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;61;-1235.264,1756.806;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;48;-2329.702,198.3191;Float;False;Property;_Tiling;Tiling;0;0;0;0;10;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;9;-1896.56,237.6091;Float;True;Property;_HeightMap;HeightMap;8;0;None;False;white;Auto;SAMPLER2D
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1848.895,126.5189;Float;False;0;FLOAT2;0.0;False;1;FLOAT;0,0;False;FLOAT2
Node;AmplifyShaderEditor.AppendNode;46;-1824.361,694.1068;Float;False;FLOAT2;0;0;0;0;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-1039.208,1760.181;Float;False;Scale_transitioned;-1;True;0;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;69;-1934.306,447.1813;Float;False;70;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;15;-1833.501,535.3672;Float;False;Tangent;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;-1407.705,1896.88;Float;False;Scale;-1;True;0;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;63;-1453.359,222.2903;Float;False;Constant;_Float1;Float 1;14;0;1;0;0;FLOAT
Node;AmplifyShaderEditor.ParallaxOcclusionMappingNode;8;-1521.361,456.8094;Float;False;0;1;50;10;0.02;0;False;1,1;True;10,0;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT2;0,0;False;FLOAT2
Node;AmplifyShaderEditor.GetLocalVarNode;71;-1481.403,140.0804;Float;False;72;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;62;-1226.694,221.7395;Float;False;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT2;0.0;False;3;FLOAT2;0.0;False;4;FLOAT2;0.0;False;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-1031.086,123.5412;Float;False;customUVs;1;False;0;FLOAT2;0.0,0;False;FLOAT2
Node;AmplifyShaderEditor.DdxOpNode;40;-941.2483,-34.10543;Float;False;0;FLOAT2;0.0,0;False;FLOAT2
Node;AmplifyShaderEditor.DdyOpNode;41;-948.549,41.49403;Float;False;0;FLOAT2;0.0,0;False;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;25;192.4145,240.4274;Float;False;Property;_RoughScale;Rough Scale;6;0;0.5;0;0;FLOAT
Node;AmplifyShaderEditor.SamplerNode;20;48.41444,336.4276;Float;True;Property;_Roughness;Roughness;5;0;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;21;48.41444,512.4279;Float;True;Property;_Occlusion;Occlusion;7;0;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;396.7047,210.9306;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;24;-292.5285,-343.4738;Float;False;Property;_NormalScale;Normal Scale;3;0;0.5;0;0;FLOAT
Node;AmplifyShaderEditor.SamplerNode;14;58.50477,-125.0694;Float;True;Property;_Normal;Normal;2;0;None;True;0;True;bump;Auto;True;Object;-1;Derivative;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;23;60.70477,34.93072;Float;True;Property;_Metallic;Metallic;4;0;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;29;663.2496,385.8717;Float;False;0;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.SamplerNode;11;60.70477,-301.0693;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;42;532.664,38.29838;Float;False;0;FLOAT;0.0;False;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;780.7046,-269.0693;Float;False;True;4;Float;ASEMaterialInspector;0;Standard;Custom_shaders/POM_adaptive;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0.0,0,0;False;13;OBJECT;0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
WireConnection;53;0;51;0
WireConnection;53;1;50;2
WireConnection;56;0;53;0
WireConnection;54;0;58;0
WireConnection;54;1;56;0
WireConnection;55;0;52;0
WireConnection;55;1;53;0
WireConnection;57;0;54;0
WireConnection;57;1;55;0
WireConnection;59;0;13;0
WireConnection;59;1;60;0
WireConnection;59;2;57;0
WireConnection;61;0;59;0
WireConnection;47;0;10;0
WireConnection;47;1;48;0
WireConnection;46;0;43;0
WireConnection;46;1;45;0
WireConnection;70;0;61;0
WireConnection;72;0;57;0
WireConnection;8;0;47;0
WireConnection;8;1;9;0
WireConnection;8;2;69;0
WireConnection;8;3;15;0
WireConnection;8;5;46;0
WireConnection;62;0;71;0
WireConnection;62;1;63;0
WireConnection;62;2;47;0
WireConnection;62;3;47;0
WireConnection;62;4;8;0
WireConnection;39;0;62;0
WireConnection;40;0;47;0
WireConnection;41;0;47;0
WireConnection;20;1;39;0
WireConnection;20;3;40;0
WireConnection;20;4;41;0
WireConnection;21;1;39;0
WireConnection;21;3;40;0
WireConnection;21;4;41;0
WireConnection;26;0;25;0
WireConnection;26;1;20;1
WireConnection;14;1;39;0
WireConnection;14;3;40;0
WireConnection;14;4;41;0
WireConnection;14;5;24;0
WireConnection;23;1;39;0
WireConnection;23;3;40;0
WireConnection;23;4;41;0
WireConnection;29;0;21;1
WireConnection;11;1;39;0
WireConnection;11;3;40;0
WireConnection;11;4;41;0
WireConnection;42;0;26;0
WireConnection;0;0;11;0
WireConnection;0;1;14;0
WireConnection;0;3;23;1
WireConnection;0;4;42;0
WireConnection;0;5;29;0
ASEEND*/
//CHKSM=20AC911AD8D013D7B36790CACAD1516E410EC8D0