// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Rocktest"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Triplanar_Tiling("Triplanar_Tiling", Float) = 5
		_T_grass("T_grass", 2D) = "white" {}
		_Grass_tiling("Grass_tiling", Float) = 0
		_Grass_height("Grass_height", Range( -0.99 , 1)) = -0.2
		_Grass_Falloff("Grass_Falloff", Range( 0 , 5)) = 3
		_Grass_NormalBlend("Grass_NormalBlend", Range( 0 , 1)) = 1
		_N1("N1", 2D) = "bump" {}
		_N2("N2", 2D) = "bump" {}
		[Toggle]_Switch_surface("Switch_surface", Float) = 0
		_Add("Add", 2D) = "white" {}
		_Add_edge("Add_edge", Range( 0 , 1)) = 0.5
		_Add_Ao("Add_Ao", Range( 0 , 1)) = 0.3
		_RockNormal("RockNormal", 2D) = "bump" {}
		_RockColor("RockColor", 2D) = "white" {}
		_Top_highlight_falloff("Top_highlight_falloff", Range( 0 , 1)) = 0.5
		_Top_Highlight_intensity("Top_Highlight_intensity", Float) = 1
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

		uniform sampler2D _RockNormal;
		uniform float _Triplanar_Tiling;
		uniform sampler2D _N1;
		uniform float4 _N1_ST;
		uniform sampler2D _N2;
		uniform float4 _N2_ST;
		uniform float _Grass_height;
		uniform float _Switch_surface;
		uniform float _Grass_NormalBlend;
		uniform float _Grass_Falloff;
		uniform float _Top_Highlight_intensity;
		uniform sampler2D _RockColor;
		uniform float _Top_highlight_falloff;
		uniform float _Add_Ao;
		uniform sampler2D _Add;
		uniform float4 _Add_ST;
		uniform float _Add_edge;
		uniform sampler2D _T_grass;
		uniform float4 _RockColor_ST;
		uniform float _Grass_tiling;


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
			float3 worldTriplanarNormal105 = TriplanarNormal( _RockNormal, _RockNormal, _RockNormal, ase_worldPos, ase_worldNormal, 1.0, _Triplanar_Tiling, 0 );
			float3 tanTriplanarNormal105 = mul( ase_worldToTangent, worldTriplanarNormal105 );
			float2 uv_N1 = i.uv_texcoord * _N1_ST.xy + _N1_ST.zw;
			float3 tex2DNode78 = UnpackScaleNormal( tex2D( _N1, uv_N1 ) ,1.0 );
			float2 uv_N2 = i.uv_texcoord * _N2_ST.xy + _N2_ST.zw;
			float3 lerpResult120 = lerp( tanTriplanarNormal105 , float3(0,0,1) , _Grass_NormalBlend);
			float3 newWorldNormal1 = WorldNormalVector( i , BlendNormals( lerpResult120 , tex2DNode78 ) );
			float smoothstepResult5 = smoothstep( _Grass_height , -1.0 , lerp(newWorldNormal1.x,-newWorldNormal1.x,_Switch_surface));
			float mask_Grass76 = ( smoothstepResult5 * _Grass_Falloff );
			float temp_output_8_0 = saturate( mask_Grass76 );
			float3 lerpResult83 = lerp( BlendNormals( tanTriplanarNormal105 , tex2DNode78 ) , UnpackScaleNormal( tex2D( _N2, uv_N2 ) ,1.0 ) , temp_output_8_0);
			o.Normal = lerpResult83;
			float4 triplanar104 = TriplanarSampling( _RockColor, _RockColor, _RockColor, ase_worldPos, ase_worldNormal, 1.0, _Triplanar_Tiling, 0 );
			float smoothstepResult109 = smoothstep( _Top_highlight_falloff , 1.0 , newWorldNormal1.y);
			float4 lerpResult113 = lerp( ( _Top_Highlight_intensity * triplanar104 ) , triplanar104 , saturate( smoothstepResult109 ));
			float2 uv_Add = i.uv_texcoord * _Add_ST.xy + _Add_ST.zw;
			float4 tex2DNode84 = tex2D( _Add, uv_Add );
			float4 lerpResult87 = lerp( lerpResult113 , ( lerpResult113 * _Add_Ao ) , tex2DNode84.r);
			float4 lerpResult91 = lerp( lerpResult87 , ( lerpResult87 + _Add_edge ) , tex2DNode84.g);
			float2 uv_RockColor = i.uv_texcoord * _RockColor_ST.xy + _RockColor_ST.zw;
			float4 lerpResult9 = lerp( lerpResult91 , tex2D( _T_grass, ( uv_RockColor * _Grass_tiling ) ) , temp_output_8_0);
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
7;29;1906;1004;524.9482;768.3486;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;100;-1169.053,-955.4335;Float;False;Property;_Triplanar_Tiling;Triplanar_Tiling;0;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;108;-1201.978,-1477.293;Float;True;Property;_RockNormal;RockNormal;12;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;99;-1151.372,-1057.096;Float;False;Constant;_Float4;Float 4;20;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;122;-799.9995,575.4741;Float;False;Property;_Grass_NormalBlend;Grass_NormalBlend;5;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;121;-741.5444,421.6939;Float;False;Constant;_Vector0;Vector 0;15;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-743.744,836.8067;Float;False;Constant;_Float1;Float 1;16;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;105;-841.1063,-1099.72;Float;True;Spherical;World;True;Rock_Normal;_Rock_Normal;bump;1;None;Mid Texture 0;_MidTexture0;white;14;None;Bot Texture 0;_BotTexture0;white;15;None;RN;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;120;-415.9932,465.3402;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;78;-572.8519,731.0165;Float;True;Property;_N1;N1;6;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;119;-225.7154,601.3589;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;110;-2389.607,610.5889;Float;False;Property;_Top_highlight_falloff;Top_highlight_falloff;14;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;1;-2688.46,837.5572;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;111;-2256.6,695.5652;Float;False;Constant;_Float3;Float 3;13;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;107;-1216.978,-1281.293;Float;True;Property;_RockColor;RockColor;13;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;116;-426.7332,-1088.506;Float;False;Property;_Top_Highlight_intensity;Top_Highlight_intensity;15;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;104;-859.3672,-887.7216;Float;True;Spherical;World;False;Rock_Albedo;_Rock_Albedo;white;0;None;Mid Texture 2;_MidTexture2;white;13;None;Bot Texture 2;_BotTexture2;white;14;None;RA;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;109;-2044.159,599.5051;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-255.522,-978.6102;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT4;0;False;1;FLOAT4
Node;AmplifyShaderEditor.SaturateNode;114;-1796.57,593.0676;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;45;-2471.622,937.0233;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;113;-33.26541,-919.7076;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;89;-903.1144,-164.3525;Float;False;Property;_Add_Ao;Add_Ao;11;0;0.3;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;-2385.816,1042.428;Float;False;Property;_Grass_height;Grass_height;3;0;-0.2;-0.99;1;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;47;-2307.621,903.0233;Float;False;Property;_Switch_surface;Switch_surface;8;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;4;-2361.816,1121.428;Float;False;Constant;_Float0;Float 0;0;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;5;-2063.197,904.5993;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;33;-2059.357,1030.391;Float;False;Property;_Grass_Falloff;Grass_Falloff;4;0;3;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;84;-839.3045,-62.36583;Float;True;Property;_Add;Add;9;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-575.4216,-174.35;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;92;-904.4377,-255.6463;Float;False;Property;_Add_edge;Add_edge;10;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-1248.545,-484.0876;Float;False;0;107;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;103;-1169.662,-348.3329;Float;False;Property;_Grass_tiling;Grass_tiling;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1851.294,904.6182;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;87;-387.2027,-175.4661;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;77;-325.5702,-512.9861;Float;False;76;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-199.2263,-185.5221;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-977.6616,-483.3329;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;76;-1671.343,899.2781;Float;False;mask_Grass;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;93;-786.2173,-558.8762;Float;True;Property;_T_grass;T_grass;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;8;-54.26329,-541.5601;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;91;-40.45473,-180.2297;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;79;-578.955,922.2526;Float;True;Property;_N2;N2;7;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;101;-218.25,739.3185;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;9;115.6417,-646.0431;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;83;58.40069,805.1938;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;44;213.6815,-375.3536;Float;False;Constant;_Float7;Float 7;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;715.5714,-524.9432;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Rocktest;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;105;0;108;0
WireConnection;105;3;100;0
WireConnection;105;4;99;0
WireConnection;120;0;105;0
WireConnection;120;1;121;0
WireConnection;120;2;122;0
WireConnection;78;5;80;0
WireConnection;119;0;120;0
WireConnection;119;1;78;0
WireConnection;1;0;119;0
WireConnection;104;0;107;0
WireConnection;104;3;100;0
WireConnection;104;4;99;0
WireConnection;109;0;1;2
WireConnection;109;1;110;0
WireConnection;109;2;111;0
WireConnection;118;0;116;0
WireConnection;118;1;104;0
WireConnection;114;0;109;0
WireConnection;45;0;1;1
WireConnection;113;0;118;0
WireConnection;113;1;104;0
WireConnection;113;2;114;0
WireConnection;47;0;1;1
WireConnection;47;1;45;0
WireConnection;5;0;47;0
WireConnection;5;1;6;0
WireConnection;5;2;4;0
WireConnection;88;0;113;0
WireConnection;88;1;89;0
WireConnection;32;0;5;0
WireConnection;32;1;33;0
WireConnection;87;0;113;0
WireConnection;87;1;88;0
WireConnection;87;2;84;1
WireConnection;90;0;87;0
WireConnection;90;1;92;0
WireConnection;102;0;94;0
WireConnection;102;1;103;0
WireConnection;76;0;32;0
WireConnection;93;1;102;0
WireConnection;8;0;77;0
WireConnection;91;0;87;0
WireConnection;91;1;90;0
WireConnection;91;2;84;2
WireConnection;79;5;80;0
WireConnection;101;0;105;0
WireConnection;101;1;78;0
WireConnection;9;0;91;0
WireConnection;9;1;93;0
WireConnection;9;2;8;0
WireConnection;83;0;101;0
WireConnection;83;1;79;0
WireConnection;83;2;8;0
WireConnection;0;0;9;0
WireConnection;0;1;83;0
WireConnection;0;4;44;0
ASEEND*/
//CHKSM=C0E2601E07EEB1849A368D76EBCE52141611218E