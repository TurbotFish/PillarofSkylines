// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Particle/Waterfall_Splash"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Splash_intensity("Splash_intensity", Float) = 0
		_Color("Color", Color) = (0,0,0,0)
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows nofog noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _Splash_intensity;
		uniform float _Cutoff = 0.2;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 uv_TextureSample0 = float4(v.texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw, 0 ,0);
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( tex2Dlod( _TextureSample0, uv_TextureSample0 ) * float4( ase_vertexNormal , 0.0 ) ) * ( _Splash_intensity * v.color.a ) ).rgb;
			v.normal = float3(-1,0,0);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = ( _Color * 0.0 ).rgb;
			o.Emission = ( _Color * 1.0 ).rgb;
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
-1913;29;1906;1004;1643.778;329.1954;1.3;True;True
Node;AmplifyShaderEditor.NormalVertexDataNode;6;-785,331;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-901,132;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Assets/Graph/Textures/FX/T_water_C_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-785.1998,513.8999;Float;False;Property;_Splash_intensity;Splash_intensity;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;20;-766.5782,610.7046;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-427.2784,515.8045;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-554,203;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;26;-576.778,25.70454;Float;False;Constant;_Float5;Float 5;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;24;-585.878,-50.99545;Float;False;Constant;_Float1;Float 1;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;22;-623.5779,-230.3955;Float;False;Property;_Color;Color;3;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;29;-874.1778,781.0045;Float;True;Property;_TextureSample1;Texture Sample 1;4;0;Assets/Graph/Textures/FX/T_water_D_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-406.1779,713.4045;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-314.1781,-2.895447;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-276,232;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-311.5781,-97.79546;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.Vector3Node;27;43.62195,411.8045;Float;False;Constant;_Vector0;Vector 0;3;0;-1,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;265.4,23.4;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Particle/Waterfall_Splash;False;False;False;False;False;False;False;False;False;True;False;True;False;False;True;False;False;Back;0;0;False;0;0;Masked;0.2;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;3;0
WireConnection;21;1;20;4
WireConnection;5;0;1;0
WireConnection;5;1;6;0
WireConnection;30;0;20;1
WireConnection;30;1;29;1
WireConnection;25;0;22;0
WireConnection;25;1;26;0
WireConnection;2;0;5;0
WireConnection;2;1;21;0
WireConnection;23;0;22;0
WireConnection;23;1;24;0
WireConnection;0;0;25;0
WireConnection;0;2;23;0
WireConnection;0;10;30;0
WireConnection;0;11;2;0
WireConnection;0;12;27;0
ASEEND*/
//CHKSM=9FC2CA83E16EE823C2AC30275C6DC09F2A37F3E3