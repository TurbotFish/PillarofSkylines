// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BObo/LoadingCrystal"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Diffuse("Diffuse", 2D) = "white" {}
		_Emission("Emission", 2D) = "white" {}
		_Tint("Tint", Color) = (0,0,0,0)
		[HDR]_E_Tint("E_Tint", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Tint;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform float4 _E_Tint;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			o.Emission = ( ( _Tint * tex2D( _Diffuse, uv_Diffuse ) ) + ( ( (0.0 + (sin( _Time.y ) - -1.0) * (0.5 - 0.0) / (1.0 - -1.0)) + _E_Tint ) * tex2D( _Emission, uv_Emission ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
7;29;1906;1004;2068.398;819.4705;1.576426;True;True
Node;AmplifyShaderEditor.RangedFloatNode;17;-1545.025,107.4678;Float;False;Constant;_Float4;Float 4;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;16;-1370.042,121.6556;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;10;-1157.224,104.3149;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-1023.228,109.0441;Float;False;Constant;_Float0;Float 0;4;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-1023.228,191.0183;Float;False;Constant;_Float1;Float 1;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-1020.075,271.4161;Float;False;Constant;_Float2;Float 2;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;15;-1040.569,373.8837;Float;False;Constant;_Float3;Float 3;4;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;11;-823.0219,109.0442;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;7;-768.9594,540.9439;Float;False;Property;_E_Tint;E_Tint;3;1;[HDR];0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-521.9246,228.8526;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;1;-999.1965,-133.7444;Float;True;Property;_Diffuse;Diffuse;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-912.1965,-333.7443;Float;False;Property;_Tint;Tint;2;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-843.9595,722.9438;Float;True;Property;_Emission;Emission;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-638.1964,-186.7443;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-232.0306,321.4488;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-279.1964,-138.7444;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;443,-47;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;BObo/LoadingCrystal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;17;0
WireConnection;10;0;16;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;11;2;13;0
WireConnection;11;3;14;0
WireConnection;11;4;15;0
WireConnection;9;0;11;0
WireConnection;9;1;7;0
WireConnection;2;0;3;0
WireConnection;2;1;1;0
WireConnection;8;0;9;0
WireConnection;8;1;6;0
WireConnection;5;0;2;0
WireConnection;5;1;8;0
WireConnection;0;2;5;0
ASEEND*/
//CHKSM=B9E404EAB6CA34F48B513645E25D13CBC3178459