// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Lazr"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Emissive("Emissive", Color) = (0,0,0,0)
		_Noise("Noise", 2D) = "white" {}
		_Noise_tiling("Noise_tiling", Float) = 0.2
		_Noise2("Noise2", 2D) = "white" {}
		_Noise2_tiling("Noise2_tiling", Float) = 0.2
		_Noise_speed("Noise_speed", Vector) = (-1,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+10" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 texcoord_0;
			float2 texcoord_1;
		};

		uniform float4 _Emissive;
		uniform sampler2D _TextureSample0;
		uniform sampler2D _Noise2;
		uniform sampler2D _Noise;
		uniform float2 _Noise_speed;
		uniform float _Noise_tiling;
		uniform float _Noise2_tiling;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = ( _Emissive * ( i.vertexColor.a * 5.0 ) ).rgb;
			float2 panner8 = ( ( i.texcoord_1 * _Noise_tiling ) + _Time.y * _Noise_speed);
			float4 tex2DNode7 = tex2D( _Noise, panner8 );
			float2 temp_output_24_0 = ( i.texcoord_1 * _Noise2_tiling );
			float4 tex2DNode22 = tex2D( _Noise2, ( tex2DNode7.r + temp_output_24_0 ) );
			o.Alpha = ( tex2D( _TextureSample0, ( i.texcoord_0 + ( ( tex2DNode22.r * 0.4 ) + float2( 0,-0.15 ) ) ) ).r * tex2DNode22.r );
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
-1913;29;1906;1004;935.856;-253.506;1.6;True;True
Node;AmplifyShaderEditor.RangedFloatNode;17;-1063.987,649.0942;Float;False;Property;_Noise_tiling;Noise_tiling;2;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1144.987,533.0942;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-1074.987,864.0942;Float;False;Constant;_Float1;Float 1;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;15;-1074.987,721.0942;Float;False;Property;_Noise_speed;Noise_speed;5;0;-1,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-862.9868,542.0942;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleTimeNode;13;-925.9867,863.0942;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-1025.19,1065.725;Float;False;Property;_Noise2_tiling;Noise2_tiling;4;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;8;-728.9868,646.0942;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-824.1901,958.7248;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;7;-522.9868,625.0942;Float;True;Property;_Noise;Noise;1;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-207.236,818.6669;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;52;-334.2559,1210.306;Float;False;Constant;_Float4;Float 4;7;0;0.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;22;-50.85971,630.8;Float;True;Property;_Noise2;Noise2;3;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;56;-307.056,1307.906;Float;False;Constant;_Vector0;Vector 0;7;0;0,-0.15;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-174.2559,1207.106;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;55;-73.45606,1339.906;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-289.3582,1074.83;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;3;-559,196;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-11.05591,1200.706;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;5;-575,364;Float;False;Constant;_Float0;Float 0;1;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-358,223;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;49;161.7441,1176.706;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;1;-567,11;Float;False;Property;_Emissive;Emissive;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-201,95;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;488.144,1202.306;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-214.7309,697.3422;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;615.1293,425.8343;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Bobo/Lazr;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;10;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;2;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;9;0
WireConnection;16;1;17;0
WireConnection;13;0;14;0
WireConnection;8;0;16;0
WireConnection;8;2;15;0
WireConnection;8;1;13;0
WireConnection;24;0;9;0
WireConnection;24;1;25;0
WireConnection;7;1;8;0
WireConnection;26;0;7;1
WireConnection;26;1;24;0
WireConnection;22;1;26;0
WireConnection;51;0;22;1
WireConnection;51;1;52;0
WireConnection;55;0;51;0
WireConnection;55;1;56;0
WireConnection;50;0;27;0
WireConnection;50;1;55;0
WireConnection;4;0;3;4
WireConnection;4;1;5;0
WireConnection;49;1;50;0
WireConnection;6;0;1;0
WireConnection;6;1;4;0
WireConnection;54;0;49;1
WireConnection;54;1;22;1
WireConnection;23;0;7;1
WireConnection;23;1;24;0
WireConnection;0;2;6;0
WireConnection;0;9;54;0
ASEEND*/
//CHKSM=53EA15FD1EA04152561A0C5C2B936E6CED804D76