// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Water2"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Water_normal("Water_normal", 2D) = "bump" {}
		_TextureSample1("Texture Sample 1", 2D) = "bump" {}
		_Vector4("Vector 4", Vector) = (0,0,0,0)
		_Normal_tiling("Normal_tiling", Vector) = (0,0,0,0)
		_Normal_intensity("Normal_intensity", Float) = 0
		_Float25("Float 25", Float) = 0
		_Smoothnest("Smoothnest", Range( 0 , 1)) = 0.5
		_Float23("Float 23", Float) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Speed("Speed", Range( 0 , 0.2)) = 0.1
		_Depth("Depth", Float) = 0
		_T_Distance("T_Distance", Float) = 0
		_T_Falloff("T_Falloff", Float) = 0
		_Show_distance("Show_distance", Range( 0 , 1)) = 0
		_Blur_intensity("Blur_intensity", Range( 0 , 0.5)) = 0
		_Ditortion_intensity("Ditortion_intensity", Range( 0 , 0.06)) = 0.292
		_C_Closeblue("C_Closeblue", Color) = (0.05936419,0.2391854,0.4485294,0)
		_C_Closestblue("C_Closestblue", Color) = (0.05936419,0.2391854,0.4485294,0)
		_C_Deepblue("C_Deepblue", Color) = (0.05936419,0.2391854,0.4485294,0)
		_Projection("Projection", 2D) = "white" {}
		_Projection_depth("Projection_depth", Float) = 2
		_Projection_tiling("Projection_tiling", Vector) = (1,1,0,0)
		_Projection_speed("Projection_speed", Range( 0 , 1)) = 0.1
		_Projection_intensity("Projection_intensity", Float) = 0
		_Emission_power("Emission_power", Range( 0 , 1)) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_vo_intensity("vo_intensity", Float) = 0.4
		_Watercolor_fog("Watercolor_fog", Color) = (0.4509804,0.4941176,0.6627451,1)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "_GrabScreen" }
		GrabPass{ "_GrabScreen" }
		GrabPass{ "_GrabScreen" }
		GrabPass{ "_GrabScreen" }
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
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
			float2 texcoord_0;
			float2 texcoord_1;
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float eyeDepth;
			float2 texcoord_2;
		};

		uniform sampler2D _Water_normal;
		uniform float2 _Normal_tiling;
		uniform float _Speed;
		uniform sampler2D _TextureSample0;
		uniform sampler2D _TextureSample1;
		uniform float2 _Vector4;
		uniform float _Normal_intensity;
		uniform float4 _C_Closestblue;
		uniform sampler2D _GrabScreen;
		uniform float _Ditortion_intensity;
		uniform float _Blur_intensity;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth;
		uniform float _T_Distance;
		uniform float _T_Falloff;
		uniform float4 _C_Deepblue;
		uniform float4 _C_Closeblue;
		uniform float4 _Watercolor_fog;
		uniform float _Float25;
		uniform float _Float23;
		uniform float _Emission_power;
		uniform sampler2D _Projection;
		uniform float _Projection_speed;
		uniform float _Projection_depth;
		uniform float2 _Projection_tiling;
		uniform float _Projection_intensity;
		uniform float _Show_distance;
		uniform float _Metallic;
		uniform float _Smoothnest;
		uniform float _vo_intensity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 0.5,0.5 ) + float2( 0,0 );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
			o.texcoord_2.xy = v.texcoord.xy * float2( 0.5,0.5 ) + float2( 0,0 );
			float2 panner343 = ( o.texcoord_2 + ( _Time.y * 0.03 ) * float2( 1,1 ));
			float4 tex2DNode338 = tex2Dlod( _TextureSample0, float4( panner343, 0.0 , 0.0 ) );
			float Perlin_mask358 = tex2DNode338.x;
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( Perlin_mask358 * ase_vertexNormal ) * _vo_intensity );
			v.normal = float3(0,1,0);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_133_0 = ( _Time.y * _Speed );
			float2 panner130 = ( i.texcoord_0 + temp_output_133_0 * float2( 1,1 ));
			float4 _Color4 = float4(0,0,1,0);
			float2 panner343 = ( i.texcoord_1 + ( _Time.y * 0.03 ) * float2( 1,1 ));
			float4 tex2DNode338 = tex2D( _TextureSample0, panner343 );
			float Perlin_mask358 = tex2DNode338.x;
			float4 lerpResult359 = lerp( float4( UnpackScaleNormal( tex2D( _Water_normal, ( _Normal_tiling * panner130 ) ) ,1.0 ) , 0.0 ) , _Color4 , Perlin_mask358);
			float2 panner326 = ( i.texcoord_0 + temp_output_133_0 * float2( -1,-1 ));
			float4 lerpResult362 = lerp( _Color4 , float4( UnpackScaleNormal( tex2D( _TextureSample1, ( _Vector4 * panner326 ) ) ,1.0 ) , 0.0 ) , Perlin_mask358);
			float3 temp_output_322_0 = BlendNormals( lerpResult359.rgb , lerpResult362.rgb );
			float4 lerpResult152 = lerp( float4( temp_output_322_0 , 0.0 ) , float4(0,0,1,0) , _Normal_intensity);
			float4 Normal_value230 = lerpResult152;
			o.Normal = Normal_value230.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 componentMask124 = ase_screenPosNorm.xy;
			float2 componentMask126 = ( temp_output_322_0 * _Ditortion_intensity ).xy;
			float2 Screen_Position129 = ( componentMask124 + componentMask126 );
			float4 screenColor80 = tex2D( _GrabScreen, Screen_Position129 );
			float3 worldSpaceViewDir102 = WorldSpaceViewDir( float4( 0,0,0,0 ) );
			float3 ase_worldPos = i.worldPos;
			float temp_output_99_0 = distance( worldSpaceViewDir102 , ase_worldPos );
			float temp_output_95_0 = ( _Blur_intensity / ( temp_output_99_0 / log( temp_output_99_0 ) ) );
			float3 worldSpaceViewDir269 = WorldSpaceViewDir( float4( 0,0,0,0 ) );
			float clampResult275 = clamp( pow( ( distance( ase_worldPos , worldSpaceViewDir269 ) / _T_Distance ) , _T_Falloff ) , 0.0 , 1.0 );
			float Distance279 = ( 1.0 - clampResult275 );
			float lerpResult276 = lerp( _Depth , 20.0 , Distance279);
			float screenDepth111 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth111 = abs( ( screenDepth111 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( lerpResult276 ) );
			float clampResult116 = clamp( pow( distanceDepth111 , 0.3 ) , 0.0 , 1.0 );
			float Depth114 = clampResult116;
			float lerpResult113 = lerp( ( temp_output_95_0 * 0.2 ) , temp_output_95_0 , Depth114);
			float Offset_value101 = lerpResult113;
			float4 screenColor83 = tex2D( _GrabScreen, ( Screen_Position129 + ( float2( 0.1,0 ) * Offset_value101 ) ) );
			float4 screenColor88 = tex2D( _GrabScreen, ( Screen_Position129 + ( float2( 0,0.1 ) * Offset_value101 ) ) );
			float4 screenColor89 = tex2D( _GrabScreen, ( Screen_Position129 + ( float2( -0.1,-0.1 ) * Offset_value101 ) ) );
			float4 SceneColor_processed154 = ( ( ( ( screenColor80 * 0.4 ) + ( screenColor83 * 0.2 ) ) + ( screenColor88 * 0.2 ) ) + ( screenColor89 * 0.2 ) );
			float4 lerpResult137 = lerp( ( _C_Closestblue * SceneColor_processed154 ) , ( _C_Deepblue * SceneColor_processed154 ) , Depth114);
			float4 clampResult148 = clamp( lerpResult137 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			fixed3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNode142 = ( 0.0 + 30.0 * pow( 1.0 - dot( ase_worldNormal, ase_worldViewDir ), 5.0 ) );
			float4 lerpResult146 = lerp( _C_Closeblue , _C_Deepblue , ( ( 1.0 - Depth114 ) * fresnelNode142 ));
			float4 clampResult147 = clamp( lerpResult146 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 temp_output_139_0 = ( clampResult148 + clampResult147 );
			float4 Albedo296 = temp_output_139_0;
			float temp_output_408_0 = ( _Float25 + _ProjectionParams.y );
			float temp_output_410_0 = saturate( ( ( i.eyeDepth + -temp_output_408_0 ) / ( _Float23 - temp_output_408_0 ) ) );
			float4 lerpResult383 = lerp( Albedo296 , _Watercolor_fog , temp_output_410_0);
			o.Albedo = lerpResult383.rgb;
			float clampResult320 = clamp( ( Normal_value230.g * 17.0 ) , 0.0 , 1.0 );
			float4 _Color3 = float4(1,1,1,0);
			float screenDepth307 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth307 = abs( ( screenDepth307 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.5 ) );
			float clampResult315 = clamp( distanceDepth307 , 0.0 , 1.0 );
			float4 lerpResult309 = lerp( ( clampResult320 * _Color3 ) , float4( 0.0,0,0,0 ) , clampResult315);
			float div321=256.0/float(150);
			float4 posterize321 = ( floor( lerpResult309 * div321 ) / div321 );
			float screenDepth333 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth333 = abs( ( screenDepth333 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 0.07 ) );
			float clampResult334 = clamp( distanceDepth333 , 0.0 , 1.0 );
			float4 lerpResult335 = lerp( _Color3 , float4( 0,0,0,0 ) , clampResult334);
			float div336=256.0/float(50);
			float4 posterize336 = ( floor( lerpResult335 * div336 ) / div336 );
			float4 Foam312 = ( ( posterize321 * float4( 0,0,0,0 ) ) + posterize336 );
			float4 appendResult373 = (float4(Normal_value230.r , Normal_value230.g , 0.0 , 0.0));
			float2 componentMask258 = ase_worldPos.yz;
			float eyeDepth173 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float temp_output_174_0 = ( eyeDepth173 - i.eyeDepth );
			float2 temp_cast_9 = (temp_output_174_0).xx;
			float2 componentMask183 = ( componentMask258 - temp_cast_9 ).xy;
			float2 lerpResult282 = lerp( _Projection_tiling , ( _Projection_tiling * 1.0 ) , Distance279);
			float2 panner235 = ( ( ( _Projection_depth + componentMask183 ) * lerpResult282 ) + ( _Time.y * _Projection_speed ) * float2( -1,-1 ));
			float4 lerpResult247 = lerp( ( tex2D( _Projection, ( appendResult373 + float4( panner235, 0.0 , 0.0 ) ).xy ) * _Projection_intensity ) , float4( 0.0,0,0,0 ) , Depth114);
			float4 Projection_color256 = lerpResult247;
			float4 lerpResult287 = lerp( float4(1,0,0,0) , float4(0,0.5862069,1,0) , Distance279);
			float4 temp_cast_14 = (0.0).xxxx;
			float4 lerpResult384 = lerp( ( Foam312 + ( ( Albedo296 * _Emission_power ) + ( Projection_color256 + ( lerpResult287 * _Show_distance ) ) ) ) , temp_cast_14 , temp_output_410_0);
			o.Emission = lerpResult384.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothnest;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float4 screenPos : TEXCOORD7;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
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
Version=13201
1927;72;1542;953;2166.029;1208.647;1.177238;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;268;-1988.402,-686.1135;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldSpaceViewDirHlpNode;269;-2010.065,-519.9998;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.DistanceOpNode;270;-1754.603,-638.4006;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;293;-1704.26,-523.1726;Float;False;Property;_T_Distance;T_Distance;11;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;294;-1429.061,-443.1718;Float;False;Property;_T_Falloff;T_Falloff;12;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;271;-1543.379,-636.1687;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;346;624.2242,1107.544;Float;False;Constant;_Float17;Float 17;25;0;0.03;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;344;609.8244,1017.943;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;171;-2105.765,-1141.345;Float;False;2096.813;992.0288;Comment;7;275;112;283;411;279;111;162;Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;273;-1377.38,-635.1687;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;339;720.5242,891.5435;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.5,0.5;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;132;-5226.223,-969.7347;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;345;800.2242,1017.944;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;134;-5275.321,-878.6334;Float;False;Property;_Speed;Speed;9;0;0.1;0;0.2;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;275;-1233.761,-628.2825;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;411;-1074.73,-554.1025;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;131;-5078.267,-1109.133;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;343;990.624,897.9432;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-5007.819,-973.7335;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;103;-4479.228,1130.369;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;326;-4787.364,-685.9573;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,-1;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;338;1280.924,1125.35;Float;True;Property;_TextureSample0;Texture Sample 0;26;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;130;-4784.499,-1104.144;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;283;-1377.361,-738.5696;Float;False;Constant;_Float14;Float 14;19;0;20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;112;-1389.872,-872.3467;Float;False;Property;_Depth;Depth;10;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;324;-4810.695,-820.3303;Float;False;Property;_Vector4;Vector 4;2;0;0,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;252;-4833.427,-1243.317;Float;False;Property;_Normal_tiling;Normal_tiling;3;0;0,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldSpaceViewDirHlpNode;102;-4497.592,1019.871;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;279;-892.3107,-546.6104;Float;False;Distance;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;325;-4566.288,-811.2302;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.BreakToComponentsNode;361;2365.923,1276.543;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;276;-1005.78,-741.4697;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;-4589.027,-1234.217;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;254;-4594.066,-976.9276;Float;False;Constant;_Float13;Float 13;17;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;323;-4571.328,-553.9402;Float;False;Constant;_Float16;Float 16;17;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;99;-4227.936,1085.379;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;360;-4404.177,-881.8566;Float;False;358;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;125;-4406.959,-1114.462;Float;True;Property;_Water_normal;Water_normal;0;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;327;-4388.118,-673.2754;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;363;-4342.269,-1299.758;Float;False;Constant;_Color4;Color 4;25;0;0,0,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DepthFade;111;-848.6198,-736.7981;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.LogOpNode;96;-4059.722,1181.955;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;358;2621.524,1281.844;Float;False;Perlin_mask;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;162;-851.5897,-851.4434;Float;False;Constant;_Float4;Float 4;10;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;104;-4051.552,983.2261;Float;False;Property;_Blur_intensity;Blur_intensity;14;0;0;0;0.5;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;145;-647.1833,-888.5977;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;98;-3914.723,1078.955;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;359;-3978.28,-918.1567;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;159;-647.5576,-782.0256;Float;False;Constant;_Float0;Float 0;10;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;160;-651.3578,-679.4265;Float;False;Constant;_Float2;Float 2;10;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;362;-3985.67,-766.5587;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;168;-3762.272,938.641;Float;False;Constant;_Float12;Float 12;10;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;95;-3747.527,1041.055;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;122;-3705.568,-692.1392;Float;False;Property;_Ditortion_intensity;Ditortion_intensity;15;0;0.292;0;0.06;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;116;-487.5161,-895.4167;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;322;-3766.794,-837.3961;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;-325.8175,-889.8157;Float;False;Depth;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ScreenPosInputsNode;127;-3382.188,-1026.437;Float;False;0;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-3375.167,-825.2382;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-3576.581,979.9022;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;115;-3762.386,1166.004;Float;False;114;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;113;-3408.854,1029.197;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;124;-3201.972,-930.3369;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ComponentMaskNode;126;-3198.972,-826.1384;Float;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;70;-4189.666,413.4026;Float;False;Constant;_Vector1;Vector 1;1;0;0.1,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;71;-4497.417,558.155;Float;False;101;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-3213.826,1032.656;Float;False;Offset_value;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;121;-2939.77,-889.4379;Float;False;2;2;0;FLOAT2;0.0,0;False;1;FLOAT2;0.0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-3939.768,435.6027;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-2793.199,-891.7539;Float;False;Screen_Position;-1;True;1;0;FLOAT2;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;72;-4190.666,533.4026;Float;False;Constant;_Vector3;Vector 3;1;0;0,0.1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;128;-4486.158,297.671;Float;False;129;0;1;FLOAT2
Node;AmplifyShaderEditor.WorldPosInputsNode;212;-4110.001,-2312.441;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ScreenDepthNode;173;-4280.395,-2167.226;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;75;-4215.064,675.0028;Float;False;Constant;_Vector5;Vector 5;1;0;-0.1,-0.1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SurfaceDepthNode;172;-4257.731,-2061.691;Float;False;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-3759.669,413.3029;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT2;0.1,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-3939.567,556.0031;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-3937.966,664.8029;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;81;-3239.426,395.877;Float;False;Constant;_Float1;Float 1;-1;0;0.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;82;-3239.228,509.177;Float;False;Constant;_Float3;Float 3;-1;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;78;-3764.772,545.7028;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT2;0,0.1,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;174;-4045.537,-2151.238;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;80;-3595.333,125.1609;Float;False;Global;_GrabScreen1;Grab Screen 1;0;0;Instance;109;True;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;258;-3866.665,-2234.401;Float;False;False;True;True;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ScreenColorNode;83;-3562.572,339.6029;Float;False;Global;_GrabScreen2;Grab Screen 2;0;0;Instance;109;True;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;153;-3320.228,-725.7902;Float;False;Constant;_Color2;Color 2;8;0;0,0,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;87;-3758.669,668.3032;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT2;-0.1,-0.1,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;259;-3619.865,-2128.3;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;253;-3311.678,-534.7062;Float;False;Property;_Normal_intensity;Normal_intensity;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-3055.128,475.6759;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-3057.13,367.7763;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.Vector2Node;199;-3592.337,-1970.738;Float;False;Property;_Projection_tiling;Projection_tiling;21;0;1,1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;86;-3233.326,617.7766;Float;False;Constant;_Float5;Float 5;-1;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;88;-3556.572,547.4031;Float;False;Global;_GrabScreen3;Grab Screen 3;0;0;Instance;109;True;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;284;-3544.5,-1839.99;Float;False;Constant;_Float15;Float 15;19;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;281;-3324.401,-1740.092;Float;False;279;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;185;-3429.836,-2192.537;Float;False;Property;_Projection_depth;Projection_depth;20;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-3285.101,-1851.593;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-2874.345,414.2757;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;152;-2954.327,-690.8881;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;91;-3229.727,721.6768;Float;False;Constant;_Float7;Float 7;-1;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-3052.228,587.3762;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ScreenColorNode;89;-3551.469,744.3031;Float;False;Global;_GrabScreen4;Grab Screen 4;0;0;Instance;109;True;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;183;-3431.636,-2113.638;Float;False;True;True;False;False;1;0;FLOAT2;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-3049.528,692.5765;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;230;-2641.176,-672.3083;Float;False;Normal_value;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;304;-3011.958,-1806.776;Float;False;230;0;1;COLOR
Node;AmplifyShaderEditor.LerpOp;282;-3076.2,-1971.19;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;2;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleTimeNode;237;-3140.069,-2395.64;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;317;-1002.56,-2011.277;Float;False;230;0;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;106;-2718.242,551.9894;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;184;-3083.036,-2204.536;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;236;-3243.366,-2308.439;Float;False;Property;_Projection_speed;Projection_speed;22;0;0.1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;305;-2777.996,-1868.495;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;138;-1877.733,970.6969;Float;False;114;0;1;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;318;-742.2603,-1998.179;Float;True;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;167;-1762.996,1185.236;Float;False;Constant;_Float11;Float 11;10;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;107;-2513.298,653.9879;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;166;-1761.196,1101.936;Float;False;Constant;_Float10;Float 10;10;0;30;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;-2915.436,-2212.637;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-2916.469,-2320.339;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;136;-1637.442,586.228;Float;False;Property;_C_Deepblue;C_Deepblue;18;0;0.05936419,0.2391854,0.4485294,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;319;-482.2604,-1996.778;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;17.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;154;-2598.452,770.3015;Float;False;SceneColor_processed;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.PannerNode;235;-2642.035,-2276.139;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,-1;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.DynamicAppendNode;373;-2467.494,-1849.036;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;149;-1640.02,395.1627;Float;False;Property;_C_Closestblue;C_Closestblue;17;0;0.05936419,0.2391854,0.4485294,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FresnelNode;142;-1586.411,1096.86;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;155;-2030.433,559.6191;Float;False;154;0;1;COLOR
Node;AmplifyShaderEditor.OneMinusNode;141;-1582.898,975.6734;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;320;-236.1612,-2002.578;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;144;-1654.271,794.9215;Float;False;Property;_C_Closeblue;C_Closeblue;16;0;0.05936419,0.2391854,0.4485294,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;310;-246.2616,-1854.577;Float;False;Constant;_Color3;Color 3;21;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-1316.158,421.1832;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.DepthFade;307;-314.1616,-1609.077;Float;False;1;0;FLOAT;1.5;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;-1332.296,1002.515;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;302;-2624.458,-2145.877;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-1329.379,615.2193;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;316;-6.260579,-1836.875;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;246;-2399.934,-1998.639;Float;False;Property;_Projection_intensity;Projection_intensity;23;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;192;-2445.336,-2211.237;Float;True;Property;_Projection;Projection;19;0;Assets/Scenes/Boris/Workshop_ressources/foam-002.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;315;-97.26056,-1645.777;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;146;-1186.22,806.4634;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.CommentaryNode;400;3304.334,-77.6236;Float;False;1047.541;403.52;Scale depth from start to end;8;405;406;403;402;408;407;404;409;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;137;-1147.737,523.4245;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.DepthFade;333;-318.5907,-1520.754;Float;False;1;0;FLOAT;0.07;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;289;651.0391,-129.2718;Float;False;Constant;_Color1;Color 1;19;0;0,0.5862069,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;288;649.4389,-308.9718;Float;False;Constant;_Color0;Color 0;19;0;1,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;148;-936.2195,605.1628;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR
Node;AmplifyShaderEditor.ProjectionParams;401;3416.657,400.3594;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;147;-951.3193,822.963;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR
Node;AmplifyShaderEditor.ClampOpNode;334;-104.89,-1520.654;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;407;3371.504,148.6831;Float;False;Property;_Float25;Float 25;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;250;-2124.935,-2065.04;Float;False;114;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;193;-2117.235,-2177.838;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;286;653.7401,49.52829;Float;False;279;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;309;155.5386,-1726.977;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;287;909.8388,-46.17153;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;247;-1933.435,-2182.039;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;408;3614.704,141.6424;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;292;655.9399,156.1286;Float;False;Property;_Show_distance;Show_distance;13;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;335;154.3092,-1589.055;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.PosterizeNode;321;326.8397,-1765.776;Float;False;150;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;139;-743.7893,709.428;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;379;536.2635,-1873.93;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;297;1543.341,186.7236;Float;False;296;0;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;257;921.8728,-141.4813;Float;False;256;0;1;FLOAT4
Node;AmplifyShaderEditor.NegateNode;402;3776.135,8.950976;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PosterizeNode;336;324.6773,-1645.355;Float;False;50;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;256;-1745.264,-2191.899;Float;False;Projection_color;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;300;1440.041,-62.37648;Float;False;Property;_Emission_power;Emission_power;24;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;403;3769.245,83.25574;Float;False;Property;_Float23;Float 23;7;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;1135.64,62.52882;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SurfaceDepthNode;409;3322.315,24.01291;Float;False;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;296;-537.0596,768.5238;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;406;4023.987,-1.255394;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;285;1313.838,68.72855;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;1750.041,-76.37648;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleSubtractOpNode;405;4026.575,107.7285;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;332;670.4305,-1712.374;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;312;836.8561,-1717.254;Float;False;Foam;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;298;1926.241,46.02351;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleDivideOpNode;404;4211.313,131.016;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;355;2653.525,1369.843;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;314;2095.04,-44.67645;Float;False;312;0;1;COLOR
Node;AmplifyShaderEditor.ColorNode;381;2301.942,-13.1994;Float;False;Property;_Watercolor_fog;Watercolor_fog;28;0;0.4509804,0.4941176,0.6627451,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;410;4484.042,231.9386;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;354;2853.124,1256.143;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;357;2883.424,1352.843;Float;False;Property;_vo_intensity;vo_intensity;27;0;0.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;313;2157.139,51.82364;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;386;2397.942,166.8006;Float;False;Constant;_Float20;Float 20;27;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;163;-439.0945,463.4356;Float;False;Constant;_Float6;Float 6;10;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;158;-266.0468,353.832;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;367;-4264.005,-1802.986;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;165;-640.6947,431.6359;Float;False;Constant;_Float9;Float 9;10;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;164;-645.6947,353.6357;Float;False;Constant;_Float8;Float 8;10;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;352;2106.823,1247.643;Float;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-3833.637,-2135.538;Float;False;2;2;0;FLOAT;0,0,0;False;1;FLOAT;0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;150;1795.68,453.7767;Float;False;Property;_Metallic;Metallic;8;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;109;-4033.36,30.6339;Float;False;Global;_GrabScreen;GrabScreen;1;0;Object;-1;True;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;277;-1190.68,-747.8696;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;342;1662.424,1196.744;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;156;-80.02074,349.3701;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;347;582.424,1505.243;Float;False;Constant;_Float18;Float 18;25;0;0.02;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;350;976.0234,1367.644;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,-1;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.LerpOp;384;2584.942,282.8006;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;151;1793.683,545.7767;Float;False;Property;_Smoothnest;Smoothnest;6;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;157;-470.7721,320.1966;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;353;1922.824,1362.443;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;2.0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;366;1410.932,463.3408;Float;False;Constant;_Color5;Color 5;25;0;0,0,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;340;709.4244,1301.789;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.3,0.3;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;351;1937.824,1251.543;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;3,3,3,3;False;1;FLOAT4
Node;AmplifyShaderEditor.MVPMatrixNode;377;-4182.557,-1888.78;Float;False;0;1;FLOAT4x4
Node;AmplifyShaderEditor.SamplerNode;341;1276.624,1337.296;Float;True;Property;_TextureSample2;Texture Sample 2;25;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;255;1852.586,335.4655;Float;False;230;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;356;3040.225,1242.443;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;383;2577.942,157.8006;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.Vector3Node;301;1870.294,662.0896;Float;False;Constant;_Vector0;Vector 0;21;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;349;793.6233,1434.843;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2259.967,288.6526;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Water2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Translucent;0.5;True;True;0;False;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;399;3343.958,351.0594;Float;False;297.1897;243;Correction for near plane clipping;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;170;-2060.636,177.2836;Float;False;2226.516;1183.07;Comment;0;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;110;-4651.156,-65.29681;Float;False;2440.799;1426.466;b;0;Blur;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;337;-1109.021,-2160.554;Float;False;2331.252;881.2509;Comment;0;Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;169;-5369.857,-1424.46;Float;False;3060.967;1172.211;Comment;0;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;295;-4522.701,-2593.392;Float;False;3300.06;1067.46;Comment;0;Projection;1,1,1,1;0;0
WireConnection;270;0;268;0
WireConnection;270;1;269;0
WireConnection;271;0;270;0
WireConnection;271;1;293;0
WireConnection;273;0;271;0
WireConnection;273;1;294;0
WireConnection;345;0;344;0
WireConnection;345;1;346;0
WireConnection;275;0;273;0
WireConnection;411;0;275;0
WireConnection;343;0;339;0
WireConnection;343;1;345;0
WireConnection;133;0;132;0
WireConnection;133;1;134;0
WireConnection;326;0;131;0
WireConnection;326;1;133;0
WireConnection;338;1;343;0
WireConnection;130;0;131;0
WireConnection;130;1;133;0
WireConnection;279;0;411;0
WireConnection;325;0;324;0
WireConnection;325;1;326;0
WireConnection;361;0;338;0
WireConnection;276;0;112;0
WireConnection;276;1;283;0
WireConnection;276;2;279;0
WireConnection;251;0;252;0
WireConnection;251;1;130;0
WireConnection;99;0;102;0
WireConnection;99;1;103;0
WireConnection;125;1;251;0
WireConnection;125;5;254;0
WireConnection;327;1;325;0
WireConnection;327;5;323;0
WireConnection;111;0;276;0
WireConnection;96;0;99;0
WireConnection;358;0;361;0
WireConnection;145;0;111;0
WireConnection;145;1;162;0
WireConnection;98;0;99;0
WireConnection;98;1;96;0
WireConnection;359;0;125;0
WireConnection;359;1;363;0
WireConnection;359;2;360;0
WireConnection;362;0;363;0
WireConnection;362;1;327;0
WireConnection;362;2;360;0
WireConnection;95;0;104;0
WireConnection;95;1;98;0
WireConnection;116;0;145;0
WireConnection;116;1;160;0
WireConnection;116;2;159;0
WireConnection;322;0;359;0
WireConnection;322;1;362;0
WireConnection;114;0;116;0
WireConnection;123;0;322;0
WireConnection;123;1;122;0
WireConnection;117;0;95;0
WireConnection;117;1;168;0
WireConnection;113;0;117;0
WireConnection;113;1;95;0
WireConnection;113;2;115;0
WireConnection;124;0;127;0
WireConnection;126;0;123;0
WireConnection;101;0;113;0
WireConnection;121;0;124;0
WireConnection;121;1;126;0
WireConnection;73;0;70;0
WireConnection;73;1;71;0
WireConnection;129;0;121;0
WireConnection;76;0;128;0
WireConnection;76;1;73;0
WireConnection;77;0;72;0
WireConnection;77;1;71;0
WireConnection;79;0;75;0
WireConnection;79;1;71;0
WireConnection;78;0;128;0
WireConnection;78;1;77;0
WireConnection;174;0;173;0
WireConnection;174;1;172;0
WireConnection;80;0;128;0
WireConnection;258;0;212;0
WireConnection;83;0;76;0
WireConnection;87;0;128;0
WireConnection;87;1;79;0
WireConnection;259;0;258;0
WireConnection;259;1;174;0
WireConnection;84;0;83;0
WireConnection;84;1;82;0
WireConnection;85;0;80;0
WireConnection;85;1;81;0
WireConnection;88;0;78;0
WireConnection;280;0;199;0
WireConnection;280;1;284;0
WireConnection;90;0;85;0
WireConnection;90;1;84;0
WireConnection;152;0;322;0
WireConnection;152;1;153;0
WireConnection;152;2;253;0
WireConnection;92;0;88;0
WireConnection;92;1;86;0
WireConnection;89;0;87;0
WireConnection;183;0;259;0
WireConnection;93;0;89;0
WireConnection;93;1;91;0
WireConnection;230;0;152;0
WireConnection;282;0;199;0
WireConnection;282;1;280;0
WireConnection;282;2;281;0
WireConnection;106;0;90;0
WireConnection;106;1;92;0
WireConnection;184;0;185;0
WireConnection;184;1;183;0
WireConnection;305;0;304;0
WireConnection;318;0;317;0
WireConnection;107;0;106;0
WireConnection;107;1;93;0
WireConnection;198;0;184;0
WireConnection;198;1;282;0
WireConnection;238;0;237;0
WireConnection;238;1;236;0
WireConnection;319;0;318;1
WireConnection;154;0;107;0
WireConnection;235;0;198;0
WireConnection;235;1;238;0
WireConnection;373;0;305;0
WireConnection;373;1;305;1
WireConnection;142;2;166;0
WireConnection;142;3;167;0
WireConnection;141;0;138;0
WireConnection;320;0;319;0
WireConnection;143;0;149;0
WireConnection;143;1;155;0
WireConnection;140;0;141;0
WireConnection;140;1;142;0
WireConnection;302;0;373;0
WireConnection;302;1;235;0
WireConnection;135;0;136;0
WireConnection;135;1;155;0
WireConnection;316;0;320;0
WireConnection;316;1;310;0
WireConnection;192;1;302;0
WireConnection;315;0;307;0
WireConnection;146;0;144;0
WireConnection;146;1;136;0
WireConnection;146;2;140;0
WireConnection;137;0;143;0
WireConnection;137;1;135;0
WireConnection;137;2;138;0
WireConnection;148;0;137;0
WireConnection;147;0;146;0
WireConnection;334;0;333;0
WireConnection;193;0;192;0
WireConnection;193;1;246;0
WireConnection;309;0;316;0
WireConnection;309;2;315;0
WireConnection;287;0;288;0
WireConnection;287;1;289;0
WireConnection;287;2;286;0
WireConnection;247;0;193;0
WireConnection;247;2;250;0
WireConnection;408;0;407;0
WireConnection;408;1;401;2
WireConnection;335;0;310;0
WireConnection;335;2;334;0
WireConnection;321;1;309;0
WireConnection;139;0;148;0
WireConnection;139;1;147;0
WireConnection;379;0;321;0
WireConnection;402;0;408;0
WireConnection;336;1;335;0
WireConnection;256;0;247;0
WireConnection;291;0;287;0
WireConnection;291;1;292;0
WireConnection;296;0;139;0
WireConnection;406;0;409;0
WireConnection;406;1;402;0
WireConnection;285;0;257;0
WireConnection;285;1;291;0
WireConnection;299;0;297;0
WireConnection;299;1;300;0
WireConnection;405;0;403;0
WireConnection;405;1;408;0
WireConnection;332;0;379;0
WireConnection;332;1;336;0
WireConnection;312;0;332;0
WireConnection;298;0;299;0
WireConnection;298;1;285;0
WireConnection;404;0;406;0
WireConnection;404;1;405;0
WireConnection;410;0;404;0
WireConnection;354;0;358;0
WireConnection;354;1;355;0
WireConnection;313;0;314;0
WireConnection;313;1;298;0
WireConnection;158;0;157;0
WireConnection;158;2;163;0
WireConnection;352;0;353;0
WireConnection;208;0;174;0
WireConnection;277;0;112;0
WireConnection;277;1;283;0
WireConnection;342;0;338;0
WireConnection;342;1;341;0
WireConnection;156;0;139;0
WireConnection;156;1;158;0
WireConnection;350;0;340;0
WireConnection;350;1;349;0
WireConnection;384;0;313;0
WireConnection;384;1;386;0
WireConnection;384;2;410;0
WireConnection;157;2;164;0
WireConnection;157;3;165;0
WireConnection;353;0;351;0
WireConnection;351;0;342;0
WireConnection;341;1;350;0
WireConnection;356;0;354;0
WireConnection;356;1;357;0
WireConnection;383;0;297;0
WireConnection;383;1;381;0
WireConnection;383;2;410;0
WireConnection;349;0;344;0
WireConnection;349;1;347;0
WireConnection;0;0;383;0
WireConnection;0;1;255;0
WireConnection;0;2;384;0
WireConnection;0;3;150;0
WireConnection;0;4;151;0
WireConnection;0;11;356;0
WireConnection;0;12;301;0
ASEEND*/
//CHKSM=A006AD5FC97C985027BC84958746B994F5084ABA