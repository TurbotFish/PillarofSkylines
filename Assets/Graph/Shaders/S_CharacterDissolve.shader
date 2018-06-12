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
		_Emissive("Emissive", 2D) = "bump" {}
		_EmissiveTint("EmissiveTint", Color) = (0,0,0,0)
		_DissolveGuide("Dissolve Guide", 2D) = "white" {}
		_DissolveNoise("Dissolve Noise", 2D) = "white" {}
		_BurnRamp("Burn Ramp", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range( 0 , 1)) = 0
		_Emission("Emission", Color) = (0,0,0,0)
		_T_pilou_marks("T_pilou_marks", 2D) = "white" {}
		[Toggle]_ToggelMarks("ToggelMarks", Float) = 1
		_Mark_Apparition("Mark_Apparition", Range( 0 , 6)) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[Toggle]_ToggleEnergyPan("ToggleEnergyPan", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float2 texcoord_0;
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
		uniform float _ToggleEnergyPan;
		uniform sampler2D _Emissive;
		uniform float4 _Emissive_ST;
		uniform sampler2D _TextureSample0;
		uniform float4 _EmissiveTint;
		uniform float _ToggelMarks;
		uniform sampler2D _T_pilou_marks;
		uniform float4 _T_pilou_marks_ST;
		uniform float _Mark_Apparition;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
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
			float2 uv_Emissive = i.uv_texcoord * _Emissive_ST.xy + _Emissive_ST.zw;
			float4 tex2DNode149 = tex2D( _Emissive, uv_Emissive );
			float2 panner171 = ( i.texcoord_0 + _Time.y * float2( 0,-0.1 ));
			float4 lerpResult169 = lerp( float4( 0,0,0,0 ) , ( ( tex2D( _TextureSample0, panner171 ).r * 4.0 ) * tex2DNode149 ) , tex2DNode149.b);
			float2 uv_T_pilou_marks = i.uv_texcoord * _T_pilou_marks_ST.xy + _T_pilou_marks_ST.zw;
			float4 tex2DNode153 = tex2D( _T_pilou_marks, uv_T_pilou_marks );
			float3 lerpResult163 = lerp( float3( 0.0,0,0 ) , float3(4,0,0) , saturate( step( tex2DNode153.g , (0.0 + (_Mark_Apparition - 0.0) * (0.6 - 0.0) / (6.0 - 0.0)) ) ));
			float3 lerpResult155 = lerp( float3( 0.0,0,0 ) , lerpResult163 , tex2DNode153.r);
			o.Emission = ( ( temp_output_126_0 + temp_output_126_0 + _Emission + ( lerp(tex2DNode149,lerpResult169,_ToggleEnergyPan) * _EmissiveTint ) ) + float4( lerp(float3( 0,0,0 ),lerpResult155,_ToggelMarks) , 0.0 ) ).rgb;
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
-1913;29;1906;1004;1258.335;186.5678;1.658929;True;True
Node;AmplifyShaderEditor.CommentaryNode;128;-1260.172,700.4832;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;6;4;71;73;111;144;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;175;-2577.044,-702.8949;Float;False;Constant;_Float3;Float 3;14;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;4;-1271.284,772.6973;Float;False;Property;_DissolveAmount;Dissolve Amount;9;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;173;-2398.044,-825.8949;Float;False;Constant;_Vector1;Vector 1;14;0;0,-0.1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;174;-2421.044,-700.8949;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;172;-2458.044,-958.8949;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;171;-2170.044,-950.8949;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.OneMinusNode;71;-1007.489,773.5432;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-1141.262,996.5222;Float;True;Property;_DissolveNoise;Dissolve Noise;7;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;144;-1157.744,1186.703;Float;True;Property;_DissolveGuide;Dissolve Guide;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;111;-816.2303,769.3278;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.7;False;4;FLOAT;1.1;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;146;-750.765,1049.24;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;168;-271.3849,1311.938;Float;False;1251.504;833.8145;Comment;13;167;165;166;160;153;159;156;161;163;155;157;162;164;Marks;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;170;-1948.044,-966.8949;Float;True;Property;_TextureSample0;Texture Sample 0;13;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;178;-1835.423,-761.5989;Float;False;Constant;_Float4;Float 4;14;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;160;-221.3849,1800.752;Float;False;Property;_Mark_Apparition;Mark_Apparition;12;0;0;0;6;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;167;-47.38489,2030.752;Float;False;Constant;_Float2;Float 2;13;0;0.6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-504.4845,789.8297;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;129;-1398.532,10.69826;Float;False;814.5701;432.0292;Burn Effect - Emission;4;113;126;114;130;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;149;-1969.504,-661.3489;Float;True;Property;_Emissive;Emissive;4;0;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;180;-1622.107,-940.226;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;165;-49.38489,1871.752;Float;False;Constant;_Float0;Float 0;13;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;166;-52.38489,1947.752;Float;False;Constant;_Float1;Float 1;13;0;6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-1464.676,-935.2677;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.TFHCRemap;164;152.6151,1817.752;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;153;-166.6542,1571.031;Float;True;Property;_T_pilou_marks;T_pilou_marks;11;0;Assets/Graph/Textures/PILOU/01_body/T_pilou_marks.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;113;-1394.234,29.91518;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;130;-1111.198,32.70279;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;159;293.6151,1682.752;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;169;-1467.901,-689.4543;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.TFHCRemap;141;-1264.828,243.1351;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1.4;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;156;90.49075,1361.938;Float;False;Constant;_Vector0;Vector 0;12;0;4,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;161;424.6151,1683.752;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;181;-1101.45,-479.5944;Float;False;Property;_ToggleEnergyPan;ToggleEnergyPan;14;0;1;2;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;151;-1099.748,-355.4658;Float;False;Property;_EmissiveTint;EmissiveTint;5;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;163;355.6151,1401.752;Float;False;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0.0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;114;-965.4023,257.3173;Float;True;Property;_BurnRamp;Burn Ramp;8;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;182;-862.7013,-352.4317;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;147;-1081.259,-176.9386;Float;False;Property;_Emission;Emission;10;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;155;551.1348,1509.93;Float;False;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0.0;False;2;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-775.9634,43.36571;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;-796.7476,-352.4658;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;78;-597.2742,-694.2451;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;143;-271.9754,-100.6004;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ToggleSwitchNode;157;736.1191,1578.258;Float;False;Property;_ToggelMarks;ToggelMarks;12;0;1;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.ColorNode;133;-531.2509,-864.4244;Float;False;Property;_AlbedoTint;AlbedoTint;2;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;132;753.4756,91.7165;Float;False;765.1592;493.9802;Created by The Four Headed Cat @fourheadedcat - www.twitter.com/fourheadedcat;1;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-128.5345,-471.8568;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.OneMinusNode;162;140.6151,1654.752;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-48.73584,596.9774;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;154;336.6986,120.941;Float;False;2;2;0;COLOR;0.0,0,0;False;1;FLOAT3;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;131;-581.9014,-460.8792;Float;True;Property;_Normal;Normal;3;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;960.3318,143.4849;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Antoine/CharacterDissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;3;False;0;0;Masked;0.5;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;174;0;175;0
WireConnection;171;0;172;0
WireConnection;171;2;173;0
WireConnection;171;1;174;0
WireConnection;71;0;4;0
WireConnection;111;0;71;0
WireConnection;146;0;144;1
WireConnection;146;1;2;1
WireConnection;146;2;144;1
WireConnection;170;1;171;0
WireConnection;73;0;111;0
WireConnection;73;1;146;0
WireConnection;180;0;170;1
WireConnection;180;1;178;0
WireConnection;176;0;180;0
WireConnection;176;1;149;0
WireConnection;164;0;160;0
WireConnection;164;1;165;0
WireConnection;164;2;166;0
WireConnection;164;3;165;0
WireConnection;164;4;167;0
WireConnection;113;0;73;0
WireConnection;130;0;113;0
WireConnection;159;0;153;2
WireConnection;159;1;164;0
WireConnection;169;1;176;0
WireConnection;169;2;149;3
WireConnection;141;0;130;0
WireConnection;161;0;159;0
WireConnection;181;0;149;0
WireConnection;181;1;169;0
WireConnection;163;1;156;0
WireConnection;163;2;161;0
WireConnection;114;1;141;0
WireConnection;182;0;181;0
WireConnection;155;1;163;0
WireConnection;155;2;153;1
WireConnection;126;0;130;0
WireConnection;126;1;114;0
WireConnection;152;0;182;0
WireConnection;152;1;151;0
WireConnection;143;0;126;0
WireConnection;143;1;126;0
WireConnection;143;2;147;0
WireConnection;143;3;152;0
WireConnection;157;1;155;0
WireConnection;134;0;133;0
WireConnection;134;1;78;0
WireConnection;162;0;153;2
WireConnection;148;0;78;4
WireConnection;148;1;73;0
WireConnection;154;0;143;0
WireConnection;154;1;157;0
WireConnection;0;0;134;0
WireConnection;0;1;131;0
WireConnection;0;2;154;0
WireConnection;0;10;148;0
ASEEND*/
//CHKSM=4DA7064BB6749FA55048677A0F1E35A4E3DCAEC6