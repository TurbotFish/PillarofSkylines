// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Cloud"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Texture("Texture", 2D) = "white" {}
		_Fade("Fade", Range( 0 , 5)) = 0
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
			float4 vertexColor : COLOR;
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _CameraDepthTexture;
		uniform float _Fade;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = i.vertexColor.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth55 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth55 = abs( ( screenDepth55 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Fade ) );
			float clampResult56 = clamp( distanceDepth55 , 0.0 , 1.0 );
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			o.Alpha = ( clampResult56 * ( tex2D( _Texture, uv_Texture ).a * i.vertexColor.a ) );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
1927;29;1906;1004;1682.855;438.4718;1.35243;True;True
Node;AmplifyShaderEditor.RangedFloatNode;59;-1159.465,117.377;Float;False;Property;_Fade;Fade;1;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;49;-1155.945,470.7249;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DepthFade;55;-832.418,169.5602;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;57;-816.418,73.56018;Float;False;Constant;_Float0;Float 0;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;47;-1281.248,218.6375;Float;True;Property;_Texture;Texture;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;58;-816.418,-4.439819;Float;False;Constant;_Float1;Float 1;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;56;-581.418,104.5602;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-830.7065,280.4958;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-403.8325,196.1722;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;65.6,-3.8;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Bobo/Cloud;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;55;0;59;0
WireConnection;56;0;55;0
WireConnection;56;1;58;0
WireConnection;56;2;57;0
WireConnection;48;0;47;4
WireConnection;48;1;49;4
WireConnection;54;0;56;0
WireConnection;54;1;48;0
WireConnection;0;2;49;0
WireConnection;0;9;54;0
ASEEND*/
//CHKSM=573CA82203AB19E6BA5DDB1842F19F375029FD35