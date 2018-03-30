// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/CloudParticle"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cloud("Cloud", 2D) = "white" {}
		_Depth("Depth", Range( 0 , 1)) = 0
		_Noise("Noise", 2D) = "white" {}
		_StartFade("StartFade", Float) = 0
		_EndFade("EndFade", Float) = 3.5
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha novertexlights nofog vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float4 vertexColor : COLOR;
			float4 screenPos;
			float eyeDepth;
		};

		uniform sampler2D _Cloud;
		uniform sampler2D _Noise;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth;
		uniform float _StartFade;
		uniform float _EndFade;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_cast_0 = (1.0).xxxx;
			float4 lerpResult36 = lerp( temp_cast_0 , unity_AmbientSky , 0.5);
			o.Emission = lerpResult36.rgb;
			float2 panner18 = ( ( i.texcoord_0 * 0.5 ) + 1.0 * _Time.y * float2( -0.05,0 ));
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth5 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth5 = abs( ( screenDepth5 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( (10.0 + (_Depth - 0.0) * (0.0 - 10.0) / (1.0 - 0.0)) ) );
			float temp_output_31_0 = ( _StartFade + _ProjectionParams.y );
			o.Alpha = ( ( ( tex2D( _Cloud, ( i.texcoord_0 + ( tex2D( _Noise, panner18 ).r * 0.1 ) ) ).r * i.vertexColor.a ) * saturate( distanceDepth5 ) ) * saturate( ( ( i.eyeDepth + -temp_output_31_0 ) / ( _EndFade - temp_output_31_0 ) ) ) );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;2099.371;878.2808;1.682704;True;True
Node;AmplifyShaderEditor.RangedFloatNode;21;-1974.974,-335.1828;Float;False;Constant;_Float5;Float 5;3;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-2015.98,-55.46785;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;22;-1778.189,1265.61;Float;False;297.1897;243;Correction for near plane clipping;1;19;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1805.095,-352.7565;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;20;-1837.313,-250.2432;Float;False;Constant;_Vector0;Vector 0;3;0;-0.05,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;18;-1630.822,-336.6472;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;23;-1791.381,810.4947;Float;False;1047.541;403.52;Scale depth from start to end;6;30;28;29;31;15;26;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;13;-1399.435,-361.5432;Float;True;Property;_Noise;Noise;2;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;17;-1266.168,-165.3036;Float;False;Constant;_Float4;Float 4;3;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ProjectionParams;24;-1705.489,1314.91;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;30;-1752.405,1093.19;Float;False;Property;_StartFade;StartFade;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;-992,492;Float;False;Constant;_Float1;Float 1;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1056.748,-310.2867;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-992,651;Float;False;Constant;_Float3;Float 3;2;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-1099,414;Float;False;Property;_Depth;Depth;1;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;-1001,573;Float;False;Constant;_Float2;Float 2;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1466.913,1096.722;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-943.9831,4.575698;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.TFHCRemap;9;-803,458;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;25;-1275.525,972.8411;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;26;-1285.94,1054.194;Float;False;Property;_EndFade;EndFade;4;0;3.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SurfaceDepthNode;32;-1725.822,908.6069;Float;False;0;0;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;5;-625,410;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;4;-638,220;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-760.5027,-24.75399;Float;True;Property;_Cloud;Cloud;0;0;Assets/Graph/Textures/FX/T_Cloud_DB.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-1066.441,913.2949;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;28;-1069.14,1068.094;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-405,114;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;27;-882.6405,969.7946;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;7;-383,346;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;37;-292.1467,-52.07312;Float;False;Constant;_Float6;Float 6;5;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-482.5877,-291.7308;Float;False;Constant;_Float0;Float 0;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;35;-563.0616,-201.8339;Float;False;unity_AmbientSky;0;1;COLOR
Node;AmplifyShaderEditor.SaturateNode;34;-386.3794,939.0392;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-212,171;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-256.8113,689.9991;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;36;-177.7229,-264.0938;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/CloudParticle;False;False;False;False;False;True;False;False;False;True;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;One;One;0;One;One;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;14;0
WireConnection;19;1;21;0
WireConnection;18;0;19;0
WireConnection;18;2;20;0
WireConnection;13;1;18;0
WireConnection;16;0;13;1
WireConnection;16;1;17;0
WireConnection;31;0;30;0
WireConnection;31;1;24;2
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;9;0;8;0
WireConnection;9;1;10;0
WireConnection;9;2;11;0
WireConnection;9;3;12;0
WireConnection;9;4;10;0
WireConnection;25;0;31;0
WireConnection;5;0;9;0
WireConnection;1;1;15;0
WireConnection;29;0;32;0
WireConnection;29;1;25;0
WireConnection;28;0;26;0
WireConnection;28;1;31;0
WireConnection;3;0;1;1
WireConnection;3;1;4;4
WireConnection;27;0;29;0
WireConnection;27;1;28;0
WireConnection;7;0;5;0
WireConnection;34;0;27;0
WireConnection;6;0;3;0
WireConnection;6;1;7;0
WireConnection;33;0;6;0
WireConnection;33;1;34;0
WireConnection;36;0;2;0
WireConnection;36;1;35;0
WireConnection;36;2;37;0
WireConnection;0;2;36;0
WireConnection;0;9;33;0
ASEEND*/
//CHKSM=95783A1B03D1198A898C5FE2A290D4675E6E0703