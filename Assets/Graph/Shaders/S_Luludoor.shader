// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Luludoor"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_T_Luludoor_C("T_Luludoor_C", 2D) = "white" {}
		_T_Luludoor_N("T_Luludoor_N", 2D) = "bump" {}
		_T_Luludoor_R("T_Luludoor_R", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _T_Luludoor_N;
		uniform float4 _T_Luludoor_N_ST;
		uniform sampler2D _T_Luludoor_C;
		uniform float4 _T_Luludoor_C_ST;
		uniform sampler2D _T_Luludoor_R;
		uniform float4 _T_Luludoor_R_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_T_Luludoor_N = i.uv_texcoord * _T_Luludoor_N_ST.xy + _T_Luludoor_N_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _T_Luludoor_N, uv_T_Luludoor_N ) ,1.0 );
			float2 uv_T_Luludoor_C = i.uv_texcoord * _T_Luludoor_C_ST.xy + _T_Luludoor_C_ST.zw;
			o.Albedo = ( tex2D( _T_Luludoor_C, uv_T_Luludoor_C ) * 0.6 ).rgb;
			float2 uv_T_Luludoor_R = i.uv_texcoord * _T_Luludoor_R_ST.xy + _T_Luludoor_R_ST.zw;
			float4 tex2DNode3 = tex2D( _T_Luludoor_R, uv_T_Luludoor_R );
			o.Metallic = tex2DNode3.g;
			o.Smoothness = ( ( 1.0 - tex2DNode3.r ) + -0.2 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;952.5004;378.1132;1;True;True
Node;AmplifyShaderEditor.SamplerNode;3;-615,-155;Float;True;Property;_T_Luludoor_R;T_Luludoor_R;2;0;Assets/Graph/Textures/PROPS/T_Luludoor_R.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-622,56;Float;True;Property;_T_Luludoor_C;T_Luludoor_C;0;0;Assets/Graph/Textures/PROPS/T_Luludoor_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-147.5004,193.8868;Float;False;Constant;_Float1;Float 1;3;0;-0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;-282.5004,-194.1132;Float;False;Constant;_Float2;Float 2;3;0;0.6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;4;-182,102;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-750,329;Float;False;Constant;_Float0;Float 0;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;6;10.49957,137.8868;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-81.50043,-191.1132;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;2;-575,293;Float;True;Property;_T_Luludoor_N;T_Luludoor_N;1;0;Assets/Graph/Textures/PROPS/T_Luludoor_N.png;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;174,-6;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Luludoor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;3;1
WireConnection;6;0;4;0
WireConnection;6;1;8;0
WireConnection;9;0;1;0
WireConnection;9;1;10;0
WireConnection;2;5;5;0
WireConnection;0;0;9;0
WireConnection;0;1;2;0
WireConnection;0;3;3;2
WireConnection;0;4;6;0
ASEEND*/
//CHKSM=47017712E4954D45C522EA281D5AA6D892C6728A