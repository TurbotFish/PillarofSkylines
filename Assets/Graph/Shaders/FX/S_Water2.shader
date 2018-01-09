// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Water"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Camera_depth_Length("Camera_depth_Length", Float) = 0
		_Camera_depth_Offset("Camera_depth_Offset", Float) = 0
		_Depth_Fade_Distance("Depth_Fade_Distance", Float) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float4 screenPos;
			float eyeDepth;
		};

		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth_Fade_Distance;
		uniform float _Camera_depth_Length;
		uniform float _Camera_depth_Offset;
		uniform float _Metallic;
		uniform float _Smoothness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Color0 = float4(0.1999998,0,1,0);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth6 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth6 = abs( ( screenDepth6 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth_Fade_Distance ) );
			float clampResult18 = clamp( ( distanceDepth6 * 0.5 ) , 0.0 , 1.0 );
			float DepthFade36 = clampResult18;
			float4 lerpResult34 = lerp( float4(0,0.8344827,1,0) , _Color0 , DepthFade36);
			float cameraDepthFade15 = (( i.eyeDepth -_ProjectionParams.y - _Camera_depth_Offset ) / _Camera_depth_Length);
			float clampResult8 = clamp( cameraDepthFade15 , 0.0 , 1.0 );
			float GlobalDepth39 = clampResult8;
			float4 lerpResult41 = lerp( lerpResult34 , _Color0 , GlobalDepth39);
			o.Albedo = lerpResult41.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float cameraDepthFade24 = (( i.eyeDepth -_ProjectionParams.y - 0.0 ) / 10.0);
			float clampResult25 = clamp( cameraDepthFade24 , 0.0 , 1.0 );
			float lerpResult21 = lerp( 0.1 , 1.0 , ( clampResult25 * clampResult18 ));
			float lerpResult7 = lerp( lerpResult21 , 1.0 , clampResult8);
			o.Alpha = lerpResult7;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float4 screenPos : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
1927;29;1906;1004;1798.049;1458.232;1.952414;True;True
Node;AmplifyShaderEditor.CommentaryNode;42;-2490.869,294.0901;Float;False;1403.474;1139.461;Comment;21;17;20;6;11;16;10;24;9;15;26;22;8;21;39;7;19;18;25;36;37;27;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2440.869,561.778;Float;False;Property;_Depth_Fade_Distance;Depth_Fade_Distance;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;27;-2198.773,344.0901;Float;False;Constant;_Float4;Float 4;3;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;-2131.944,474.8412;Float;False;Constant;_Float2;Float 2;3;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;6;-2142.178,570.5947;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-2105.161,715.4865;Float;False;Constant;_Float0;Float 0;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;-2161.39,1171.765;Float;False;Property;_Camera_depth_Length;Camera_depth_Length;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CameraDepthFade;24;-2000.594,353.7988;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;-2111.161,800.4865;Float;False;Constant;_Float1;Float 1;0;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-2162.39,1249.765;Float;False;Property;_Camera_depth_Offset;Camera_depth_Offset;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1941.944,463.8412;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;18;-1763.34,574.7004;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CameraDepthFade;15;-1862.391,1213.765;Float;False;2;0;FLOAT;1.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;25;-1756.897,370.2332;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;8;-1586.562,1235.814;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1484.802,726.1491;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;33;-1490.992,-489.2595;Float;False;Constant;_Color1;Color 1;0;0;0,0.8344827,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;38;-1480.87,-306.0682;Float;False;36;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;22;-1802.646,817.6334;Float;False;Constant;_Float3;Float 3;3;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-1506.711,581.1935;Float;False;DepthFade;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;5;-1488.335,-683.1371;Float;False;Constant;_Color0;Color 0;0;0;0.1999998,0,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;40;-1155.971,-360.8401;Float;False;39;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-1330.394,1318.552;Float;False;GlobalDepth;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;34;-1104.605,-512.4071;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;21;-1495.473,860.2117;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;29;-470.1323,6.625244;Float;False;Property;_Smoothness;Smoothness;4;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;28;-465.1323,-68.37476;Float;False;Property;_Metallic;Metallic;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;41;-809.1182,-461.5232;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;7;-1327.754,1186.208;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-1513.372,450.5231;Float;False;CameraDepthFade;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;0;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;17;0
WireConnection;24;0;27;0
WireConnection;19;0;6;0
WireConnection;19;1;20;0
WireConnection;18;0;19;0
WireConnection;18;1;9;0
WireConnection;18;2;10;0
WireConnection;15;0;11;0
WireConnection;15;1;16;0
WireConnection;25;0;24;0
WireConnection;25;1;9;0
WireConnection;25;2;10;0
WireConnection;8;0;15;0
WireConnection;8;1;9;0
WireConnection;8;2;10;0
WireConnection;26;0;25;0
WireConnection;26;1;18;0
WireConnection;36;0;18;0
WireConnection;39;0;8;0
WireConnection;34;0;33;0
WireConnection;34;1;5;0
WireConnection;34;2;38;0
WireConnection;21;0;22;0
WireConnection;21;1;10;0
WireConnection;21;2;26;0
WireConnection;41;0;34;0
WireConnection;41;1;5;0
WireConnection;41;2;40;0
WireConnection;7;0;21;0
WireConnection;7;1;10;0
WireConnection;7;2;8;0
WireConnection;37;0;25;0
WireConnection;0;0;41;0
WireConnection;0;3;28;0
WireConnection;0;4;29;0
WireConnection;0;9;7;0
ASEEND*/
//CHKSM=89F46146F6A4BFF7D878D28EE5E76FC41CBF150E