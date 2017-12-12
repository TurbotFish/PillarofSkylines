// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Eye"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Normal("Normal", 2D) = "white" {}
		_Normal_Tiling("Normal_Tiling", Float) = 1
		_Noise("Noise", 2D) = "white" {}
		_Noise_tiling("Noise_tiling", Float) = 0.2
		_Noise_panner("Noise_panner", Vector) = (0.2,0,0,0)
		_Noise_intensity("Noise_intensity", Range( 0 , 1)) = 0
		_VO_intensity("VO_intensity", Float) = 0
		_MaskA("MaskA", 2D) = "white" {}
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
			float2 texcoord_2;
			float3 worldPos;
			float3 worldNormal;
			float2 texcoord_3;
			float2 texcoord_4;
			float2 texcoord_5;
		};

		uniform sampler2D _TextureSample0;
		uniform sampler2D _TextureSample1;
		uniform sampler2D _MaskA;
		uniform sampler2D _Normal;
		uniform sampler2D _Noise;
		uniform float2 _Noise_panner;
		uniform float _Noise_tiling;
		uniform float _Noise_intensity;
		uniform float _Normal_Tiling;
		uniform float _VO_intensity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_2.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_3.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			float3 temp_cast_0 = (0.0).xxx;
			o.texcoord_4.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			float2 panner10 = ( ( o.texcoord_4 * _Noise_tiling ) + 1.0 * _Time.y * _Noise_panner);
			float4 tex2DNode6 = tex2Dlod( _Noise, float4( panner10, 0.0 , 0.0 ) );
			float3 ase_vertexNormal = v.normal.xyz;
			o.texcoord_5.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			float4 tex2DNode20 = tex2Dlod( _MaskA, float4( o.texcoord_5, 0.0 , 0.0 ) );
			float3 lerpResult21 = lerp( temp_cast_0 , ( ( tex2DNode6.r * ase_vertexNormal ) * _VO_intensity ) , tex2DNode20.g);
			v.vertex.xyz += lerpResult21;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 tex2DNode23 = tex2D( _TextureSample0, i.texcoord_0 );
			float2 panner50 = ( ( i.texcoord_1 * 2.0 ) + 1.0 * _Time.y * float2( 0.5,0 ));
			float3 lerpResult59 = lerp( float3( 0,0,0 ) , float3(2,0,0) , ( pow( tex2D( _TextureSample1, panner50 ).r , 5.0 ) * 5.0 ));
			float4 tex2DNode20 = tex2D( _MaskA, i.texcoord_2 );
			float3 lerpResult54 = lerp( float3( 0,0,0 ) , lerpResult59 , tex2DNode20.r);
			float3 ase_worldPos = i.worldPos;
			fixed3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNode25 = ( 0.0 + 1.0 * pow( 1.0 - dot( ase_worldNormal, ase_worldViewDir ), 5.0 ) );
			float3 lerpResult28 = lerp( ( tex2DNode23.r + lerpResult54 ) , float3(3,0,0) , fresnelNode25);
			o.Albedo = lerpResult28;
			float3 temp_cast_0 = (0.0).xxx;
			float3 temp_cast_1 = (0.0).xxx;
			float2 panner10 = ( ( i.texcoord_3 * _Noise_tiling ) + 1.0 * _Time.y * _Noise_panner);
			float4 tex2DNode6 = tex2D( _Noise, panner10 );
			float clampResult44 = clamp( pow( UnpackScaleNormal( tex2D( _Normal, ( ( i.texcoord_2 + ( tex2DNode6.r * _Noise_intensity ) ) * _Normal_Tiling ) ) ,1.0 ).r , 5.0 ) , 0.0 , 1.0 );
			float3 lerpResult38 = lerp( temp_cast_1 , float3(3,0,0) , clampResult44);
			float3 lerpResult48 = lerp( temp_cast_0 , lerpResult38 , tex2DNode20.r);
			float clampResult77 = clamp( ( tex2DNode23.b * 1.0 ) , 0.0 , 1.0 );
			float3 lerpResult71 = lerp( lerpResult48 , float3(1,0,0) , clampResult77);
			o.Emission = lerpResult71;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;1948.865;844.2364;2.190006;True;True
