// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Eye_Cloud"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Tiling("Tiling", Vector) = (0,0,0,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
		};

		uniform sampler2D _TextureSample0;
		uniform float2 _Tiling;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * _Tiling + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = float3(0.5,0,0);
			float2 panner57 = ( i.texcoord_0 + 1.0 * _Time.y * float2( 0,0.3 ));
			float clampResult67 = clamp( ( pow( tex2D( _TextureSample0, panner57 ).r , 6.0 ) * 5.0 ) , 0.0 , 1.0 );
			float clampResult77 = clamp( pow( ( i.texcoord_1.y * 7.0 ) , 4.0 ) , 0.0 , 1.0 );
			float lerpResult70 = lerp( 0.0 , clampResult67 , clampResult77);
			float clampResult82 = clamp( pow( ( i.texcoord_1.y * 1.3 ) , 7.0 ) , 0.0 , 1.0 );
			float lerpResult83 = lerp( lerpResult70 , 0.0 , clampResult82);
			o.Alpha = ( lerpResult83 * 0.2 );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
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
-1913;29;1906;1004;2459.042;684.3425;1.725501;True;True
Node;AmplifyShaderEditor.Vector2Node;65;-2012.647,791.4128;Float;False;Property;_Tiling;Tiling;1;0;0,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-1851.828,679.0595;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;59;-1783.898,811.9091;Float;False;Constant;_Vector0;Vector 0;10;0;0,0.3;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;57;-1584.53,680.8958;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TextureCoordinatesNode;86;-1704.999,100.7604;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;62;-1254.148,861.2765;Float;False;Constant;_Float0;Float 0;10;0;6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;56;-1377.567,652.4147;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;72;-1344.74,274.9897;Float;False;Constant;_Float4;Float 4;2;0;7;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;74;-1208.946,352.7384;Float;False;Constant;_Float5;Float 5;2;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;61;-1073.767,639.1236;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;64;-992.3016,880.7095;Float;False;Constant;_Float2;Float 2;8;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-1188.693,256.4954;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;78;-1317.736,-36.92134;Float;False;Constant;_Float6;Float 6;2;0;1.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;73;-1032.743,258.1722;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;68;-687.8215,879.7844;Float;False;Constant;_Float1;Float 1;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1165.14,-53.69014;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-821.5874,643.0232;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-1185.393,42.55288;Float;False;Constant;_Float7;Float 7;2;0;7;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;69;-679.8918,977.2167;Float;False;Constant;_Float3;Float 3;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;67;-471.6513,641.5837;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;81;-1009.19,-52.01332;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;77;-788.7782,240.6877;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;82;-837.6964,-81.57635;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;70;-617.8893,192.0802;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;83;-594.3362,-118.1053;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;88;-362.5586,90.40748;Float;False;Constant;_Float8;Float 8;2;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;60;-204.8454,-268.4268;Float;False;Constant;_Vector2;Vector 2;10;0;0.5,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-196.9106,21.38739;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Eye_Cloud;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;58;0;65;0
WireConnection;57;0;58;0
WireConnection;57;2;59;0
WireConnection;56;1;57;0
WireConnection;61;0;56;1
WireConnection;61;1;62;0
WireConnection;71;0;86;2
WireConnection;71;1;72;0
WireConnection;73;0;71;0
WireConnection;73;1;74;0
WireConnection;79;0;86;2
WireConnection;79;1;78;0
WireConnection;63;0;61;0
WireConnection;63;1;64;0
WireConnection;67;0;63;0
WireConnection;67;1;68;0
WireConnection;67;2;69;0
WireConnection;81;0;79;0
WireConnection;81;1;80;0
WireConnection;77;0;73;0
WireConnection;77;1;68;0
WireConnection;77;2;69;0
WireConnection;82;0;81;0
WireConnection;82;1;68;0
WireConnection;82;2;69;0
WireConnection;70;1;67;0
WireConnection;70;2;77;0
WireConnection;83;0;70;0
WireConnection;83;2;82;0
WireConnection;87;0;83;0
WireConnection;87;1;88;0
WireConnection;0;2;60;0
WireConnection;0;9;87;0
ASEEND*/
//CHKSM=C78FF510BB66CF4A65D1B6901C1DCD0DE851CBFF