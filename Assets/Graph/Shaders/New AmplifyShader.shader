// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Rotate"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Speed("Speed", Range( 0 , 0.1)) = 0.05
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			fixed filler;
		};

		uniform float _Speed;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 appendResult36 = (float4(1.0 , 0.0 , 0.0 , 0.0));
			float mulTime59 = _Time.y * _Speed;
			float temp_output_60_0 = (0.0 + (frac( mulTime59 ) - 0.0) * (90.0 - 0.0) / (1.0 - 0.0));
			float temp_output_32_0 = cos( temp_output_60_0 );
			float temp_output_33_0 = sin( temp_output_60_0 );
			float4 appendResult39 = (float4(0.0 , temp_output_32_0 , -temp_output_33_0 , 0.0));
			float4 appendResult40 = (float4(0.0 , temp_output_33_0 , temp_output_32_0 , 0.0));
			float4 appendResult91 = (float4(temp_output_32_0 , 0.0 , temp_output_33_0 , 0.0));
			float4 appendResult92 = (float4(0.0 , 1.0 , 0.0 , 0.0));
			float4 appendResult93 = (float4(-temp_output_33_0 , 0.0 , temp_output_32_0 , 0.0));
			float4 appendResult98 = (float4(temp_output_32_0 , -temp_output_33_0 , 0.0 , 0.0));
			float4 appendResult97 = (float4(temp_output_33_0 , temp_output_32_0 , 0.0 , 0.0));
			float4 appendResult96 = (float4(0.0 , 0.0 , 1.0 , 0.0));
			v.vertex.xyz += mul( float4( ase_vertex3Pos , 0.0 ), ( ( float4x4(appendResult36, appendResult39, appendResult40, float4( 0,0,0,0 )) + float4x4(appendResult91, appendResult92, appendResult93, float4( 0,0,0,0 )) ) + float4x4(appendResult98, appendResult97, appendResult96, float4( 0,0,0,0 )) ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1915;68;1906;1004;1459.356;876.5431;1.878148;True;True
Node;AmplifyShaderEditor.CommentaryNode;105;-2680.534,251.1285;Float;False;794.0994;412.7998;Abgle;7;60;63;62;61;58;59;64;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-2633.714,538.4121;Float;False;Property;_Speed;Speed;0;0;0.05;0;0.1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;59;-2336.635,540.3284;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;62;-2331.435,375.2284;Float;False;Constant;_Float4;Float 4;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;61;-2330.135,301.1285;Float;False;Constant;_Float3;Float 3;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;63;-2332.735,451.9283;Float;False;Constant;_Float5;Float 5;1;0;90;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.FractNode;58;-2132.535,541.6282;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;60;-2084.435,332.3284;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;104;-1732.455,72.6209;Float;False;377.9998;387;Base values;5;33;38;41;32;37;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SinOpNode;33;-1669.455,272.6209;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;101;-1009.395,-493.3248;Float;False;516.4427;501.9999;X;4;36;39;40;34;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;102;-998.1564,86.81382;Float;False;516.4426;501.9999;Y;4;91;92;93;94;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1674.455,192.6209;Float;False;Constant;_Float0;Float 0;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CosOpNode;32;-1667.455,349.6209;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;38;-1682.455,122.6209;Float;False;Constant;_Float1;Float 1;0;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;41;-1518.455,317.6209;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;103;-977.3499,608.0391;Float;False;516.4428;502;Z;4;96;97;98;99;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;93;-937.1565,409.8138;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;91;-948.1564,136.8138;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;40;-948.3951,-170.3248;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;92;-941.1565,276.8138;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;36;-959.3951,-443.3248;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;39;-952.3951,-303.3249;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;97;-920.3499,798.0391;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.MatrixFromVectors;34;-752.9523,-322.3249;Float;False;FLOAT4x4;4;0;FLOAT4;0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT4;0,0,0;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4x4
Node;AmplifyShaderEditor.MatrixFromVectors;94;-741.7138,257.8138;Float;False;FLOAT4x4;4;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4x4
Node;AmplifyShaderEditor.DynamicAppendNode;98;-927.3499,658.0391;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;96;-916.3499,931.0391;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-164.3215,136.4751;Float;False;2;2;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT4x4;0.0;False;1;FLOAT4x4
Node;AmplifyShaderEditor.MatrixFromVectors;99;-731.6411,773.6721;Float;False;FLOAT4x4;4;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4x4
Node;AmplifyShaderEditor.PosVertexDataNode;113;204.1644,42.82319;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;100;16.07491,177.6968;Float;False;2;2;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT4x4;0.0;False;1;FLOAT4x4
Node;AmplifyShaderEditor.VectorFromMatrixNode;111;217.3066,194.7172;Float;False;Row;0;1;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;122;194.9124,-305.3679;Float;False;Constant;_Vector0;Vector 0;1;0;1,1,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;112;-2130.556,108.2018;Float;False;Constant;_Float6;Float 6;1;0;46;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;116;211.4451,-106.8873;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;462.9443,197.3468;Float;False;2;2;0;FLOAT3;0.0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT4x4;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;652.4172,-26.47375;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Rotate;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;59;0;64;0
WireConnection;58;0;59;0
WireConnection;60;0;58;0
WireConnection;60;1;61;0
WireConnection;60;2;62;0
WireConnection;60;3;61;0
WireConnection;60;4;63;0
WireConnection;33;0;60;0
WireConnection;32;0;60;0
WireConnection;41;0;33;0
WireConnection;93;0;41;0
WireConnection;93;1;37;0
WireConnection;93;2;32;0
WireConnection;91;0;32;0
WireConnection;91;1;37;0
WireConnection;91;2;33;0
WireConnection;40;0;37;0
WireConnection;40;1;33;0
WireConnection;40;2;32;0
WireConnection;92;0;37;0
WireConnection;92;1;38;0
WireConnection;92;2;37;0
WireConnection;36;0;38;0
WireConnection;36;1;37;0
WireConnection;36;2;37;0
WireConnection;39;0;37;0
WireConnection;39;1;32;0
WireConnection;39;2;41;0
WireConnection;97;0;33;0
WireConnection;97;1;32;0
WireConnection;97;2;37;0
WireConnection;34;0;36;0
WireConnection;34;1;39;0
WireConnection;34;2;40;0
WireConnection;94;0;91;0
WireConnection;94;1;92;0
WireConnection;94;2;93;0
WireConnection;98;0;32;0
WireConnection;98;1;41;0
WireConnection;98;2;37;0
WireConnection;96;0;37;0
WireConnection;96;1;37;0
WireConnection;96;2;38;0
WireConnection;95;0;34;0
WireConnection;95;1;94;0
WireConnection;99;0;98;0
WireConnection;99;1;97;0
WireConnection;99;2;96;0
WireConnection;100;0;95;0
WireConnection;100;1;99;0
WireConnection;111;0;99;0
WireConnection;115;0;113;0
WireConnection;115;1;100;0
WireConnection;0;11;115;0
ASEEND*/
//CHKSM=91254B4C3B7D219640A6EC92F0D0F33FCA761A76