Node;AmplifyShaderEditor.RangedFloatNode;9;-1756,57;Float;False;Property;_Noise_tiling;Noise_tiling;3;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-1829,-56;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1577,-55;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;11;-1442,22;Float;False;Property;_Noise_panner;Noise_panner;4;0;0.2,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;10;-1257,-51;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;56;-651.7183,-378.2838;Float;False;Constant;_Float6;Float 6;10;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-1039,-71;Float;True;Property;_Noise;Noise;2;0;Assets/Scenes/Boris/Workshop_ressources/Perlin_noise.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;41;-1029.113,-175.1157;Float;False;Property;_Noise_intensity;Noise_intensity;5;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-730.7311,-514.1406;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-386.4544,-516.796;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;52;-389.1529,-382.2845;Float;False;Constant;_Vector2;Vector 2;10;0;0.5,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1037,126;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-657.1125,-55.11572;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-749,125;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;14;-759,242;Float;False;Property;_Normal_Tiling;Normal_Tiling;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;50;-209.0535,-487.0229;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;49;-18.73573,-512.5689;Float;True;Property;_TextureSample1;Texture Sample 1;9;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;58;127.0862,-311.6412;Float;False;Constant;_Float9;Float 9;10;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-554,214;Float;False;Constant;_Float1;Float 1;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-586,44;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.PowerNode;57;307.4134,-503.7289;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;-473.0014,-102.3308;Float;False;Constant;_Emission_power;Emission_power;9;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-392.9791,93.28053;Float;True;Property;_Normal;Normal;0;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;60;405.4174,-674.9092;Float;False;Constant;_Vector3;Vector 3;10;0;2,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;469.2908,-504.8885;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-426.365,-845.2156;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;45;-236.5004,-187.853;Float;False;Constant;_Float7;Float 7;9;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;46;-231.3562,-13.97838;Float;False;Constant;_Float8;Float 8;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;47;-232.5192,-112.2856;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;44;-48.51391,-118.7919;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;59;632.7868,-512.8761;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;20;-404.7436,897.6764;Float;True;Property;_MaskA;MaskA;7;0;Assets/Graph/Textures/LIFE/T_eye_Mask_A.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;17;-355.4597,692.9683;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;39;107.8524,-113.0815;Float;False;Constant;_Vector1;Vector 1;8;0;3,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;23;-125.3861,-839.151;Float;True;Property;_TextureSample0;Texture Sample 0;8;0;Assets/Graph/Textures/LIFE/T_eye_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;74;580.8208,275.7882;Float;False;Constant;_Float5;Float 5;10;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;54;818.5114,-516.4575;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;27;-23.26444,-960.8109;Float;False;Constant;_Float3;Float 3;8;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;26;-27.44379,-1040.443;Float;False;Constant;_Float2;Float 2;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-119.4598,650.9683;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;19;-124.4598,750.9683;Float;False;Property;_VO_intensity;VO_intensity;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;38;290.6543,-126.104;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;735.2983,233.1035;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;22;-92.81915,835.2407;Float;False;Constant;_Float0;Float 0;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;48;453.3716,-128.0515;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.FresnelNode;25;155.303,-1046.569;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;80;721.5374,-1001.399;Float;False;Constant;_Vector4;Vector 4;10;0;3,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;53;1024.559,-576.6975;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.Vector3Node;72;671.4912,62.29962;Float;False;Constant;_Vector0;Vector 0;10;0;1,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;77;897.528,274.0294;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;77.54023,657.9683;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;29;251.2705,-1144.579;Float;False;Constant;_Float4;Float 4;8;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;71;905.6926,141.9994;Float;False;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;28;1235.134,-607.972;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;21;274.7218,782.6149;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1539.96,-44.50392;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Eye;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;7;0
WireConnection;8;1;9;0
WireConnection;10;0;8;0
WireConnection;10;2;11;0
WireConnection;6;1;10;0
WireConnection;55;0;51;0
WireConnection;55;1;56;0
WireConnection;40;0;6;1
WireConnection;40;1;41;0
WireConnection;5;0;4;0
WireConnection;5;1;40;0
WireConnection;50;0;55;0
WireConnection;50;2;52;0
WireConnection;49;1;50;0
WireConnection;13;0;5;0
WireConnection;13;1;14;0
WireConnection;57;0;49;1
WireConnection;57;1;58;0
WireConnection;3;1;13;0
WireConnection;3;5;12;0
WireConnection;61;0;57;0
WireConnection;61;1;58;0
WireConnection;47;0;3;1
WireConnection;47;1;43;0
WireConnection;44;0;47;0
WireConnection;44;1;45;0
WireConnection;44;2;46;0
WireConnection;59;1;60;0
WireConnection;59;2;61;0
WireConnection;20;1;4;0
WireConnection;23;1;24;0
WireConnection;54;1;59;0
WireConnection;54;2;20;1
WireConnection;16;0;6;1
WireConnection;16;1;17;0
WireConnection;38;0;45;0
WireConnection;38;1;39;0
WireConnection;38;2;44;0
WireConnection;75;0;23;3
WireConnection;75;1;74;0
WireConnection;48;0;45;0
WireConnection;48;1;38;0
WireConnection;48;2;20;1
WireConnection;25;2;26;0
WireConnection;25;3;27;0
WireConnection;53;0;23;1
WireConnection;53;1;54;0
WireConnection;77;0;75;0
WireConnection;77;1;45;0
WireConnection;77;2;46;0
WireConnection;18;0;16;0
WireConnection;18;1;19;0
WireConnection;71;0;48;0
WireConnection;71;1;72;0
WireConnection;71;2;77;0
WireConnection;28;0;53;0
WireConnection;28;1;80;0
WireConnection;28;2;25;0
WireConnection;21;0;22;0
WireConnection;21;1;18;0
WireConnection;21;2;20;2
WireConnection;0;0;28;0
WireConnection;0;2;71;0
WireConnection;0;11;21;0
ASEEND*/
//CHKSM=5E7362644F67B6C9B68508055F8E11546983583A