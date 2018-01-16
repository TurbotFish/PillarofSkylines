// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Antoine/SpriteDissolve"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Emissive("Emissive", Color) = (1,1,1,0)
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Emissive_intensity("Emissive_intensity", Range( 0 , 10)) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_MainTex("MainTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Emissive;
		uniform float _Emissive_intensity;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Cutoff = 0.5;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = ( _Emissive * ( _Emissive_intensity * 0.3 ) ).rgb;
			o.Alpha = 1;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float clampResult31 = clamp( _Emissive_intensity , 0.0 , 1.0 );
			float lerpResult29 = lerp( 1.0 , ( tex2D( _TextureSample0, uv_TextureSample0 ).r * (2.0 + (_Emissive_intensity - 0.0) * (0.0 - 2.0) / (10.0 - 0.0)) ) , clampResult31);
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			clip( ( lerpResult29 * tex2D( _MainTex, uv_MainTex ).a ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1908;107;1906;1004;1734.958;1238.481;1.486474;True;True
Node;AmplifyShaderEditor.RangedFloatNode;35;-28.413,-862.2509;Float;False;Constant;_Float3;Float 3;8;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;-283.2132,-437.5509;Float;False;Property;_Emissive_intensity;Emissive_intensity;2;0;0;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-33.61302,-775.1514;Float;False;Constant;_Float4;Float 4;8;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;27;-984.4128,-828.0509;Float;False;Constant;_Float2;Float 2;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;40;-386.7256,-1088.347;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-984.413,-960.8507;Float;False;Constant;_Float1;Float 1;7;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;32;273.1871,-688.8511;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;415.587,-791.2505;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;39;-185.7123,-344.8512;Float;False;Constant;_Float5;Float 5;8;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;31;-486.8134,-772.0507;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;29;593.1868,-906.4504;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;10.58769,-417.651;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;21;-632.0478,-576.1376;Float;False;Property;_Emissive;Emissive;0;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;41;-551.7242,-214.299;Float;True;Property;_MainTex;MainTex;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;679.0765,-569.5676;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-21.61299,-589.3511;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;961.3998,-233.1;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Antoine/SpriteDissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Masked;0.5;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;23;0
WireConnection;32;1;27;0
WireConnection;32;2;35;0
WireConnection;32;3;36;0
WireConnection;32;4;27;0
WireConnection;33;0;40;1
WireConnection;33;1;32;0
WireConnection;31;0;23;0
WireConnection;31;1;27;0
WireConnection;31;2;25;0
WireConnection;29;0;25;0
WireConnection;29;1;33;0
WireConnection;29;2;31;0
WireConnection;37;0;23;0
WireConnection;37;1;39;0
WireConnection;42;0;29;0
WireConnection;42;1;41;4
WireConnection;22;0;21;0
WireConnection;22;1;37;0
WireConnection;0;2;22;0
WireConnection;0;10;42;0
ASEEND*/
//CHKSM=9801665D03119C1B321D680207E127D525885439