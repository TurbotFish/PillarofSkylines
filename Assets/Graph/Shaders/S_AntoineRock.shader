// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Antoine/Rocks"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Triplanar_Tiling("Triplanar_Tiling", Float) = 5
		_Ground_Position("Ground_Position", Float) = 0
		_Ground_Falloff("Ground_Falloff", Float) = 2
		_Grass_Height("Grass_Height", Float) = 0
		_Grass_Height_Falloff("Grass_Height_Falloff", Float) = 2
		_Grass_tiling("Grass_tiling", Float) = 0
		_Grass_Density("Grass_Density", Range( -0.99 , 1)) = -0.2
		_Grass_Falloff("Grass_Falloff", Range( 0 , 5)) = 3
		_Grass_NormalBlend("Grass_NormalBlend", Range( 0 , 1)) = 1
		_Grass_Noise_Intensity_Min("Grass_Noise_Intensity_Min", Range( 0 , 1)) = 0
		_Grass_Noise_Intensity_Max("Grass_Noise_Intensity_Max", Range( 0 , 1)) = 0
		_FalloffNoise("FalloffNoise", 2D) = "white" {}
		_FalloffNoiseTiling("FalloffNoiseTiling", Float) = 0
		_Rocks_Albedo_TP("Rocks_Albedo_TP", 2D) = "white" {}
		_Rocks_Normal_TP("Rocks_Normal_TP", 2D) = "bump" {}
		_Grass_Normal("Grass_Normal", 2D) = "white" {}
		_Rock_Normal_local("Rock_Normal_local", 2D) = "bump" {}
		[Toggle]_Switch_surface("Switch_surface", Float) = 1
		[Toggle]_SwitchSurface1("SwitchSurface1", Float) = 0
		_Top_Highlight_intensity("Top_Highlight_intensity", Float) = 1
		_Top_highlight_falloff("Top_highlight_falloff", Range( 0 , 1)) = 0.5
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Add("Add", 2D) = "white" {}
		_Add_edge("Add_edge", Range( 0 , 1)) = 0.5
		_Add_Ao("Add_Ao", Range( 0 , 1)) = 0.3
		_Grass_Albedo("Grass_Albedo", 2D) = "white" {}
		_Grass_Normal_Intensity("Grass_Normal_Intensity", Float) = 0
		_TintMoss("Tint Moss", Color) = (0,0,0,0)
		_Tintrock("Tint rock", Color) = (0,0,0,0)
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

		uniform sampler2D _Rocks_Normal_TP;
		uniform float _Triplanar_Tiling;
		uniform sampler2D _Rock_Normal_local;
		uniform float4 _Rock_Normal_local_ST;
		uniform sampler2D _Grass_Normal;
		uniform float _Grass_tiling;
		uniform float _Grass_Normal_Intensity;
		uniform float _Grass_Density;
		uniform float _Switch_surface;
		uniform float _Grass_NormalBlend;
		uniform float _Grass_Falloff;
		uniform float _Grass_Noise_Intensity_Min;
		uniform float _Grass_Noise_Intensity_Max;
		uniform sampler2D _FalloffNoise;
		uniform float _FalloffNoiseTiling;
		uniform float _SwitchSurface1;
		uniform float _Grass_Height;
		uniform float _Grass_Height_Falloff;
		uniform float _Ground_Position;
		uniform float _Ground_Falloff;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform float _Top_Highlight_intensity;
		uniform sampler2D _Rocks_Albedo_TP;
		uniform float4 _Tintrock;
		uniform float _Top_highlight_falloff;
		uniform float _Add_Ao;
		uniform sampler2D _Add;
		uniform float4 _Add_ST;
		uniform float _Add_edge;
		uniform float4 _TintMoss;
		uniform sampler2D _Grass_Albedo;


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
			float3 worldTriplanarNormal105 = TriplanarNormal( _Rocks_Normal_TP, _Rocks_Normal_TP, _Rocks_Normal_TP, ase_worldPos, ase_worldNormal, 1.0, _Triplanar_Tiling, 0 );
			float3 tanTriplanarNormal105 = mul( ase_worldToTangent, worldTriplanarNormal105 );
			float2 uv_Rock_Normal_local = i.uv_texcoord * _Rock_Normal_local_ST.xy + _Rock_Normal_local_ST.zw;
			float3 tex2DNode78 = UnpackScaleNormal( tex2D( _Rock_Normal_local, uv_Rock_Normal_local ) ,1.0 );
			float3 worldTriplanarNormal151 = TriplanarNormal( _Grass_Normal, _Grass_Normal, _Grass_Normal, ase_worldPos, ase_worldNormal, 1.0, _Grass_tiling, 0 );
			float3 tanTriplanarNormal151 = mul( ase_worldToTangent, worldTriplanarNormal151 );
			float3 lerpResult162 = lerp( float3(0,0,1) , tanTriplanarNormal151 , _Grass_Normal_Intensity);
			float3 lerpResult120 = lerp( tanTriplanarNormal105 , float3(0,0,1) , _Grass_NormalBlend);
			float3 newWorldNormal1 = WorldNormalVector( i , BlendNormals( lerpResult120 , tex2DNode78 ) );
			float smoothstepResult5 = smoothstep( _Grass_Density , -1.0 , lerp(newWorldNormal1.x,-newWorldNormal1.x,_Switch_surface));
			float4 triplanar140 = TriplanarSampling( _FalloffNoise, _FalloffNoise, _FalloffNoise, ase_worldPos, ase_worldNormal, 2.0, _FalloffNoiseTiling, 0 );
			float smoothstepResult144 = smoothstep( _Grass_Noise_Intensity_Min , _Grass_Noise_Intensity_Max , triplanar140.x);
			float blendOpSrc138 = ( ( lerp(ase_worldPos.x,-ase_worldPos.x,_SwitchSurface1) + _Grass_Height ) * _Grass_Height_Falloff );
			float blendOpDest138 = smoothstepResult144;
			float lerpResult139 = lerp( 0.0 , ( ( smoothstepResult5 * _Grass_Falloff ) * smoothstepResult144 ) , ( saturate(  ( blendOpSrc138 > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpSrc138 - 0.5 ) ) * ( 1.0 - blendOpDest138 ) ) : ( 2.0 * blendOpSrc138 * blendOpDest138 ) ) )));
			float temp_output_128_0 = ( lerp(ase_worldPos.x,-ase_worldPos.x,_SwitchSurface1) + _Ground_Position );
			float smoothstepResult173 = smoothstep( 0.0 , 2.0 , -temp_output_128_0);
			float clampResult164 = clamp( ( temp_output_128_0 * _Ground_Falloff ) , 0.0 , 0.5 );
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			float lerpResult171 = lerp( ( smoothstepResult173 / 5.0 ) , clampResult164 , tex2D( _TextureSample1, uv_TextureSample1 ).r);
			float blendOpSrc131 = lerpResult171;
			float blendOpDest131 = smoothstepResult144;
			float mask_Grass76 = ( lerpResult139 + ( saturate(  ( blendOpSrc131 > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpSrc131 - 0.5 ) ) * ( 1.0 - blendOpDest131 ) ) : ( 2.0 * blendOpSrc131 * blendOpDest131 ) ) )) );
			float temp_output_8_0 = saturate( mask_Grass76 );
			float3 lerpResult83 = lerp( BlendNormals( tanTriplanarNormal105 , tex2DNode78 ) , lerpResult162 , temp_output_8_0);
			o.Normal = lerpResult83;
			float4 triplanar104 = TriplanarSampling( _Rocks_Albedo_TP, _Rocks_Albedo_TP, _Rocks_Albedo_TP, ase_worldPos, ase_worldNormal, 1.0, _Triplanar_Tiling, 0 );
			float4 temp_output_160_0 = ( triplanar104 * _Tintrock );
			float smoothstepResult109 = smoothstep( _Top_highlight_falloff , 1.0 , newWorldNormal1.y);
			float4 lerpResult113 = lerp( ( _Top_Highlight_intensity * temp_output_160_0 ) , temp_output_160_0 , saturate( smoothstepResult109 ));
			float2 uv_Add = i.uv_texcoord * _Add_ST.xy + _Add_ST.zw;
			float4 tex2DNode84 = tex2D( _Add, uv_Add );
			float4 lerpResult87 = lerp( lerpResult113 , ( lerpResult113 * _Add_Ao ) , tex2DNode84.r);
			float4 lerpResult91 = lerp( lerpResult87 , ( lerpResult87 + _Add_edge ) , tex2DNode84.g);
			float4 triplanar152 = TriplanarSampling( _Grass_Albedo, _Grass_Albedo, _Grass_Albedo, ase_worldPos, ase_worldNormal, 1.0, _Grass_tiling, 0 );
			float4 lerpResult9 = lerp( lerpResult91 , ( _TintMoss * triplanar152 ) , temp_output_8_0);
			o.Albedo = lerpResult9.rgb;
			o.Smoothness = 0.0;
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
169;129;1677;861;1322.239;1001.758;1.3;True;True
Node;AmplifyShaderEditor.RangedFloatNode;100;-1169.053,-955.4335;Float;False;Property;_Triplanar_Tiling;Triplanar_Tiling;0;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;99;-1151.372,-1057.096;Float;False;Constant;_Float4;Float 4;20;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;108;-1201.978,-1477.293;Float;True;Property;_Rocks_Normal_TP;Rocks_Normal_TP;14;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TriplanarNode;105;-841.1063,-1099.72;Float;True;Spherical;World;True;Rock_Normal_TP;_Rock_Normal_TP;bump;1;None;Mid Texture 0;_MidTexture0;white;14;None;Bot Texture 0;_BotTexture0;white;15;None;RN;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-743.744,836.8067;Float;False;Constant;_Float1;Float 1;16;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;121;-741.5444,421.6939;Float;False;Constant;_Vector0;Vector 0;15;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;122;-799.9995,575.4741;Float;False;Property;_Grass_NormalBlend;Grass_NormalBlend;8;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;123;-2991.305,2015.604;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;78;-572.8519,731.0165;Float;True;Property;_Rock_Normal_local;Rock_Normal_local;16;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;120;-415.9932,465.3402;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.NegateNode;124;-2774.913,2167.08;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;119;-225.7154,601.3589;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;126;-2895.545,2261.113;Float;False;Property;_Ground_Position;Ground_Position;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;125;-2611.167,2100.856;Float;False;Property;_SwitchSurface1;SwitchSurface1;18;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;1;-2597.615,648.5991;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;107;-1216.978,-1281.293;Float;True;Property;_Rocks_Albedo_TP;Rocks_Albedo_TP;13;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.NegateNode;45;-2471.622,937.0233;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-2298.363,2143.007;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;111;-2256.6,695.5652;Float;False;Constant;_Float3;Float 3;13;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;142;-2964.989,2790.635;Float;False;Constant;_falloff;falloff;14;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;133;-2860.528,1798.15;Float;False;Property;_Grass_Height;Grass_Height;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;176;-2168.612,2291.101;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;110;-2389.607,610.5889;Float;False;Property;_Top_highlight_falloff;Top_highlight_falloff;20;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;127;-2605.366,2302.473;Float;False;Property;_Ground_Falloff;Ground_Falloff;2;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;158;-721.4951,-697.7345;Float;False;Property;_Tintrock;Tint rock;28;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;47;-2307.621,903.0233;Float;False;Property;_Switch_surface;Switch_surface;17;0;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;4;-2361.816,1121.428;Float;False;Constant;_Float0;Float 0;0;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;-2385.816,1042.428;Float;False;Property;_Grass_Density;Grass_Density;6;0;-0.2;-0.99;1;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;104;-859.3672,-887.7216;Float;True;Spherical;World;False;Rock_Albedo_TP;_Rock_Albedo_TP;white;0;None;Mid Texture 2;_MidTexture2;white;13;None;Bot Texture 2;_BotTexture2;white;14;None;RA;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;141;-2956.105,2574.675;Float;False;Property;_FalloffNoiseTiling;FalloffNoiseTiling;12;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;-491.114,-861.2817;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;150;-3175.592,3250.631;Float;False;Property;_Grass_Noise_Intensity_Max;Grass_Noise_Intensity_Max;10;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;135;-2269.528,1498.236;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;173;-2028.121,2292.126;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;5;-2063.197,904.5993;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;140;-2709.452,2583.629;Float;True;Spherical;World;False;FalloffNoise;_FalloffNoise;white;11;Assets/Plugins/AmplifyShaderEditor/Examples/Community/2Sided/2smask.png;Mid Texture 3;_MidTexture3;white;23;None;Bot Texture 3;_BotTexture3;white;24;None;FalloffNoise;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;33;-2101.357,1045.391;Float;False;Property;_Grass_Falloff;Grass_Falloff;7;0;3;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-2077.067,2149.589;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;149;-3172.196,3023.148;Float;False;Property;_Grass_Noise_Intensity_Min;Grass_Noise_Intensity_Min;9;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;134;-2666.742,1798.49;Float;False;Property;_Grass_Height_Falloff;Grass_Height_Falloff;4;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;116;-426.7332,-1088.506;Float;False;Property;_Top_Highlight_intensity;Top_Highlight_intensity;19;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;109;-2044.159,599.5051;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-255.522,-978.6102;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT4;0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;170;-1996.524,2579.697;Float;True;Property;_TextureSample1;Texture Sample 1;21;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-2044.128,1505.844;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1851.294,904.6182;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;114;-1796.57,593.0676;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;164;-1916.673,2152.481;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;144;-2213.017,2601.009;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;175;-1835.332,2307.509;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;89;-903.1144,-164.3525;Float;False;Property;_Add_Ao;Add_Ao;24;0;0.3;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;171;-1688.575,2351.272;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-1585.734,917.5146;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;138;-1800.888,1712.409;Float;False;HardLight;True;2;0;FLOAT;0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;113;-33.26541,-919.7076;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;84;-839.3045,-62.36583;Float;True;Property;_Add;Add;22;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;139;-1536.822,1160.843;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-575.4216,-174.35;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.BlendOpsNode;131;-1613.849,2180.299;Float;False;HardLight;True;2;0;FLOAT;0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;153;-1980.225,-273.2238;Float;False;Constant;_Float13;Float 13;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;-904.4377,-255.6463;Float;False;Property;_Add_edge;Add_edge;23;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;103;-1976.782,-408.8313;Float;False;Property;_Grass_tiling;Grass_tiling;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;87;-387.2027,-175.4661;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;132;-1299.798,1181.049;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;152;-1596.277,-479.3052;Float;True;Spherical;World;False;Grass_Albedo;_Grass_Albedo;white;25;None;Mid Texture 1;_MidTexture1;white;26;None;Bot Texture 1;_BotTexture1;white;27;None;Grass_Albedo;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;151;-1584.685,-216.13;Float;True;Spherical;World;True;Grass_Normal;_Grass_Normal;white;15;None;Mid Texture 0;_MidTexture0;white;26;None;Bot Texture 0;_BotTexture0;white;27;None;Grass_Normal;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;163;-1456.928,114.6912;Float;False;Constant;_Vector1;Vector 1;28;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;159;-1465.495,-658.7345;Float;False;Property;_TintMoss;Tint Moss;27;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-199.2263,-185.5221;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;161;-1480.328,26.2912;Float;False;Property;_Grass_Normal_Intensity;Grass_Normal_Intensity;26;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;76;-1074.811,908.4071;Float;False;mask_Grass;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;77;-325.5702,-512.9861;Float;False;76;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;91;-40.45473,-180.2297;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.BlendNormalsNode;101;-218.25,739.3185;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;162;-1174.828,-90.70882;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SaturateNode;8;-54.26329,-541.5601;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-1044.495,-505.7345;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;44;351.9514,-13.41164;Float;False;Constant;_Float7;Float 7;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;9;457.2498,-304.4349;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;83;395.9421,455.4521;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;911.2405,261.3221;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Antoine/Rocks;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;105;0;108;0
WireConnection;105;3;100;0
WireConnection;105;4;99;0
WireConnection;78;5;80;0
WireConnection;120;0;105;0
WireConnection;120;1;121;0
WireConnection;120;2;122;0
WireConnection;124;0;123;1
WireConnection;119;0;120;0
WireConnection;119;1;78;0
WireConnection;125;0;123;1
WireConnection;125;1;124;0
WireConnection;1;0;119;0
WireConnection;45;0;1;1
WireConnection;128;0;125;0
WireConnection;128;1;126;0
WireConnection;176;0;128;0
WireConnection;47;0;1;1
WireConnection;47;1;45;0
WireConnection;104;0;107;0
WireConnection;104;3;100;0
WireConnection;104;4;99;0
WireConnection;160;0;104;0
WireConnection;160;1;158;0
WireConnection;135;0;125;0
WireConnection;135;1;133;0
WireConnection;173;0;176;0
WireConnection;5;0;47;0
WireConnection;5;1;6;0
WireConnection;5;2;4;0
WireConnection;140;3;141;0
WireConnection;140;4;142;0
WireConnection;130;0;128;0
WireConnection;130;1;127;0
WireConnection;109;0;1;2
WireConnection;109;1;110;0
WireConnection;109;2;111;0
WireConnection;118;0;116;0
WireConnection;118;1;160;0
WireConnection;136;0;135;0
WireConnection;136;1;134;0
WireConnection;32;0;5;0
WireConnection;32;1;33;0
WireConnection;114;0;109;0
WireConnection;164;0;130;0
WireConnection;144;0;140;1
WireConnection;144;1;149;0
WireConnection;144;2;150;0
WireConnection;175;0;173;0
WireConnection;171;0;175;0
WireConnection;171;1;164;0
WireConnection;171;2;170;1
WireConnection;143;0;32;0
WireConnection;143;1;144;0
WireConnection;138;0;136;0
WireConnection;138;1;144;0
WireConnection;113;0;118;0
WireConnection;113;1;160;0
WireConnection;113;2;114;0
WireConnection;139;1;143;0
WireConnection;139;2;138;0
WireConnection;88;0;113;0
WireConnection;88;1;89;0
WireConnection;131;0;171;0
WireConnection;131;1;144;0
WireConnection;87;0;113;0
WireConnection;87;1;88;0
WireConnection;87;2;84;1
WireConnection;132;0;139;0
WireConnection;132;1;131;0
WireConnection;152;3;103;0
WireConnection;152;4;153;0
WireConnection;151;3;103;0
WireConnection;151;4;153;0
WireConnection;90;0;87;0
WireConnection;90;1;92;0
WireConnection;76;0;132;0
WireConnection;91;0;87;0
WireConnection;91;1;90;0
WireConnection;91;2;84;2
WireConnection;101;0;105;0
WireConnection;101;1;78;0
WireConnection;162;0;163;0
WireConnection;162;1;151;0
WireConnection;162;2;161;0
WireConnection;8;0;77;0
WireConnection;155;0;159;0
WireConnection;155;1;152;0
WireConnection;9;0;91;0
WireConnection;9;1;155;0
WireConnection;9;2;8;0
WireConnection;83;0;101;0
WireConnection;83;1;162;0
WireConnection;83;2;8;0
WireConnection;0;0;9;0
WireConnection;0;1;83;0
WireConnection;0;4;44;0
ASEEND*/
//CHKSM=F323DBACEE572580A3631022FE66B3CAE6F2E329