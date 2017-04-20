// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/WaterSample"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_WaterNormal("Water Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 0
		_DeepColor("Deep Color", Color) = (0,0,0,0)
		_ShalowColor("Shalow Color", Color) = (1,1,1,0)
		_WaterDepth("Water Depth", Float) = 0
		_WaterFalloff("Water Falloff", Float) = 0
		_WaterSpecular("Water Specular", Float) = 0
		_WaterSmoothness("Water Smoothness", Float) = 0
		_Distortion("Distortion", Float) = 0.5
		_Foam("Foam", 2D) = "white" {}
		_FoamDepth("Foam Depth", Float) = 0
		_FoamFalloff("Foam Falloff", Float) = 0
		_FoamSpecular("Foam Specular", Float) = 0
		_FoamSmoothness("Foam Smoothness", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" }
		Cull Back
		ZTest LEqual
		GrabPass{ "_WaterGrab" }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular  
		struct Input
		{
			float2 uv_WaterNormal;
			float4 screenPos;
			float2 uv_Foam;
		};

		uniform half _NormalScale;
		uniform sampler2D _WaterNormal;
		uniform half4 _DeepColor;
		uniform half4 _ShalowColor;
		uniform sampler2D _CameraDepthTexture;
		uniform half _WaterDepth;
		uniform half _WaterFalloff;
		uniform half _FoamDepth;
		uniform half _FoamFalloff;
		uniform sampler2D _Foam;
		uniform sampler2D _WaterGrab;
		uniform half _Distortion;
		uniform half _WaterSpecular;
		uniform half _FoamSpecular;
		uniform half _WaterSmoothness;
		uniform half _FoamSmoothness;

		void surf( Input input , inout SurfaceOutputStandardSpecular output )
		{
			float3 temp_output_24_0 = BlendNormals( UnpackScaleNormal( tex2D( _WaterNormal,(abs( input.uv_WaterNormal+_Time[1] * float2(-0.03,0 )))) ,_NormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal,(abs( input.uv_WaterNormal+_Time[1] * float2(0.04,0.04 )))) ,_NormalScale ) );
			output.Normal = temp_output_24_0;
			float eyeDepth1 = LinearEyeDepth(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(input.screenPos)).r);
			float temp_output_89_0 = abs( ( eyeDepth1 - input.screenPos.w ) );
			float temp_output_94_0 = saturate( pow( ( temp_output_89_0 + _WaterDepth ) , _WaterFalloff ) );
			float temp_output_114_0 = ( saturate( pow( ( temp_output_89_0 + _FoamDepth ) , _FoamFalloff ) ) * tex2D( _Foam,(abs( input.uv_Foam+_Time[1] * float2(-0.01,0.01 )))).r );
			output.Albedo = lerp( lerp( lerp( _DeepColor , _ShalowColor , temp_output_94_0 ) , half4(1,1,1,0) , temp_output_114_0 ) , tex2D( _WaterGrab,( half3( ( float2( input.screenPos.x , input.screenPos.y ) / input.screenPos.w ) ,  0.0 ) + ( temp_output_24_0 * _Distortion ) ).xy) , temp_output_94_0 ).rgb;
			half3 temp_cast_5 = lerp( _WaterSpecular , _FoamSpecular , temp_output_114_0 );
			output.Specular = temp_cast_5;
			output.Smoothness = lerp( _WaterSmoothness , _FoamSmoothness , temp_output_114_0 );
			output.Occlusion = 0.0;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3002
