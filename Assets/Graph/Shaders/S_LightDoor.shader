// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Lightdoor"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_PlayerPos("PlayerPos", Color) = (0,0,0,0)
		_Emissive("Emissive", Float) = 3.5
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "DisableBatching" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow noambient nolightmap  nofog noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
		};

		uniform float _Emissive;
		uniform float4 _PlayerPos;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 temp_cast_0 = (0.0).xxx;
			o.Albedo = temp_cast_0;
			float2 appendResult22 = (float2(_PlayerPos.r , _PlayerPos.b));
			float4 transform27 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float2 appendResult23 = (float2(transform27.x , transform27.z));
			float temp_output_87_0 = (0.5 + (( 1.0 - ( ( distance( appendResult22 , appendResult23 ) - 20.0 ) * 0.02 ) ) - 0.0) * (1.0 - 0.5) / (1.0 - 0.0));
			float lerpResult85 = lerp( 0.0 , _Emissive , ( step( ( 1.0 - i.texcoord_0.x ) , temp_output_87_0 ) * step( i.texcoord_0.x , temp_output_87_0 ) ));
			float3 temp_cast_1 = (lerpResult85).xxx;
			o.Emission = temp_cast_1;
			o.Metallic = 1.0;
			o.Smoothness = 0.0;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;352.2313;-982.9131;1;True;True
Node;AmplifyShaderEditor.ColorNode;21;-1747.256,1952.001;Float;False;Property;_PlayerPos;PlayerPos;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;27;-1490.923,2124.384;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;20;-1531.17,1958.099;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;23;-1264.877,2139.817;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.DynamicAppendNode;22;-1261.641,1960.538;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.DistanceOpNode;17;-1078.702,2067.862;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;93;-1075.001,2192.604;Float;False;Constant;_Float1;Float 1;1;0;20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;92;-909.9401,2092.513;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;96;-888.8684,2215.431;Float;False;Constant;_Float3;Float 3;1;0;0.02;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-732.587,2089.002;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;91;-350.1774,2190.612;Float;False;Constant;_Float6;Float 6;1;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;94;-541.1863,2099.538;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;-203.1023,1797.552;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;89;-335.1774,2022.612;Float;False;Constant;_Float2;Float 2;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;90;-340.1774,2099.612;Float;False;Constant;_Float5;Float 5;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;80;59.44153,1689.359;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;87;-142.8671,2028.019;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;81;216.4415,1674.359;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StepOpNode;78;222.4415,1887.359;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;458.4415,1776.359;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;69;678.454,1305.726;Float;False;Property;_Emissive;Emissive;2;0;3.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;86;691.2399,1412.239;Float;False;Constant;_Float4;Float 4;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;85;881.3103,1378.732;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;98;704.0263,1171.228;Float;False;Constant;_Float8;Float 8;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;99;856.3328,1643.67;Float;False;Constant;_Float9;Float 9;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;97;857.1617,1563.708;Float;False;Constant;_Float7;Float 7;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1081.489,1402.463;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Lightdoor;False;False;False;False;True;False;True;False;False;True;False;True;False;True;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;21;0
WireConnection;23;0;27;1
WireConnection;23;1;27;3
WireConnection;22;0;20;0
WireConnection;22;1;20;2
WireConnection;17;0;22;0
WireConnection;17;1;23;0
WireConnection;92;0;17;0
WireConnection;92;1;93;0
WireConnection;95;0;92;0
WireConnection;95;1;96;0
WireConnection;94;0;95;0
WireConnection;80;0;76;1
WireConnection;87;0;94;0
WireConnection;87;1;89;0
WireConnection;87;2;90;0
WireConnection;87;3;91;0
WireConnection;87;4;90;0
WireConnection;81;0;80;0
WireConnection;81;1;87;0
WireConnection;78;0;76;1
WireConnection;78;1;87;0
WireConnection;84;0;81;0
WireConnection;84;1;78;0
WireConnection;85;0;86;0
WireConnection;85;1;69;0
WireConnection;85;2;84;0
WireConnection;0;0;98;0
WireConnection;0;2;85;0
WireConnection;0;3;97;0
WireConnection;0;4;99;0
ASEEND*/
//CHKSM=A41BE494A5E81767077896801C08E47F63491687