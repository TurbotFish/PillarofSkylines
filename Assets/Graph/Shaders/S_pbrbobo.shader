// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/PBR"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_BaseColor("BaseColor", 2D) = "white" {}
		_RMAo("RMAo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_Roughness("Roughness", Float) = 0.15
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

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _BaseColor;
		uniform float4 _BaseColor_ST;
		uniform sampler2D _RMAo;
		uniform float4 _RMAo_ST;
		uniform float _Roughness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,1.0 );
			float2 uv_BaseColor = i.uv_texcoord * _BaseColor_ST.xy + _BaseColor_ST.zw;
			o.Albedo = tex2D( _BaseColor, uv_BaseColor ).rgb;
			float2 uv_RMAo = i.uv_texcoord * _RMAo_ST.xy + _RMAo_ST.zw;
			float4 tex2DNode4 = tex2D( _RMAo, uv_RMAo );
			o.Metallic = tex2DNode4.g;
			o.Smoothness = ( 1.0 - ( tex2DNode4.r + _Roughness ) );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
1927;29;1906;1004;1192;330;1;True;True
Node;AmplifyShaderEditor.SamplerNode;4;-723,257;Float;True;Property;_RMAo;RMAo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-434,435;Float;False;Property;_Roughness;Roughness;3;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-905,94;Float;False;Constant;_Float0;Float 0;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-263,361;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-733,44;Float;True;Property;_Normal;Normal;2;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;5;-285,274;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-733,-184;Float;True;Property;_BaseColor;BaseColor;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/PBR;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;4;1
WireConnection;6;1;7;0
WireConnection;2;5;3;0
WireConnection;5;0;6;0
WireConnection;0;0;1;0
WireConnection;0;1;2;0
WireConnection;0;3;4;2
WireConnection;0;4;5;0
ASEEND*/
//CHKSM=52036369F8294CB9EA011FA05098DE45AD04728C