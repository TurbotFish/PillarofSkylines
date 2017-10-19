// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Cloud"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Color("Color", Color) = (0,0,0,0)
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Scale("Scale", Float) = 1
		_Power("Power", Float) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha 
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
			INTERNAL_DATA
		};

		uniform sampler2D _TextureSample1;
		uniform float4 _Color;
		uniform float _Scale;
		uniform float _Power;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 temp_cast_1 = (0.5).xxx;
			float3 desaturateVar45 = lerp( ( saturate( ( _Color > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( _Color - 0.5 ) ) * ( 1.0 - tex2D( _TextureSample1, ( ( mul( UNITY_MATRIX_V , float4( i.worldNormal , 0.0 ) ) * 0.5 ) + temp_cast_1 ).xy ) ) ) : ( 2.0 * _Color * tex2D( _TextureSample1, ( ( mul( UNITY_MATRIX_V , float4( i.worldNormal , 0.0 ) ) * 0.5 ) + temp_cast_1 ).xy ) ) ) )).rgb,dot(( saturate( ( _Color > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( _Color - 0.5 ) ) * ( 1.0 - tex2D( _TextureSample1, ( ( mul( UNITY_MATRIX_V , float4( i.worldNormal , 0.0 ) ) * 0.5 ) + temp_cast_1 ).xy ) ) ) : ( 2.0 * _Color * tex2D( _TextureSample1, ( ( mul( UNITY_MATRIX_V , float4( i.worldNormal , 0.0 ) ) * 0.5 ) + temp_cast_1 ).xy ) ) ) )).rgb,float3(0.299,0.587,0.114)),1.0);
			o.Emission = desaturateVar45;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelFinalVal25 = (0.0 + _Scale*pow( 1.0 - dot( ase_worldNormal, worldViewDir ) , _Power));
			o.Alpha = lerp( 1.0 , 0.0 , clamp( fresnelFinalVal25 , 0.0 , 1.0 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
1927;29;1906;1004;2211.604;1028.85;2.021277;True;True
Node;AmplifyShaderEditor.WorldNormalVector;34;-1027.822,-606.3298;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ViewMatrixNode;33;-947.6342,-700.4636;Float;False;0;1;FLOAT4x4
Node;AmplifyShaderEditor.RangedFloatNode;35;-799.9084,-542.3029;Float;False;Constant;_Float3;Float 3;-1;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-789.0909,-670.6777;Float;False;2;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT3;0.0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-621.8387,-634.2319;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-446.5293,-584.155;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;28;-1412.701,308.6;Float;False;Property;_Power;Power;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;27;-1415.901,223.7999;Float;False;Property;_Scale;Scale;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;-1116.7,79.10001;Float;False;Constant;_Float1;Float 1;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-1117.7,-5.900009;Float;False;Constant;_Float0;Float 0;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;25;-1044.701,273.3999;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;2;-197.9001,-403.3;Float;False;Property;_Color;Color;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;39;-251.0844,-626.3387;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;26;-802.9009,297.6997;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;46;-417.1721,-126.1505;Float;False;Constant;_Float2;Float 2;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;44;192.7989,-532.4995;Float;False;Overlay;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.DesaturateOpNode;45;-172.7337,-126.1505;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;29;-507.901,111.0001;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;65.6,-3.8;Float;False;True;2;Float;ASEMaterialInspector;0;Unlit;Bobo/Cloud;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;0;33;0
WireConnection;36;1;34;0
WireConnection;37;0;36;0
WireConnection;37;1;35;0
WireConnection;38;0;37;0
WireConnection;38;1;35;0
WireConnection;25;2;27;0
WireConnection;25;3;28;0
WireConnection;39;1;38;0
WireConnection;26;0;25;0
WireConnection;26;1;5;0
WireConnection;26;2;6;0
WireConnection;44;0;39;0
WireConnection;44;1;2;0
WireConnection;45;0;44;0
WireConnection;45;1;46;0
WireConnection;29;0;6;0
WireConnection;29;1;5;0
WireConnection;29;2;26;0
WireConnection;0;2;45;0
WireConnection;0;9;29;0
ASEEND*/
//CHKSM=E5AC468D21433B118720850AFCBDA1C79AB3DD63