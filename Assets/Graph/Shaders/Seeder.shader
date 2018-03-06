// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Seeder"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Mask("Mask", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "DisableBatching" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _Mask;
		uniform float _Cutoff = 0.5;


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, clamp( p - K.xxx, 0.0, 1.0 ), c.y );
		}


		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float3 hsvTorgb8 = RGBToHSV( tex2D( _TextureSample0, uv_TextureSample0 ).rgb );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );;
			float lerpResult13 = lerp( 0.0 , 0.25 , (0.0 + (ase_objectScale.y - 0.5) * (1.0 - 0.0) / (1.0 - 0.5)));
			float temp_output_11_0 = ( hsvTorgb8.x + lerpResult13 );
			float3 hsvTorgb7 = HSVToRGB( float3(temp_output_11_0,hsvTorgb8.y,hsvTorgb8.z) );
			float3 temp_cast_1 = (1.0).xxx;
			float3 temp_cast_2 = (1.0).xxx;
			float3 ifLocalVar23 = 0;
			if( temp_output_11_0 <= 0.27 )
				ifLocalVar23 = temp_cast_2;
			else
				ifLocalVar23 = hsvTorgb7;
			float3 ifLocalVar20 = 0;
			if( temp_output_11_0 >= 0.22 )
				ifLocalVar20 = ifLocalVar23;
			else
				ifLocalVar20 = hsvTorgb7;
			o.Emission = ifLocalVar20;
			o.Alpha = 1;
			clip( _Mask - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;1863.746;614.4524;1.401851;True;True
Node;AmplifyShaderEditor.RangedFloatNode;18;-733.87,295.0014;Float;False;Constant;_Float4;Float 4;1;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;17;-732.87,195.0014;Float;False;Constant;_Float3;Float 3;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-728.87,115.0014;Float;False;Constant;_Float2;Float 2;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ObjectScaleNode;2;-925.87,-50.9986;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;19;-1119.87,-315.9986;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;15;-547.87,79.0014;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-784.87,-336.9986;Float;False;Constant;_Float1;Float 1;0;0;0.25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-781.87,-415.9986;Float;False;Constant;_Float0;Float 0;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;13;-483.87,-333.9986;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RGBToHSVNode;8;-508.87,-217.9986;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-217.87,-281.9986;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.HSVToRGBNode;7;-76.87,-207.9986;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;22;-684.4366,407.6624;Float;False;Constant;_Float6;Float 6;2;0;0.27;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;23;-468.8153,380.8867;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;21;-463.126,291.7745;Float;False;Constant;_Float5;Float 5;2;0;0.22;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;20;-276.5046,279.9989;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ColorNode;3;-771.87,-224.9986;Float;False;Property;_Color0;Color 0;1;0;1,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;5;-235.87,-41.9986;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;24;-281.0562,514.0378;Float;False;Property;_Mask;Mask;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;150.5407,161.2221;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Bobo/Seeder;False;False;False;False;True;True;True;True;True;True;True;True;False;True;False;False;False;Back;0;0;False;0;0;Masked;0.5;True;False;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;2;2
WireConnection;15;1;18;0
WireConnection;15;2;17;0
WireConnection;15;3;16;0
WireConnection;13;0;14;0
WireConnection;13;1;12;0
WireConnection;13;2;15;0
WireConnection;8;0;19;0
WireConnection;11;0;8;1
WireConnection;11;1;13;0
WireConnection;7;0;11;0
WireConnection;7;1;8;2
WireConnection;7;2;8;3
WireConnection;23;0;11;0
WireConnection;23;1;22;0
WireConnection;23;2;7;0
WireConnection;23;3;17;0
WireConnection;23;4;17;0
WireConnection;20;0;11;0
WireConnection;20;1;21;0
WireConnection;20;2;23;0
WireConnection;20;3;23;0
WireConnection;20;4;7;0
WireConnection;0;2;20;0
WireConnection;0;10;24;0
ASEEND*/
//CHKSM=7BA9C6BA9D169235CE5E8D8535ADCC50D4ACBE89