// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Antoine/EyeDissolve"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_DissolveNoise("DissolveNoise", 2D) = "white" {}
		_DissolveRamp("DissolveRamp", 2D) = "white" {}
		_DissolveAmount("DissolveAmount", Range( 0 , 1)) = 0
		_AlbedoTint("AlbedoTint", Color) = (0,0,0,0)
		_EmissiveAdd("EmissiveAdd", Color) = (0,0,0,0)
		_NormalIntensity("NormalIntensity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalIntensity;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _AlbedoTint;
		uniform float _DissolveAmount;
		uniform sampler2D _DissolveNoise;
		uniform float4 _DissolveNoise_ST;
		uniform sampler2D _DissolveRamp;
		uniform float4 _EmissiveAdd;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float4 lerpResult153 = lerp( float4( float3(0,0,1) , 0.0 ) , tex2D( _Normal, uv_Normal ) , _NormalIntensity);
			o.Normal = lerpResult153.xyz;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( tex2D( _Albedo, uv_Albedo ) * _AlbedoTint ).xyz;
			float2 uv_DissolveNoise = i.uv_texcoord * _DissolveNoise_ST.xy + _DissolveNoise_ST.zw;
			float temp_output_140_0 = ( (-0.6 + (( 1.0 - _DissolveAmount ) - 0.0) * (0.6 - -0.6) / (1.0 - 0.0)) + tex2D( _DissolveNoise, uv_DissolveNoise ).r );
			float clampResult142 = clamp( (-4.0 + (temp_output_140_0 - 0.0) * (4.0 - -4.0) / (1.0 - 0.0)) , 0.0 , 1.0 );
			float temp_output_143_0 = ( 1.0 - clampResult142 );
			float2 appendResult144 = float2( temp_output_143_0 , 0 );
			o.Emission = ( ( temp_output_143_0 * tex2D( _DissolveRamp, appendResult144 ) ) + _EmissiveAdd ).xyz;
			o.Alpha = 1;
			clip( temp_output_140_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1044;54.01486;-811.4383;1.229998;True;True
Node;AmplifyShaderEditor.CommentaryNode;133;246.9285,1626.827;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;2;138;136;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;136;295.2587,1699.041;Float;False;Property;_DissolveAmount;DissolveAmount;5;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;137;559.0541,1699.887;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;139;687.8707,1700.671;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.6;False;4;FLOAT;0.6;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;138;656.7424,1915.693;Float;True;Property;_DissolveNoise;DissolveNoise;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;140;894.6166,1683.173;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;141;336.1486,1397.64;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-4.0;False;4;FLOAT;4.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;142;416.6672,1207.059;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;143;586.7029,1199.846;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;134;321.3685,1165.842;Float;False;814.5701;432.0292;Burn Effect - Emission;1;145;;1,1,1,1;0;0
Node;AmplifyShaderEditor.AppendNode;144;592.8632,1423.845;Float;False;FLOAT2;0;0;0;0;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;145;789.1581,1412.756;Float;True;Property;_DissolveRamp;DissolveRamp;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;148;808.8964,545.7397;Float;False;Property;_AlbedoTint;AlbedoTint;6;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;149;838.8696,800.3932;Float;False;Constant;_Vector1;Vector 1;8;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;146;1002.936,785.8557;Float;False;Property;_NormalIntensity;NormalIntensity;8;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;1011.938,1242.509;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT4;0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;147;769.3422,347.7836;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;True;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;151;749.5997,972.0642;Float;True;Property;_Normal;Normal;2;0;None;True;0;True;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;150;1149.003,1452.087;Float;False;Property;_EmissiveAdd;EmissiveAdd;7;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;154;1246.04,1191.777;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.CommentaryNode;135;1358.494,1143.465;Float;False;765.1592;493.9802;Created by The Four Headed Cat @fourheadedcat - www.twitter.com/fourheadedcat;1;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;1215.896,627.7398;Float;False;2;2;0;FLOAT4;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;153;1210.616,928.116;Float;False;3;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1734.89,1219.917;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Antoine/EyeDissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;3;False;0;0;Masked;0.5;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;137;0;136;0
WireConnection;139;0;137;0
WireConnection;140;0;139;0
WireConnection;140;1;138;1
WireConnection;141;0;140;0
WireConnection;142;0;141;0
WireConnection;143;0;142;0
WireConnection;144;0;143;0
WireConnection;145;1;144;0
WireConnection;152;0;143;0
WireConnection;152;1;145;0
WireConnection;154;0;152;0
WireConnection;154;1;150;0
WireConnection;155;0;147;0
WireConnection;155;1;148;0
WireConnection;153;0;149;0
WireConnection;153;1;151;0
WireConnection;153;2;146;0
WireConnection;0;0;155;0
WireConnection;0;1;153;0
WireConnection;0;2;154;0
WireConnection;0;10;140;0
ASEEND*/
//CHKSM=AB713CEE98F4DE993DE7441DAFC6A64B551E4EDB