// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Antoine/Buildings_Objspace"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_CracksFalloff("CracksFalloff", Float) = 2
		_GlobalSmoothness("GlobalSmoothness", Float) = 0
		_GlobalMetallic("GlobalMetallic", Float) = 0
		_ObjectNormal("ObjectNormal", 2D) = "bump" {}
		_ObjectNormalIntensity("ObjectNormalIntensity", Float) = 1
		_Tint1("Tint1", Color) = (0,0,0,0)
		_Albedo1("Albedo1", 2D) = "white" {}
		_Normal1("Normal1", 2D) = "bump" {}
		_Height1("Height1", 2D) = "white" {}
		_Tint2("Tint2", Color) = (0,0,0,0)
		_Albedo2("Albedo2", 2D) = "white" {}
		_Normal2("Normal2", 2D) = "bump" {}
		_Tiling2("Tiling 2", Float) = 0
		_Tiling1("Tiling 1", Float) = 0
		_addGrass("addGrass", Float) = 0
		_Float2("Float 2", Range( 0 , 1)) = 0
		_Noise("Noise", 2D) = "white" {}
		_TRANSITION("TRANSITION", Float) = 0
		_TRANSITION_FALLOFF("TRANSITION_FALLOFF", Float) = 0.2
		[Toggle]_UseTransition("UseTransition", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
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

		uniform sampler2D _Normal1;
		uniform float _Tiling1;
		uniform sampler2D _Normal2;
		uniform float _Tiling2;
		uniform sampler2D _Height1;
		uniform float _CracksFalloff;
		uniform sampler2D _Noise;
		uniform float _TRANSITION;
		uniform float _TRANSITION_FALLOFF;
		uniform float _Float2;
		uniform float _ObjectNormalIntensity;
		uniform sampler2D _ObjectNormal;
		uniform float4 _ObjectNormal_ST;
		uniform float _UseTransition;
		uniform sampler2D _Albedo1;
		uniform float4 _Tint1;
		uniform sampler2D _Albedo2;
		uniform float4 _Tint2;
		uniform float _addGrass;
		uniform float _GlobalMetallic;
		uniform float _GlobalSmoothness;


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
			float3 localTangent = mul( unity_WorldToObject, float4( ase_worldTangent, 0 ) );
			float3 localBitangent = mul( unity_WorldToObject, float4( ase_worldBitangent, 0 ) );
			float3 localNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float3x3 objectToTangent = float3x3(localTangent, localBitangent, localNormal);
			float3 localPos = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) );
			float3 worldTriplanarNormal169 = TriplanarNormal( _Normal1, _Normal1, _Normal1, localPos, localNormal, 2.0, _Tiling1, 0 );
			float3 tanTriplanarNormal169 = mul( objectToTangent, worldTriplanarNormal169 );
			float3 worldTriplanarNormal173 = TriplanarNormal( _Normal2, _Normal2, _Normal2, localPos, localNormal, 2.0, _Tiling2, 0 );
			float3 tanTriplanarNormal173 = mul( objectToTangent, worldTriplanarNormal173 );
			float4 triplanar203 = TriplanarSampling( _Height1, _Height1, _Height1, localPos, localNormal, 2.0, _Tiling1, 0 );
			float smoothstepResult216 = smoothstep( 0.5 , 0.9 , ( 1.0 - triplanar203.x ));
			float lerpResult218 = lerp( 0.0 , smoothstepResult216 , _CracksFalloff);
			float4 triplanar256 = TriplanarSampling( _Noise, _Noise, _Noise, localPos, localNormal, 1.0, 0.2, 0 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float blendOpSrc251 = triplanar256.x;
			float blendOpDest251 = saturate( ( ( ase_vertex3Pos.x + _TRANSITION ) * _TRANSITION_FALLOFF ) );
			float temp_output_251_0 = ( saturate( ( 1.0 - ( ( 1.0 - blendOpDest251) / blendOpSrc251) ) ));
			float blendOpSrc225 = lerpResult218;
			float blendOpDest225 = ( 1.0 - temp_output_251_0 );
			float temp_output_248_0 = saturate( step( ( 1.0 - ( saturate( min( blendOpSrc225 , blendOpDest225 ) )) ) , _Float2 ) );
			float3 lerpResult181 = lerp( tanTriplanarNormal169 , tanTriplanarNormal173 , temp_output_248_0);
			float2 uv_ObjectNormal = i.uv_texcoord * _ObjectNormal_ST.xy + _ObjectNormal_ST.zw;
			float3 tex2DNode223 = UnpackScaleNormal( tex2D( _ObjectNormal, uv_ObjectNormal ) ,_ObjectNormalIntensity );
			float3 appendResult270 = (float3(( lerpResult181.x + tex2DNode223.x ) , ( lerpResult181.y + tex2DNode223.y ) , tex2DNode223.b));
			o.Normal = appendResult270;
			float4 triplanar168 = TriplanarSampling( _Albedo1, _Albedo1, _Albedo1, localPos, localNormal, 2.0, _Tiling1, 0 );
			float4 triplanar172 = TriplanarSampling( _Albedo2, _Albedo2, _Albedo2, localPos, localNormal, 2.0, _Tiling2, 0 );
			float4 lerpResult145 = lerp( ( triplanar168 * _Tint1 ) , ( ( triplanar172 * _Tint2 ) + _addGrass ) , temp_output_248_0);
			float4 lerpResult271 = lerp( ( lerpResult145 * float4(0.8,0.7529412,0.6196079,1) ) , lerpResult145 , temp_output_248_0);
			float4 lerpResult235 = lerp( lerpResult271 , lerpResult145 , temp_output_251_0);
			o.Albedo = lerp(lerpResult145,lerpResult235,_UseTransition).xyz;
			o.Metallic = _GlobalMetallic;
			o.Smoothness = _GlobalSmoothness;
			o.Alpha = 1;
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
-1913;29;1906;1004;6865.628;1065.234;1.880399;True;True
Node;AmplifyShaderEditor.CommentaryNode;176;-6997.153,-99.20592;Float;False;1288.591;712.0464;MATERIAL 1;7;168;169;203;167;171;226;227;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;239;-4758.651,-7.292558;Float;False;Property;_TRANSITION;TRANSITION;18;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;242;-4746.646,-179.5276;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;238;-4531.913,-73.68996;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;182;-7524.654,-1530.427;Float;False;1832.1;1376.4;Mask;11;218;216;213;215;217;206;225;230;256;257;258;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-6934.218,218.6871;Float;False;Constant;_Falloff1;Falloff 1;14;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;167;-6925.334,2.727903;Float;False;Property;_Tiling1;Tiling 1;13;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;260;-4500.772,36.8949;Float;False;Property;_TRANSITION_FALLOFF;TRANSITION_FALLOFF;19;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;203;-6675.394,408.4216;Float;True;Spherical;Object;False;Height1;_Height1;white;8;Assets/Plugins/AmplifyShaderEditor/Examples/Community/2Sided/2smask.png;Mid Texture 4;_MidTexture4;white;23;None;Bot Texture 4;_BotTexture4;white;24;None;Height1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;-4395.772,-79.1051;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;258;-6812.382,-1257.924;Float;False;Constant;_Float4;Float 4;20;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;257;-6808.676,-1181.335;Float;False;Constant;_Float3;Float 3;20;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;237;-4261.929,-80.08643;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;256;-6593.733,-1264.1;Float;True;Spherical;Object;False;Noise;_Noise;white;17;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;Mid Texture 1;_MidTexture1;white;20;None;Bot Texture 1;_BotTexture1;white;21;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;215;-7083.479,-408.487;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;217;-6873.57,-445.971;Float;False;Constant;_Float19;Float 19;36;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;213;-6870.87,-370.9948;Float;False;Constant;_Float17;Float 17;36;0;0.9;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;251;-4268.508,-652.4492;Float;True;ColorBurn;True;2;0;FLOAT;0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;206;-7061.615,-522.0535;Float;False;Property;_CracksFalloff;CracksFalloff;0;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;216;-6621.677,-394.9926;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;265;-6112.22,-892.3804;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;218;-6515.265,-725.5457;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;178;-7000.097,652.1028;Float;False;1313.85;569.1168;MATERIAL 2;8;172;173;175;174;228;229;232;233;;1,1,1,1;0;0
Node;AmplifyShaderEditor.BlendOpsNode;225;-6207.848,-699.567;Float;False;Darken;True;2;0;FLOAT;0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;175;-6916.814,923.2588;Float;False;Constant;_Falloff2;Falloff 2;14;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;174;-6914.47,692.312;Float;False;Property;_Tiling2;Tiling 2;12;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;245;-5738.922,-382.8235;Float;False;Property;_Float2;Float 2;16;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;229;-6266.069,868.8919;Float;False;Property;_Tint2;Tint2;9;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;246;-5507.837,-689.7139;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;172;-6637.805,714.0914;Float;True;Spherical;Object;False;Albedo2;_Albedo2;white;10;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;Albedo2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;-5993.377,772.5299;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;232;-5955.669,1128.584;Float;False;Property;_addGrass;addGrass;15;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;226;-6275.98,133.8443;Float;False;Property;_Tint1;Tint1;5;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StepOpNode;247;-5536.577,-599.8433;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;168;-6678.681,11.68195;Float;True;Spherical;Object;False;Albedo1;_Albedo1;white;6;Assets/Plugins/AmplifyShaderEditor/Examples/Community/2Sided/2smask.png;Mid Texture 3;_MidTexture3;white;23;None;Bot Texture 3;_BotTexture3;white;24;None;Albedo1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;180;-5550.691,329.4026;Float;False;862.168;403.9806;Normal bus;4;181;223;222;268;;1,0.625,0.625,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;227;-5957.288,18.48223;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SaturateNode;248;-5406.837,-567.7139;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;179;-5555.134,-13.13421;Float;False;333.6433;285.563;Albedo bus;1;145;;1,0.625,0.625,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;233;-5817.669,841.5842;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;173;-6637.07,913.8245;Float;True;Spherical;Object;True;Normal2;_Normal2;bump;11;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;Normal2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;169;-6677.946,213.1649;Float;True;Spherical;Object;True;Normal1;_Normal1;bump;7;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;Normal1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;222;-5522.226,591.1853;Float;False;Property;_ObjectNormalIntensity;ObjectNormalIntensity;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;145;-5518.965,28.74757;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;243;-4883.721,-463.357;Float;False;Constant;_Color0;Color 0;22;0;0.8,0.7529412,0.6196079,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;223;-5246.698,534.7845;Float;True;Property;_ObjectNormal;ObjectNormal;3;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;181;-5521.879,385.1062;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-4580.079,-377.6595;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.BreakToComponentsNode;267;-4957.602,328.4893;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;268;-4928.689,465.8371;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;271;-4402.307,-345.0416;Float;False;3;0;FLOAT4;0.0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;269;-4653.996,450.3468;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;266;-4645.733,347.078;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;235;-4053.139,-197.8177;Float;False;3;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;193;-5603.508,1372.114;Float;False;Property;_GlobalSmoothness;GlobalSmoothness;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;192;-5588.478,1156.41;Float;False;Property;_GlobalMetallic;GlobalMetallic;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;230;-6159.572,-492.4158;Float;False;Property;_addLerp;addLerp;14;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;270;-4490.829,404.9085;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.ToggleSwitchNode;250;-4649.139,195.2609;Float;False;Property;_UseTransition;UseTransition;20;0;1;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-4303.112,428.7882;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Antoine/Buildings_Objspace;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;238;0;242;1
WireConnection;238;1;239;0
WireConnection;203;3;167;0
WireConnection;203;4;171;0
WireConnection;259;0;238;0
WireConnection;259;1;260;0
WireConnection;237;0;259;0
WireConnection;256;3;258;0
WireConnection;256;4;257;0
WireConnection;215;0;203;1
WireConnection;251;0;256;1
WireConnection;251;1;237;0
WireConnection;216;0;215;0
WireConnection;216;1;217;0
WireConnection;216;2;213;0
WireConnection;265;0;251;0
WireConnection;218;1;216;0
WireConnection;218;2;206;0
WireConnection;225;0;218;0
WireConnection;225;1;265;0
WireConnection;246;0;225;0
WireConnection;172;3;174;0
WireConnection;172;4;175;0
WireConnection;228;0;172;0
WireConnection;228;1;229;0
WireConnection;247;0;246;0
WireConnection;247;1;245;0
WireConnection;168;3;167;0
WireConnection;168;4;171;0
WireConnection;227;0;168;0
WireConnection;227;1;226;0
WireConnection;248;0;247;0
WireConnection;233;0;228;0
WireConnection;233;1;232;0
WireConnection;173;3;174;0
WireConnection;173;4;175;0
WireConnection;169;3;167;0
WireConnection;169;4;171;0
WireConnection;145;0;227;0
WireConnection;145;1;233;0
WireConnection;145;2;248;0
WireConnection;223;5;222;0
WireConnection;181;0;169;0
WireConnection;181;1;173;0
WireConnection;181;2;248;0
WireConnection;244;0;145;0
WireConnection;244;1;243;0
WireConnection;267;0;181;0
WireConnection;268;0;223;0
WireConnection;271;0;244;0
WireConnection;271;1;145;0
WireConnection;271;2;248;0
WireConnection;269;0;267;1
WireConnection;269;1;268;1
WireConnection;266;0;267;0
WireConnection;266;1;268;0
WireConnection;235;0;271;0
WireConnection;235;1;145;0
WireConnection;235;2;251;0
WireConnection;270;0;266;0
WireConnection;270;1;269;0
WireConnection;270;2;223;3
WireConnection;250;0;145;0
WireConnection;250;1;235;0
WireConnection;0;0;250;0
WireConnection;0;1;270;0
WireConnection;0;3;192;0
WireConnection;0;4;193;0
ASEEND*/
//CHKSM=C27D510F219A3A174B50B5FDB5430FD22B2E821D