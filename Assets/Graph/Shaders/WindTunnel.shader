// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Wind/WindTunnel"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Emission("Emission", Float) = 0
		_Albedo("Albedo", 2D) = "white" {}
		_TilingOpacity("TilingOpacity", Vector) = (1,4,0,0)
		_Tiling("Tiling", Vector) = (1,4,0,0)
		_Opacity("Opacity", 2D) = "white" {}
		_GeneralOpacity("GeneralOpacity", Float) = 0
		_RotationSpeed("RotationSpeed", Vector) = (1,2,0,0)
		_RotationSpeedOpacity("RotationSpeedOpacity", Vector) = (1,2,0,0)
		_Highlight("Highlight", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_TextureSample2("Texture Sample 2", 2D) = "white" {}
		_StrokeAIntensity("StrokeAIntensity", Float) = 0
		_StrokeBIntensity("StrokeBIntensity", Float) = 0
		_StrokeCIntensity("StrokeCIntensity", Float) = 0
		_StrokeCTiling("StrokeCTiling", Vector) = (1,4,0,0)
		_StrokeBTiling("StrokeBTiling", Vector) = (1,4,0,0)
		_StrokeATiling("StrokeATiling", Vector) = (1,4,0,0)
		_SpeedStrokeC("SpeedStrokeC", Vector) = (1,2,0,0)
		_SpeedStrokeB("SpeedStrokeB", Vector) = (1,2,0,0)
		_SpeedStrokeA("SpeedStrokeA", Vector) = (1,2,0,0)
		_Tint("Tint", Color) = (0,0,0,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
			float2 texcoord_2;
			float4 vertexColor : COLOR;
			float2 texcoord_3;
			float2 texcoord_4;
			float2 texcoord_5;
			float2 texcoord_6;
		};

		uniform float4 _Tint;
		uniform sampler2D _TextureSample0;
		uniform float2 _SpeedStrokeA;
		uniform float2 _StrokeATiling;
		uniform float _StrokeAIntensity;
		uniform sampler2D _TextureSample2;
		uniform float2 _SpeedStrokeC;
		uniform float2 _StrokeCTiling;
		uniform float _StrokeCIntensity;
		uniform float _Highlight;
		uniform sampler2D _Albedo;
		uniform float2 _RotationSpeed;
		uniform float2 _Tiling;
		uniform float _Emission;
		uniform sampler2D _Opacity;
		uniform float2 _RotationSpeedOpacity;
		uniform float2 _TilingOpacity;
		uniform float _GeneralOpacity;
		uniform sampler2D _TextureSample1;
		uniform float2 _SpeedStrokeB;
		uniform float2 _StrokeBTiling;
		uniform float _StrokeBIntensity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * _StrokeATiling + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * _StrokeCTiling + float2( 0,0 );
			o.texcoord_2.xy = v.texcoord.xy * _Tiling + float2( 0,0 );
			o.texcoord_3.xy = v.texcoord.xy * _TilingOpacity + float2( 0,0 );
			o.texcoord_4.xy = v.texcoord.xy * _StrokeATiling + float2( 0,0 );
			float2 panner54 = ( o.texcoord_4 + _Time.y * _SpeedStrokeA);
			float lerpResult68 = lerp( 0.0 , tex2Dlod( _TextureSample0, float4( panner54, 0.0 , 0.0 ) ).r , _StrokeAIntensity);
			o.texcoord_5.xy = v.texcoord.xy * _StrokeBTiling + float2( 0,0 );
			float2 panner58 = ( o.texcoord_5 + _Time.y * _SpeedStrokeB);
			float lerpResult72 = lerp( 0.0 , tex2Dlod( _TextureSample1, float4( panner58, 0.0 , 0.0 ) ).r , _StrokeBIntensity);
			o.texcoord_6.xy = v.texcoord.xy * _StrokeCTiling + float2( 0,0 );
			float2 panner62 = ( o.texcoord_6 + _Time.y * _SpeedStrokeC);
			float lerpResult73 = lerp( 0.0 , tex2Dlod( _TextureSample2, float4( panner62, 0.0 , 0.0 ) ).r , _StrokeCIntensity);
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( lerpResult68 + lerpResult72 + lerpResult73 ) * ase_vertexNormal );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 panner54 = ( i.texcoord_0 + _Time.y * _SpeedStrokeA);
			float lerpResult68 = lerp( 0.0 , tex2D( _TextureSample0, panner54 ).r , _StrokeAIntensity);
			float2 panner62 = ( i.texcoord_1 + _Time.y * _SpeedStrokeC);
			float lerpResult73 = lerp( 0.0 , tex2D( _TextureSample2, panner62 ).r , _StrokeCIntensity);
			float lerpResult84 = lerp( 0.0 , ( lerpResult68 + lerpResult73 ) , _Highlight);
			float2 panner6 = ( i.texcoord_2 + _Time.y * _RotationSpeed);
			float4 tex2DNode2 = tex2D( _Albedo, panner6 );
			o.Albedo = ( _Tint * ( lerpResult84 + tex2DNode2 ) ).rgb;
			float lerpResult98 = lerp( _Emission , 0.0 , i.vertexColor.r);
			float3 temp_cast_1 = (lerpResult98).xxx;
			o.Emission = temp_cast_1;
			float2 panner91 = ( i.texcoord_3 + _Time.y * _RotationSpeedOpacity);
			float lerpResult97 = lerp( ( lerpResult84 + ( tex2DNode2.r * tex2D( _Opacity, panner91 ).r ) ) , 0.0 , i.vertexColor.r);
			float lerpResult82 = lerp( 0.0 , lerpResult97 , _GeneralOpacity);
			o.Alpha = lerpResult82;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
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
-1913;32;1906;1001;1254.405;1031.339;1.3;True;True
Node;AmplifyShaderEditor.Vector2Node;52;-2101.831,-1906.621;Float;False;Property;_StrokeATiling;StrokeATiling;19;0;1,4;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;60;-2071.351,-1397.255;Float;False;Property;_StrokeCTiling;StrokeCTiling;17;0;1,4;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-1936.585,-1899.991;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TimeNode;43;-2840.117,-972.3813;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;53;-2292.008,-1913.351;Float;False;Property;_SpeedStrokeA;SpeedStrokeA;22;0;1,2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;61;-2260.228,-1401.957;Float;False;Property;_SpeedStrokeC;SpeedStrokeC;20;0;1,2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;59;-1906.105,-1388.597;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;54;-1711.03,-1873.127;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;88;-2101.075,-780.2036;Float;False;Property;_TilingOpacity;TilingOpacity;2;0;1,4;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;11;-2056.667,-1014.295;Float;False;Property;_Tiling;Tiling;3;0;1,4;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;62;-1680.55,-1361.733;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;56;-2081.511,-1649.231;Float;False;Property;_StrokeBTiling;StrokeBTiling;18;0;1,4;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;49;-1365.487,-1391.256;Float;True;Property;_TextureSample2;Texture Sample 2;13;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;89;-1935.828,-773.5736;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;90;-2355.952,-782.9337;Float;False;Property;_RotationSpeedOpacity;RotationSpeedOpacity;7;0;1,2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;71;-1067.625,-1410.994;Float;False;Property;_StrokeCIntensity;StrokeCIntensity;16;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;69;-1066.625,-1929.524;Float;False;Property;_StrokeAIntensity;StrokeAIntensity;14;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1891.421,-1007.665;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;13;-2245.544,-1021.025;Float;False;Property;_RotationSpeed;RotationSpeed;6;0;1,2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;65;-2821.412,-1244.133;Float;False;Constant;_Zero;Zero;20;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;47;-1346.541,-1910.337;Float;True;Property;_TextureSample0;Texture Sample 0;11;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;57;-2270.388,-1655.961;Float;False;Property;_SpeedStrokeB;SpeedStrokeB;21;0;1,2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;55;-1916.265,-1642.601;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;6;-1665.866,-980.8004;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.LerpOp;73;-855.3138,-1402.072;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;68;-851.2164,-1932.899;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;91;-1710.273,-746.709;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;58;-1690.71,-1615.736;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;2;-755.851,-894.7265;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;85;-752.6445,-1132.807;Float;False;Property;_Highlight;Highlight;10;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-560.6519,-1402.292;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;12;-987.7813,-392.9674;Float;True;Property;_Opacity;Opacity;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;84;-445.5277,-1170.395;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-685.7047,-387.3401;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;48;-1366.622,-1646.758;Float;True;Property;_TextureSample1;Texture Sample 1;12;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;70;-1072.625,-1659.066;Float;False;Property;_StrokeBIntensity;StrokeBIntensity;15;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;72;-864.2349,-1653.634;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;94;-1320.131,-221.3633;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-546.3469,-396.6642;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-58.13229,-880.6918;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;106;213.295,-848.04;Float;False;Property;_Tint;Tint;23;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;97;-420.8104,-163.3578;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;81;-537.2242,79.79057;Float;False;Property;_GeneralOpacity;GeneralOpacity;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-380.533,-1660.432;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;99;-332.5988,-585.9243;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;93;-243.048,-278.1587;Float;False;Property;_Emission;Emission;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;107;187.2954,-344.939;Float;False;Property;_RefractionAngle;RefractionAngle;24;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;419.9949,-617.9401;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;82;-73.08356,-120.2189;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-83.15308,-590.7836;Float;False;2;2;0;FLOAT;0,0,0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;78;-1270.046,173.8997;Float;False;Property;_Mask;Mask;8;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;77;-1098.611,43.2262;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;102;-1275.371,64.77334;Float;False;Property;_Float1;Float 1;9;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;98;-57.16602,-266.9578;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;680.3398,-468.4676;Float;False;True;6;Float;ASEMaterialInspector;0;0;StandardSpecular;Wind/WindTunnel;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;0;False;0;0;Transparent;0.3;True;True;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;1;1;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;0;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;51;0;52;0
WireConnection;59;0;60;0
WireConnection;54;0;51;0
WireConnection;54;2;53;0
WireConnection;54;1;43;2
WireConnection;62;0;59;0
WireConnection;62;2;61;0
WireConnection;62;1;43;2
WireConnection;49;1;62;0
WireConnection;89;0;88;0
WireConnection;10;0;11;0
WireConnection;47;1;54;0
WireConnection;55;0;56;0
WireConnection;6;0;10;0
WireConnection;6;2;13;0
WireConnection;6;1;43;2
WireConnection;73;0;65;0
WireConnection;73;1;49;1
WireConnection;73;2;71;0
WireConnection;68;0;65;0
WireConnection;68;1;47;1
WireConnection;68;2;69;0
WireConnection;91;0;89;0
WireConnection;91;2;90;0
WireConnection;91;1;43;2
WireConnection;58;0;55;0
WireConnection;58;2;57;0
WireConnection;58;1;43;2
WireConnection;2;1;6;0
WireConnection;101;0;68;0
WireConnection;101;1;73;0
WireConnection;12;1;91;0
WireConnection;84;0;65;0
WireConnection;84;1;101;0
WireConnection;84;2;85;0
WireConnection;103;0;2;1
WireConnection;103;1;12;1
WireConnection;48;1;58;0
WireConnection;72;0;65;0
WireConnection;72;1;48;1
WireConnection;72;2;70;0
WireConnection;86;0;84;0
WireConnection;86;1;103;0
WireConnection;83;0;84;0
WireConnection;83;1;2;0
WireConnection;97;0;86;0
WireConnection;97;1;65;0
WireConnection;97;2;94;1
WireConnection;50;0;68;0
WireConnection;50;1;72;0
WireConnection;50;2;73;0
WireConnection;104;0;106;0
WireConnection;104;1;83;0
WireConnection;82;0;65;0
WireConnection;82;1;97;0
WireConnection;82;2;81;0
WireConnection;100;0;50;0
WireConnection;100;1;99;0
WireConnection;77;3;102;0
WireConnection;77;4;78;0
WireConnection;98;0;93;0
WireConnection;98;1;65;0
WireConnection;98;2;94;1
WireConnection;0;0;104;0
WireConnection;0;2;98;0
WireConnection;0;9;82;0
WireConnection;0;11;100;0
ASEEND*/
//CHKSM=6D35C622FCAE1076E3F1C3F492730FDC5C4572E8