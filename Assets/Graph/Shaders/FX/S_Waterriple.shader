// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Particle/Waterripple"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.1
		_Panner_speed("Panner_speed", Vector) = (0,-0.7,0,0)
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Noise_tiling("Noise_tiling", Float) = 0.2
		_Noise_intensity("Noise_intensity", Range( 0 , 1)) = 0.4
		_Texture0("Texture 0", 2D) = "white" {}
		_Color0("Color 0", Color) = (0,0,0,0)
		_Ripple_Tiling("Ripple_Tiling", Vector) = (0,0,0,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
		};

		uniform float4 _Color0;
		uniform sampler2D _Texture0;
		uniform float2 _Panner_speed;
		uniform sampler2D _TextureSample1;
		uniform float _Noise_tiling;
		uniform float _Noise_intensity;
		uniform float2 _Ripple_Tiling;
		uniform float _Cutoff = 0.1;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _Color0.rgb;
			o.Alpha = 1;
			float2 panner13 = ( ( i.texcoord_0 * _Noise_tiling ) + 1.0 * _Time.y * float2( 0.1,0 ));
			float2 panner2 = ( ( ( tex2D( _TextureSample1, panner13 ).r * _Noise_intensity ) + ( i.texcoord_1 * _Ripple_Tiling ) ) + 1.0 * _Time.y * _Panner_speed);
			clip( ( tex2D( _Texture0, panner2 ).r * pow( tex2D( _Texture0, i.texcoord_1 ).g , 2.0 ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;1545.843;480.6074;1.377981;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-1906,-200;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-1820,-72;Float;False;Property;_Noise_tiling;Noise_tiling;3;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1650,-193;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;14;-1840.313,-330.9818;Float;False;Constant;_Vector1;Vector 1;4;0;0.1,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;13;-1640.313,-330.9818;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1405.318,45.50506;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;21;-1313.548,168.2703;Float;False;Property;_Ripple_Tiling;Ripple_Tiling;7;0;0,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-1402,-80;Float;False;Property;_Noise_intensity;Noise_intensity;4;0;0.4;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-1415,-281;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Assets/Scenes/Boris/Workshop_ressources/Perlin_noise.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1088.014,35.92078;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1041,-166;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;5;-923,165;Float;False;Property;_Panner_speed;Panner_speed;1;0;0,-0.7;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-877,35;Float;False;2;2;0;FLOAT;0,0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;15;-765.2892,-183.093;Float;True;Property;_Texture0;Texture 0;5;0;Assets/Graph/Textures/FX/T_water_E_C.png;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;23;-437.9463,550.1226;Float;False;Constant;_Float0;Float 0;8;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;18;-513.2892,235.907;Float;True;Property;_TextureSample2;Texture Sample 2;0;0;Assets/Graph/Textures/FX/T_water_E_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;2;-725,42;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,1;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;1;-509,26;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Assets/Graph/Textures/FX/T_water_E_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;24;-196.7995,501.8932;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-184.2892,240.907;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;19;-235.8447,-222.0255;Float;False;Property;_Color0;Color 0;6;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;97,2;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Particle/Waterripple;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Masked;0.1;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;7;0
WireConnection;8;1;9;0
WireConnection;13;0;8;0
WireConnection;13;2;14;0
WireConnection;6;1;13;0
WireConnection;20;0;3;0
WireConnection;20;1;21;0
WireConnection;11;0;6;1
WireConnection;11;1;12;0
WireConnection;10;0;11;0
WireConnection;10;1;20;0
WireConnection;18;0;15;0
WireConnection;18;1;3;0
WireConnection;2;0;10;0
WireConnection;2;2;5;0
WireConnection;1;0;15;0
WireConnection;1;1;2;0
WireConnection;24;0;18;2
WireConnection;24;1;23;0
WireConnection;17;0;1;1
WireConnection;17;1;24;0
WireConnection;0;2;19;0
WireConnection;0;10;17;0
ASEEND*/
//CHKSM=885839EDA2431DC4237871E15A83F49ADD9BC689