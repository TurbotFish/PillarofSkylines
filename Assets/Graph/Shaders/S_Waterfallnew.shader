// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Waterfall"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_TextureSample3("Texture Sample 3", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_Distortion_intensity("Distortion_intensity", Range( 0 , 1)) = 0.5
		_Color2("Color2", Color) = (0,0,0,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
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
			float2 texcoord_2;
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform float4 _Color2;
		uniform sampler2D _Noise;
		uniform sampler2D _TextureSample3;
		uniform float _Distortion_intensity;
		uniform float _Smoothness;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,0.05 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 2,0.5 ) + float2( 0,0 );
			o.texcoord_2.xy = v.texcoord.xy * float2( 2,0.5 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner21 = ( i.texcoord_0 + 1.0 * _Time.y * float2( 0,0.2 ));
			float2 panner7 = ( i.texcoord_1 + 1.0 * _Time.y * float2( 0,0.5 ));
			float2 panner33 = ( i.texcoord_2 + 1.0 * _Time.y * float2( 0,1 ));
			float temp_output_35_0 = ( tex2D( _Noise, ( ( tex2D( _TextureSample3, panner21 ).r * _Distortion_intensity ) + panner7 ) ).g + ( tex2D( _Noise, panner33 ).g * 0.5 ) );
			float4 lerpResult42 = lerp( _Color , _Color2 , saturate( pow( temp_output_35_0 , 5.0 ) ));
			o.Albedo = lerpResult42.rgb;
			o.Smoothness = _Smoothness;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float lerpResult5 = lerp( 0.0 , temp_output_35_0 , tex2D( _TextureSample0, uv_TextureSample0 ).r);
			o.Alpha = lerpResult5;
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
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
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
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
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
7;29;1906;1004;2227.735;1414.046;2.994836;True;True
Node;AmplifyShaderEditor.Vector2Node;20;-2104.759,-787.4813;Float;False;Constant;_Vector2;Vector 2;3;0;1,0.05;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-1939.759,-808.4813;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;22;-1870.759,-675.4812;Float;False;Constant;_Vector3;Vector 3;3;0;0,0.2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;30;-1499,68.62148;Float;False;Constant;_Vector4;Vector 4;2;0;2,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;21;-1678.759,-800.4812;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;4;-1481.533,-205.6438;Float;False;Constant;_Vector0;Vector 0;2;0;2,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;31;-1230,199.6215;Float;False;Constant;_Vector5;Vector 5;2;0;0,1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;-1298,80.62148;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;24;-1357.142,-605.7117;Float;False;Property;_Distortion_intensity;Distortion_intensity;4;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;18;-1466.759,-806.4813;Float;True;Property;_TextureSample3;Texture Sample 3;2;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1280.533,-193.6438;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;8;-1212.533,-74.64381;Float;False;Constant;_Vector1;Vector 1;2;0;0,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;7;-1010.533,-187.6438;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;12;-687.4581,-832.6305;Float;True;Property;_Noise;Noise;1;0;Assets/Plugins/AmplifyShaderEditor/Examples/Assets/Textures/Misc/TempNoise.tga;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1050.679,-665.3239;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;33;-1028,86.62148;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-804.7451,-204.5858;Float;False;2;2;0;FLOAT;0,0;False;1;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;37;-410.3505,251.2531;Float;False;Constant;_Float2;Float 2;3;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;26;-592.3973,53.43761;Float;True;Property;_TextureSample2;Texture Sample 2;2;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-258.7034,112.7185;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-610.6982,-208.8956;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;44;-361.1594,-547.078;Float;False;Constant;_Float1;Float 1;7;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;35;19.59214,7.570284;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;43;-198.0385,-563.2537;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;45;40.78711,-572.9131;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;55.71098,-123.7587;Float;False;Constant;_Float0;Float 0;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;41;235.4719,-485.2194;Float;False;Property;_Color2;Color2;5;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;39;249.8746,-682.0132;Float;False;Property;_Color;Color;3;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-952,350;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Assets/Graph/Textures/FX/T_WaterfallMasks.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;42;512.0396,-592.9748;Float;True;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;5;271.8365,-13.64728;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;40;263.5345,-170.2656;Float;False;Property;_Smoothness;Smoothness;6;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;731.8365,-182.6473;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Waterfall;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;20;0
WireConnection;21;0;19;0
WireConnection;21;2;22;0
WireConnection;32;0;30;0
WireConnection;18;1;21;0
WireConnection;3;0;4;0
WireConnection;7;0;3;0
WireConnection;7;2;8;0
WireConnection;23;0;18;1
WireConnection;23;1;24;0
WireConnection;33;0;32;0
WireConnection;33;2;31;0
WireConnection;17;0;23;0
WireConnection;17;1;7;0
WireConnection;26;0;12;0
WireConnection;26;1;33;0
WireConnection;36;0;26;2
WireConnection;36;1;37;0
WireConnection;2;0;12;0
WireConnection;2;1;17;0
WireConnection;35;0;2;2
WireConnection;35;1;36;0
WireConnection;43;0;35;0
WireConnection;43;1;44;0
WireConnection;45;0;43;0
WireConnection;42;0;39;0
WireConnection;42;1;41;0
WireConnection;42;2;45;0
WireConnection;5;0;6;0
WireConnection;5;1;35;0
WireConnection;5;2;1;1
WireConnection;0;0;42;0
WireConnection;0;4;40;0
WireConnection;0;9;5;0
ASEEND*/
//CHKSM=7C24E51BC4B6C9F0C98343B5A71250DE843A3E5A