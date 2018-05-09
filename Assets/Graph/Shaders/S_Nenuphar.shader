// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Nenuphar"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Vo_intensity("Vo_intensity", Float) = 0.4
		_Vo_speed("Vo_speed", Vector) = (0.01,0,0,0)
		_Vo_range("Vo_range", Float) = 0.01
		_Tint("Tint", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float2 texcoord_0;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Tint;
		uniform sampler2D _TextureSample0;
		uniform float2 _Vo_speed;
		uniform float _Vo_range;
		uniform float _Vo_intensity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float2 panner14 = ( ( o.texcoord_0 * _Vo_range ) + 1.0 * _Time.y * _Vo_speed);
			v.vertex.xyz += ( ( float3(1,0,0) * ( tex2Dlod( _TextureSample0, float4( panner14, 0.0 , 0.0 ) ).r * _Vo_intensity ) ) * 2.0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 lerpResult33 = lerp( i.vertexColor , _Tint , i.vertexColor.a);
			o.Albedo = ( tex2D( _Albedo, uv_Albedo ) * lerpResult33 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1924;80;1906;1004;4719.458;1138.746;2.644084;True;True
Node;AmplifyShaderEditor.RangedFloatNode;26;-1423.654,364.8177;Float;False;Property;_Vo_range;Vo_range;5;0;0.01;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-1483.085,242.1835;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;16;-1439.714,441.2239;Float;False;Property;_Vo_speed;Vo_speed;4;0;0.01,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1237.654,380.8177;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;14;-1101.634,380.5609;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;13;-910.8949,345.2389;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;28;-762.6535,545.8175;Float;False;Property;_Vo_intensity;Vo_intensity;3;0;0.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;32;-265.9431,-625.3825;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;31;-300.8735,-453.355;Float;False;Property;_Tint;Tint;6;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;11;-697.3088,113.4626;Float;False;Constant;_Vector0;Vector 0;2;0;1,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-597.6533,369.3179;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-351.3495,-275.5536;Float;True;Property;_Albedo;Albedo;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;-292.8159,404.6484;Float;False;Constant;_Float0;Float 0;2;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;33;35.1456,-524.3141;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-258.5031,264.3694;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-17.37209,-251.2589;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-99.04922,369.3264;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;3;-343.276,-68.66719;Float;True;Property;_Normal;Normal;1;0;Assets/Graph/Textures/VEGETATION/T_nenu_N.png;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;422.6104,51.59663;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Nenuphar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Translucent;5;True;True;0;False;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;25;0;15;0
WireConnection;25;1;26;0
WireConnection;14;0;25;0
WireConnection;14;2;16;0
WireConnection;13;1;14;0
WireConnection;27;0;13;1
WireConnection;27;1;28;0
WireConnection;33;0;32;0
WireConnection;33;1;31;0
WireConnection;33;2;32;4
WireConnection;8;0;11;0
WireConnection;8;1;27;0
WireConnection;29;0;2;0
WireConnection;29;1;33;0
WireConnection;9;0;8;0
WireConnection;9;1;10;0
WireConnection;0;0;29;0
WireConnection;0;1;3;0
WireConnection;0;11;9;0
ASEEND*/
//CHKSM=135D196C5DDC02E7FC85E9A87142972B2D7136D7