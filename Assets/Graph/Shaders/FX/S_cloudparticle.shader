// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/CloudParticle"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Mask("Mask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _CameraDepthTexture;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 temp_cast_0 = (0.3).xxx;
			o.Emission = temp_cast_0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth3 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth3 = abs( ( screenDepth3 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.0 ) );
			float clampResult4 = clamp( distanceDepth3 , 0.0 , 1.0 );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float lerpResult25 = lerp( 0.0 , clampResult4 , tex2D( _Mask, uv_Mask ).r);
			o.Alpha = ( lerpResult25 * i.vertexColor.a );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;1421.32;890.3081;1.115725;True;True
Node;AmplifyShaderEditor.RangedFloatNode;5;-1117.7,-5.900009;Float;False;Constant;_Float0;Float 0;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;3;-1102.8,-175.2;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;-1116.7,79.10001;Float;False;Constant;_Float1;Float 1;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;27;-776.001,-206.7003;Float;True;Property;_Mask;Mask;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;4;-737.7001,68.1;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;25;-333.801,15.49971;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;28;-498.201,191.0995;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;52;-255.3866,-354.7599;Float;False;Constant;_Float2;Float 2;2;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1453.469,-918.256;Float;False;2;2;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT3;0.0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-1286.217,-881.8102;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-218.8009,235.0995;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;36;-851.8661,-926.3562;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;30;-1692.2,-853.9081;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-1110.907,-831.7334;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;49;-659.9886,-410.8134;Float;False;unity_AmbientSky;0;1;COLOR
Node;AmplifyShaderEditor.ViewMatrixNode;31;-1612.012,-948.042;Float;False;0;1;FLOAT4x4
Node;AmplifyShaderEditor.RangedFloatNode;33;-1464.286,-789.8813;Float;False;Constant;_Float3;Float 3;-1;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3.2,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Bobo/CloudParticle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;4;2;6;0
WireConnection;25;0;5;0
WireConnection;25;1;4;0
WireConnection;25;2;27;1
WireConnection;32;0;31;0
WireConnection;32;1;30;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;29;0;25;0
WireConnection;29;1;28;4
WireConnection;35;0;34;0
WireConnection;35;1;33;0
WireConnection;0;2;52;0
WireConnection;0;9;29;0
ASEEND*/
//CHKSM=D98584CF14F6EB6FCB7B440C3B7FD327A0E030B1