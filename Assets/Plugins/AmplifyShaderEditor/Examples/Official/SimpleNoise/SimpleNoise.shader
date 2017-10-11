// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SimpleNoise"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.28
		_Size("Size", Vector) = (0,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Strength("Strength", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float2 _Size;
		uniform float _Strength;
		uniform float _MaskClipValue = 0.28;


		float3 mod289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float simplePerlin2D205 = snoise( ( _Size * uv_TextureSample0 ) );
			float2 temp_cast_0 = (( simplePerlin2D205 * _Strength )).xx;
			o.Emission = tex2D( _TextureSample0, ( uv_TextureSample0 + temp_cast_0 ) ).xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
98;99;1455;745;-995.7839;446.3843;1;True;True
Node;AmplifyShaderEditor.Vector2Node;199;1168.76,-127.3016;Float;False;Property;_Size;Size;1;0;0,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;198;1095.557,21.3978;Float;False;0;210;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;1398.767,-79.30207;Float;False;2;0;FLOAT2;0.0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;211;1538.458,108.999;Float;False;Property;_Strength;Strength;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NoiseGeneratorNode;205;1546.165,-24.90001;Float;False;Simplex2D;1;0;FLOAT2;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;1715.259,47.89908;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;213;1604.246,-180.3998;Float;False;0;210;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;214;1868.557,-58.60012;Float;False;2;0;FLOAT2;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;210;2028.653,-126.8006;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2385.304,-300.1997;Float;False;True;6;Float;ASEMaterialInspector;0;Standard;ASESampleShaders/SimpleNoise;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Masked;0.28;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;1;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0.0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;200;0;199;0
WireConnection;200;1;198;0
WireConnection;205;0;200;0
WireConnection;212;0;205;0
WireConnection;212;1;211;0
WireConnection;214;0;213;0
WireConnection;214;1;212;0
WireConnection;210;1;214;0
WireConnection;0;2;210;0
ASEEND*/
//CHKSM=20FC2B3F546EAB487458C27F354238C632CF0567