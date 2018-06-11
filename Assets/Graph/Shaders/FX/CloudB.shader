// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/VolCloudB"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_DepthDistance("DepthDistance", Float) = 10
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Texture0("Texture 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+6" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			float2 uv_texcoord;
			float2 texcoord_0;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthDistance;
		uniform sampler2D _Texture0;
		uniform float4 _Texture0_ST;
		uniform sampler2D _TextureSample1;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 0.3,0.3 ) + float2( 0.3,0.3 );
		}

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth140 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth140 = abs( ( screenDepth140 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthDistance ) );
			float lerpResult161 = lerp( 0.6 , 0.8 , saturate( distanceDepth140 ));
			float3 ase_worldPos = i.worldPos;
			float3 temp_output_5_0_g1 = ( ( ase_worldPos - _WorldSpaceCameraPos ) / 60.0 );
			float dotResult8_g1 = dot( temp_output_5_0_g1 , temp_output_5_0_g1 );
			float clampResult10_g1 = clamp( dotResult8_g1 , 0.0 , 1.0 );
			float lerpResult170 = lerp( lerpResult161 , 0.8 , saturate( pow( clampResult10_g1 , 0.7 ) ));
			float3 temp_cast_0 = (lerpResult170).xxx;
			o.Emission = temp_cast_0;
			float2 uv_Texture0 = i.uv_texcoord * _Texture0_ST.xy + _Texture0_ST.zw;
			float2 panner179 = ( i.texcoord_0 + _Time.y * float2( 0.02,0 ));
			float lerpResult195 = lerp( 0.0 , tex2D( _Texture0, uv_Texture0 ).r , tex2D( _Texture0, ( i.texcoord_0 + ( tex2D( _TextureSample1, panner179 ).r * 0.5 ) ) ).g);
			float screenDepth174 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth174 = abs( ( screenDepth174 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 5.0 ) );
			o.Alpha = ( ( lerpResult195 * i.vertexColor.a ) * saturate( distanceDepth174 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1905;31;1906;1004;2894.142;291.4745;1.552217;True;True
Node;AmplifyShaderEditor.RangedFloatNode;182;-1990.537,807.1321;Float;False;Constant;_Float4;Float 4;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;196;-2221.001,670.0693;Float;False;Constant;_Vector1;Vector 1;3;0;0.3,0.3;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;197;-2220.48,823.0173;Float;False;Constant;_Vector2;Vector 2;3;0;0.3,0.3;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;183;-1826.478,809.1829;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;185;-1975.156,531.3082;Float;False;Constant;_Vector0;Vector 0;3;0;0.02,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;180;-1992.588,662.5553;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;179;-1713.688,667.6821;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;187;-1242.018,884.0347;Float;False;Constant;_Float7;Float 7;3;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;178;-1415.305,663.5806;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1053.35,710.7476;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;188;-817.2083,276.4725;Float;True;Property;_Texture0;Texture 0;2;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;144;-1188.146,130.101;Float;False;Property;_DepthDistance;DepthDistance;0;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;184;-804.2288,484.0026;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;176;-297.5476,92.35425;Float;False;Constant;_Float2;Float 2;2;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;140;-980.3206,135.1689;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldSpaceCameraPos;167;-917.9548,-361.632;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;160;-494.3025,424.4242;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;166;-793.2187,-253.8353;Float;False;Constant;_Float5;Float 5;12;0;60;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;165;-797.8385,-162.9781;Float;False;Constant;_Float3;Float 3;12;0;0.7;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;194;-500.8954,633.1527;Float;True;Property;_TextureSample2;Texture Sample 2;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;164;-758.0025,119.6241;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;195;-125.6006,599.7866;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;163;-812.0025,-4.375869;Float;False;Constant;_Float1;Float 1;2;0;0.6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;162;-819.0025,-74.37585;Float;False;Constant;_Float0;Float 0;2;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;172;-375.5476,250.9542;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FunctionNode;168;-526.8068,-343.1526;Float;False;SphereMask;-1;;1;3;0;FLOAT3;0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;174;-142.8476,92.35425;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-146.7475,414.7543;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;177;113.2524,146.9543;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;161;-562.2025,19.12414;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;169;-204.9567,-340.0727;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;171;-136.3475,-165.0457;Float;False;Constant;_Float6;Float 6;2;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;52.15247,325.0543;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;170;77.16618,-210.2883;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;293.6,-65.6;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Bobo/VolCloudB;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;6;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;1;1;1;6;1;1;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;183;0;182;0
WireConnection;180;0;196;0
WireConnection;180;1;197;0
WireConnection;179;0;180;0
WireConnection;179;2;185;0
WireConnection;179;1;183;0
WireConnection;178;1;179;0
WireConnection;186;0;178;1
WireConnection;186;1;187;0
WireConnection;184;0;180;0
WireConnection;184;1;186;0
WireConnection;140;0;144;0
WireConnection;160;0;188;0
WireConnection;160;1;184;0
WireConnection;194;0;188;0
WireConnection;164;0;140;0
WireConnection;195;1;194;1
WireConnection;195;2;160;2
WireConnection;168;0;167;0
WireConnection;168;1;166;0
WireConnection;168;2;165;0
WireConnection;174;0;176;0
WireConnection;173;0;195;0
WireConnection;173;1;172;4
WireConnection;177;0;174;0
WireConnection;161;0;163;0
WireConnection;161;1;162;0
WireConnection;161;2;164;0
WireConnection;169;0;168;0
WireConnection;175;0;173;0
WireConnection;175;1;177;0
WireConnection;170;0;161;0
WireConnection;170;1;171;0
WireConnection;170;2;169;0
WireConnection;0;2;170;0
WireConnection;0;9;175;0
ASEEND*/
//CHKSM=105A7AB9093D1E1CBE7F1F6A4F362550270C4973