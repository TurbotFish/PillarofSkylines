// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/needlecrystal_dissolve"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Texture0("Texture 0", 2D) = "white" {}
		_Noise_tiling("Noise_tiling", Float) = 1
		_Pan_speed("Pan_speed", Range( 0 , 0.3)) = 0.2
		_Transition("Transition", Range( 0 , 1)) = 0
		_Trans_noise("Trans_noise", 2D) = "white" {}
		_Trans_tiling("Trans_tiling", Range( 0 , 0.5)) = 0.15
		_Concrete_N("Concrete_N", 2D) = "bump" {}
		_Concrete_D("Concrete_D", 2D) = "white" {}
		_Light_trans("Light_trans", Range( 0 , 0.3)) = 0.1
		[Toggle]_Reverse_Transition("Reverse_Transition", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
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
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform sampler2D _Concrete_N;
		uniform float _Reverse_Transition;
		uniform sampler2D _Trans_noise;
		uniform float _Trans_tiling;
		uniform float _Transition;
		uniform sampler2D _Concrete_D;
		uniform sampler2D _Texture0;
		uniform float _Pan_speed;
		uniform float _Noise_tiling;
		uniform float4 _Concrete_N_ST;
		uniform float _Light_trans;
		uniform float _Cutoff = 0.5;


		inline float3 TriplanarNormal( sampler2D topBumpMap, sampler2D midBumpMap, sampler2D botBumpMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign(worldNormal);
			half3 xNorm; half3 yNorm; half3 zNorm;
			if(vertex == 1){
			xNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );
			yNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zx).xy,0,0) ) );
			zNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );
			} else {
			xNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zx ) );
			zNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			}
			xNorm = normalize( half3( xNorm.xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ) );
			yNorm = normalize( half3( yNorm.xy + worldNormal.zx, worldNormal.y));
			zNorm = normalize( half3( zNorm.xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ) );
			xNorm = xNorm.zyx;
			yNorm = yNorm.yzx;
			zNorm = zNorm.xyz;
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			if(vertex == 1){
			xNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );
			yNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zx).xy,0,0) ) );
			zNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );
			} else {
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.zx ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			}
			return xNorm* projNormal.x + yNorm* projNormal.y + zNorm* projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal65 = TriplanarNormal( _Concrete_N, _Concrete_N, _Concrete_N, ase_worldPos, ase_worldNormal, 1.0, 0.15, 0 );
			float3 tanTriplanarNormal65 = mul( ase_worldToTangent, worldTriplanarNormal65 );
			float4 triplanar42 = TriplanarSampling( _Trans_noise, _Trans_noise, _Trans_noise, ase_worldPos, ase_worldNormal, 1.0, _Trans_tiling, 0 );
			float temp_output_58_0 = ( saturate( (0.0 + (lerp(-ase_worldPos.y,ase_worldPos.y,_Reverse_Transition) - -20.0) * (1.0 - 0.0) / (15.0 - -20.0)) ) * triplanar42.x );
			float temp_output_46_0 = step( temp_output_58_0 , _Transition );
			float3 lerpResult71 = lerp( float3(1,0,1) , tanTriplanarNormal65 , temp_output_46_0);
			o.Normal = lerpResult71;
			float4 temp_cast_0 = (0.3).xxxx;
			float4 triplanar62 = TriplanarSampling( _Concrete_D, _Concrete_D, _Concrete_D, ase_worldPos, ase_worldNormal, 1.0, 0.15, 0 );
			float4 lerpResult61 = lerp( temp_cast_0 , triplanar62 , temp_output_46_0);
			o.Albedo = lerpResult61.xyz;
			float2 appendResult33 = (float2(_Pan_speed , 0.0));
			float2 appendResult27 = (float2(ase_worldPos.y , ase_worldPos.z));
			float2 panner25 = ( ( appendResult27 * _Noise_tiling ) + _Time.y * appendResult33);
			float2 appendResult30 = (float2(0.0 , _Pan_speed));
			float2 appendResult15 = (float2(ase_worldPos.x , ase_worldPos.y));
			float2 panner7 = ( ( appendResult15 * _Noise_tiling ) + _Time.y * appendResult30);
			float4 triplanar39 = TriplanarSampling( _Texture0, _Texture0, _Texture0, ase_worldPos, ase_worldNormal, 1.0, 0.05, 0 );
			float4 lerpResult19 = lerp( tex2D( _Texture0, panner25 ) , tex2D( _Texture0, ( panner7 + triplanar39.x ) ) , saturate( -ase_worldNormal.z ));
			float2 uv_Concrete_N = i.uv_texcoord * _Concrete_N_ST.xy + _Concrete_N_ST.zw;
			float4 lerpResult35 = lerp( lerpResult19 , ( lerpResult19 * 1.3 ) , ( uv_Concrete_N.y * 0.0 ));
			float4 temp_cast_2 = (2.0).xxxx;
			float4 lerpResult76 = lerp( lerpResult35 , temp_cast_2 , step( temp_output_58_0 , ( _Transition + (0.0 + (_Transition - 0.0) * (_Light_trans - 0.0) / (1.0 - 0.0)) ) ));
			float4 temp_cast_3 = (0.0).xxxx;
			float4 lerpResult43 = lerp( lerpResult76 , temp_cast_3 , temp_output_46_0);
			o.Emission = lerpResult43.rgb;
			o.Alpha = 1;
			clip( ( 1.0 - temp_output_46_0 ) - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
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
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
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
Version=13201
169;129;1677;861;1078.406;571.8992;2.181728;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;14;-2364.402,-108.857;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;15;-2178.201,-81.45676;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;3;-2238.93,260.2255;Float;False;Property;_Noise_tiling;Noise_tiling;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;32;-2527.014,604.0018;Float;False;Property;_Pan_speed;Pan_speed;3;0;0.2;0;0.3;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;31;-2397.414,524.0018;Float;False;Constant;_Float3;Float 3;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;28;-2364.647,54.10153;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-2024.13,402.1254;Float;False;Constant;_Float2;Float 2;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;40;-2153.977,1156.417;Float;False;Constant;_Float6;Float 6;4;0;0.05;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;12;-1881.336,414.9495;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;27;-2178.446,81.50168;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-2010.501,-17.75668;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;2;-1797.136,-230.4504;Float;True;Property;_Texture0;Texture 0;1;0;Assets/Plugins/Ikari/Vertex Painter/Examples/Assets/Textures/Noise/Plasma Noise.png;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.DynamicAppendNode;30;-2218.214,544.8022;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WorldPosInputsNode;49;-848.1614,1225.226;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;7;-1686.335,233.9496;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.DynamicAppendNode;33;-2208.614,648.8016;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WorldNormalVector;17;-1433.591,897.6075;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NegateNode;50;-609.0176,1200.129;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;39;-1978.714,1133.032;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;3;Assets/Scenes/Aloïs/WindNoiseTex.jpg;Mid Texture 0;_MidTexture0;white;4;None;Bot Texture 0;_BotTexture0;white;5;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2017.146,103.6018;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;54;-623.9348,1443.412;Float;False;Constant;_Float10;Float 10;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;53;-625.6774,1351.057;Float;False;Constant;_Float9;Float 9;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;55;-622.1923,1539.253;Float;False;Constant;_Float11;Float 11;5;0;-20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;25;-1670.019,622.4257;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-1350.165,244.1098;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.NegateNode;23;-1225.591,963.2075;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;82;-459.7949,1224.088;Float;False;Property;_Reverse_Transition;Reverse_Transition;10;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;-634.3903,1626.381;Float;False;Constant;_Float12;Float 12;5;0;15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;52;-411.3433,1366.74;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;45;-738.0243,1775.373;Float;False;Property;_Trans_tiling;Trans_tiling;6;0;0.15;0;0.5;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;60;-1001.816,1673.397;Float;True;Property;_Trans_noise;Trans_noise;5;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;81;-366.7192,1037.64;Float;False;Constant;_Float16;Float 16;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;47;-182.8613,1241.445;Float;False;Property;_Transition;Transition;4;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;8;-1179.908,211.3315;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Assets/Plugins/Ikari/Vertex Painter/Examples/Assets/Textures/Noise/Plasma Noise.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;24;-1168.791,587.0078;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Assets/Plugins/Ikari/Vertex Painter/Examples/Assets/Textures/Noise/Plasma Noise.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;59;-607.7357,1861.461;Float;False;Constant;_Float13;Float 13;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;21;-1046.391,936.0074;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-374.7192,960.8401;Float;False;Constant;_Float14;Float 14;9;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;75;-680.8069,1071.869;Float;False;Property;_Light_trans;Light_trans;9;0;0.1;0;0.3;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;42;-445.9637,1757.778;Float;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;3;Assets/Graph/Textures/_OLD/Architecture/Pillar/T_Bridge01_D.png;Mid Texture 0;_MidTexture0;white;4;None;Bot Texture 0;_BotTexture0;white;5;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-819.9076,470.8313;Float;False;Constant;_Float0;Float 0;2;0;1.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;19;-826.491,340.2075;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;38;-689.6415,745.61;Float;False;Constant;_Float5;Float 5;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;79;-126.7192,1029.64;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-725.1746,607.9857;Float;False;0;66;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;57;-238.8305,1368.483;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;78;108.279,881.8799;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-655.0074,336.4315;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-54.11993,1342.344;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-465.5241,638.4769;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;63;197.7829,-275.5287;Float;True;Property;_Concrete_D;Concrete_D;8;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;64;307.2335,138.2155;Float;False;Constant;_Float7;Float 7;7;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;66;296.0705,448.4827;Float;True;Property;_Concrete_N;Concrete_N;7;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;67;561.3135,593.7269;Float;False;Constant;_Float8;Float 8;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;77;-70.07465,639.7349;Float;False;Constant;_Float15;Float 15;8;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;73;247.4792,886.1782;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;35;-364.1746,387.5853;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;76;111.0999,571.0342;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TriplanarNode;62;698.1259,-284.7267;Float;True;Spherical;World;False;Top Texture 2;_TopTexture2;white;6;None;Mid Texture 1;_MidTexture1;white;7;None;Bot Texture 1;_BotTexture1;white;8;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;483.3087,-376.6842;Float;False;Constant;_Float1;Float 1;2;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;72;951.4803,606.7512;Float;True;Constant;_Vector0;Vector 0;8;0;1,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;44;-357.1926,293.7295;Float;False;Constant;_Float4;Float 4;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;65;775.316,394.6898;Float;True;Spherical;World;True;Top Texture 0;_TopTexture0;white;7;None;Mid Texture 0;_MidTexture0;white;7;None;Bot Texture 0;_BotTexture0;white;8;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StepOpNode;46;121.9044,1332.64;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;101;1462.627,848.175;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;43;-157.2167,395.1174;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;71;1343.005,344.3237;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;61;1098.608,-340.0075;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1772.722,86.9331;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/needlecrystal_dissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;True;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;14;1
WireConnection;15;1;14;2
WireConnection;12;0;13;0
WireConnection;27;0;28;2
WireConnection;27;1;28;3
WireConnection;16;0;15;0
WireConnection;16;1;3;0
WireConnection;30;0;31;0
WireConnection;30;1;32;0
WireConnection;7;0;16;0
WireConnection;7;2;30;0
WireConnection;7;1;12;0
WireConnection;33;0;32;0
WireConnection;33;1;31;0
WireConnection;50;0;49;2
WireConnection;39;0;2;0
WireConnection;39;3;40;0
WireConnection;26;0;27;0
WireConnection;26;1;3;0
WireConnection;25;0;26;0
WireConnection;25;2;33;0
WireConnection;25;1;12;0
WireConnection;41;0;7;0
WireConnection;41;1;39;1
WireConnection;23;0;17;3
WireConnection;82;0;50;0
WireConnection;82;1;49;2
WireConnection;52;0;82;0
WireConnection;52;1;55;0
WireConnection;52;2;56;0
WireConnection;52;3;53;0
WireConnection;52;4;54;0
WireConnection;8;0;2;0
WireConnection;8;1;41;0
WireConnection;24;0;2;0
WireConnection;24;1;25;0
WireConnection;21;0;23;0
WireConnection;42;0;60;0
WireConnection;42;3;45;0
WireConnection;42;4;59;0
WireConnection;19;0;24;0
WireConnection;19;1;8;0
WireConnection;19;2;21;0
WireConnection;79;0;47;0
WireConnection;79;1;80;0
WireConnection;79;2;81;0
WireConnection;79;3;80;0
WireConnection;79;4;75;0
WireConnection;57;0;52;0
WireConnection;78;0;47;0
WireConnection;78;1;79;0
WireConnection;4;0;19;0
WireConnection;4;1;5;0
WireConnection;58;0;57;0
WireConnection;58;1;42;1
WireConnection;37;0;34;2
WireConnection;37;1;38;0
WireConnection;73;0;58;0
WireConnection;73;1;78;0
WireConnection;35;0;19;0
WireConnection;35;1;4;0
WireConnection;35;2;37;0
WireConnection;76;0;35;0
WireConnection;76;1;77;0
WireConnection;76;2;73;0
WireConnection;62;0;63;0
WireConnection;62;3;64;0
WireConnection;62;4;67;0
WireConnection;65;0;66;0
WireConnection;65;3;64;0
WireConnection;65;4;67;0
WireConnection;46;0;58;0
WireConnection;46;1;47;0
WireConnection;101;0;46;0
WireConnection;43;0;76;0
WireConnection;43;1;44;0
WireConnection;43;2;46;0
WireConnection;71;0;72;0
WireConnection;71;1;65;0
WireConnection;71;2;46;0
WireConnection;61;0;6;0
WireConnection;61;1;62;0
WireConnection;61;2;46;0
WireConnection;0;0;61;0
WireConnection;0;1;71;0
WireConnection;0;2;43;0
WireConnection;0;10;101;0
ASEEND*/
//CHKSM=3AF78908689539F81FAE2776DB3E0153CA773E29