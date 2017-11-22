// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Grass"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_Vector0("Vector 0", Vector) = (0,0,1,0)
		_Transluscency("Transluscency", Color) = (0.7103218,1,0.3602941,0)
		_Color("Color", 2D) = "white" {}
		_Alpha("Alpha", 2D) = "white" {}
		_Emissive("Emissive", Float) = 0
		_Time_scale("Time_scale", Float) = 1
		_VO_intensity("VO_intensity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float2 texcoord_0;
		};

		struct SurfaceOutputStandardCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			fixed3 Translucency;
		};

		uniform sampler2D _Color;
		uniform float4 _Color_ST;
		uniform float _Emissive;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float4 _Transluscency;
		uniform sampler2D _Alpha;
		uniform float4 _Alpha_ST;
		uniform float _Time_scale;
		uniform float3 _Vector0;
		uniform float _VO_intensity;
		uniform float _Cutoff = 0.2;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 temp_cast_0 = (0.0).xxx;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime29 = _Time.y * _Time_scale;
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float3 lerpResult7 = lerp( temp_cast_0 , ( ( sin( ( ( ase_worldPos.x * 2.0 ) + mulTime29 ) ) * _Vector0 ) * _VO_intensity ) , o.texcoord_0.y);
			v.vertex.xyz += lerpResult7;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			UNITY_GI(gi, s, data);
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_Color = i.uv_texcoord * _Color_ST.xy + _Color_ST.zw;
			float4 tex2DNode1 = tex2D( _Color, uv_Color );
			o.Albedo = tex2DNode1.rgb;
			o.Emission = ( tex2DNode1 * _Emissive ).rgb;
			o.Translucency = _Transluscency.rgb;
			o.Alpha = 1;
			float2 uv_Alpha = i.uv_texcoord * _Alpha_ST.xy + _Alpha_ST.zw;
			float4 tex2DNode2 = tex2D( _Alpha, uv_Alpha );
			clip( ( tex2DNode2.a ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;2229.153;674.0696;2.184908;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;34;-1084.672,245.7235;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;31;-1069.486,485.1858;Float;False;Property;_Time_scale;Time_scale;13;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;38;-1050.672,404.7234;Float;False;Constant;_Float4;Float 4;6;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;29;-883.4861,490.1858;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-874.6721,383.0085;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-694.8334,435.2731;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;28;-550.1499,464.2888;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;30;-593.2736,547.0107;Float;False;Property;_Vector0;Vector 0;8;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-413.2736,498.0107;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;33;-396.2526,606.9089;Float;False;Property;_VO_intensity;VO_intensity;14;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-255.2526,493.9089;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1221.599,660.8126;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-612,-127;Float;True;Property;_Color;Color;10;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-223,403;Float;False;Constant;_Float3;Float 3;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-418,82;Float;False;Property;_Emissive;Emissive;12;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-636,187;Float;True;Property;_Alpha;Alpha;11;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;7;-18,432;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-259,263;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-185,26;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;42;-49.58081,117.0964;Float;False;Property;_Transluscency;Transluscency;9;0;0.7103218,1,0.3602941,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;302.3529,-35;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Grass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;0;False;0;0;Masked;0.2;True;True;0;False;TransparentCutout;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;31;0
WireConnection;37;0;34;1
WireConnection;37;1;38;0
WireConnection;36;0;37;0
WireConnection;36;1;29;0
WireConnection;28;0;36;0
WireConnection;26;0;28;0
WireConnection;26;1;30;0
WireConnection;32;0;26;0
WireConnection;32;1;33;0
WireConnection;7;0;16;0
WireConnection;7;1;32;0
WireConnection;7;2;6;2
WireConnection;3;0;2;1
WireConnection;3;1;2;4
WireConnection;4;0;1;0
WireConnection;4;1;5;0
WireConnection;0;0;1;0
WireConnection;0;2;4;0
WireConnection;0;7;42;0
WireConnection;0;10;3;0
WireConnection;0;11;7;0
ASEEND*/
//CHKSM=7847BBD62F1B39A70EFFCAD128706800725D6610