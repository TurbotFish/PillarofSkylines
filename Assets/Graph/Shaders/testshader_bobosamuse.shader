// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "bla"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
		};

		uniform sampler2D _GrabTexture;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos3 = ase_screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale3 = -1.0;
			#else
			float scale3 = 1.0;
			#endif
			float halfPosW3 = ase_screenPos3.w * 0.5;
			ase_screenPos3.y = ( ase_screenPos3.y - halfPosW3 ) * _ProjectionParams.x* scale3 + halfPosW3;
			#ifdef UNITY_SINGLE_PASS_STEREO
			ase_screenPos3.xy = TransformStereoScreenSpaceTex(ase_screenPos3.xy, ase_screenPos3.w);
			#endif
			float2 componentMask4 = ase_screenPos3.xy;
			float4 screenColor45 = tex2D( _GrabTexture, componentMask4 );
			float4 screenColor42 = tex2D( _GrabTexture, ( componentMask4 + float2( -0.001,-0.001 ) ) );
			float4 screenColor43 = tex2D( _GrabTexture, ( componentMask4 + float2( 0.001,0.001 ) ) );
			float4 temp_cast_0 = (10.0).xxxx;
			float4 temp_cast_1 = (0.0).xxxx;
			float4 temp_cast_2 = (1.0).xxxx;
			float4 clampResult19 = clamp( pow( ( ( screenColor42 - screenColor43 ) * 5.0 ) , temp_cast_0 ) , temp_cast_1 , temp_cast_2 );
			float3 desaturateVar56 = lerp( clampResult19.rgb,dot(clampResult19.rgb,float3(0.299,0.587,0.114)).xxx,1.0);
			o.Emission = ( screenColor45 * float4( ( 1.0 - desaturateVar56 ) , 0.0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-100;198;1906;1004;1485.89;749.2068;1.93841;True;True
Node;AmplifyShaderEditor.GrabScreenPosition;3;-1014,-238;Float;False;1;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;17;-745,7;Float;False;Constant;_Vector0;Vector 0;0;0;0.001,0.001;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;49;-739.176,-137.2007;Float;False;Constant;_Vector1;Vector 1;0;0;-0.001,-0.001;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;4;-768,-242;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-504,-47;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-500.176,-222.2007;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ScreenColorNode;43;-340.505,-64.16004;Float;False;Global;_GrabScreen2;Grab Screen 2;0;0;Instance;2;False;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;42;-335.2701,-239.3987;Float;False;Global;_GrabScreen1;Grab Screen 1;0;0;Instance;2;False;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;52;209.0241,-310.3168;Float;False;Constant;_Float2;Float 2;0;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;16;3.559709,-180.6548;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;54;405.2545,-317.8535;Float;False;Constant;_Float3;Float 3;0;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;213.8079,-417.2693;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;21;583.27,-183.0802;Float;False;Constant;_Float1;Float 1;0;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;579.4699,-276.0803;Float;False;Constant;_Float0;Float 0;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;53;407.6115,-426.2833;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.ClampOpNode;19;584.2318,-418.7158;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;57;790.6076,-313.8642;Float;False;Constant;_Float4;Float 4;0;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DesaturateOpNode;56;756.9067,-415.7961;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.ScreenColorNode;45;-326.2122,185.45;Float;False;Global;_GrabScreen3;Grab Screen 3;0;0;Instance;2;False;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;47;944.502,-419.4768;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ScreenColorNode;2;-308.7873,-428.1775;Float;False;Global;_GrabScreen0;Grab Screen 0;0;0;Object;-1;False;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;1131.418,-1.757447;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;41;1347.086,-50.99312;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;bla;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;3;0
WireConnection;12;0;4;0
WireConnection;12;1;17;0
WireConnection;48;0;4;0
WireConnection;48;1;49;0
WireConnection;43;0;12;0
WireConnection;42;0;48;0
WireConnection;16;0;42;0
WireConnection;16;1;43;0
WireConnection;51;0;16;0
WireConnection;51;1;52;0
WireConnection;53;0;51;0
WireConnection;53;1;54;0
WireConnection;19;0;53;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;56;0;19;0
WireConnection;56;1;57;0
WireConnection;45;0;4;0
WireConnection;47;0;56;0
WireConnection;46;0;45;0
WireConnection;46;1;47;0
WireConnection;41;2;46;0
ASEEND*/
//CHKSM=639110C33882B71EBD1C5620C5E1F4A61CC7A0AE