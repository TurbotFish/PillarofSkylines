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
		_Cellfect("Cellfect", Range( 0 , 1)) = 0.1
		_Mask("Mask", 2D) = "white" {}
		[Toggle]_Toggle_Wetness("Toggle_Wetness", Float) = 0
		_Grass_Hue("Grass_Hue", Range( 0 , 1)) = 0
		_Normal_intensity("Normal_intensity", Float) = 1
		[Toggle]_East("East", Float) = 1
		_Override_Normals("Override_Normals", Range( 0 , 1)) = 0
		_Tpnormal("Tpnormal", 2D) = "white" {}
		_BlendEast("BlendEast", Int) = 0
		_Tint("Tint", Color) = (1,1,1,0)
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
		uniform float _Normal_intensity;
		uniform sampler2D _Tpnormal;
		uniform float _Override_Normals;
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
		uniform float _Grass_Hue;
		uniform float _Cellfect;
		uniform float _East;
		uniform float _Toggle_Wetness;
		uniform sampler2D _Mask;
		uniform float4 _Tint;
		uniform int _BlendEast;


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


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, clamp( p - K.xxx, 0.0, 1.0 ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 _Overrided_Normals = float3(0,1,0);
			float3 lerpResult270 = lerp( _Overrided_Normals , -_Overrided_Normals , (float)_BlendEast);
			float4 lerpResult241 = lerp( float4( ase_vertexNormal , 0.0 ) , ( UNITY_MATRIX_M[0] * float4( lerpResult270 , 0.0 ) ) , _Override_Normals);
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
			float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * tangentSign;
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 worldTriplanarNormal105 = TriplanarNormal( _RockNormal, _RockNormal, _RockNormal, ase_worldPos, ase_worldNormal, 5.0, _Triplanar_Tiling, 1 );
			float3 tanTriplanarNormal105 = mul( ase_worldToTangent, worldTriplanarNormal105 );
			float3 lerpResult120 = lerp( tanTriplanarNormal105 , float3(0,0,1) , _Grass_NormalBlend);
			float4 uv_N1 = float4(v.texcoord * _N1_ST.xy + _N1_ST.zw, 0 ,0);
			float3 tex2DNode78 = UnpackScaleNormal( tex2Dlod( _N1, uv_N1 ) ,1.0 );
			float3x3 tangentToWorld = CreateTangentToWorldPerVertex( ase_worldNormal, ase_worldTangent, v.tangent.w );
			float3 tangentNormal1 = BlendNormals( lerpResult120 , tex2DNode78 );
			float3 modWorldNormal1 = (tangentToWorld[0] * tangentNormal1.x + tangentToWorld[1] * tangentNormal1.y + tangentToWorld[2] * tangentNormal1.z);
			float smoothstepResult5 = smoothstep( _Grass_height , -1.0 , lerp(modWorldNormal1.x,-modWorldNormal1.x,_Switch_surface));
			float mask_Grass76 = ( smoothstepResult5 * _Grass_Falloff );
			float temp_output_8_0 = saturate( mask_Grass76 );
			float4 lerpResult262 = lerp( float4( ase_vertexNormal , 0.0 ) , lerpResult241 , temp_output_8_0);
			v.normal = lerpResult262.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal105 = TriplanarNormal( _RockNormal, _RockNormal, _RockNormal, ase_worldPos, ase_worldNormal, 5.0, _Triplanar_Tiling, 0 );
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
			float3 lerpResult193 = lerp( float3(0,0,1) , lerpResult83 , _Normal_intensity);
			float4 triplanar244 = TriplanarSampling( _Tpnormal, _Tpnormal, _Tpnormal, ase_worldPos, ase_worldNormal, 3.0, 0.0015, 0 );
			float4 lerpResult261 = lerp( float4( lerpResult193 , 0.0 ) , ( float4( float3(0,0,1) , 0.0 ) * ( triplanar244 * 2.0 ) ) , _Override_Normals);
			o.Normal = lerpResult261.xyz;
			float4 temp_cast_3 = (0.0).xxxx;
			float4 triplanar104 = TriplanarSampling( _RockColor, _RockColor, _RockColor, ase_worldPos, ase_worldNormal, 5.0, _Triplanar_Tiling, 0 );
			float smoothstepResult109 = smoothstep( _Top_highlight_falloff , 1.0 , newWorldNormal1.y);
			float4 lerpResult113 = lerp( ( _Top_Highlight_intensity * triplanar104 ) , triplanar104 , saturate( smoothstepResult109 ));
			float2 uv_Add = i.uv_texcoord * _Add_ST.xy + _Add_ST.zw;
			float4 tex2DNode84 = tex2D( _Add, uv_Add );
			float4 lerpResult87 = lerp( lerpResult113 , ( lerpResult113 * _Add_Ao ) , tex2DNode84.r);
			float4 lerpResult91 = lerp( lerpResult87 , ( lerpResult87 + _Add_edge ) , tex2DNode84.g);
			float2 uv_RockColor = i.uv_texcoord * _RockColor_ST.xy + _RockColor_ST.zw;
			float3 hsvTorgb190 = RGBToHSV( tex2D( _T_grass, ( uv_RockColor * _Grass_tiling ) ).rgb );
			float3 hsvTorgb189 = HSVToRGB( float3(( hsvTorgb190.x + _Grass_Hue ),hsvTorgb190.y,hsvTorgb190.z) );
			float4 lerpResult9 = lerp( lerpResult91 , float4( hsvTorgb189 , 0.0 ) , temp_output_8_0);
			float4 lerpResult176 = lerp( temp_cast_3 , lerpResult9 , ( 1.0 - _Cellfect ));
			float4 lerpResult202 = lerp( lerpResult176 , ( lerpResult176 * 0.3 ) , saturate( ( lerp(( -ase_worldPos.x + -110.0 ),( ase_worldPos.x + -110.0 ),_East) * 0.05 ) ));
			float4 triplanar223 = TriplanarSampling( _Mask, _Mask, _Mask, ase_worldPos, ase_worldNormal, 2.0, 0.01, 0 );
			float temp_output_230_0 = saturate( lerp(0.0,triplanar223.y,_Toggle_Wetness) );
			float4 lerpResult231 = lerp( lerpResult202 , ( lerpResult202 * 0.5 ) , temp_output_230_0);
			o.Albedo = ( lerpResult231 * _Tint ).xyz;
			float4 temp_cast_8 = (0.0).xxxx;
			float4 lerpResult177 = lerp( temp_cast_8 , lerpResult9 , _Cellfect);
			float4 temp_output_181_0 = ( lerpResult177 * 1.2 );
			o.Emission = temp_output_181_0.xyz;
			float lerpResult228 = lerp( 0.0 , 0.8 , temp_output_230_0);
			o.Smoothness = lerpResult228;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:forward vertex:vertexDataFunc 

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
-1913;29;1906;1004;-1250.646;1311.862;1.493807;True;True
Node;AmplifyShaderEditor.TexturePropertyNode;108;-1201.978,-1477.293;Float;True;Property;_RockNormal;RockNormal;12;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;100;-1190.353,-928.6334;Float;False;Property;_Triplanar_Tiling;Triplanar_Tiling;0;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;99;-1183.372,-1057.096;Float;False;Constant;_Float4;Float 4;20;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-743.744,836.8067;Float;False;Constant;_Float1;Float 1;16;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;105;-841.1063,-1099.72;Float;True;Spherical;World;True;Rock_Normal;_Rock_Normal;bump;1;None;Mid Texture 0;_MidTexture0;white;14;None;Bot Texture 0;_BotTexture0;white;15;None;RN;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;121;-741.5444,421.6939;Float;False;Constant;_Vector0;Vector 0;15;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;122;-799.9995,575.4741;Float;False;Property;_Grass_NormalBlend;Grass_NormalBlend;5;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;120;-415.9932,465.3402;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;78;-572.8519,731.0165;Float;True;Property;_N1;N1;6;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;119;-225.7154,601.3589;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;111;-2256.6,695.5652;Float;False;Constant;_Float3;Float 3;13;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;1;-3051.133,888.7651;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;110;-2389.607,610.5889;Float;False;Property;_Top_highlight_falloff;Top_highlight_falloff;14;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;107;-1216.978,-1281.293;Float;True;Property;_RockColor;RockColor;13;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TriplanarNode;104;-859.3672,-887.7216;Float;True;Spherical;World;False;Rock_Albedo;_Rock_Albedo;white;0;None;Mid Texture 2;_MidTexture2;white;13;None;Bot Texture 2;_BotTexture2;white;14;None;RA;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;116;-413.8761,-1091.077;Float;False;Property;_Top_Highlight_intensity;Top_Highlight_intensity;15;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;109;-2044.159,599.5051;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;103;-1769.24,-362.1561;Float;False;Property;_Grass_tiling;Grass_tiling;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-1848.123,-497.9108;Float;False;0;107;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-255.522,-978.6102;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT4;0;False;1;FLOAT4
Node;AmplifyShaderEditor.SaturateNode;114;-1796.57,593.0676;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;45;-2471.622,937.0233;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;47;-2307.621,903.0233;Float;False;Property;_Switch_surface;Switch_surface;8;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-1577.24,-497.1561;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;6;-2385.816,1042.428;Float;False;Property;_Grass_height;Grass_height;3;0;-0.2;-0.99;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;113;-33.26541,-919.7076;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;4;-2743.688,1177.436;Float;False;Constant;_Float0;Float 0;0;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;89;-903.1144,-164.3525;Float;False;Property;_Add_Ao;Add_Ao;11;0;0.3;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SmoothstepOpNode;5;-2445.07,960.6072;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;93;-1385.795,-572.6993;Float;True;Property;_T_grass;T_grass;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;84;-839.3045,-62.36583;Float;True;Property;_Add;Add;9;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;33;-2059.357,1030.391;Float;False;Property;_Grass_Falloff;Grass_Falloff;4;0;3;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-575.4216,-174.35;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1851.294,904.6182;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RGBToHSVNode;190;-1037.715,-554.0339;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;192;-1151.756,-683.6258;Float;False;Property;_Grass_Hue;Grass_Hue;21;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;-904.4377,-255.6463;Float;False;Property;_Add_edge;Add_edge;10;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;87;-387.2027,-175.4661;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.WorldPosInputsNode;201;1321.705,-236.7046;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;77;-325.5702,-512.9861;Float;False;76;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-199.2263,-185.5221;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;191;-792.3546,-661.1631;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;76;-1671.343,899.2781;Float;False;mask_Grass;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;206;1359.919,-80.34737;Float;False;Constant;_Gradient_placement;Gradient_placement;26;0;-110;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;217;1547.815,36.3358;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;8;-21.76329,-556.5601;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;179;322.5906,-500.4862;Float;False;Property;_Cellfect;Cellfect;16;0;0.1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.HSVToRGBNode;189;-707.6876,-536.7551;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;216;1675.025,47.57212;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;205;1634.665,-213.2405;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;91;-40.45473,-180.2297;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;178;407.9906,-746.6862;Float;False;Constant;_Float2;Float 2;21;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;212;1867.118,47.10461;Float;False;Property;_East;East;24;0;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;9;186.7514,-664.5933;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.CommentaryNode;220;2427.524,-1482.921;Float;False;1025.273;287.7188;Wetness_Mask;2;226;223;;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;180;627.9905,-806.6862;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;209;1644.12,-75.1086;Float;False;Constant;_Gradient;Gradient;27;0;0.05;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;221;2487.415,-1393.324;Float;False;Constant;_Float10;Float 10;22;0;0.01;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;222;2477.524,-1310.202;Float;False;Constant;_Float12;Float 12;22;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;176;635.9905,-732.6862;Float;False;3;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;223;2657.479,-1432.921;Float;True;Spherical;World;False;Mask;_Mask;white;18;None;Mid Texture 5;_MidTexture5;white;44;None;Bot Texture 5;_BotTexture5;white;45;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;204;1646.739,-394.6702;Float;False;Constant;_Float8;Float 8;26;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;243;1495.94,900.3036;Float;False;Constant;_Overrided_Normals;Overrided_Normals;26;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;1789.494,-204.7669;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;225;3041.035,-1428.825;Float;False;Constant;_Float14;Float 14;22;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;1798.662,-415.6253;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RelayNode;224;3054.773,-1337.089;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;207;1980.707,-194.2894;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;248;1814.117,1072.525;Float;False;Constant;_Normaloverridetile;Normaloverridetile;28;0;0.0015;0;0.005;0;1;FLOAT
Node;AmplifyShaderEditor.MMatrixNode;264;1776.965,717.0036;Float;False;0;1;FLOAT4x4
Node;AmplifyShaderEditor.BlendNormalsNode;101;-218.25,739.3185;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.IntNode;275;1649.974,1294.813;Float;False;Property;_BlendEast;BlendEast;27;0;0;0;1;INT
Node;AmplifyShaderEditor.RangedFloatNode;249;1941.873,1152.282;Float;False;Constant;_Float18;Float 18;28;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;273;1789.074,923.0134;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;79;-578.955,922.2526;Float;True;Property;_N2;N2;7;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;270;1938.574,902.2134;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;233;2383.689,-583.9532;Float;False;Constant;_Float16;Float 16;29;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;226;3203.797,-1398.322;Float;False;Property;_Toggle_Wetness;Toggle_Wetness;19;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;202;2002.566,-374.2458;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.Vector3Node;195;214.2111,524.4655;Float;False;Constant;_Vector1;Vector 1;25;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;244;2116.293,1084.942;Float;True;Spherical;World;False;Tpnormal;_Tpnormal;white;26;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;Mid Texture 1;_MidTexture1;white;28;None;Bot Texture 1;_BotTexture1;white;29;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;83;58.40069,805.1938;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.VectorFromMatrixNode;267;1918.956,725.3417;Float;False;Row;0;1;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;194;226.0635,974.8537;Float;False;Property;_Normal_intensity;Normal_intensity;22;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;247;2316.659,1292.321;Float;False;Constant;_Float9;Float 9;28;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;256;2119.244,639.095;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;232;2589.468,-558.9358;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SaturateNode;230;3520.885,-1371.709;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;265;2116.628,787.5261;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;193;521.2939,734.5747;Float;False;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;2491.101,1228.508;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.Vector3Node;260;2373.111,1452.078;Float;False;Constant;_Vector3;Vector 3;26;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;242;2123.818,995.2725;Float;False;Property;_Override_Normals;Override_Normals;25;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;245;2677.768,1260.518;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;182;636.1904,-472.2863;Float;False;Constant;_Float6;Float 6;22;0;1.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;277;2852.979,-806.983;Float;False;Property;_Tint;Tint;28;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;231;2800.589,-588.6976;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;229;2333.686,-282.8979;Float;False;Constant;_Float15;Float 15;29;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;241;2392.285,781.4144;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RelayNode;251;2550.396,923.5224;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;177;636.9905,-607.6862;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;44;2333.882,-378.2867;Float;False;Constant;_Float7;Float 7;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;188;826.8285,-1046.656;Float;False;Property;_Fresnel_Power;Fresnel_Power;20;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;186;1325.406,-1061.465;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;197;1372.411,-1335.922;Float;False;Constant;_Float5;Float 5;25;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;184;1059.807,-1067.865;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;239;2348.797,-877.3488;Float;False;Constant;_Float13;Float 13;26;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;187;828.8285,-1127.656;Float;False;Property;_Fresnel_Scale;Fresnel_Scale;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;181;828.1905,-617.0862;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;196;1625.411,-1097.922;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;276;3180.082,-608.811;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;262;2618.854,678.7216;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.NegateNode;218;1551.908,112.0692;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;185;1255.007,-767.0649;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;238;2322.258,-949.0043;Float;False;Constant;_Float11;Float 11;26;0;0.01;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;200;1247.411,-1233.922;Float;False;Property;_Light_color;Light_color;23;0;1,0.475862,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;219;1020.217,-1332.031;Float;False;unity_AmbientSky;0;1;COLOR
Node;AmplifyShaderEditor.LerpOp;228;2748.55,-319.3123;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;261;2749.853,957.7654;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3217.876,-466.9975;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Rocktest;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;DeferredOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
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
WireConnection;47;0;1;1
WireConnection;47;1;45;0
WireConnection;102;0;94;0
WireConnection;102;1;103;0
WireConnection;113;0;118;0
WireConnection;113;1;104;0
WireConnection;113;2;114;0
WireConnection;5;0;47;0
WireConnection;5;1;6;0
WireConnection;5;2;4;0
WireConnection;93;1;102;0
WireConnection;88;0;113;0
WireConnection;88;1;89;0
WireConnection;32;0;5;0
WireConnection;32;1;33;0
WireConnection;190;0;93;0
WireConnection;87;0;113;0
WireConnection;87;1;88;0
WireConnection;87;2;84;1
WireConnection;90;0;87;0
WireConnection;90;1;92;0
WireConnection;191;0;190;1
WireConnection;191;1;192;0
WireConnection;76;0;32;0
WireConnection;217;0;201;1
WireConnection;8;0;77;0
WireConnection;189;0;191;0
WireConnection;189;1;190;2
WireConnection;189;2;190;3
WireConnection;216;0;217;0
WireConnection;216;1;206;0
WireConnection;205;0;201;1
WireConnection;205;1;206;0
WireConnection;91;0;87;0
WireConnection;91;1;90;0
WireConnection;91;2;84;2
WireConnection;212;0;216;0
WireConnection;212;1;205;0
WireConnection;9;0;91;0
WireConnection;9;1;189;0
WireConnection;9;2;8;0
WireConnection;180;0;179;0
WireConnection;176;0;178;0
WireConnection;176;1;9;0
WireConnection;176;2;180;0
WireConnection;223;3;221;0
WireConnection;223;4;222;0
WireConnection;208;0;212;0
WireConnection;208;1;209;0
WireConnection;203;0;176;0
WireConnection;203;1;204;0
WireConnection;224;0;223;2
WireConnection;207;0;208;0
WireConnection;101;0;105;0
WireConnection;101;1;78;0
WireConnection;273;0;243;0
WireConnection;79;5;80;0
WireConnection;270;0;243;0
WireConnection;270;1;273;0
WireConnection;270;2;275;0
WireConnection;226;0;225;0
WireConnection;226;1;224;0
WireConnection;202;0;176;0
WireConnection;202;1;203;0
WireConnection;202;2;207;0
WireConnection;244;3;248;0
WireConnection;244;4;249;0
WireConnection;83;0;101;0
WireConnection;83;1;79;0
WireConnection;83;2;8;0
WireConnection;267;0;264;0
WireConnection;232;0;202;0
WireConnection;232;1;233;0
WireConnection;230;0;226;0
WireConnection;265;0;267;0
WireConnection;265;1;270;0
WireConnection;193;0;195;0
WireConnection;193;1;83;0
WireConnection;193;2;194;0
WireConnection;246;0;244;0
WireConnection;246;1;247;0
WireConnection;245;0;260;0
WireConnection;245;1;246;0
WireConnection;231;0;202;0
WireConnection;231;1;232;0
WireConnection;231;2;230;0
WireConnection;241;0;256;0
WireConnection;241;1;265;0
WireConnection;241;2;242;0
WireConnection;251;0;193;0
WireConnection;177;0;178;0
WireConnection;177;1;9;0
WireConnection;177;2;179;0
WireConnection;186;0;184;0
WireConnection;184;2;187;0
WireConnection;184;3;188;0
WireConnection;181;0;177;0
WireConnection;181;1;182;0
WireConnection;196;0;197;0
WireConnection;196;1;219;0
WireConnection;196;2;186;0
WireConnection;276;0;231;0
WireConnection;276;1;277;0
WireConnection;262;0;256;0
WireConnection;262;1;241;0
WireConnection;262;2;8;0
WireConnection;218;0;206;0
WireConnection;185;0;196;0
WireConnection;185;1;181;0
WireConnection;228;0;44;0
WireConnection;228;1;229;0
WireConnection;228;2;230;0
WireConnection;261;0;251;0
WireConnection;261;1;245;0
WireConnection;261;2;242;0
WireConnection;0;0;276;0
WireConnection;0;1;261;0
WireConnection;0;2;181;0
WireConnection;0;4;228;0
WireConnection;0;12;262;0
ASEEND*/
//CHKSM=068CB1B9C421B2C6D0062EE4A3B1431973A6E6C9