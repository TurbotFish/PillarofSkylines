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
		_T_Luludoor_H("T_Luludoor_H", 2D) = "white" {}
		_Emissive("Emissive", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
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
		uniform sampler2D _T_Luludoor_H;
		uniform float4 _T_Luludoor_H_ST;
		uniform float _Emissive;
		uniform sampler2D _T_Luludoor_R;
		uniform float4 _T_Luludoor_R_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_T_Luludoor_N = i.uv_texcoord * _T_Luludoor_N_ST.xy + _T_Luludoor_N_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _T_Luludoor_N, uv_T_Luludoor_N ) ,1.0 );
			float2 uv_T_Luludoor_C = i.uv_texcoord * _T_Luludoor_C_ST.xy + _T_Luludoor_C_ST.zw;
			o.Albedo = ( tex2D( _T_Luludoor_C, uv_T_Luludoor_C ) * 0.6 ).rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			float2 uv_T_Luludoor_H = i.uv_texcoord * _T_Luludoor_H_ST.xy + _T_Luludoor_H_ST.zw;
			float4 lerpResult14 = lerp( ( float4(1,0.8230372,0.3006055,0) * 1.5 ) , temp_cast_1 , step( tex2D( _T_Luludoor_H, uv_T_Luludoor_H ).r , 0.5 ));
			o.Emission = ( lerpResult14 * _Emissive ).rgb;
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
-1913;29;1906;1004;1382.294;-48.42251;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;15;-696.927,491.9988;Float;False;Constant;_Float4;Float 4;4;0;1.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;17;-716.4271,319.0988;Float;False;Constant;_Color0;Color 0;4;0;1,0.8230372,0.3006055,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;11;-1077.413,634.377;Float;True;Property;_T_Luludoor_H;T_Luludoor_H;3;0;Assets/Graph/Textures/PROPS/T_Luludoor_H.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-944.2437,834.5134;Float;False;Constant;_Float3;Float 3;4;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-821.2721,-431.7702;Float;True;Property;_T_Luludoor_R;T_Luludoor_R;2;0;Assets/Graph/Textures/PROPS/T_Luludoor_R.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StepOpNode;12;-718.2437,662.5134;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-699.527,562.1988;Float;False;Constant;_Float5;Float 5;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-427.8271,438.6988;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;14;-480.6436,617.8134;Float;False;3;0;COLOR;0.0;False;1;COLOR;0.0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;5;-956.2721,52.22981;Float;False;Constant;_Float0;Float 0;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;-488.7725,-470.8834;Float;False;Constant;_Float2;Float 2;3;0;0.6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;-455.2942,782.4225;Float;False;Property;_Emissive;Emissive;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-353.7725,-82.88336;Float;False;Constant;_Float1;Float 1;3;0;-0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;4;-388.2721,-174.7702;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-828.2721,-220.7702;Float;True;Property;_T_Luludoor_C;T_Luludoor_C;0;0;Assets/Graph/Textures/PROPS/T_Luludoor_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-241.2942,623.4225;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-195.7726,-138.8834;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-287.7726,-467.8834;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;2;-781.2721,16.22981;Float;True;Property;_T_Luludoor_N;T_Luludoor_N;1;0;Assets/Graph/Textures/PROPS/T_Luludoor_N.png;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;174,-6;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Luludoor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;11;1
WireConnection;12;1;13;0
WireConnection;18;0;17;0
WireConnection;18;1;15;0
WireConnection;14;0;18;0
WireConnection;14;1;16;0
WireConnection;14;2;12;0
WireConnection;4;0;3;1
WireConnection;19;0;14;0
WireConnection;19;1;20;0
WireConnection;6;0;4;0
WireConnection;6;1;8;0
WireConnection;9;0;1;0
WireConnection;9;1;10;0
WireConnection;2;5;5;0
WireConnection;0;0;9;0
WireConnection;0;1;2;0
WireConnection;0;2;19;0
WireConnection;0;3;3;2
WireConnection;0;4;6;0
ASEEND*/
//CHKSM=4C86647A47C702CF0D1E2B67EF5D7B8239AAA33D