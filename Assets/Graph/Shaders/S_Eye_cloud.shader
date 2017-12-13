// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Eye_Cloud"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Tiling("Tiling", Vector) = (0,0,0,0)
		_Cloud_opacity("Cloud_opacity", Float) = 0.2
		_Vector4("Vector 4", Vector) = (0,0.3,0,0)
		_Cloud("Cloud", 2D) = "white" {}
		_Hue("Hue", Float) = 0
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
			float2 texcoord_2;
		};

		uniform float _Hue;
		uniform sampler2D _Cloud;
		uniform float2 _Tiling;
		uniform float2 _Vector4;
		uniform float _Cloud_opacity;


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
			o.texcoord_0.xy = v.texcoord.xy * _Tiling + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 3,-0.91 ) + float2( 0,0 );
			o.texcoord_2.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 hsvTorgb115 = RGBToHSV( float3(0.5,0,0) );
			float3 hsvTorgb116 = HSVToRGB( float3(( hsvTorgb115.x + _Hue ),hsvTorgb115.y,hsvTorgb115.z) );
			o.Emission = hsvTorgb116;
			float2 panner57 = ( i.texcoord_0 + 1.0 * _Time.y * float2( 0,0.1 ));
			float2 panner105 = ( i.texcoord_1 + 1.0 * _Time.y * _Vector4);
			float clampResult67 = clamp( ( ( pow( tex2D( _Cloud, panner57 ).r , 6.0 ) * 5.0 ) * ( pow( tex2D( _Cloud, panner105 ).r , 4.0 ) * 3.0 ) ) , 0.0 , 1.0 );
			float clampResult77 = clamp( pow( ( i.texcoord_2.y * 7.0 ) , 6.0 ) , 0.0 , 1.0 );
			float lerpResult70 = lerp( 0.0 , clampResult67 , clampResult77);
			float clampResult82 = clamp( pow( ( i.texcoord_2.y * 1.3 ) , 7.0 ) , 0.0 , 1.0 );
			float lerpResult83 = lerp( lerpResult70 , 0.0 , clampResult82);
			o.Alpha = ( lerpResult83 * ( (1.0 + (sin( _Time.y ) - -1.0) * (5.0 - 1.0) / (1.0 - -1.0)) * _Cloud_opacity ) );
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
-1913;29;1906;1004;936.8712;678.959;1;True;True
Node;AmplifyShaderEditor.Vector2Node;65;-2350.806,841.2602;Float;False;Property;_Tiling;Tiling;0;0;0,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;102;-2274.734,1293.206;Float;False;Constant;_Vector1;Vector 1;2;0;3,-0.91;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-2089.887,743.2069;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;104;-2046.314,1187.353;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;103;-1978.384,1320.202;Float;False;Property;_Vector4;Vector 4;2;0;0,0.3;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;59;-2021.957,876.0565;Float;False;Constant;_Panner;Panner;4;0;0,0.1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;57;-1822.589,745.0432;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;105;-1779.016,1189.189;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;100;-2049.49,384.6088;Float;True;Property;_Cloud;Cloud;3;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SamplerNode;113;-1572.053,1160.708;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;56;-1615.626,716.5621;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;62;-1492.207,925.4239;Float;False;Constant;_Float0;Float 0;10;0;6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;106;-1448.635,1369.57;Float;False;Constant;_Float13;Float 13;10;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;107;-1186.788,1389.003;Float;False;Constant;_Float14;Float 14;8;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;108;-1268.254,1147.417;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;72;-1344.74,274.9897;Float;False;Constant;_Float4;Float 4;2;0;7;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;86;-1704.999,100.7604;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;61;-1311.826,703.271;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;64;-1230.36,944.8569;Float;False;Constant;_Float2;Float 2;8;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-1059.646,707.1706;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-1016.075,1151.316;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;74;-1208.946,352.7384;Float;False;Constant;_Float5;Float 5;2;0;6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;95;-1534.689,-531.8998;Float;False;Constant;_Float11;Float 11;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-1188.693,256.4954;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;78;-1317.736,-36.92134;Float;False;Constant;_Float6;Float 6;2;0;1.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-655.2823,936.8921;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1165.14,-53.69014;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;73;-1032.743,258.1722;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;94;-1383.586,-529.049;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-1185.393,42.55288;Float;False;Constant;_Float7;Float 7;2;0;7;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;68;-925.8801,943.9318;Float;False;Constant;_Float1;Float 1;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;69;-917.9504,1041.364;Float;False;Constant;_Float3;Float 3;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;60;-204.8454,-268.4268;Float;False;Constant;_Vector2;Vector 2;10;0;0.5,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SinOpNode;89;-1197.046,-537.0752;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;-817.6624,-701.5345;Float;False;Constant;_Float9;Float 9;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;77;-788.7782,240.6877;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;93;-819.0879,-634.536;Float;False;Constant;_Float10;Float 10;3;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;81;-1009.19,-52.01332;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;97;-819.0876,-855.4886;Float;False;Constant;_Float12;Float 12;3;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;67;-709.71,705.7311;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;118;96.12878,-333.959;Float;False;Property;_Hue;Hue;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RGBToHSVNode;115;33.11816,-237.9079;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;90;-678.476,-552.1578;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;70;-617.8893,192.0802;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;82;-837.6964,-81.57635;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;88;-689.8121,-378.4134;Float;False;Property;_Cloud_opacity;Cloud_opacity;1;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-348.6726,-509.0919;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;83;-594.3362,-118.1053;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;117;267.1288,-313.959;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-196.9106,21.38739;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;91;-819.0878,-777.0861;Float;False;Constant;_Float8;Float 8;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.HSVToRGBNode;116;400.7183,-221.9079;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;344,20.8;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Eye_Cloud;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;58;0;65;0
WireConnection;104;0;102;0
WireConnection;57;0;58;0
WireConnection;57;2;59;0
WireConnection;105;0;104;0
WireConnection;105;2;103;0
WireConnection;113;0;100;0
WireConnection;113;1;105;0
WireConnection;56;0;100;0
WireConnection;56;1;57;0
WireConnection;108;0;113;1
WireConnection;108;1;106;0
WireConnection;61;0;56;1
WireConnection;61;1;62;0
WireConnection;63;0;61;0
WireConnection;63;1;64;0
WireConnection;110;0;108;0
WireConnection;110;1;107;0
WireConnection;71;0;86;2
WireConnection;71;1;72;0
WireConnection;114;0;63;0
WireConnection;114;1;110;0
WireConnection;79;0;86;2
WireConnection;79;1;78;0
WireConnection;73;0;71;0
WireConnection;73;1;74;0
WireConnection;94;0;95;0
WireConnection;89;0;94;0
WireConnection;77;0;73;0
WireConnection;77;1;68;0
WireConnection;77;2;69;0
WireConnection;81;0;79;0
WireConnection;81;1;80;0
WireConnection;67;0;114;0
WireConnection;67;1;68;0
WireConnection;67;2;69;0
WireConnection;115;0;60;0
WireConnection;90;0;89;0
WireConnection;90;1;93;0
WireConnection;90;2;92;0
WireConnection;90;3;92;0
WireConnection;90;4;97;0
WireConnection;70;1;67;0
WireConnection;70;2;77;0
WireConnection;82;0;81;0
WireConnection;82;1;68;0
WireConnection;82;2;69;0
WireConnection;96;0;90;0
WireConnection;96;1;88;0
WireConnection;83;0;70;0
WireConnection;83;2;82;0
WireConnection;117;0;115;1
WireConnection;117;1;118;0
WireConnection;87;0;83;0
WireConnection;87;1;96;0
WireConnection;116;0;117;0
WireConnection;116;1;115;2
WireConnection;116;2;115;3
WireConnection;0;2;116;0
WireConnection;0;9;87;0
ASEEND*/
//CHKSM=7406062A8B00333CE1A4414773D07BF11B92C9B1