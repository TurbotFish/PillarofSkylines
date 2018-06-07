// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Waterfall"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_GradientFog("GradientFog", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_Color("Color", Color) = (0.4584775,0.7794118,0.6898488,0)
		_Color2("Color2", Color) = (0.4584775,0.7794118,0.6898488,0)
		_Texture1("Texture 1", 2D) = "white" {}
		_TextureSample3("Texture Sample 3", 2D) = "white" {}
		_Tiling("Tiling", Vector) = (0,0,0,0)
		_TopFoam("TopFoam", 2D) = "white" {}
		_FoamDistortion("FoamDistortion", Range( 0 , 1)) = 0
		_FoamColor("FoamColor", Color) = (0.7843138,0.8862746,1,0)
		_TextureSample0("Texture Sample 0", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+1" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
			float2 texcoord_2;
			float2 texcoord_3;
			float eyeDepth;
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform sampler2D _Texture1;
		uniform float2 _Tiling;
		uniform float _FoamDistortion;
		uniform sampler2D _TopFoam;
		uniform float4 _Color2;
		uniform float4 _Color;
		uniform float4 _FoamColor;
		uniform sampler2D _GradientFog;
		uniform float _Smoothness;
		uniform sampler2D _TextureSample3;
		uniform float4 _TextureSample3_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 2,0.5 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_2.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_3.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner79 = ( ( i.texcoord_1 * _Tiling ) + 1.0 * _Time.y * float2( 0,0.2 ));
			float cos80 = cos( 0.0 );
			float sin80 = sin( 0.0 );
			float2 rotator80 = mul( panner79 - float2( 0,0 ) , float2x2( cos80 , -sin80 , sin80 , cos80 )) + float2( 0,0 );
			float4 tex2DNode76 = tex2D( _Texture1, rotator80 );
			float temp_output_150_0 = ( tex2DNode76.r * _FoamDistortion );
			float2 panner152 = ( i.texcoord_2 + 1.0 * _Time.y * float2( 0,0.1 ));
			float lerpResult140 = lerp( tex2D( _TopFoam, ( panner152 + temp_output_150_0 ) ).r , 0.0 , i.texcoord_3.y);
			float temp_output_146_0 = saturate( step( lerpResult140 , 0.08 ) );
			float3 lerpResult176 = lerp( UnpackNormal( tex2D( _TextureSample0, ( i.texcoord_0 + temp_output_150_0 ) ) ) , float3(0,0,1) , temp_output_146_0);
			o.Normal = lerpResult176;
			float4 lerpResult178 = lerp( _Color2 , _Color , saturate( ( lerpResult140 * 3.0 ) ));
			float4 lerpResult154 = lerp( lerpResult178 , _FoamColor , temp_output_146_0);
			float temp_output_54_0 = ( 0.0 + _ProjectionParams.y );
			float2 temp_cast_0 = (saturate( ( ( i.eyeDepth + -temp_output_54_0 ) / ( 600.0 - temp_output_54_0 ) ) )).xx;
			float temp_output_61_0 = ( 30.0 + _ProjectionParams.y );
			float4 lerpResult73 = lerp( lerpResult154 , tex2D( _GradientFog, temp_cast_0 ) , saturate( ( ( i.eyeDepth + -temp_output_61_0 ) / ( 207.0 - temp_output_61_0 ) ) ));
			o.Emission = lerpResult73.rgb;
			o.Smoothness = _Smoothness;
			float2 panner90 = ( ( i.texcoord_1 * _Tiling ) + 1.0 * _Time.y * float2( 0,0.5 ));
			float cos91 = cos( 0.0 );
			float sin91 = sin( 0.0 );
			float2 rotator91 = mul( panner90 - float2( 0,0 ) , float2x2( cos91 , -sin91 , sin91 , cos91 )) + float2( 0,0 );
			float temp_output_95_0 = ( tex2DNode76.b * tex2D( _Texture1, rotator91 ).b );
			float2 uv_TextureSample3 = i.uv_texcoord * _TextureSample3_ST.xy + _TextureSample3_ST.zw;
			float4 tex2DNode78 = tex2D( _TextureSample3, uv_TextureSample3 );
			float lerpResult107 = lerp( 15.0 , 0.0 , tex2DNode78.g);
			float lerpResult85 = lerp( 0.0 , saturate( ( temp_output_95_0 * lerpResult107 ) ) , tex2DNode78.r);
			o.Alpha = saturate( ( step( ( 1.0 - lerpResult85 ) , 0.4 ) * pow( lerpResult85 , 2.0 ) * ( 1.0 - step( lerpResult140 , 0.02 ) ) ) );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;1324.162;708.6888;1.364797;True;True
Node;AmplifyShaderEditor.Vector2Node;103;-2091.284,1480.948;Float;False;Property;_Tiling;Tiling;6;0;0,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-2133.663,1308.072;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-1732.005,1299.263;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;84;-1747.86,1431.388;Float;False;Constant;_Vector0;Vector 0;3;0;0,0.2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-1715.446,1573.729;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;92;-1731.301,1705.854;Float;False;Constant;_Vector1;Vector 1;3;0;0,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;79;-1539.984,1329.212;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;93;-1504.047,1742.849;Float;False;Constant;_Float11;Float 11;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;90;-1523.425,1603.677;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RotatorNode;80;-1339.155,1339.781;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;77;-1372.626,1096.672;Float;True;Property;_Texture1;Texture 1;4;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RotatorNode;91;-1322.596,1614.248;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TextureCoordinatesNode;139;-1922.249,-50.03288;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;76;-1080.191,1216.466;Float;True;Property;_TextureSample2;Texture Sample 2;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;153;-1999.43,66.40881;Float;False;Constant;_Vector2;Vector 2;7;0;0,0.1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;151;-1799.43,129.4088;Float;False;Property;_FoamDistortion;FoamDistortion;8;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;51;-328.9785,-514.7522;Float;False;297.1897;243;Correction for near plane clipping;1;52;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;49;-324.0344,-1404.559;Float;False;1047.541;403.52;Scale depth from start to end;8;63;60;59;58;57;56;54;53;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-873.1448,1744.883;Float;False;Constant;_Float1;Float 1;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;108;-874.5892,1666.878;Float;False;Constant;_Float3;Float 3;4;0;15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;88;-1078.43,1424.341;Float;True;Property;_TextureSample4;Texture Sample 4;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;78;-1127.266,1935.608;Float;True;Property;_TextureSample3;Texture Sample 3;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;152;-1644.43,-34.59119;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-1498.647,123.6754;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-748.9998,1396.155;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;107;-686.8003,1697.213;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ProjectionParams;52;-244.4316,-460.7132;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;53;-256.8644,-1178.252;Float;False;Constant;_Float2;Float 2;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;142;-1454.956,-6.245043;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;50;-368.6025,-943.4351;Float;False;1047.541;403.52;Scale depth from start to end;8;69;68;66;65;64;62;61;55;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-483.1213,1676.989;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-13.66447,-1185.292;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;55;-301.4326,-717.1284;Float;False;Constant;_Float4;Float 4;0;0;30;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;141;-1237.673,190.4817;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;138;-1325.057,-8.997484;Float;True;Property;_TopFoam;TopFoam;7;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;181;-528.692,-230.0742;Float;False;Constant;_Float5;Float 5;11;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-58.23233,-724.1691;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;109;-337.2236,1692.879;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;57;147.7654,-1317.984;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;140;-891.4,75.51342;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SurfaceDepthNode;58;-306.0534,-1302.922;Float;False;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;140.8764,-1243.679;Float;False;Constant;_Float6;Float 6;3;0;600;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;85;-131.2333,1545.797;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0,0,0,0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;144;-824.8057,-6.855028;Float;False;Constant;_Float16;Float 16;6;0;0.08;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;165;203.4432,1291.125;Float;False;Constant;_Float0;Float 0;8;0;0.02;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;62;96.30857,-782.5558;Float;False;Constant;_Float8;Float 8;2;0;207;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;59;398.2065,-1219.206;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;64;103.1985,-856.8605;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;60;395.6183,-1328.19;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SurfaceDepthNode;65;-350.6216,-841.7985;Float;False;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;170;-52.24007,444.2435;Float;False;Constant;_Vector3;Vector 3;9;0;2,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;180;-353.045,-194.9833;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;66;351.0505,-867.0668;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;126;294.8327,1650.901;Float;False;Constant;_Float14;Float 14;7;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;47;131.3095,-288.5853;Float;False;Property;_Color;Color;2;0;0.4584775,0.7794118,0.6898488,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;169;156.439,425.7317;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;63;582.9442,-1195.919;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;179;126.3676,-122.6197;Float;False;Property;_Color2;Color2;3;0;0.4584775,0.7794118,0.6898488,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;123;211.8182,1502.458;Float;False;Constant;_Float12;Float 12;7;0;0.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;182;-132.6942,-192.7955;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;68;353.6384,-758.083;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;122;205.2659,1402.424;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;164;408.2574,1132.303;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;143;-447.2409,31.35285;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;69;538.3763,-734.7955;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;172;425.0627,481.4542;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.PowerNode;125;465.6645,1606.77;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;167;660.8772,1268.859;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;71;190.848,-521.2011;Float;True;Property;_GradientFog;GradientFog;0;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SaturateNode;146;-185.1143,42.25489;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;178;496.4972,-177.9924;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StepOpNode;121;417.2884,1364.502;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;67;791.9586,-1177.588;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;149;417.5669,110.8632;Float;False;Property;_FoamColor;FoamColor;9;0;0.7843138,0.8862746,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;177;665.5732,630.2931;Float;True;Constant;_Vector4;Vector 4;10;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;154;707.099,-47.54692;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;72;465.3161,-518.8275;Float;True;Property;_TextureSample1;Texture Sample 1;30;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;168;573.7971,408.9027;Float;True;Property;_TextureSample0;Texture Sample 0;10;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;643.334,1372.657;Float;True;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;70;775.7086,-716.4647;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;145;-613.118,139.3343;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;176;978.8979,462.6219;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SaturateNode;127;869.9663,1374.723;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;75;1225.16,292.2001;Float;False;Property;_Smoothness;Smoothness;1;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;97;-546.4088,1456.051;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;73;1007.845,-26.69855;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;98;-736.668,1531.802;Float;False;Constant;_Float13;Float 13;6;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1493.658,81.32716;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Waterfall;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;0;False;0;0;Transparent;0.5;True;False;1;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;86;0;81;0
WireConnection;86;1;103;0
WireConnection;89;0;81;0
WireConnection;89;1;103;0
WireConnection;79;0;86;0
WireConnection;79;2;84;0
WireConnection;90;0;89;0
WireConnection;90;2;92;0
WireConnection;80;0;79;0
WireConnection;80;2;93;0
WireConnection;91;0;90;0
WireConnection;91;2;93;0
WireConnection;76;0;77;0
WireConnection;76;1;80;0
WireConnection;88;0;77;0
WireConnection;88;1;91;0
WireConnection;152;0;139;0
WireConnection;152;2;153;0
WireConnection;150;0;76;1
WireConnection;150;1;151;0
WireConnection;95;0;76;3
WireConnection;95;1;88;3
WireConnection;107;0;108;0
WireConnection;107;1;105;0
WireConnection;107;2;78;2
WireConnection;142;0;152;0
WireConnection;142;1;150;0
WireConnection;104;0;95;0
WireConnection;104;1;107;0
WireConnection;54;0;53;0
WireConnection;54;1;52;2
WireConnection;138;1;142;0
WireConnection;61;0;55;0
WireConnection;61;1;52;2
WireConnection;109;0;104;0
WireConnection;57;0;54;0
WireConnection;140;0;138;1
WireConnection;140;2;141;2
WireConnection;85;1;109;0
WireConnection;85;2;78;1
WireConnection;59;0;56;0
WireConnection;59;1;54;0
WireConnection;64;0;61;0
WireConnection;60;0;58;0
WireConnection;60;1;57;0
WireConnection;180;0;140;0
WireConnection;180;1;181;0
WireConnection;66;0;65;0
WireConnection;66;1;64;0
WireConnection;169;0;170;0
WireConnection;63;0;60;0
WireConnection;63;1;59;0
WireConnection;182;0;180;0
WireConnection;68;0;62;0
WireConnection;68;1;61;0
WireConnection;122;0;85;0
WireConnection;164;0;140;0
WireConnection;164;1;165;0
WireConnection;143;0;140;0
WireConnection;143;1;144;0
WireConnection;69;0;66;0
WireConnection;69;1;68;0
WireConnection;172;0;169;0
WireConnection;172;1;150;0
WireConnection;125;0;85;0
WireConnection;125;1;126;0
WireConnection;167;0;164;0
WireConnection;146;0;143;0
WireConnection;178;0;179;0
WireConnection;178;1;47;0
WireConnection;178;2;182;0
WireConnection;121;0;122;0
WireConnection;121;1;123;0
WireConnection;67;0;63;0
WireConnection;154;0;178;0
WireConnection;154;1;149;0
WireConnection;154;2;146;0
WireConnection;72;0;71;0
WireConnection;72;1;67;0
WireConnection;168;1;172;0
WireConnection;124;0;121;0
WireConnection;124;1;125;0
WireConnection;124;2;167;0
WireConnection;70;0;69;0
WireConnection;145;0;140;0
WireConnection;176;0;168;0
WireConnection;176;1;177;0
WireConnection;176;2;146;0
WireConnection;127;0;124;0
WireConnection;97;0;95;0
WireConnection;97;1;98;0
WireConnection;73;0;154;0
WireConnection;73;1;72;0
WireConnection;73;2;70;0
WireConnection;0;1;176;0
WireConnection;0;2;73;0
WireConnection;0;4;75;0
WireConnection;0;9;127;0
ASEEND*/
//CHKSM=055C6A0362FEB1F455669B55BCBFEC4EF33D4BE0