// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Gravifloor"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Noise("Noise", 2D) = "white" {}
		_Flake("Flake", 2D) = "white" {}
		_Color("Color", Color) = (0.0660143,0.08088237,0.07764099,0)
		_Crystal_color("Crystal_color", Color) = (0.0660143,0.08088237,0.07764099,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Latexturequibouge("Latexturequibouge", 2D) = "white" {}
		_Thingyspeed("Thingyspeed", Range( 0 , 0.1)) = 0.1
		_CrystalFloor_N("CrystalFloor_N", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Emissive_mult("Emissive_mult", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _CrystalFloor_N;
		uniform float4 _CrystalFloor_N_ST;
		uniform sampler2D _Flake;
		uniform sampler2D _Noise;
		uniform float3 _PlayerPos;
		uniform sampler2D _Latexturequibouge;
		uniform float _Thingyspeed;
		uniform float4 _Crystal_color;
		uniform float4 _Color;
		uniform float _Emissive_mult;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _Metallic;


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
			float2 uv_CrystalFloor_N = i.uv_texcoord * _CrystalFloor_N_ST.xy + _CrystalFloor_N_ST.zw;
			float3 tex2DNode179 = UnpackScaleNormal( tex2D( _CrystalFloor_N, uv_CrystalFloor_N ) ,1.0 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal60 = TriplanarNormal( _Flake, _Flake, _Flake, ase_worldPos, ase_worldNormal, 3.0, 1.0, 0 );
			float3 tanTriplanarNormal60 = mul( ase_worldToTangent, worldTriplanarNormal60 );
			float3 lerpResult67 = lerp( tanTriplanarNormal60 , tex2DNode179 , 0.5);
			float3 temp_output_115_0 = BlendNormals( tex2DNode179 , lerpResult67 );
			float4 triplanar20 = TriplanarSampling( _Noise, _Noise, _Noise, ase_worldPos, ase_worldNormal, 3.0, 0.2, 0 );
			float3 temp_output_5_0_g2 = ( ( ase_worldPos - _PlayerPos ) / 10.0 );
			float dotResult8_g2 = dot( temp_output_5_0_g2 , temp_output_5_0_g2 );
			float clampResult10_g2 = clamp( dotResult8_g2 , 0.0 , 1.0 );
			float temp_output_41_0 = saturate( ( ( ( pow( triplanar20.x , 3.0 ) * 3.0 ) * pow( clampResult10_g2 , 1.0 ) ) + (0.0 + (pow( clampResult10_g2 , 1.0 ) - -0.4) * (1.0 - 0.0) / (1.0 - -0.4)) ) );
			float lerpResult49 = lerp( 0.0 , 2.0 , step( temp_output_41_0 , 0.75 ));
			float smoothstepResult43 = smoothstep( 0.0 , 1.0 , temp_output_41_0);
			float lerpResult54 = lerp( 0.0 , lerpResult49 , smoothstepResult43);
			float3 lerpResult202 = lerp( temp_output_115_0 , -temp_output_115_0 , saturate( lerpResult54 ));
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float3 temp_cast_0 = (3.0).xxx;
			float4 triplanar155 = TriplanarSampling( _Latexturequibouge, _Latexturequibouge, _Latexturequibouge, ase_worldPos, ase_worldNormal, 1.0, 0.02, 0 );
			float2 appendResult161 = (float2(triplanar155.x , triplanar155.y));
			float2 temp_output_157_0 = ( appendResult161 * 0.1 );
			float mulTime149 = _Time.y * _Thingyspeed;
			float2 _Vector0 = float2(0,0.2);
			float2 appendResult128 = (float2(ase_worldPos.x , ase_worldPos.y));
			float2 panner145 = ( ( appendResult128 * 0.02 ) + mulTime149 * _Vector0);
			float2 appendResult129 = (float2(ase_worldPos.z , ase_worldPos.x));
			float2 panner144 = ( ( appendResult129 * 0.02 ) + mulTime149 * _Vector0);
			float2 appendResult127 = (float2(ase_worldPos.y , ase_worldPos.z));
			float2 panner143 = ( ( appendResult127 * 0.02 ) + mulTime149 * _Vector0);
			float3 weightedBlendVar133 = pow( abs( ase_vertexNormal ) , temp_cast_0 );
			float4 weightedAvg133 = ( ( weightedBlendVar133.x*tex2D( _Latexturequibouge, ( temp_output_157_0 + panner145 ) ) + weightedBlendVar133.y*tex2D( _Latexturequibouge, ( temp_output_157_0 + panner144 ) ) + weightedBlendVar133.z*tex2D( _Latexturequibouge, ( temp_output_157_0 + panner143 ) ) )/( weightedBlendVar133.x + weightedBlendVar133.y + weightedBlendVar133.z ) );
			float4 temp_cast_1 = (3.0).xxxx;
			float4 temp_output_163_0 = pow( weightedAvg133 , temp_cast_1 );
			float4 temp_output_175_0 = saturate( temp_output_163_0 );
			float3 lerpResult177 = lerp( lerpResult202 , -lerpResult202 , temp_output_175_0.r);
			o.Normal = lerpResult177;
			float4 lerpResult5 = lerp( _Crystal_color , _Color , smoothstepResult43);
			o.Albedo = lerpResult5.rgb;
			float4 temp_cast_4 = (0.0).xxxx;
			float3 temp_output_5_0_g3 = ( ( ase_worldPos - _PlayerPos ) / 4.0 );
			float dotResult8_g3 = dot( temp_output_5_0_g3 , temp_output_5_0_g3 );
			float clampResult10_g3 = clamp( dotResult8_g3 , 0.0 , 1.0 );
			float4 lerpResult97 = lerp( ( _Crystal_color * 1.3 ) , temp_cast_4 , pow( clampResult10_g3 , 0.3 ));
			float4 temp_cast_5 = (0.0).xxxx;
			float4 lerpResult103 = lerp( lerpResult97 , temp_cast_5 , smoothstepResult43);
			float4 lerpResult162 = lerp( lerpResult103 , _Crystal_color , ( temp_output_163_0 * 0.5 ).r);
			float blendOpSrc197 = ( ( tex2DNode179.r + tex2DNode179.g ) * 1.0 );
			float4 blendOpDest197 = lerpResult162;
			o.Emission = ( ( blendOpDest197/ ( 1.0 - blendOpSrc197 ) ) * _Emissive_mult ).rgb;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float temp_output_182_0 = ( tex2D( _TextureSample0, uv_TextureSample0 ).r * 0.5 );
			float lerpResult120 = lerp( 0.0 , temp_output_182_0 , smoothstepResult43);
			float lerpResult173 = lerp( lerpResult120 , temp_output_182_0 , temp_output_175_0.r);
			float lerpResult207 = lerp( 0.0 , lerpResult173 , _Metallic);
			o.Metallic = lerpResult207;
			float lerpResult64 = lerp( 0.8 , 0.5 , smoothstepResult43);
			float lerpResult172 = lerp( lerpResult64 , 0.0 , temp_output_175_0.r);
			o.Smoothness = lerpResult172;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:forward 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
-1913;29;1906;1004;3977.443;1807.957;4.661835;True;True
Node;AmplifyShaderEditor.CommentaryNode;96;-2494.426,1707.017;Float;False;706.5302;650.533;SphereMask;9;104;101;102;8;19;9;12;20;40;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2444.426,1757.017;Float;False;Constant;_Float11;Float 11;2;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-1733.49,-44.24797;Float;False;Constant;_Float1;Float 1;1;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;20;-2172.376,1826.152;Float;True;Spherical;World;False;Noise;_Noise;white;1;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;Mid Texture 0;_MidTexture0;white;2;None;Bot Texture 0;_BotTexture0;white;3;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;46;-1619.943,1873.429;Float;False;1861.19;673.233;Mask;17;58;43;45;44;41;31;34;26;38;37;29;35;36;30;27;28;39;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2400.048,2015.036;Float;False;Constant;_Float2;Float 2;1;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-2411.048,2186.036;Float;False;Constant;_Float4;Float 4;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;28;-1550.087,2075.43;Float;False;Constant;_Float6;Float 6;3;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RelayNode;39;-1542.876,1936.876;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;19;-2424.273,1869.767;Float;False;Global;_PlayerPos;_PlayerPos;1;0;0,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;194;-2075.744,-834.4601;Float;False;Constant;_Float37;Float 37;10;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;139;-750.6965,-1681.159;Float;True;Property;_Latexturequibouge;Latexturequibouge;6;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.WorldPosInputsNode;125;-1788.736,-1296.753;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;171;-2121.123,-949.2692;Float;False;Constant;_Float31;Float 31;1;0;0.02;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;27;-1375.088,1923.43;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;8;-2139.048,2024.036;Float;False;SphereMask;-1;;2;3;0;FLOAT3;0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;30;-1373.088,2022.43;Float;False;Constant;_Float7;Float 7;3;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;127;-1453.701,-1428.537;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;154;-1465.595,-1087.501;Float;False;Constant;_Float23;Float 23;8;0;0.02;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;155;-1920.862,-971.1823;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;8;None;Mid Texture 0;_MidTexture0;white;9;None;Bot Texture 0;_BotTexture0;white;10;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;129;-1451.701,-1324.537;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;150;-1154.053,-952.731;Float;False;Property;_Thingyspeed;Thingyspeed;7;0;0.1;0;0.1;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;128;-1453.701,-1215.537;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;36;-1562.943,2359.07;Float;False;Constant;_Float9;Float 9;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;37;-1569.943,2443.07;Float;False;Constant;_Float10;Float 10;3;0;-0.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-1210.088,1924.43;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;35;-1562.943,2287.07;Float;False;Constant;_Float8;Float 8;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RelayNode;38;-1538.915,2185.083;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;-1163.746,-1332.58;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleTimeNode;149;-923.053,-1060.131;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-1155.438,-1464.12;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;-1167.9,-1212.118;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;146;-1153.453,-1070.731;Float;False;Constant;_Vector0;Vector 0;8;0;0,0.2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;158;-1484.015,-845.4781;Float;False;Constant;_Float24;Float 24;9;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;161;-1572.729,-949.9393;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1014.641,2071.647;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;34;-1341.088,2228.43;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;145;-940.4531,-1200.731;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;143;-946.4531,-1461.731;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-1432.015,-942.9782;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.NormalVertexDataNode;134;-716.4931,-1867.275;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;144;-948.4531,-1331.731;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;95;-528.7885,733.4227;Float;False;1164.812;555.8927;Normall;6;115;67;60;68;61;179;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;57;-571.0401,1365.319;Float;False;1113.396;435.8761;Inchallah emission;8;54;55;49;47;51;50;48;124;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-813.5533,2075.687;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;160;-691.1925,-1341.437;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;159;-694.1925,-1200.437;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.AbsOpNode;135;-517.4932,-1857.275;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;141;-516.3118,-1776.549;Float;False;Constant;_Float21;Float 21;8;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;156;-690.7145,-1472.478;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;48;-521.0401,1665.09;Float;False;Constant;_Float5;.;2;0;0.75;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;41;-661.2689,2075.964;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;181;-368.3859,614.7046;Float;False;Constant;_Float32;Float 32;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;45;-664.5257,2272.997;Float;False;Constant;_Float13;Float 13;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;44;-669.4106,2181.809;Float;False;Constant;_Float12;Float 12;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;61;-478.7885,971.1834;Float;False;Constant;_Float17;Float 17;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;138;-449.7061,-1225.911;Float;True;Property;_TextureSample3;Texture Sample 3;7;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;137;-448.7061,-1421.911;Float;True;Property;_TextureSample2;Texture Sample 2;7;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;140;-243.7712,-1854.224;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;136;-454.7061,-1628.911;Float;True;Property;_TextureSample1;Texture Sample 1;7;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;50;-336.8319,1453.974;Float;False;Constant;_Float14;Float 14;2;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;60;-309.6889,953.4774;Float;True;Spherical;World;True;Flake;_Flake;white;2;None;Mid Texture 0;_MidTexture0;white;4;None;Bot Texture 0;_BotTexture0;white;5;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;179;-306.7536,746.7305;Float;True;Property;_CrystalFloor_N;CrystalFloor_N;8;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;51;-334.6869,1531.824;Float;False;Constant;_Float15;Float 15;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;47;-322.8906,1622.934;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;43;-496.8038,2088.991;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;68;-167.5537,1156.315;Float;False;Constant;_Float20;Float 20;4;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;119;-1403.181,-352.3546;Float;False;Property;_Crystal_color;Crystal_color;4;0;0.0660143,0.08088237,0.07764099,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WeightedBlendNode;133;-7.190157,-1457.233;Float;False;5;0;FLOAT3;0,0,0;False;1;COLOR;0;False;2;COLOR;0.0,0,0,0;False;3;COLOR;0.0,0,0,0;False;4;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;164;2.353394,-1311.892;Float;False;Constant;_Float25;Float 25;8;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;102;-2401.513,2092.648;Float;False;Constant;_Float28;Float 28;3;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;104;-2408.381,2265.833;Float;False;Constant;_Float29;Float 29;3;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;55;-184.3961,1415.319;Float;False;Constant;_Float16;Float 16;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;99;-299.2725,-161.4695;Float;False;Constant;_Float27;Float 27;3;0;1.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;67;68.02356,1072.035;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RelayNode;58;36.58991,2098.013;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;49;-163.2851,1507.337;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;170;193.1348,-1346.769;Float;False;Constant;_Float30;Float 30;8;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;98;-85.41741,-64.92382;Float;False;Constant;_Float26;Float 26;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-84.65542,-185.4973;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.PowerNode;163;205.2424,-1451.333;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.BlendNormalsNode;115;252.7838,1119.734;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;54;-5.7107,1493.729;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;101;-2135.513,2148.648;Float;False;SphereMask;-1;;3;3;0;FLOAT3;0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;97;118.2567,-38.70831;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;373.115,-1447.148;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;114;-262.8001,310.9608;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;183;-99.32457,523.2312;Float;False;Constant;_Float34;Float 34;9;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;124;171.2033,1487.532;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;204;441.8559,1485.085;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;146.3825,346.0937;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;200;1116.9,-456.9893;Float;False;Constant;_Float35;Float 35;10;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;198;1110.879,-362.6651;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;103;306.7983,-38.82861;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;121;264.9727,535.7709;Float;False;Constant;_Float3;Float 3;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;202;636.0317,1397.847;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RelayNode;167;371.886,-919.3787;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;108;-1415.495,75.27348;Float;False;Property;_Color;Color;3;0;0.0660143,0.08088237,0.07764099,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;1249.355,-366.6789;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;162;545.9419,-100.6773;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RelayNode;201;759.475,1065.638;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;65;309.1171,144.6095;Float;False;Constant;_Float18;Float 18;4;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;175;365.5485,-1000.641;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;120;482.4821,336.3119;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;66;313.9731,231.6885;Float;False;Constant;_Float19;Float 19;4;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;176;479.6047,62.93262;Float;False;Constant;_Float33;Float 33;8;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;210;1509.855,-339.8923;Float;False;Property;_Emissive_mult;Emissive_mult;11;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;197;1234.76,-217.6098;Float;False;ColorDodge;False;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;5;-697.868,-69.46329;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;205;919.7623,26.20044;Float;False;Constant;_Float22;Float 22;10;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;173;668.1874,282.2383;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;206;906.7153,949.7735;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;64;479.821,157.6881;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;208;596.1771,441.7081;Float;False;Property;_Metallic;Metallic;10;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;203;440.4488,1396.44;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;178;677.3965,867.2822;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.TriplanarNode;1;-1543.49,-132.248;Float;True;Spherical;World;False;Albedo;_Albedo;white;0;Assets/Graph/Textures/CRYSTALS/T_crystals_D.png;Mid Texture 0;_MidTexture0;white;1;None;Bot Texture 0;_BotTexture0;white;2;None;Diffuse;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;184;709.0266,-520.0929;Float;True;Property;_TextureSample4;Texture Sample 4;9;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;207;959.1771,265.7081;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;117;-989.7612,-38.75911;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RelayNode;186;890.226,-209.041;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;172;676.6741,110.5109;Float;False;3;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-1754.49,-124.248;Float;False;Constant;_Float0;Float 0;1;0;0.05;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;177;955.5683,730.2245;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;1534.555,-166.9923;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1136.836,-62.31799;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Bobo/Gravifloor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;DeferredOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;1;5;5;10;True;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;3;40;0
WireConnection;20;4;3;0
WireConnection;39;0;20;1
WireConnection;27;0;39;0
WireConnection;27;1;28;0
WireConnection;8;0;19;0
WireConnection;8;1;9;0
WireConnection;8;2;12;0
WireConnection;127;0;125;2
WireConnection;127;1;125;3
WireConnection;155;0;139;0
WireConnection;155;3;171;0
WireConnection;155;4;194;0
WireConnection;129;0;125;3
WireConnection;129;1;125;1
WireConnection;128;0;125;1
WireConnection;128;1;125;2
WireConnection;29;0;27;0
WireConnection;29;1;30;0
WireConnection;38;0;8;0
WireConnection;153;0;129;0
WireConnection;153;1;154;0
WireConnection;149;0;150;0
WireConnection;151;0;127;0
WireConnection;151;1;154;0
WireConnection;152;0;128;0
WireConnection;152;1;154;0
WireConnection;161;0;155;1
WireConnection;161;1;155;2
WireConnection;26;0;29;0
WireConnection;26;1;38;0
WireConnection;34;0;38;0
WireConnection;34;1;37;0
WireConnection;34;2;36;0
WireConnection;34;3;35;0
WireConnection;34;4;36;0
WireConnection;145;0;152;0
WireConnection;145;2;146;0
WireConnection;145;1;149;0
WireConnection;143;0;151;0
WireConnection;143;2;146;0
WireConnection;143;1;149;0
WireConnection;157;0;161;0
WireConnection;157;1;158;0
WireConnection;144;0;153;0
WireConnection;144;2;146;0
WireConnection;144;1;149;0
WireConnection;31;0;26;0
WireConnection;31;1;34;0
WireConnection;160;0;157;0
WireConnection;160;1;144;0
WireConnection;159;0;157;0
WireConnection;159;1;145;0
WireConnection;135;0;134;0
WireConnection;156;0;157;0
WireConnection;156;1;143;0
WireConnection;41;0;31;0
WireConnection;138;0;139;0
WireConnection;138;1;159;0
WireConnection;137;0;139;0
WireConnection;137;1;160;0
WireConnection;140;0;135;0
WireConnection;140;1;141;0
WireConnection;136;0;139;0
WireConnection;136;1;156;0
WireConnection;60;3;61;0
WireConnection;60;4;3;0
WireConnection;179;5;181;0
WireConnection;47;0;41;0
WireConnection;47;1;48;0
WireConnection;43;0;41;0
WireConnection;43;1;44;0
WireConnection;43;2;45;0
WireConnection;133;0;140;0
WireConnection;133;1;138;0
WireConnection;133;2;137;0
WireConnection;133;3;136;0
WireConnection;67;0;60;0
WireConnection;67;1;179;0
WireConnection;67;2;68;0
WireConnection;58;0;43;0
WireConnection;49;0;51;0
WireConnection;49;1;50;0
WireConnection;49;2;47;0
WireConnection;100;0;119;0
WireConnection;100;1;99;0
WireConnection;163;0;133;0
WireConnection;163;1;164;0
WireConnection;115;0;179;0
WireConnection;115;1;67;0
WireConnection;54;0;55;0
WireConnection;54;1;49;0
WireConnection;54;2;58;0
WireConnection;101;0;19;0
WireConnection;101;1;102;0
WireConnection;101;2;104;0
WireConnection;97;0;100;0
WireConnection;97;1;98;0
WireConnection;97;2;101;0
WireConnection;165;0;163;0
WireConnection;165;1;170;0
WireConnection;124;0;54;0
WireConnection;204;0;115;0
WireConnection;182;0;114;1
WireConnection;182;1;183;0
WireConnection;198;0;179;1
WireConnection;198;1;179;2
WireConnection;103;0;97;0
WireConnection;103;1;98;0
WireConnection;103;2;58;0
WireConnection;202;0;115;0
WireConnection;202;1;204;0
WireConnection;202;2;124;0
WireConnection;167;0;165;0
WireConnection;199;0;198;0
WireConnection;199;1;200;0
WireConnection;162;0;103;0
WireConnection;162;1;119;0
WireConnection;162;2;167;0
WireConnection;201;0;202;0
WireConnection;175;0;163;0
WireConnection;120;0;121;0
WireConnection;120;1;182;0
WireConnection;120;2;58;0
WireConnection;197;0;199;0
WireConnection;197;1;162;0
WireConnection;5;0;119;0
WireConnection;5;1;108;0
WireConnection;5;2;58;0
WireConnection;173;0;120;0
WireConnection;173;1;182;0
WireConnection;173;2;175;0
WireConnection;206;0;201;0
WireConnection;64;0;65;0
WireConnection;64;1;66;0
WireConnection;64;2;58;0
WireConnection;203;0;115;0
WireConnection;178;0;201;0
WireConnection;1;3;2;0
WireConnection;1;4;3;0
WireConnection;207;0;205;0
WireConnection;207;1;173;0
WireConnection;207;2;208;0
WireConnection;117;0;119;0
WireConnection;117;1;108;0
WireConnection;117;2;1;1
WireConnection;186;0;5;0
WireConnection;172;0;64;0
WireConnection;172;1;176;0
WireConnection;172;2;175;0
WireConnection;177;0;201;0
WireConnection;177;1;206;0
WireConnection;177;2;175;0
WireConnection;209;0;197;0
WireConnection;209;1;210;0
WireConnection;0;0;186;0
WireConnection;0;1;177;0
WireConnection;0;2;209;0
WireConnection;0;3;207;0
WireConnection;0;4;172;0
ASEEND*/
//CHKSM=5875DCF255227BF7C45E678F07A0607CF51344EE