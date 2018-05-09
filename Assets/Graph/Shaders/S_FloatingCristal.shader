// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/FloatingCristal"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,0)
		_Metallic("Metallic", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Normal("Normal", 2D) = "white" {}
		_Emission("Emission", 2D) = "white" {}
		[HDR]_Emission_Intensity("Emission_Intensity", Color) = (0,0,0,0)
		_Detail_Normal("Detail_Normal", 2D) = "white" {}
		_DetailNormal_Tiling("DetailNormal_Tiling", Float) = 20
		_PlayerPos("PlayerPos", Color) = (0,0,0,0)
		_VO_intensity("VO_intensity", Float) = 0
		_Circle_Add("Circle_Add", Float) = -4
		_Circle_Fallof("Circle_Fallof", Range( 0 , 1)) = 0.05
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "DisableBatching" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float2 texcoord_0;
			float4 screenPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Detail_Normal;
		uniform float _DetailNormal_Tiling;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Tint;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _Emission_Intensity;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _Smoothness;
		uniform float4 _PlayerPos;
		uniform float _Circle_Add;
		uniform float _Circle_Fallof;
		uniform float _VO_intensity;
		uniform float _Cutoff = 0.5;


		inline float Dither8x8Bayer( int x, int y )
		{
			const float dither[ 64 ] = {
				 1, 49, 13, 61,  4, 52, 16, 64,
				33, 17, 45, 29, 36, 20, 48, 32,
				 9, 57,  5, 53, 12, 60,  8, 56,
				41, 25, 37, 21, 44, 28, 40, 24,
				 3, 51, 15, 63,  2, 50, 14, 62,
				35, 19, 47, 31, 34, 18, 46, 30,
				11, 59,  7, 55, 10, 58,  6, 54,
				43, 27, 39, 23, 42, 26, 38, 22};
			int r = y * 8 + x;
			return dither[r] / 64;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float2 appendResult22 = (float2(_PlayerPos.r , _PlayerPos.b));
			float4 transform27 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float2 appendResult23 = (float2(transform27.x , transform27.z));
			float temp_output_19_0 = saturate( ( ( distance( appendResult22 , appendResult23 ) + _Circle_Add ) * _Circle_Fallof ) );
			float3 appendResult37 = (float3(0.0 , 0.0 , _VO_intensity));
			v.vertex.xyz += ( temp_output_19_0 * appendResult37 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = BlendNormals( UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,0.0 ) , UnpackNormal( tex2D( _Detail_Normal, ( i.texcoord_0 * _DetailNormal_Tiling ) ) ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( tex2D( _Albedo, uv_Albedo ) * _Tint ).rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 temp_cast_1 = (2.0).xxxx;
			o.Emission = pow( ( tex2D( _Emission, uv_Emission ) * _Emission_Intensity ) , temp_cast_1 ).rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			o.Metallic = tex2D( _Metallic, uv_Metallic ).r;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen62 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither62 = Dither8x8Bayer( fmod(clipScreen62.x, 8), fmod(clipScreen62.y, 8) );
			float2 appendResult22 = (float2(_PlayerPos.r , _PlayerPos.b));
			float4 transform27 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float2 appendResult23 = (float2(transform27.x , transform27.z));
			float temp_output_19_0 = saturate( ( ( distance( appendResult22 , appendResult23 ) + _Circle_Add ) * _Circle_Fallof ) );
			float lerpResult53 = lerp( 1.0 , ( dither62 * ( 1.0 - temp_output_19_0 ) ) , temp_output_19_0);
			clip( lerpResult53 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;1642.326;-471.0662;1.563973;True;True
Node;AmplifyShaderEditor.ColorNode;21;-1212.466,1668.5;Float;False;Property;_PlayerPos;PlayerPos;10;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;20;-995.3793,1674.598;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;27;-955.1327,1840.883;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;23;-729.0869,1856.317;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.DynamicAppendNode;22;-725.8503,1677.037;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;30;-574.4634,1923.709;Float;False;Property;_Circle_Add;Circle_Add;12;0;-4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;17;-542.912,1784.361;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-386.8192,1795.947;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-407.5386,1946.292;Float;False;Property;_Circle_Fallof;Circle_Fallof;13;0;0.05;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-259.9386,1798.392;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;19;-122.454,1799.513;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-957.9313,1293.196;Float;False;Property;_DetailNormal_Tiling;DetailNormal_Tiling;9;0;20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-951.5576,1155.524;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;7;-546.1891,711.9133;Float;True;Property;_Emission;Emission;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;38;-353.5386,2058.392;Float;False;Constant;_Float2;Float 2;10;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;68;-353.6118,1482.958;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DitheringNode;62;-381.2357,1396.711;Float;False;1;2;0;FLOAT;0.0;False;1;SAMPLER2D;;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;8;-468.4297,922.2461;Float;False;Property;_Emission_Intensity;Emission_Intensity;7;1;[HDR];0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;39;-358.7386,2145.492;Float;False;Property;_VO_intensity;VO_intensity;11;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-697.8835,1164.447;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;54;-157.9887,1257.151;Float;False;Constant;_Float0;Float 0;13;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-156.6773,1441.546;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-209.6567,820.2665;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;10;-535.9912,1154.249;Float;True;Property;_Detail_Normal;Detail_Normal;8;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-475,-19;Float;False;Property;_Tint;Tint;2;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-549.2884,483.3857;Float;True;Property;_Normal;Normal;5;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-558,-221;Float;True;Property;_Albedo;Albedo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;52;-185.4783,937.9597;Float;False;Constant;_Float3;Float 3;11;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;37;-158.5386,2101.292;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;53;49.60014,1277.618;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;4;-554,164;Float;True;Property;_Metallic;Metallic;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-236.5,-66.19999;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;5;-537,353;Float;False;Property;_Smoothness;Smoothness;4;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;51;9.476685,846.1957;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.BlendNormalsNode;15;-130.6226,630.3297;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;19.94727,1798.251;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;56;-156.9342,1350.522;Float;False;Constant;_Float1;Float 1;14;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;323.6202,1025.329;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/FloatingCristal;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;True;Back;0;0;False;0;0;Masked;0.5;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;21;0
WireConnection;23;0;27;1
WireConnection;23;1;27;3
WireConnection;22;0;20;0
WireConnection;22;1;20;2
WireConnection;17;0;22;0
WireConnection;17;1;23;0
WireConnection;32;0;17;0
WireConnection;32;1;30;0
WireConnection;35;0;32;0
WireConnection;35;1;36;0
WireConnection;19;0;35;0
WireConnection;68;0;19;0
WireConnection;12;0;11;0
WireConnection;12;1;14;0
WireConnection;57;0;62;0
WireConnection;57;1;68;0
WireConnection;9;0;7;0
WireConnection;9;1;8;0
WireConnection;10;1;12;0
WireConnection;37;0;38;0
WireConnection;37;1;38;0
WireConnection;37;2;39;0
WireConnection;53;0;54;0
WireConnection;53;1;57;0
WireConnection;53;2;19;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;51;0;9;0
WireConnection;51;1;52;0
WireConnection;15;0;6;0
WireConnection;15;1;10;0
WireConnection;33;0;19;0
WireConnection;33;1;37;0
WireConnection;0;0;2;0
WireConnection;0;1;15;0
WireConnection;0;2;51;0
WireConnection;0;3;4;1
WireConnection;0;4;5;0
WireConnection;0;10;53;0
WireConnection;0;11;33;0
ASEEND*/
//CHKSM=A801439E97581938C6A049FD080492A2C0A52606