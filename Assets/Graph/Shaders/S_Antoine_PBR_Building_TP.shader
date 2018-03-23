// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Antoine/Buildings"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_GroundPosition("GroundPosition", Float) = 0
		_CracksPosition("CracksPosition", Float) = 0
		_GroundFalloff("GroundFalloff", Float) = 2
		_CracksFalloff("CracksFalloff", Float) = 2
		_FalloffNoise("FalloffNoise", 2D) = "white" {}
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
		[Toggle]_SwitchSurface("SwitchSurface", Float) = 1
		_addLerp("addLerp", Float) = 0
		_addGrass("addGrass", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
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
		uniform float _SwitchSurface;
		uniform float _GroundPosition;
		uniform float _GroundFalloff;
		uniform sampler2D _FalloffNoise;
		uniform float4 _FalloffNoise_ST;
		uniform sampler2D _Height1;
		uniform float _CracksPosition;
		uniform float _CracksFalloff;
		uniform float _addLerp;
		uniform float _ObjectNormalIntensity;
		uniform sampler2D _ObjectNormal;
		uniform float4 _ObjectNormal_ST;
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
			float3 worldTriplanarNormal169 = TriplanarNormal( _Normal1, _Normal1, _Normal1, ase_worldPos, ase_worldNormal, 2.0, _Tiling1, 0 );
			float3 tanTriplanarNormal169 = mul( ase_worldToTangent, worldTriplanarNormal169 );
			float3 worldTriplanarNormal173 = TriplanarNormal( _Normal2, _Normal2, _Normal2, ase_worldPos, ase_worldNormal, 2.0, _Tiling2, 0 );
			float3 tanTriplanarNormal173 = mul( ase_worldToTangent, worldTriplanarNormal173 );
			float2 uv_FalloffNoise = i.uv_texcoord * _FalloffNoise_ST.xy + _FalloffNoise_ST.zw;
			float4 tex2DNode156 = tex2D( _FalloffNoise, uv_FalloffNoise );
			float blendOpSrc165 = ( ( lerp(ase_worldPos.x,-ase_worldPos.x,_SwitchSurface) + _GroundPosition ) * _GroundFalloff );
			float blendOpDest165 = tex2DNode156.r;
			float4 triplanar203 = TriplanarSampling( _Height1, _Height1, _Height1, ase_worldPos, ase_worldNormal, 2.0, _Tiling1, 0 );
			float smoothstepResult216 = smoothstep( 0.5 , 0.9 , ( 1.0 - triplanar203.x ));
			float lerpResult218 = lerp( 0.0 , smoothstepResult216 , ( ( lerp(ase_worldPos.x,-ase_worldPos.x,_SwitchSurface) + _CracksPosition ) * _CracksFalloff ));
			float blendOpSrc225 = lerpResult218;
			float blendOpDest225 = tex2DNode156.r;
			float clampResult220 = clamp( ( ( saturate(  ( blendOpSrc165 > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpSrc165 - 0.5 ) ) * ( 1.0 - blendOpDest165 ) ) : ( 2.0 * blendOpSrc165 * blendOpDest165 ) ) )) + ( saturate( min( blendOpSrc225 , blendOpDest225 ) )) + _addLerp ) , 0.0 , 0.5 );
			float3 lerpResult181 = lerp( tanTriplanarNormal169 , tanTriplanarNormal173 , clampResult220);
			float2 uv_ObjectNormal = i.uv_texcoord * _ObjectNormal_ST.xy + _ObjectNormal_ST.zw;
			o.Normal = BlendNormals( lerpResult181 , UnpackScaleNormal( tex2D( _ObjectNormal, uv_ObjectNormal ) ,_ObjectNormalIntensity ) );
			float4 triplanar168 = TriplanarSampling( _Albedo1, _Albedo1, _Albedo1, ase_worldPos, ase_worldNormal, 2.0, _Tiling1, 0 );
			float4 triplanar172 = TriplanarSampling( _Albedo2, _Albedo2, _Albedo2, ase_worldPos, ase_worldNormal, 2.0, _Tiling2, 0 );
			float4 lerpResult145 = lerp( ( triplanar168 * _Tint1 ) , ( ( triplanar172 * _Tint2 ) + _addGrass ) , clampResult220);
			o.Albedo = lerpResult145.xyz;
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
1927;72;1542;953;7170.908;1356.636;1.3;True;True
Node;AmplifyShaderEditor.CommentaryNode;182;-7524.654,-1530.427;Float;False;1832.1;1376.4;Mask;22;219;218;216;208;213;215;207;217;165;154;156;206;152;153;188;205;150;190;146;220;225;230;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;176;-6997.153,-99.20592;Float;False;1288.591;712.0464;MATERIAL 1;7;168;169;203;167;171;226;227;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;146;-7308.989,-1494.612;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;171;-6934.218,218.6871;Float;False;Constant;_Falloff1;Falloff 1;14;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;190;-7239.191,-1281.114;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;167;-6925.334,2.727903;Float;False;Property;_Tiling1;Tiling 1;28;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;203;-6675.394,408.4216;Float;True;Spherical;World;False;Height1;_Height1;white;13;Assets/Plugins/AmplifyShaderEditor/Examples/Community/2Sided/2smask.png;Mid Texture 4;_MidTexture4;white;23;None;Bot Texture 4;_BotTexture4;white;24;None;Height1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;205;-7485.058,-405.4781;Float;False;Property;_CracksPosition;CracksPosition;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;188;-7027.345,-1352.537;Float;False;Property;_SwitchSurface;SwitchSurface;32;0;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;217;-6873.57,-445.971;Float;False;Constant;_Float19;Float 19;36;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;206;-7291.273,-405.1371;Float;False;Property;_CracksFalloff;CracksFalloff;3;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;207;-7065.657,-696.5912;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;215;-7083.479,-408.487;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;213;-6870.87,-370.9948;Float;False;Constant;_Float17;Float 17;36;0;0.9;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;150;-7309.645,-1021.899;Float;False;Property;_GroundPosition;GroundPosition;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;152;-6718.644,-1311.412;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;216;-6621.677,-394.9926;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-6874.486,-698.9662;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;153;-7089.86,-1011.158;Float;False;Property;_GroundFalloff;GroundFalloff;2;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;-6527.473,-1313.787;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;178;-7000.097,652.1028;Float;False;1313.85;569.1168;MATERIAL 2;8;172;173;175;174;228;229;232;233;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;218;-6515.265,-725.5457;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;156;-6653.876,-1037.22;Float;True;Property;_FalloffNoise;FalloffNoise;4;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;165;-6257.55,-1083.764;Float;False;HardLight;True;2;0;FLOAT;0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;230;-6159.572,-492.4158;Float;False;Property;_addLerp;addLerp;41;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;175;-6916.814,923.2588;Float;False;Constant;_Falloff2;Falloff 2;14;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;174;-6914.47,692.312;Float;False;Property;_Tiling2;Tiling 2;26;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;225;-6207.848,-699.567;Float;False;Darken;True;2;0;FLOAT;0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;172;-6637.805,714.0914;Float;True;Spherical;World;False;Albedo2;_Albedo2;white;15;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;Albedo2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;219;-5930.3,-916.6219;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;180;-5550.691,329.4026;Float;False;862.168;403.9806;Normal bus;4;224;181;223;222;;1,0.625,0.625,1;0;0
Node;AmplifyShaderEditor.ColorNode;229;-6266.069,868.8919;Float;False;Property;_Tint2;Tint2;14;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;232;-5955.669,1128.584;Float;False;Property;_addGrass;addGrass;42;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;226;-6275.98,133.8443;Float;False;Property;_Tint1;Tint1;10;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;-5993.377,772.5299;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;173;-6637.07,913.8245;Float;True;Spherical;World;True;Normal2;_Normal2;bump;16;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;Normal2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;222;-5522.226,591.1853;Float;False;Property;_ObjectNormalIntensity;ObjectNormalIntensity;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;220;-5874.743,-606.7418;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;168;-6678.681,11.68195;Float;True;Spherical;World;False;Albedo1;_Albedo1;white;11;Assets/Plugins/AmplifyShaderEditor/Examples/Community/2Sided/2smask.png;Mid Texture 3;_MidTexture3;white;23;None;Bot Texture 3;_BotTexture3;white;24;None;Albedo1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;169;-6677.946,213.1649;Float;True;Spherical;World;True;Normal1;_Normal1;bump;12;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;Normal1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;57;-2848.219,1110.142;Float;False;381;720.9998;Green;3;135;136;137;Material_2;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;181;-5521.879,385.1062;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.CommentaryNode;58;-2839.75,248.3901;Float;False;380.051;742.651;Red;3;133;134;131;Material_1;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;233;-5817.669,841.5842;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;223;-5246.698,534.7845;Float;True;Property;_ObjectNormal;ObjectNormal;7;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;179;-5555.134,-13.13421;Float;False;333.6433;285.563;Albedo bus;1;145;;1,0.625,0.625,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;56;-2843.17,2012.426;Float;False;381;721.0001;Blue;3;138;139;140;Material_3;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;227;-5957.288,18.48223;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;79;143.4044,80.64252;Float;False;Constant;_Float0;Float 0;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;28;-2065.466,-816.8553;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-1847.413,2245.603;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;45;-2922.283,-760.2347;Float;False;44;0;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;26;-2840.265,-1193.855;Float;False;Constant;_Color0;Color 0;5;0;0.2156863,0.282353,0.4196079,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;136;-2813.204,1602.932;Float;True;Spherical;World;False;RME2;_RME2;white;36;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;rme2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;64;-2202.644,2467.847;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;61;-2187.519,2241.581;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.TriplanarNode;140;-2818.438,2292.148;Float;True;Spherical;World;True;N3;_N3;bump;39;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;n3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-3451.781,-962.29;Float;False;Constant;_Float4;Float 4;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1776.269,474.3281;Float;False;66;0;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;124;-3550.697,630.8495;Float;False;Property;_normal_intensity;normal_intensity;24;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;133;-2817.815,500.051;Float;True;Spherical;World;True;N1;_N1;bump;30;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;n1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-1262.344,344.9939;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;84;-3505.25,1335.453;Float;False;Property;_Tiling_2;Tiling_2;33;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;47;-2753.316,2745.213;Float;True;Property;_MatMask;MatMask;18;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;193;-5603.508,1372.114;Float;False;Property;_GlobalSmoothness;GlobalSmoothness;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;23;-4092.681,-1279.79;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;131;-2818.55,298.5682;Float;True;Spherical;World;False;C1;_C1;white;29;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;c1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;78;-1964.932,1221.943;Float;False;Property;_Edge_color;Edge_color;22;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-3456.982,-1154.69;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-92.70422,258.2063;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1711.571,992.7242;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;20;-2813.565,-1008.955;Float;False;Constant;_Float6;Float 6;5;0;1.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;8;-4316.181,-1326.29;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;30;-4158.896,-972.6129;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;77;-1285.273,977.2715;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-1536.513,1099.718;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;73;-1352.58,458.9671;Float;False;Property;_Emissive;Emissive;9;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;134;-2806.584,720.4589;Float;True;Spherical;World;False;RME1;_RME1;white;31;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;rme1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;10;-3636.381,-1187.19;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;83;-3485.962,367.9121;Float;False;Property;_Tiling_1;Tiling_1;27;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;135;-2823.191,1183.02;Float;True;Spherical;World;False;C2;_C2;white;34;None;Mid Texture 1;_MidTexture1;white;23;None;Bot Texture 1;_BotTexture1;white;24;None;c2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-1810.213,2101.192;Float;False;Albedo;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.PowerNode;37;-3654.416,-873.7239;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;60;-2013.376,2089.159;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;11;-3824.881,-1098.79;Float;False;Constant;_Float1;Float 1;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;87;-3485.047,2113.123;Float;False;Property;_Tiling_3;Tiling_3;37;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2550.965,-1083.055;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;40;-2345.035,-902.2158;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ToggleSwitchNode;27;-1851.062,-719.4846;Float;False;Property;_Highlight_platform;Highlight_platform;23;0;1;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-2558.699,-958.1456;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.BreakToComponentsNode;71;-1576.677,477.0505;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;69;-501.1378,436.6901;Float;False;63;0;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;62;-2033.215,2242.74;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;29;-2435.473,-661.6988;Float;False;Property;_Highlight_value;Highlight_value;25;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;65;-2043.664,2465.889;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;17;-2356.603,-1033.983;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.BlendNormalsNode;91;-171.6362,614.7443;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;76;-2019.939,1013.368;Float;False;Property;_Ao;Ao;21;0;0.2;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-3632.481,-1032.49;Float;False;Constant;_Float2;Float 2;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;192;-5588.478,1156.41;Float;False;Property;_GlobalMetallic;GlobalMetallic;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;32;-4578.008,-740.7599;Float;False;Constant;_Float5;Float 5;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;137;-2824.435,1382.524;Float;True;Spherical;World;True;N2;_N2;bump;35;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;n2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;138;-2819.174,2090.666;Float;True;Spherical;World;False;C3;_C3;white;38;None;Mid Texture 2;_MidTexture2;white;23;None;Bot Texture 2;_BotTexture2;white;24;None;c3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;-755.1174,692.275;Float;False;Constant;_Float13;Float 13;21;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;59;-2179.172,2089.921;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;22;-3900.281,-1255.09;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;14;-3294.482,-1157.29;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;75;-1541.29,962.881;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;34;-3842.916,-785.324;Float;False;Constant;_Float8;Float 8;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;46;-1754.117,240.5636;Float;True;Property;_AddMask;AddMask;19;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;39;-3312.518,-843.8239;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;132;-3944.316,1135.939;Float;False;Constant;_Float10;Float 10;14;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;15;-3453.082,-1038.99;Float;False;Constant;_Float3;Float 3;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;35;-3918.317,-941.624;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;33;-3471.118,-725.5237;Float;False;Constant;_Float7;Float 7;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;5;103.0172,250.43;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;139;-2808.492,2511.271;Float;True;Spherical;World;False;RME3;_RME3;white;40;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;rme3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-4758.705,-810.9601;Float;False;Constant;_Float9;Float 9;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-200.214,389.9137;Float;False;Property;_Roughness_add;Roughness_add;20;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1120.736,341.8653;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;224;-4944.812,396.9128;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;145;-5518.965,28.74757;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-1857.862,2468.752;Float;False;RME;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3475.018,-841.2239;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;90;-539.907,649.7944;Float;True;Property;_Normal;Normal;17;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-4303.112,428.7882;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Antoine/Buildings;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;190;0;146;1
WireConnection;203;3;167;0
WireConnection;203;4;171;0
WireConnection;188;0;146;1
WireConnection;188;1;190;0
WireConnection;207;0;188;0
WireConnection;207;1;205;0
WireConnection;215;0;203;1
WireConnection;152;0;188;0
WireConnection;152;1;150;0
WireConnection;216;0;215;0
WireConnection;216;1;217;0
WireConnection;216;2;213;0
WireConnection;208;0;207;0
WireConnection;208;1;206;0
WireConnection;154;0;152;0
WireConnection;154;1;153;0
WireConnection;218;1;216;0
WireConnection;218;2;208;0
WireConnection;165;0;154;0
WireConnection;165;1;156;1
WireConnection;225;0;218;0
WireConnection;225;1;156;1
WireConnection;172;3;174;0
WireConnection;172;4;175;0
WireConnection;219;0;165;0
WireConnection;219;1;225;0
WireConnection;219;2;230;0
WireConnection;228;0;172;0
WireConnection;228;1;229;0
WireConnection;173;3;174;0
WireConnection;173;4;175;0
WireConnection;220;0;219;0
WireConnection;168;3;167;0
WireConnection;168;4;171;0
WireConnection;169;3;167;0
WireConnection;169;4;171;0
WireConnection;181;0;169;0
WireConnection;181;1;173;0
WireConnection;181;2;220;0
WireConnection;233;0;228;0
WireConnection;233;1;232;0
WireConnection;223;5;222;0
WireConnection;227;0;168;0
WireConnection;227;1;226;0
WireConnection;28;0;45;0
WireConnection;28;1;40;0
WireConnection;28;2;29;0
WireConnection;63;0;62;0
WireConnection;136;3;84;0
WireConnection;136;4;132;0
WireConnection;64;0;136;0
WireConnection;64;1;134;0
WireConnection;64;2;47;1
WireConnection;61;0;137;0
WireConnection;61;1;133;0
WireConnection;61;2;47;1
WireConnection;140;3;87;0
WireConnection;140;4;132;0
WireConnection;133;3;83;0
WireConnection;133;4;132;0
WireConnection;80;0;46;3
WireConnection;23;0;8;3
WireConnection;131;3;83;0
WireConnection;131;4;132;0
WireConnection;12;0;10;0
WireConnection;12;1;13;0
WireConnection;6;0;71;0
WireConnection;6;1;7;0
WireConnection;74;0;60;0
WireConnection;74;1;76;0
WireConnection;77;0;75;0
WireConnection;77;1;130;0
WireConnection;77;2;46;1
WireConnection;130;0;135;0
WireConnection;130;1;78;0
WireConnection;134;3;83;0
WireConnection;134;4;132;0
WireConnection;10;0;22;0
WireConnection;10;1;11;0
WireConnection;135;3;84;0
WireConnection;135;4;132;0
WireConnection;44;0;77;0
WireConnection;37;0;35;0
WireConnection;37;1;34;0
WireConnection;60;0;59;0
WireConnection;60;1;138;0
WireConnection;60;2;47;2
WireConnection;18;0;45;0
WireConnection;18;1;26;0
WireConnection;40;0;17;0
WireConnection;40;1;21;0
WireConnection;40;2;39;0
WireConnection;27;0;45;0
WireConnection;27;1;28;0
WireConnection;21;0;45;0
WireConnection;21;1;20;0
WireConnection;71;0;70;0
WireConnection;62;0;61;0
WireConnection;62;1;140;0
WireConnection;62;2;47;2
WireConnection;65;0;64;0
WireConnection;65;1;139;0
WireConnection;65;2;47;2
WireConnection;17;0;45;0
WireConnection;17;1;18;0
WireConnection;17;2;14;0
WireConnection;91;0;69;0
WireConnection;91;1;90;0
WireConnection;137;3;84;0
WireConnection;137;4;132;0
WireConnection;138;3;87;0
WireConnection;138;4;132;0
WireConnection;59;0;135;0
WireConnection;59;1;131;0
WireConnection;59;2;47;1
WireConnection;22;0;23;0
WireConnection;22;1;15;0
WireConnection;22;2;16;0
WireConnection;14;0;12;0
WireConnection;14;1;15;0
WireConnection;14;2;16;0
WireConnection;75;0;60;0
WireConnection;75;1;74;0
WireConnection;75;2;46;2
WireConnection;39;0;38;0
WireConnection;39;1;33;0
WireConnection;39;2;32;0
WireConnection;35;0;30;2
WireConnection;35;1;33;0
WireConnection;35;2;32;0
WireConnection;5;0;6;0
WireConnection;139;3;87;0
WireConnection;139;4;132;0
WireConnection;72;0;80;0
WireConnection;72;1;73;0
WireConnection;224;0;181;0
WireConnection;224;1;223;0
WireConnection;145;0;227;0
WireConnection;145;1;233;0
WireConnection;145;2;220;0
WireConnection;66;0;65;0
WireConnection;38;0;37;0
WireConnection;38;1;36;0
WireConnection;90;5;92;0
WireConnection;0;0;145;0
WireConnection;0;1;224;0
WireConnection;0;3;192;0
WireConnection;0;4;193;0
ASEEND*/
//CHKSM=6F113DF9E846BFDB41EF6DF4A249E9B64B5D29DC