438;92;1030;770;2620.71;835.5839;1.900001;True;False
Node;AmplifyShaderEditor.CommentaryNode;159;-863.7005,-467.5007;Float;1113.201;508.3005;Depths controls and colors;11;87;94;12;13;156;157;11;88;10;6;143
Node;AmplifyShaderEditor.CommentaryNode;153;-843.9032,402.718;Float;1083.102;484.2006;Foam controls and texture;9;116;105;106;115;111;110;112;113;114
Node;AmplifyShaderEditor.CommentaryNode;152;-2053.601,-256.6997;Float;828.5967;315.5001;Screen depth difference to get intersection and fading effect with terrain and obejcts;4;89;2;1;3
Node;AmplifyShaderEditor.CommentaryNode;151;-935.9057,-1082.484;Float;1281.603;457.1994;Blend panning normals to fake noving ripples;7;19;23;24;21;22;17;48
Node;AmplifyShaderEditor.CommentaryNode;150;467.1957,-1501.783;Float;985.6011;418.6005;Get screen color for refraction and disturbe it with normals;8;67;96;66;68;97;98;65;149
Node;AmplifyShaderEditor.AppendNode;67;712.195,-1440.384;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;1041.296,-1346.683;Float;FLOAT2;0.0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.ScreenPosInputsNode;66;517.1959,-1451.783;Float;False
Node;AmplifyShaderEditor.SimpleDivideOpNode;68;885.8966,-1387.683;Float;FLOAT2;0.0,0;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;710.096,-1203.183;Float;Property;_Distortion;Distortion;9;0.5;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;888.1974,-1279.783;Float;FLOAT3;0.0,0,0;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;65;1232.797,-1350.483;Float;Global;_WaterGrab;WaterGrab;-1;Object;-1;FLOAT2;0,0
Node;AmplifyShaderEditor.SamplerNode;23;-269.2061,-1024.185;Float;Property;_Normal2;Normal2;-1;None;True;0;True;bump;Auto;True;Instance;17;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.BlendNormalsNode;24;170.697,-879.6849;Float;FLOAT3;0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-885.9058,-1005.185;Float;0;17;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.SamplerNode;17;-256.3054,-814.2847;Float;Property;_WaterNormal;Water Normal;1;None;True;0;True;bump;Auto;True;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;48;-557.3063,-795.3858;Float;Property;_NormalScale;Normal Scale;2;0;0;0
Node;AmplifyShaderEditor.AbsOpNode;89;-1389.004,-112.5834;Float;FLOAT;0.0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-2003.601,-153.1996;Float;False
Node;AmplifyShaderEditor.ScreenDepthNode;1;-1798.601,-206.6997;Float;0;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;3;-1574.201,-110.3994;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.PannerNode;116;-573.2014,720.3181;Float;-0.01;0.01;FLOAT2;0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SamplerNode;105;-304.4021,674.9185;Float;Property;_Foam;Foam;15;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.TextureCoordinatesNode;106;-793.9032,700.119;Float;0;105;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-542.0016,452.718;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;111;-722.2024,526.6185;Float;Property;_FoamDepth;Foam Depth;16;0;0;0
Node;AmplifyShaderEditor.PowerNode;110;-357.2024,461.6185;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;112;-531.4025,588.5187;Float;Property;_FoamFalloff;Foam Falloff;17;0;0;0
Node;AmplifyShaderEditor.SaturateNode;113;-136.0011,509.618;Float;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;80.19891,604.0181;Float;FLOAT;0;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;155;-1106.507,7.515848;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;158;-1075.608,-163.0834;Float;FLOAT;0.0
Node;AmplifyShaderEditor.PowerNode;87;-455.8059,-118.1832;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SaturateNode;94;-249.5044,-96.98394;Float;FLOAT;0.0
Node;AmplifyShaderEditor.ColorNode;12;-697.5002,-417.5007;Float;Property;_DeepColor;Deep Color;3;0,0,0,0
Node;AmplifyShaderEditor.LerpOp;13;60.50008,-220.6998;Float;COLOR;0,0,0,0;COLOR;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;156;-131.5076,-325.9835;Float;COLOR;0.0,0,0,0
Node;AmplifyShaderEditor.WireNode;157;-149.1077,-261.9834;Float;COLOR;0.0,0,0,0
Node;AmplifyShaderEditor.ColorNode;11;-455.0999,-328.3;Float;Property;_ShalowColor;Shalow Color;4;1,1,1,0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;-632.0056,-204.5827;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;10;-636.2005,-79.20019;Float;Property;_WaterFalloff;Water Falloff;6;0;0;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-813.7005,-128.1996;Float;Property;_WaterDepth;Water Depth;5;0;0;0
Node;AmplifyShaderEditor.WireNode;149;487.4943,-1188.882;Float;FLOAT3;0.0,0,0
Node;AmplifyShaderEditor.LerpOp;117;323.797,77.91843;Float;COLOR;0.0,0,0,0;COLOR;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.ColorNode;108;58.99682,146.0182;Float;Constant;_Color0;Color 0;-1;1,1,1,0
Node;AmplifyShaderEditor.LerpOp;93;1559.196,-1006.285;Float;COLOR;0,0,0,0;COLOR;0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;161;660.4934,-750.6837;Float;COLOR;0.0,0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;104;753.9969,-565.4819;Float;Property;_WaterSpecular;Water Specular;7;0;0;0
Node;AmplifyShaderEditor.LerpOp;130;955.7971,-465.8806;Float;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;131;756.1969,-467.1806;Float;Property;_FoamSpecular;Foam Specular;18;0;0;0
Node;AmplifyShaderEditor.LerpOp;133;1139.597,-182.68;Float;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;132;914.3978,-199.48;Float;Property;_FoamSmoothness;Foam Smoothness;19;0;0;0
Node;AmplifyShaderEditor.RangedFloatNode;26;920.1959,-279.1855;Float;Property;_WaterSmoothness;Water Smoothness;8;0;0;0
Node;AmplifyShaderEditor.WireNode;162;1312.293,-894.3823;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;143;95.69542,-321.0839;Float;FLOAT;0.0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1838.601,-748.1998;Half;True;2;Half;ASEMaterialInspector;StandardSpecular;ASESampleShaders/WaterSample;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;False;False;0;True;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;103;1644.795,-389.9843;Float;Constant;_Occlusion;Occlusion;-1;0;0;0
Node;AmplifyShaderEditor.WireNode;154;-922.7065,390.316;Float;FLOAT;0.0
Node;AmplifyShaderEditor.PannerNode;19;-610.9061,-919.9849;Float;0.04;0.04;FLOAT2;0,0;FLOAT;0.0
Node;AmplifyShaderEditor.PannerNode;22;-613.2062,-1032.484;Float;-0.03;0;FLOAT2;0,0;FLOAT;0.0
WireConnection;67;0;66;1
WireConnection;67;1;66;2
WireConnection;96;0;68;0
WireConnection;96;1;98;0
WireConnection;68;0;67;0
WireConnection;68;1;66;4
WireConnection;98;0;149;0
WireConnection;98;1;97;0
WireConnection;65;0;96;0
WireConnection;23;1;22;0
WireConnection;23;5;48;0
WireConnection;24;0;23;0
WireConnection;24;1;17;0
WireConnection;17;1;19;0
WireConnection;17;5;48;0
WireConnection;89;0;3;0
WireConnection;1;0;2;0
WireConnection;3;0;1;0
WireConnection;3;1;2;4
WireConnection;116;0;106;0
WireConnection;105;1;116;0
WireConnection;115;0;154;0
WireConnection;115;1;111;0
WireConnection;110;0;115;0
WireConnection;110;1;112;0
WireConnection;113;0;110;0
WireConnection;114;0;113;0
WireConnection;114;1;105;1
WireConnection;155;0;89;0
WireConnection;158;0;89;0
WireConnection;87;0;88;0
WireConnection;87;1;10;0
WireConnection;94;0;87;0
WireConnection;13;0;156;0
WireConnection;13;1;157;0
WireConnection;13;2;94;0
WireConnection;156;0;12;0
WireConnection;157;0;11;0
WireConnection;88;0;158;0
WireConnection;88;1;6;0
WireConnection;149;0;24;0
WireConnection;117;0;13;0
WireConnection;117;1;108;0
WireConnection;117;2;114;0
WireConnection;93;0;161;0
WireConnection;93;1;65;0
WireConnection;93;2;162;0
WireConnection;161;0;117;0
WireConnection;130;0;104;0
WireConnection;130;1;131;0
WireConnection;130;2;114;0
WireConnection;133;0;26;0
WireConnection;133;1;132;0
WireConnection;133;2;114;0
WireConnection;162;0;143;0
WireConnection;143;0;94;0
WireConnection;0;0;93;0
WireConnection;0;1;24;0
WireConnection;0;3;130;0
WireConnection;0;4;133;0
WireConnection;0;5;103;0
WireConnection;154;0;155;0
WireConnection;19;0;21;0
WireConnection;22;0;21;0
ASEEND*/
//CHKSM=22A2E9E308C0CF16A5E0681667DCB3ED2796A855
