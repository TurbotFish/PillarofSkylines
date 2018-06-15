// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Antoine/Sand"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TriplanarTiling("TriplanarTiling", Float) = 1
		_Tint("Tint", Color) = (1,1,1,1)
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_Tpnormal("Tpnormal", 2D) = "white" {}
		_Override_normals("Override_normals", Range( 0 , 1)) = 0
		_BlendEast("Blend East", Int) = 0
		[Toggle]_OverrideManualNormal("OverrideManualNormal", Float) = 0
		[Toggle]_OverrideManualNormalB("OverrideManualNormalB", Float) = 0
		_OverridenNormal("OverridenNormal", Vector) = (0,1,0,0)
		_OverridenLocalNormal("OverridenLocalNormal", Vector) = (0,1,0,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "DisableBatching" = "True" }
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
		};

		uniform float _OverrideManualNormalB;
		uniform sampler2D _Normal;
		uniform float _TriplanarTiling;
		uniform sampler2D _Tpnormal;
		uniform float _Override_normals;
		uniform float3 _OverridenNormal;
		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform float _OverrideManualNormal;
		uniform int _BlendEast;
		uniform float3 _OverridenLocalNormal;


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 _Vector3 = float3(0,1,0);
			float3 lerpResult58 = lerp( _Vector3 , -_Vector3 , (float)_BlendEast);
			float4 lerpResult67 = lerp( float4( ase_vertexNormal , 0.0 ) , ( UNITY_MATRIX_M[0] * float4( lerpResult58 , 0.0 ) ) , _Override_normals);
			v.normal = lerp(lerpResult67,float4( _OverridenLocalNormal , 0.0 ),_OverrideManualNormal).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal6 = TriplanarNormal( _Normal, _Normal, _Normal, ase_worldPos, ase_worldNormal, 1.0, _TriplanarTiling, 0 );
			float3 tanTriplanarNormal6 = mul( ase_worldToTangent, worldTriplanarNormal6 );
			float4 triplanar33 = TriplanarSampling( _Tpnormal, _Tpnormal, _Tpnormal, ase_worldPos, ase_worldNormal, 3.0, 0.0015, 0 );
			float4 lerpResult69 = lerp( float4( tanTriplanarNormal6 , 0.0 ) , ( float4( float3(0,0,1) , 0.0 ) * ( triplanar33 * 2.0 ) ) , _Override_normals);
			o.Normal = lerp(lerpResult69,float4( _OverridenNormal , 0.0 ),_OverrideManualNormalB).xyz;
			float4 triplanar5 = TriplanarSampling( _Albedo, _Albedo, _Albedo, ase_worldPos, ase_worldNormal, 1.0, _TriplanarTiling, 0 );
			o.Albedo = ( _Tint * triplanar5 ).rgb;
			o.Smoothness = tanTriplanarNormal6.x;
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
-1913;29;1906;1004;2287.355;367.6736;1.714463;True;True
Node;AmplifyShaderEditor.Vector3Node;52;-1429.268,1095.138;Float;False;Constant;_Vector3;Vector 3;26;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;31;-1087.778,1358.784;Float;False;Constant;_Float1;Float 1;28;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;32;-1215.534,1279.028;Float;False;Constant;_Float4;Float 4;28;0;0.0015;0;0.005;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;54;-1136.134,1117.848;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.IntNode;53;-1275.234,1489.646;Float;False;Property;_BlendEast;Blend East;7;0;0;0;1;INT
Node;AmplifyShaderEditor.TriplanarNode;33;-913.3587,1291.445;Float;True;Spherical;World;False;Tpnormal;_Tpnormal;white;5;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;Mid Texture 2;_MidTexture2;white;28;None;Bot Texture 2;_BotTexture2;white;29;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-1192.78,-365.5075;Float;False;Constant;_Float2;Float 2;0;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-1207.9,-451.3;Float;False;Property;_TriplanarTiling;TriplanarTiling;0;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;60;-608.5492,1487.155;Float;False;Constant;_Float10;Float 10;28;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.MMatrixNode;56;-1148.243,911.8379;Float;False;0;1;FLOAT4x4
Node;AmplifyShaderEditor.LerpOp;58;-986.6341,1097.048;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.VectorFromMatrixNode;59;-1006.252,920.176;Float;False;Row;0;1;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;6;-884.9,-322.9;Float;True;Spherical;World;True;Normal;_Normal;white;3;None;Mid Texture 0;_MidTexture0;white;2;None;Bot Texture 0;_BotTexture0;white;3;None;Normal;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;62;-552.097,1646.912;Float;False;Constant;_Vector6;Vector 6;26;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-434.107,1423.342;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.NormalVertexDataNode;66;-805.9642,833.9292;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;63;-801.39,1190.107;Float;False;Property;_Override_normals;Override_normals;6;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-808.5802,982.3604;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-247.44,1455.352;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RelayNode;71;-374.8121,1118.357;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.TriplanarNode;5;-882.9,-526.3;Float;True;Spherical;World;False;Albedo;_Albedo;white;2;None;Mid Texture 0;_MidTexture0;white;2;None;Bot Texture 0;_BotTexture0;white;3;None;Albedo;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;67;-532.9232,976.2488;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.Vector3Node;75;-716.9072,324.9694;Float;False;Property;_OverridenLocalNormal;OverridenLocalNormal;10;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;69;-175.355,1152.6;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;1;-798.9,-718.2999;Float;False;Property;_Tint;Tint;1;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;73;-715.2379,152.5183;Float;False;Property;_OverridenNormal;OverridenNormal;9;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;24;-754.4485,-79.65419;Float;False;Constant;_Vector0;Vector 0;7;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;72;-363.103,274.4345;Float;False;Property;_OverrideManualNormal;OverrideManualNormal;8;0;0;2;0;FLOAT4;0.0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;70;-306.3541,873.5559;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;17;-292.9483,-409.8542;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;18;-538.6483,34.74581;Float;False;Property;_NormalIntensity;NormalIntensity;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-448.9,-555.2999;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ToggleSwitchNode;74;-362.0134,158.6664;Float;False;Property;_OverrideManualNormalB;OverrideManualNormalB;8;0;0;2;0;FLOAT4;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;65,-86;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Antoine/Sand;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;54;0;52;0
WireConnection;33;3;32;0
WireConnection;33;4;31;0
WireConnection;58;0;52;0
WireConnection;58;1;54;0
WireConnection;58;2;53;0
WireConnection;59;0;56;0
WireConnection;6;3;3;0
WireConnection;6;4;13;0
WireConnection;64;0;33;0
WireConnection;64;1;60;0
WireConnection;65;0;59;0
WireConnection;65;1;58;0
WireConnection;68;0;62;0
WireConnection;68;1;64;0
WireConnection;71;0;6;0
WireConnection;5;3;3;0
WireConnection;5;4;13;0
WireConnection;67;0;66;0
WireConnection;67;1;65;0
WireConnection;67;2;63;0
WireConnection;69;0;71;0
WireConnection;69;1;68;0
WireConnection;69;2;63;0
WireConnection;72;0;67;0
WireConnection;72;1;75;0
WireConnection;70;0;66;0
WireConnection;70;1;67;0
WireConnection;17;0;24;0
WireConnection;17;1;6;0
WireConnection;17;2;18;0
WireConnection;2;0;1;0
WireConnection;2;1;5;0
WireConnection;74;0;69;0
WireConnection;74;1;73;0
WireConnection;0;0;2;0
WireConnection;0;1;74;0
WireConnection;0;4;6;1
WireConnection;0;12;72;0
ASEEND*/
//CHKSM=D184543485A730BF2DA0FEA2E1A92356E36599C6