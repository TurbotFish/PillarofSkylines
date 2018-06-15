// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Particle/Waterfall_Splash"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		_Texture1("Texture 1", 2D) = "white" {}
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample2("Texture Sample 2", 2D) = "white" {}
		_Splash_intensity("Splash_intensity", Float) = 0
		_Color("Color", Color) = (0,0,0,0)
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "DisableBatching" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows nofog noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float eyeDepth;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _Texture1;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform sampler2D _TextureSample2;
		uniform float4 _TextureSample2_ST;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _Splash_intensity;
		uniform float _Cutoff = 0.2;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
			float4 uv_TextureSample2 = float4(v.texcoord * _TextureSample2_ST.xy + _TextureSample2_ST.zw, 0 ,0);
			float4 uv_TextureSample0 = float4(v.texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw, 0 ,0);
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( ( tex2Dlod( _TextureSample2, uv_TextureSample2 ) * tex2Dlod( _TextureSample0, uv_TextureSample0 ) ) * float4( ase_vertexNormal , 0.0 ) ) * ( _Splash_intensity * v.color.a ) ).rgb;
			v.normal = float3(-1,0,0);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_42_0 = ( 0.0 + _ProjectionParams.y );
			float2 temp_cast_0 = (saturate( ( ( i.eyeDepth + -temp_output_42_0 ) / ( 600.0 - temp_output_42_0 ) ) )).xx;
			float temp_output_46_0 = ( 30.0 + _ProjectionParams.y );
			float4 lerpResult60 = lerp( ( _Color * 1.0 ) , tex2D( _Texture1, temp_cast_0 ) , saturate( ( ( i.eyeDepth + -temp_output_46_0 ) / ( 207.0 - temp_output_46_0 ) ) ));
			o.Emission = lerpResult60.rgb;
			o.Alpha = 1;
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			clip( ( i.vertexColor.r * tex2D( _TextureSample1, uv_TextureSample1 ).r ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;2114.91;1519.92;2.26338;True;True
Node;AmplifyShaderEditor.CommentaryNode;37;-757.8076,-567.7443;Float;False;297.1897;243;Correction for near plane clipping;1;40;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;36;-752.8635,-1457.551;Float;False;1047.541;403.52;Scale depth from start to end;8;54;51;47;45;44;43;42;39;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-685.6935,-1231.244;Float;False;Constant;_Float2;Float 2;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;38;-797.4316,-996.4274;Float;False;1047.541;403.52;Scale depth from start to end;8;56;53;52;50;49;48;46;41;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ProjectionParams;40;-673.2607,-513.7053;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;41;-730.2617,-770.1207;Float;False;Constant;_Float4;Float 4;0;0;30;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-442.4935,-1238.284;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;45;-281.0636,-1370.976;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;44;-287.9526,-1296.671;Float;False;Constant;_Float7;Float 7;3;0;600;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-487.0614,-777.1614;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SurfaceDepthNode;43;-734.8824,-1355.914;Float;False;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;47;-30.62252,-1272.198;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-33.21074,-1381.182;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;50;-325.6305,-909.8527;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SurfaceDepthNode;48;-779.4507,-894.7908;Float;False;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;49;-332.5204,-835.548;Float;False;Constant;_Float9;Float 9;2;0;207;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;34;-1249.892,-72.01172;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Assets/Graph/Textures/FX/T_water_C_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-1252,115.1;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Assets/Graph/Textures/FX/T_water_C_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;54;154.1153,-1248.911;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;53;-75.19063,-811.0753;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-77.77852,-920.059;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;57;363.1296,-1230.58;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;55;-237.981,-574.1932;Float;True;Property;_Texture1;Texture 1;1;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.ColorNode;22;-623.5779,-230.3955;Float;False;Property;_Color;Color;5;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;24;-585.878,-50.99545;Float;False;Constant;_Float1;Float 1;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;6;-785,331;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;56;109.5473,-787.7877;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-857.2915,59.28827;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;3;-795.8306,489.0948;Float;False;Property;_Splash_intensity;Splash_intensity;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;20;-768.35,573.497;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-475.1167,400.6383;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;29;-874.1778,781.0045;Float;True;Property;_TextureSample1;Texture Sample 1;6;0;Assets/Graph/Textures/FX/T_water_D_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-554,203;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-311.5781,-97.79546;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SaturateNode;59;346.8796,-769.457;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;58;36.48708,-571.8196;Float;True;Property;_TextureSample4;Texture Sample 4;30;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;60;137.3406,-194.8832;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-276,232;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-406.1779,713.4045;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;27;286.8816,437.7688;Float;False;Constant;_Vector0;Vector 0;3;0;-1,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;265.4,23.4;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Particle/Waterfall_Splash;False;False;False;False;False;False;False;False;False;True;False;True;False;True;True;False;False;Back;0;0;False;0;0;Custom;0.2;True;True;0;True;TransparentCutout;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;42;0;39;0
WireConnection;42;1;40;2
WireConnection;45;0;42;0
WireConnection;46;0;41;0
WireConnection;46;1;40;2
WireConnection;47;0;44;0
WireConnection;47;1;42;0
WireConnection;51;0;43;0
WireConnection;51;1;45;0
WireConnection;50;0;46;0
WireConnection;54;0;51;0
WireConnection;54;1;47;0
WireConnection;53;0;49;0
WireConnection;53;1;46;0
WireConnection;52;0;48;0
WireConnection;52;1;50;0
WireConnection;57;0;54;0
WireConnection;56;0;52;0
WireConnection;56;1;53;0
WireConnection;35;0;34;0
WireConnection;35;1;1;0
WireConnection;21;0;3;0
WireConnection;21;1;20;4
WireConnection;5;0;35;0
WireConnection;5;1;6;0
WireConnection;23;0;22;0
WireConnection;23;1;24;0
WireConnection;59;0;56;0
WireConnection;58;0;55;0
WireConnection;58;1;57;0
WireConnection;60;0;23;0
WireConnection;60;1;58;0
WireConnection;60;2;59;0
WireConnection;2;0;5;0
WireConnection;2;1;21;0
WireConnection;30;0;20;1
WireConnection;30;1;29;1
WireConnection;0;2;60;0
WireConnection;0;10;30;0
WireConnection;0;11;2;0
WireConnection;0;12;27;0
ASEEND*/
//CHKSM=87A66306E9A7EA19C65810616063B59562E4CFD9