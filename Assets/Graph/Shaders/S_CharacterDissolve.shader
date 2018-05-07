// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Antoine/CharacterDissolve"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "bump" {}
		_AlbedoTint("AlbedoTint", Color) = (0,0,0,0)
		_Normal("Normal", 2D) = "bump" {}
		_DissolveGuide("Dissolve Guide", 2D) = "white" {}
		_DissolveNoise("Dissolve Noise", 2D) = "white" {}
		_BurnRamp("Burn Ramp", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range( 0 , 1)) = 0
		_Emission("Emission", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _AlbedoTint;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _DissolveAmount;
		uniform sampler2D _DissolveGuide;
		uniform float4 _DissolveGuide_ST;
		uniform sampler2D _DissolveNoise;
		uniform float4 _DissolveNoise_ST;
		uniform sampler2D _BurnRamp;
		uniform float4 _Emission;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,0.0 );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode78 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( _AlbedoTint * tex2DNode78 ).rgb;
			float2 uv_DissolveGuide = i.uv_texcoord * _DissolveGuide_ST.xy + _DissolveGuide_ST.zw;
			float4 tex2DNode144 = tex2D( _DissolveGuide, uv_DissolveGuide );
			float2 uv_DissolveNoise = i.uv_texcoord * _DissolveNoise_ST.xy + _DissolveNoise_ST.zw;
			float lerpResult146 = lerp( tex2DNode144.r , tex2D( _DissolveNoise, uv_DissolveNoise ).r , tex2DNode144.r);
			float temp_output_73_0 = ( (-0.7 + (( 1.0 - _DissolveAmount ) - 0.0) * (1.1 - -0.7) / (1.0 - 0.0)) + lerpResult146 );
			float clampResult113 = clamp( temp_output_73_0 , 0 , 1 );
			float temp_output_130_0 = ( 1.0 - clampResult113 );
			float2 temp_cast_1 = ((0 + (temp_output_130_0 - 0) * (1.4 - 0) / (1 - 0))).xx;
			float4 temp_output_126_0 = ( temp_output_130_0 * tex2D( _BurnRamp, temp_cast_1 ) );
			float2 temp_cast_2 = ((0 + (temp_output_130_0 - 0) * (1.4 - 0) / (1 - 0))).xx;
			o.Emission = ( temp_output_126_0 + temp_output_126_0 + _Emission ).rgb;
			o.Alpha = 1;
			clip( ( tex2DNode78.a * temp_output_73_0 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1044;2069.177;1038.603;1.789006;True;True
Node;AmplifyShaderEditor.CommentaryNode;128;-1260.172,700.4832;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;6;4;71;73;111;144;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1271.284,772.6973;Float;False;Property;_DissolveAmount;Dissolve Amount;7;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;144;-1157.744,1186.703;Float;True;Property;_DissolveGuide;Dissolve Guide;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-1141.262,996.5222;Float;True;Property;_DissolveNoise;Dissolve Noise;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;71;-1007.489,773.5432;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;146;-750.765,1049.24;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;111;-816.2303,769.3278;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.7;False;4;FLOAT;1.1;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-504.4845,789.8297;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;129;-1398.532,10.69826;Float;False;814.5701;432.0292;Burn Effect - Emission;4;113;126;114;130;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;113;-1394.234,29.91518;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;130;-1111.198,32.70279;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;141;-1264.828,243.1351;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1.4;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;114;-965.4023,257.3173;Float;True;Property;_BurnRamp;Burn Ramp;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;133;-531.2509,-864.4244;Float;False;Property;_AlbedoTint;AlbedoTint;2;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-775.9634,43.36571;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;78;-597.2742,-694.2451;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;147;-1081.259,-176.9386;Float;False;Property;_Emission;Emission;8;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-48.73584,596.9774;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;131;-581.9014,-460.8792;Float;True;Property;_Normal;Normal;3;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;143;-510.918,9.50761;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.CommentaryNode;132;144.1929,26.72195;Float;False;765.1592;493.9802;Created by The Four Headed Cat @fourheadedcat - www.twitter.com/fourheadedcat;1;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-235.2509,-696.4244;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;437.7457,109.222;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Antoine/CharacterDissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;3;False;0;0;Masked;0.5;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;71;0;4;0
WireConnection;146;0;144;1
WireConnection;146;1;2;1
WireConnection;146;2;144;1
WireConnection;111;0;71;0
WireConnection;73;0;111;0
WireConnection;73;1;146;0
WireConnection;113;0;73;0
WireConnection;130;0;113;0
WireConnection;141;0;130;0
WireConnection;114;1;141;0
WireConnection;126;0;130;0
WireConnection;126;1;114;0
WireConnection;148;0;78;4
WireConnection;148;1;73;0
WireConnection;143;0;126;0
WireConnection;143;1;126;0
WireConnection;143;2;147;0
WireConnection;134;0;133;0
WireConnection;134;1;78;0
WireConnection;0;0;134;0
WireConnection;0;1;131;0
WireConnection;0;2;143;0
WireConnection;0;10;148;0
ASEEND*/
//CHKSM=647018CEA703D08BE0E4AB6AAB7695B21D7E7DE7